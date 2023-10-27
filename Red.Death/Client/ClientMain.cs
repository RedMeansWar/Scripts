using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using SharpConfig;
using static CitizenFX.Core.Native.API;

namespace Red.Death.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool isDead, keyHeld, auomaticRespawn, enableRagdoll;
        private int reviveDelay;
        private int startTime;
        private int reviveKey = 51;
        private int respawnKey = 45;
        private Ped PlayerPed = Game.PlayerPed;
        private Vector3 reviveAt;
        private Tuple<string, string> anim;

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
        public ClientMain() => ReadConfigFile();
        #endregion

        #region Commands
        [Command("respawn")]
        private async void RespawnCommand()
        {
            bool shouldRespawn = ReviveCommand();

            if (!shouldRespawn)
            {
                return;
            }

            Vector4 spawnPos = spawnLocations.OrderBy(p => Vector3.DistanceSquared((Vector3)p, PlayerPed.Position)).First();

            PlayerPed.Weapons.RemoveAll();

            NetworkFadeOutEntity(PlayerPed.Handle, false, false);

            while (NetworkIsEntityFading(PlayerPed.Handle))
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

            StartPlayerTeleport(PlayerPed.Handle, spawnPos.X, spawnPos.Y, spawnPos.Z, spawnPos.W, false, true, true);

            while (IsPlayerTeleportActive())
            {
                await Delay(0);
            }

            NetworkFadeInEntity(PlayerPed.Handle, false);
            Screen.Fading.FadeIn(500);
        }

        [Command("revive")]
        private bool ReviveCommand()
        {
            Tick -= DeadTick;
            Tick -= ControlsTick;
            isDead = false;

            NetworkResurrectLocalPlayer(reviveAt.X, reviveAt.Y, reviveAt.Z + 1f, PlayerPed.Heading, false, false);

            PlayerPed.ClearBloodDamage();
            PlayerPed.Task.ClearAll();
            PlayerPed.IsInvincible = false;
            PlayerPed.IsFireProof = false;
            PlayerPed.IsExplosionProof = false;
            PlayerPed.IsCollisionProof = false;
            PlayerPed.IsMeleeProof = false;

            return true;
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
                Debug.WriteLine($"[Death]: Config file has not been configured correctly.");
            }
        }

        private void OnDeath()
        {
            isDead = true;
            Random random = new();

            reviveAt = PlayerPed.Position;
            NetworkResurrectLocalPlayer(reviveAt.X, reviveAt.Y, reviveAt.Z + 0.1f, PlayerPed.Heading, false, false);

            PlayerPed.IsInvincible = true;
            PlayerPed.IsFireProof = true;
            PlayerPed.IsExplosionProof = true;
            PlayerPed.IsCollisionProof = true;
            PlayerPed.IsMeleeProof = true;
            PlayerPed.Health = PlayerPed.MaxHealth;
            PlayerPed.Armor = 0;

            if (enableRagdoll is false)
            {
                anim = animList[random.Next(animList.Count)];
            }
            else
            {
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
            if (!keyHeld && (Game.IsControlPressed(0, (Control)reviveKey) || Game.IsControlPressed(0, (Control)respawnKey)))
            {
                Screen.ShowSubtitle($"Hold for ~r~{Math.Ceiling((double)(startTime + reviveDelay - Game.GameTime) / 1000)} ~s~more second(s)", 110);

                keyHeld = Game.IsControlPressed(0, (Control)reviveKey) || Game.IsControlPressed(0, (Control)respawnKey);

                await Delay(0);
            }

            if (Game.IsControlPressed(0, (Control)reviveKey))
            {
                ReviveCommand();
            }
            else if (Game.IsControlPressed(0, (Control)reviveKey))
            {
                Screen.Fading.FadeOut(1000);

                while (!Screen.Fading.IsFadedOut)
                {
                    await Delay(0);
                }

                RespawnCommand();
            }
        }

        [Tick]
        private async Task DeadTick()
        {
            if (!isDead && PlayerPed.IsDead)
            {
                OnDeath();
            }

            await Delay(250);
        }
        #endregion
    }
}