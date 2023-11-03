using System;
using System.Collections.Generic;
using System.Linq;
using Red.Common.Client.Misc;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Misc.MathExtender;

namespace Red.Common.Client
{
    public class Client : BaseScript
    {
        protected static Ped PlayerPed = Game.PlayerPed;
        protected static Ped Character = Game.Player.Character;
        protected static Player Player = Game.Player;
        protected static Vector3 PlayerPosition = PlayerPed.Position;
        protected static Random random = new();

        protected static readonly IReadOnlyList<Control> cameraControls = new List<Control>()
        {
            Control.LookBehind, Control.LookDown, Control.LookDownOnly, Control.LookLeft, Control.LookLeftOnly, Control.LookLeftRight, Control.LookRight,
            Control.LookRightOnly, Control.LookUp, Control.LookUpDown, Control.LookUpOnly, Control.ScaledLookDownOnly, Control.ScaledLookLeftOnly,
            Control.ScaledLookLeftRight, Control.ScaledLookUpDown, Control.ScaledLookUpOnly,  Control.VehicleDriveLook, Control.VehicleDriveLook2,
            Control.VehicleLookBehind, Control.VehicleLookLeft, Control.VehicleLookRight, Control.NextCamera, Control.VehicleFlyAttackCamera, Control.VehicleCinCam,
        };

        protected static readonly IReadOnlyList<Control> movementControls = new List<Control>()
        {
            Control.MoveDown, Control.MoveDownOnly, Control.MoveLeft, Control.MoveLeftOnly, Control.MoveLeftRight, Control.MoveRight, Control.MoveRightOnly,
            Control.MoveUp, Control.MoveUpDown, Control.MoveUpOnly, Control.VehicleFlyMouseControlOverride, Control.VehicleMouseControlOverride,
            Control.VehicleMoveDown, Control.VehicleMoveDownOnly, Control.VehicleMoveLeft, Control.VehicleMoveLeftRight, Control.VehicleMoveRight,
            Control.VehicleMoveRightOnly, Control.VehicleMoveUp, Control.VehicleMoveUpDown, Control.VehicleSubMouseControlOverride, Control.Duck, Control.SelectWeapon
        };

        protected static readonly IReadOnlyList<Control> attackControls = new List<Control>()
        {
            Control.Attack, Control.Attack2, Control.MeleeAttack1, Control.MeleeAttack2, Control.MeleeAttackAlternate, Control.MeleeAttackHeavy,
            Control.MeleeAttackLight, Control.MeleeBlock, Control.VehicleAttack, Control.VehicleAttack2, Control.VehicleFlyAttack, Control.VehicleFlyAttack2,
            Control.VehiclePassengerAttack
        };

        protected static readonly IReadOnlyList<WeaponHash> automaticWeapons = new List<WeaponHash>()
        {
            WeaponHash.MicroSMG, WeaponHash.MachinePistol, WeaponHash.MiniSMG, WeaponHash.SMG, WeaponHash.SMGMk2, WeaponHash.AssaultSMG, WeaponHash.CombatPDW, 
            WeaponHash.MG, WeaponHash.CombatMG, WeaponHash.CombatMGMk2, WeaponHash.Gusenberg, WeaponHash.AssaultRifle, WeaponHash.AssaultRifleMk2, WeaponHash.CarbineRifle, 
            WeaponHash.CarbineRifleMk2, WeaponHash.AdvancedRifle, WeaponHash.SpecialCarbine, WeaponHash.SpecialCarbineMk2, WeaponHash.BullpupRifle, WeaponHash.BullpupRifleMk2, 
            WeaponHash.CompactRifle
        };

        public static float CalculateDistanceTo(Vector3 vec1, Vector3 vec2, bool useZ) => useZ ? (float)Math.Sqrt((double)((Vector3)vec1).DistanceToSquared(vec2)) : (float)Math.Sqrt(Math.Pow(x: (double)vec2.X - vec1.X, 2.0) + Math.Pow(vec2.Y - (double)vec1.Y, 2.0));

        #region Weapons
        public static Weapon GiveWeapon(WeaponHash weaponHash, bool equip) => PlayerPed.Weapons.Give(weaponHash, 9999, equip, true);
        public static Weapon GiveWeapon(WeaponHash weaponHash, int ammoAmount, bool equip) => PlayerPed.Weapons.Give(weaponHash, ammoAmount, equip, true);
        public static Weapon GiveWeapon(uint weaponHash, bool equip) => PlayerPed.Weapons.Give((WeaponHash)weaponHash, 9999, equip, true);
        public static Weapon GiveWeapon(uint weaponHash, int ammoAmount, bool equip) => PlayerPed.Weapons.Give((WeaponHash)weaponHash, ammoAmount, equip, true);

        public static bool IsWeaponAutomatic(Weapon weapon) => automaticWeapons.Contains(weapon);
        #endregion

