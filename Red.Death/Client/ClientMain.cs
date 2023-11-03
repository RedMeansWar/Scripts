using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Red.Common.Client.Diagnostics;
using SharpConfig;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Death.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool isDead, keyHeld, automaticRespawn, enableRagdoll;
        protected int reviveDelay, respawnKey, reviveKey;
        private int startTime;
        private Vector3 coordsToReviveAt;
        private Tuple<string, string> animToPlay;

        private readonly List<Tuple<string, string>> animList = new()
        {
            Tuple.Create("missfinale_c1@", "lying_dead_player0"),
            Tuple.Create("dead", "dead_a")
        };

        private readonly List<Vector4> spawnLocations = new()
        {
            new(336.82f, -1400.24f, 32.51f, 51.16f),
            new(300.16f, -579.36f, 43.26f, 76.7f),
            new(-247.55f, 6331.49f, 32.43f, 226.04f),
            new(-448.45f, -334.07f, 34.5f, 95.1f)
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            ReadConfigFile();
            TriggerEvent("chat:addSuggestion", "/revive", "Revives you in place.", "");
            TriggerEvent("chat:addSuggestion", "/respawn", "Respawns you at the nearest hospital.", "");
        }
        #endregion

        #region Commands
        [Command("revive")]
        private bool Revive()
        {
            Tick -= DeadTick;
            Tick -= ControlsTick;
            isDead = false;

            NetworkResurrectLocalPlayer(coordsToReviveAt.X, coordsToReviveAt.Y, coordsToReviveAt.Z + 1f, Game.PlayerPed.Heading, false, false);

            Game.PlayerPed.Health = Game.PlayerPed.MaxHealth;
            Game.PlayerPed.ClearBloodDamage();
            Game.PlayerPed.Task.ClearAll();
            Game.PlayerPed.IsInvincible = false;
            Game.PlayerPed.IsFireProof = false;
            Game.PlayerPed.IsExplosionProof = false;
            Game.PlayerPed.IsCollisionProof = false;
            Game.PlayerPed.IsMeleeProof = false;

            return true;
        }

        [Command("respawn")]
        private async void OnRespawnCommand()
        {
            bool shouldRespawn = Revive();

            if (!shouldRespawn)
            {
                return;
            }

            Vector4 spawnPos = spawnLocations.OrderBy(p => Vector3.DistanceSquared((Vector3)p, Game.PlayerPed.Position)).First();

            Game.PlayerPed.Weapons.RemoveAll();

            NetworkFadeOutEntity(Game.PlayerPed.Handle, true, false);
            while (NetworkIsEntityFading(Game.PlayerPed.Handle))
            {
                await Delay(0);
            }

            RequestCollisionAtCoord(spawnPos.X, spawnPos.Y, spawnPos.Z);
            NewLoadSceneStart(spawnPos.X, spawnPos.Y, spawnPos.Z, spawnPos.X, spawnPos.Y, spawnPos.Z, 3f, 0);

            int startTime = Game.GameTime;

            while (IsNetworkLoadingScene())
            {
                if (GetGameTimer() - startTime > 2000)
                {
                    break;
                }

                await Delay(0);
            }

            StartPlayerTeleport(Game.Player.Handle, spawnPos.X, spawnPos.Y, spawnPos.Z, spawnPos.W, false, true, true);

            while (IsPlayerTeleportActive())
            {
                await Delay(0);
            }

            NetworkFadeInEntity(Game.PlayerPed.Handle, false);
            Screen.Fading.FadeIn(500);
        }
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Death", "EnableRagdoll") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                enableRagdoll = loaded["Death"]["EnableRagdoll"].BoolValue;
                reviveDelay = loaded["Death"]["ReviveDelay"].IntValue;
                reviveKey = loaded["Death"]["ReviveKey"].IntValue;
                respawnKey = loaded["Death"]["RespawnKey"].IntValue;
            }
            else
            {
                Log.Error($"[Death]: Config file has not been configured correctly.");
            }
        }

        private async void OnDeath()
        {
            isDead = true;
            Random random = new();

            coordsToReviveAt = Game.PlayerPed.Position;
            NetworkResurrectLocalPlayer(coordsToReviveAt.X, coordsToReviveAt.Y, coordsToReviveAt.Z + 0.1f, Game.PlayerPed.Heading, false, false);

            Game.PlayerPed.IsInvincible = true;
            Game.PlayerPed.IsFireProof = true;
            Game.PlayerPed.IsExplosionProof = true;
            Game.PlayerPed.IsCollisionProof = true;
            Game.PlayerPed.IsMeleeProof = true;
            Game.PlayerPed.Health = Game.PlayerPed.MaxHealth;
            Game.PlayerPed.Armor = 0;

            if (enableRagdoll is false)
            {
                await Delay(0);
                animToPlay = animList[random.Next(animList.Count)];
            }
            else
            {
                await Delay(0);
                SetPedToRagdoll(GetPlayerPed(-1), 60000, 60000, 0, false, false, false);
            }

            DisableAutomaticRespawn(true);

            Tick += DeadTick;
            Tick += ControlsTick;
        }
        #endregion

        #region Ticks
        private async Task ControlsTick()
        {
            if (!keyHeld && (Game.IsControlPressed(0, Control.Context) || Game.IsControlPressed(0, Control.Reload)))
            {
                keyHeld = true;
                startTime = Game.GameTime;

                while (keyHeld && Game.GameTime - startTime < reviveDelay)
                {
                    Screen.ShowSubtitle($"Hold for ~r~{Math.Ceiling((double)(startTime + reviveDelay - Game.GameTime) / 1000)} ~s~more second(s)", 110);

                    keyHeld = Game.IsControlPressed(0, (Control)reviveKey) || Game.IsControlPressed(0, (Control)respawnKey);

                    await Delay(0);
                }

                if (Game.IsControlPressed(0, (Control)reviveKey))
                {
                    Revive();
                }
                else if (Game.IsControlPressed(0, (Control)respawnKey))
                {
                    Screen.Fading.FadeOut(1000);
                    while (!Screen.Fading.IsFadedOut)
                    {
                        await Delay(0);
                    }

                    OnRespawnCommand();
                }
            }
            else
            {
                keyHeld = false;
            }

            await Delay(0);
        }

        private async Task DeadTick()
        {
            DisableFirstPersonCamThisFrame();
            DisplayHudWhenDeadThisFrame();
            AllowPauseMenuWhenDeadThisFrame();

            Game.DisableControlThisFrame(0, Control.Context);
            Game.DisableControlThisFrame(0, Control.Reload);

            if (enableRagdoll is false && !IsEntityPlayingAnim(Game.PlayerPed.Handle, animToPlay.Item1, animToPlay.Item2, 3))
            {
                PlayAnim(animToPlay.Item1, animToPlay.Item2, 50f, -1, (AnimationFlags)1);
            }

            if (!keyHeld)
            {
                Screen.DisplayHelpTextThisFrame("Hold ~INPUT_CONTEXT~ to revive in place, or hold ~INPUT_RELOAD~ to respawn at the closest hospital.");
            }

            await Delay(0);
        }

        [Tick]
        private async Task CheckDeathTick()
        {
            if (!isDead && Game.PlayerPed.IsDead)
            {
                OnDeath();
            }

            await Delay(250);

            Exports["spawnmanager"].setAutoSpawn(false);
        }
        #endregion
    }
}