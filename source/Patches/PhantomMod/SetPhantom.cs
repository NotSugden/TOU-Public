using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;

namespace TownOfUs.PhantomMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public class SetPhantom
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (WillBePhantom == null) return;
            var localPlayer = PlayerControl.LocalPlayer;
            var localPlayerId = localPlayer.PlayerId;
            var exiled = __instance.exiledPlayer?.Object?.PlayerId == localPlayerId;
            if (exiled) localPlayer.Data.IsDead = true;
            else if (!localPlayer.Data.IsDead) return;

            if (localPlayerId != WillBePhantom.PlayerId) return;
            Vent vent;
            if (localPlayer.Is(RoleEnum.Phantom))
            {
                vent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];
                localPlayer.NetTransform.RpcSnapTo(vent.transform.position);
                vent.Use();
				localPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
                return;
            }
            Role.RoleDictionary.Remove(localPlayer.PlayerId);
            var phantom = new Phantom(PlayerControl.LocalPlayer);
            phantom.RegenTask();
            RemoveTasks(PlayerControl.LocalPlayer);
            localPlayer.MyPhysics.ResetMoveState(true);
            localPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
            MessageWriter msg = AmongUsClient.Instance.StartRpcImmediately(
                localPlayer.NetId,
                (byte)CustomRPC.ForceSetPhantom,
                SendOption.Reliable,
                -1
            );
            AmongUsClient.Instance.FinishRpcImmediately(msg);
            vent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];
            localPlayer.NetTransform.RpcSnapTo(vent.transform.position + new Vector3(0f, 0.5f, 0f));
			localPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
			localPlayer.Collider.enabled = true;
            vent.Use();
        }

        public static void RemoveTasks(PlayerControl player)
        {
            foreach (PlayerTask playerTask in player.myTasks)
            {
                var task = playerTask.TryCast<NormalPlayerTask>();
                if (task == null) continue;
                task.Initialize();
                if (task.TaskType == TaskTypes.PickUpTowels)
                {
                    foreach (TowelTaskConsole towelTaskConsole in UnityEngine.Object.FindObjectsOfType<TowelTaskConsole>())
                    {
                        towelTaskConsole.Image.color = Color.white;
                    }
                }
                task.taskStep = 0;
                var taskData = player.Data.FindTaskById(playerTask.Id);
                taskData.Complete = false;
            }
        }

        private static Action OnClick(Phantom role)
        {
            void Listener()
            {
                if (MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;
                role.Caught = true;
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.CatchPhantom,
                    SendOption.Reliable,
                    -1
                );
                messageWriter.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            }
            return Listener;
        }

        public static void AddCollider(Phantom role)
        {
            var player = role.Player;
            var boxCollider2D = player.Collider = player.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
			boxCollider2D.enabled = true;
            var passiveButton = player.gameObject.AddComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnMouseOut = new Button.ButtonClickedEvent();
            passiveButton.OnMouseOver = new Button.ButtonClickedEvent();

            passiveButton.OnClick.AddListener(OnClick(role));
        }

        public static PlayerControl WillBePhantom;

        public static Vector2 StartPosition;
    }
}