        public static Player GetClosestPlayerToClient(float radius = 2f)
        {
            Player closestPlayer = null;
            PlayerList PlayerList = PlayerList.Players;

            float closestDistance = float.MaxValue;
            float distance = Vector3.DistanceSquared(Character.Position, PlayerPosition);

            foreach (Player player in PlayerList)
            {
                if (player is null || player == Game.Player)
                    continue;

                if (distance < closestDistance && distance < radius)
                {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }

            return closestPlayer;
        }

        public static Vehicle GetClosestVehicleToPlayer(float radius = 2f)
        {
            RaycastResult raycast = World.RaycastCapsule(PlayerPosition, PlayerPosition, radius, (IntersectOptions)10, PlayerPed);

            return raycast.HitEntity as Vehicle;
        }

        public static bool CannotDoAction(Ped ped)
        {
            return ped.IsCuffed
            || ped.IsDead || ped.IsBeingStunned
            || ped.IsClimbing || ped.IsDiving || ped.IsFalling
            || ped.IsGettingIntoAVehicle || ped.IsJumping || ped.IsJumpingOutOfVehicle
            || ped.IsRagdoll || ped.IsSwimmingUnderWater || ped.IsVaulting;
        }

        #region Spawn Local Vehicle
        public static void SpawnLocalVehicle(Vehicle vehicle, Vector3 coords, float heading) => CreateVehicle(vehicle.Model, coords.X, coords.Y, coords.Z, heading, false, false);
        public static void SpawnLocalVehicle(uint vehicle, Vector3 coords, float heading) => CreateVehicle(vehicle, coords.X, coords.Y, coords.Z, heading, false, false);
        #endregion

        public static float CorrectHeading(Entity entity)
        {
            float headingNumber = 360f - entity.Heading;

            if ((double)headingNumber > 360.0f)
            {
                headingNumber -= 360f;
            }

            return headingNumber;
        }

        #region Model Checker
        public static bool DoesModelExist(uint modelHash) => IsModelInCdimage(modelHash);
        public static bool DoesModelExist(string modelName) => DoesModelExist((uint)GetHashKey(modelName));
        #endregion

        #region Extensions
        public static Player GetClosestPlayer(float radius = 2f) => GetClosestPlayerToClient(radius);
        public static Player GetClosestPlayerToPed(float radius = 2f) => GetClosestPlayerToClient(radius);
        public static Player GetClosestPlayerInRadius(float radius = 2f) => GetClosestPlayerToClient(radius);

        public static Vehicle GetClosestVehicle(float radius = 2f) => GetClosestVehicleToPlayer(radius);
        public static Vehicle GetClosestVehicleToPed(float radius = 2f) => GetClosestVehicleToPlayer(radius);
        public static Vehicle GetClosestVehicleInRadius(float radius = 2f) => GetClosestVehicleToPlayer(radius);
        #endregion

        #region Disable Actions
        public static void DisableAttackControls()
        {
            foreach (Control control in attackControls)
            {
                Game.DisableControlThisFrame(0, control);
            }
        }

        public static void DisableMovement(bool disableCameraMovement = false)
        {
            foreach (Control control in movementControls)
            {
                Game.DisableControlThisFrame(0, control);
            }

            if (disableCameraMovement is true)
            {
                foreach (Control control in cameraControls)
                {
                    Game.DisableControlThisFrame(0, control);
                }
            }
        }
        #endregion

        #region Heading Calculation
        public static float CalculateHeadingTowardsEntity(Entity entity, Entity targetEntity)
        {
            Vector3 dirToTargetEnt = targetEntity.Position - entity.Position;
            dirToTargetEnt.Normalize();

            return ConvertDirectionToHeading(dirToTargetEnt);
        }

        public static float CalculateHeadingTowardsPosition(Vector3 start, Vector3 target)
        {
            Vector3 dirToTargetEnt = target - start;
            dirToTargetEnt.Normalize();

            return ConvertDirectionToHeading(dirToTargetEnt);
        }
        #endregion

        #region Teleportation
        // Forked from vMenu
        public static void TeleportPlayer(int playerId, Vector3 pos, bool withVehicle, bool freezePlayer)
        {
            PlayerList Players = PlayerList.Players;
            Player player = Players[playerId];


            if (freezePlayer && player != null && Character != null && Character.Exists())
            {
                Character.IsPositionFrozen = true;

                FreezeEntityPosition(Character.Handle, true);
                ClearPedTasksImmediately(Character.Handle);
                ClearPedTasks(Character.Handle);
            }

            if (withVehicle && player != null && Character != null && Character.Exists() && player.Character.IsInVehicle())
            {
                SetPedCoordsKeepVehicle(Character.Handle, pos.X, pos.Y, pos.Z);
            }
        }
        #endregion

        #region Field of View
        public static void ForceFirstPerson() => SetFollowPedCamViewMode(4);
        public static void ForceThirdPersonClose() => SetFollowPedCamViewMode(0);
        public static void ForceThirdPerson() => SetFollowPedCamViewMode(1);
        public static void ForceThirdPersonFar() => SetFollowPedCamViewMode(2);
        public static void ForceCinematic() => SetFollowPedCamViewMode(3);
        #endregion

        #region Animations
        public static void ClearAllTaskImmediately() => PlayerPed.Task.ClearAllImmediately();
        public static void ClearAllTasks() => PlayerPed.Task.ClearAll();

        public static void PlayAnim(string dictionary, string name) => PlayerPed.Task.PlayAnimation(dictionary, name);
        public static void PlayAnim(string dictionary, string name, float blindInSpeed, int duration, AnimationFlags flags) => PlayerPed.Task.PlayAnimation(dictionary, name, blindInSpeed, duration, flags);
        public static void PlayAnim(string dictionary, string name, float blindInSpeed, float blindOutSpeed, int duration, AnimationFlags flags, float playbackRate) => PlayerPed.Task.PlayAnimation(dictionary, name, blindInSpeed, blindOutSpeed, duration, flags, playbackRate);
        public static void PlayAnim(string dictionary, string name, int blindInSpeed, int duration, AnimationFlags flags) => PlayAnim(dictionary, name, ConvertIntToFloat(blindInSpeed), duration, flags);
        public static void PlayAnim(string dictionary, string name, int blindInSpeed, int blindOutSpeed, int duration, AnimationFlags flags, int playbackRate) => PlayerPed.Task.PlayAnimation(dictionary, name, ConvertIntToFloat(blindInSpeed), ConvertIntToFloat(blindOutSpeed), duration, flags, ConvertIntToFloat(playbackRate));
        #endregion

        #region Props
        public static void SpawnPreviewProp(string model)
        {
            Prop previewProp;

            Vector3 pedPos = PlayerPed.Position;
            RaycastResult raycast = World.RaycastCapsule(pedPos, pedPos, 2f, (IntersectOptions)10, PlayerPed);

            if (!raycast.DitHitEntity)
            {
                Vector3 previewCoords = PlayerPed.GetOffsetPosition(new(0.0f, 1.2f, 1.32f));

                previewProp = new(CreateObjectNoOffset((uint)GetHashKey(model), previewCoords.X, previewCoords.Y, previewCoords.Z, false, false, false))
                {
                    Rotation = PlayerPed.Rotation + new Vector3(-20f, 0f, 0f),
                    IsCollisionEnabled = false,
                    Opacity = 100
                };
            }
        }

        public static void DeleteProp(string modelName)
        {
            int hash = GetHashKey(modelName);
            Vector3 entityCoords = GetEntityCoords(PlayerPedId(), true);
            Vector3 coords = new()
            {
                X = entityCoords.X,
                Y = entityCoords.Y,
                Z = entityCoords.Z
            };

            if (DoesObjectOfTypeExistAtCoords(coords.X, coords.Z, coords.Z, 1.5f, ConvertIntToUInt(hash), true))
            {
                int prop = GetClosestObjectOfType(coords.X, coords.Y, coords.Y, 1.5f, ConvertIntToUInt(hash), false, false, false);
                DeleteObject(ref prop);
            }
        }

        public static void PlaceObjectOnGround(int model) => PlaceObjectOnGroundProperly(model);
        public static void PlaceObjectOnGround(string model) => PlaceObjectOnGroundProperly(GetHashKey(model));
        #endregion

        #region Entity
        public static void DeleteEntity(string entity) => DeleteEntity(entity);
        public static void SetEntityOpacity(int entity, int alpha, bool changeSkin = false) => SetEntityAlpha(entity, alpha, changeSkin ? 1 : 0);
        #endregion

        #region Chat Addons
        public static void AddChatMessage(string author, string text, bool multiLineEnabled = true, int r = 255, int g = 255, int b = 255)
        {
            TriggerEvent("chat:addMessage", new
            {
                multiline = multiLineEnabled,
                args = new[] { author, text },
                color = new[] { r, g, b }
            });
        }

        public static void AddChatMessage(string author, string text, int r = 255, int g = 255, int b = 255)
        {
            TriggerEvent("chat:addMessage", new
            {
                args = new[] { author, text },
                color = new[] { r, g, b }
            });
        }

        public static void AddChatMessage(string author, string text)
        {
            TriggerEvent("chat:addMessage", new
            {
                args = new[] { author, text },
            });
        }
        #endregion

        #region Players
        public static List<int> GetPlayers()
        {
            List<int> players = new();

            for (int i = 0; i <= 255; i++)
            {
                if (NetworkIsPlayerActive(i))
                {
                    players.Add(i);
                }
            }

            return players;
        }
        #endregion
    }
}