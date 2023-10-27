using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Red.Common.Client.Misc;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Diagnostics.Log;
using static Red.Common.Client.Misc.MathExtender;
using System.Linq;

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

        public static Vehicle GetClosestVehicle(float radius = 2f) => GetClosestVehicleToPlayer(radius);
        public static Vehicle GetClosestVehicleToPed(float radius = 2f) => GetClosestVehicleToPlayer(radius);
        #endregion

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
        public static async void SpawnProp(string modelName)
        {
            var player = Game.PlayerPed;
            var coords = player.Position;
            var heading = player.Heading;

            RequestModel((uint)GetHashKey(modelName));
            while (!HasModelLoaded((uint)GetHashKey(modelName)))
            {
                await Delay(0);
            }

            var offsetCoords = GetOffsetFromEntityInWorldCoords(player.Handle, 0.0f, 0.75f, 0.0f);
            var prop = CreateObjectNoOffset((uint)GetHashKey(modelName), offsetCoords.X, offsetCoords.Y, offsetCoords.Z, false, true, false);
            SetEntityHeading(prop, heading);
            PlaceObjectOnGround(prop);
            SetEntityCollision(prop, false, true);
            SetEntityOpacity(prop, 100);
            FreezeEntityPosition(prop, true);
            SetModelAsNoLongerNeeded((uint)GetHashKey(modelName));

            while (true)
            {
                await Delay(0);

                offsetCoords = GetOffsetFromEntityInWorldCoords(player.Handle, 0.0f, 0.75f, 0.0f);
                heading = player.Heading;

                SetEntityCoordsNoOffset(prop, offsetCoords.X, offsetCoords.Y, offsetCoords.Z, false, false, false);
                SetEntityHeading(prop, heading);
                PlaceObjectOnGroundProperly(prop);
                DisableControlAction(PadIndex.Unknown, 38, true); // E
                DisableControlAction(PadIndex.Unknown, 140, true); // R

                if (IsDisabledControlJustPressed(PadIndex.Unknown, 38)) // E
                {
                    var propCoords = GetEntityCoords(prop, false);
                    var propHeading = GetEntityHeading(prop);
                    DeleteObject(ref prop);

                    RequestModel((uint)GetHashKey(modelName));
                    while (!HasModelLoaded((uint)GetHashKey(modelName)))
                    {
                        await Delay(0);
                    }

                    prop = CreateObjectNoOffset((uint)GetHashKey(modelName), propCoords.X, propCoords.Y, propCoords.Z, true, true, true);
                    SetEntityHeading(prop, propHeading);
                    PlaceObjectOnGroundProperly(prop);
                    FreezeEntityPosition(prop, true);
                    SetEntityInvincible(prop, true);
                    SetModelAsNoLongerNeeded((uint)GetHashKey(modelName));

                    return;
                }

                if (IsDisabledControlJustPressed(PadIndex.Unknown, 140)) // R
                {
                    DeleteObject(ref prop);
                    return;
                }
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

        public static void DeleteEntity(string entity) => DeleteEntity(entity);
        public static void SetEntityOpacity(int entity, int alpha, bool changeSkin = false) => SetEntityAlpha(entity, alpha, changeSkin ? 1 : 0);
       
    }
}