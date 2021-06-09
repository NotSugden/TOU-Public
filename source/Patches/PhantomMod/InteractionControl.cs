using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.PhantomMod
{
    class InteractionControl
    {
        [HarmonyPatch(typeof(DeconControl), nameof(DeconControl.CanUse))]
        public class DeconControl_CanUse
        {
            public static bool Prefix(
                DeconControl __instance,
                [HarmonyArgument(0)] GameData.PlayerInfo pc,
                [HarmonyArgument(1)] ref bool canUse,
                [HarmonyArgument(2)] ref bool couldUse,
                ref float __result
            )
            {
                if (__instance.System.CurState > DeconSystem.States.Idle)
                {
                    canUse = false;
                    couldUse = false;
                    __result = 0f;
                    return false;
                }
                var num = float.MaxValue;
                var playerControl = pc.Object;
                var truePosition = playerControl.GetTruePosition();
                var position = __instance.transform.position;
                position.y -= 0.1f;
                couldUse = playerControl.CanMove && (
                    !pc.IsDead || (playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(playerControl).Caught)
                ) && !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipAndObjectsMask, false);
                canUse = couldUse && __instance.cooldown == 0f;
                if (canUse)
                {
                    num = Vector2.Distance(truePosition, position);
                    canUse &= (num <= __instance.UsableDistance);
                }
                __result = num;
                return false;
            }
        }

        [HarmonyPatch(typeof(DeconControl), nameof(DeconControl.Use))]
        public static class DeconControl_Use
        {
            public static bool Prefix(DeconControl __instance)
            {
                __instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out _);
                if (!canUse) return false;
                __instance.cooldown = 6f;
                if (Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlaySound(__instance.UseSound, false, 1f);
                }
                __instance.OnUse.Invoke();
                return false;
            }
        }

        public static bool DoorUsable(
            GameData.PlayerInfo pc,
            Transform transform,
            PlainDoor door,
            float usableDistance,
            ref bool canUse,
            ref bool couldUse,
            ref float __result
        )
        {
            var player = pc.Object;
            var num = Vector2.Distance(player.GetTruePosition(), transform.position);
            couldUse = (!pc.IsDead || (player.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(player).Caught)) && !door.Open;
            canUse = couldUse && num <= usableDistance;
            __result = num;
            return false;
        }

        [HarmonyPatch(typeof(DoorConsole), nameof(DoorConsole.CanUse))]
        public class DoorConsole_CanUse
        {
            public static bool Prefix(
                DoorConsole __instance,
                [HarmonyArgument(0)] GameData.PlayerInfo pc,
                [HarmonyArgument(1)] ref bool canUse,
                [HarmonyArgument(2)] ref bool couldUse,
                ref float __result
            ) => DoorUsable(pc, __instance.transform, __instance.MyDoor, __instance.UsableDistance, ref canUse, ref couldUse, ref __result);
        }

        [HarmonyPatch(typeof(DoorConsole), nameof(DoorConsole.Use))]
        public class DoorConsole_Use
        {
            public static bool Prefix(DoorConsole __instance)
            {
                __instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out _);
                if (!canUse) return false;
                PlayerControl.LocalPlayer.NetTransform.Halt();
                var minigame = Object.Instantiate(__instance.MinigamePrefab, Camera.main.transform);
                minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                minigame.Cast<IDoorMinigame>().SetDoor(__instance.MyDoor);
                minigame.Begin(null);
                return false;
            }
        }

        [HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.CanUse))]
        public class OpenDoorConsole_CanUse
        {
            public static bool Prefix(
                OpenDoorConsole __instance,
                [HarmonyArgument(0)] GameData.PlayerInfo pc,
                [HarmonyArgument(1)] ref bool canUse,
                [HarmonyArgument(2)] ref bool couldUse,
                ref float __result
            ) => DoorUsable(pc, __instance.transform, __instance.MyDoor, __instance.UsableDistance, ref canUse, ref couldUse, ref __result);
        }

        [HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.Use))]
        public class OpenDoorConsole_Use
        {
            public static bool Prefix(OpenDoorConsole __instance)
            {
                __instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out _);
                if (!canUse) return false;
                __instance.MyDoor.SetDoorway(true);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.CanUse))]
        public class PlatformConsoleUse
        {
            // Token: 0x0600031F RID: 799 RVA: 0x00010034 File Offset: 0x0000E234
            public static bool Prefix(
                PlatformConsole __instance,
                [HarmonyArgument(0)] GameData.PlayerInfo pc,
                [HarmonyArgument(1)] ref bool canUse,
                [HarmonyArgument(2)] ref bool couldUse,
                ref float __result
            )
            {
                var num = float.MaxValue;
                var player = pc.Object;
                couldUse = (!pc.IsDead ||
                    (player.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(player).Caught)
                ) && player.CanMove && !__instance.Platform.InUse && Vector2.Distance(__instance.Platform.transform.position, __instance.transform.position) < 2f;
                canUse = couldUse;
                if (canUse)
                {
                    var truePosition = player.GetTruePosition();
                    var position = __instance.transform.position;
                    num = Vector2.Distance(truePosition, position);
                    canUse &= num <= __instance.UsableDistance &&
                        !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipAndObjectsMask, false);
                }
                __result = num;
                return false;
            }
        }
    }
}
