using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CustomHats;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;

        public static Dictionary<PlayerControl, Color> OldColors = new Dictionary<PlayerControl, Color>();

        public static List<WinningPlayerData> PotentialWinners = new List<WinningPlayerData>();

        public static void SetSkin(PlayerControl Player, uint skin)
        {
            Player.MyPhysics.SetSkin(skin);
        }

        public static void UpdateName(PlayerControl player)
        {
            var hatId = player.Data.HatId;
            float y;
            if (hatId == 0U)
                y = 1.5f;
            else if (HatCreation.TallIds.Contains(player.Data.HatId))
                y = 2.2f;
            else
                y = 2.0f;
            player.nameText.transform.localPosition = new Vector3(
                0f, y, -0.5f
            );
        }

        public static void Morph(PlayerControl player, PlayerControl morphTo)
        {
            if (CamouflageUnCamouflage.IsCamoed) return;

            var morphedData = morphTo.Data;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                player.nameText.text = morphedData.PlayerName;

            var colorId = morphedData.ColorId;
            PlayerControl.SetPlayerMaterialColors(colorId, player.myRend);
            player.HatRenderer.SetHat(morphTo.Data.HatId, colorId);
            UpdateName(player);

            var hatManager = HatManager.Instance;
            var skinId = morphedData.SkinId;
            if (
                player.MyPhysics.Skin.skin.ProdId != hatManager.GetSkinById(skinId).ProdId
            ) SetSkin(player, skinId);

            var morphedPet = hatManager.GetPetById(morphedData.PetId);

            if (player.CurrentPet.ProdId != morphedPet.ProdId)
            {
                Object.Destroy(player.CurrentPet.gameObject);

                var pet = player.CurrentPet = Object.Instantiate(morphedPet);
                pet.transform.position = player.transform.position;
                pet.Source = player;
                pet.Visible = player.Visible;
            }

            PlayerControl.SetPlayerMaterialColors(colorId, player.CurrentPet.rend);
        }

        public static void Unmorph(PlayerControl player)
        {
            var playerData = player.Data;
            var colorId = playerData.ColorId;
            player.nameText.text = playerData.PlayerName;
            PlayerControl.SetPlayerMaterialColors(colorId, player.myRend);
            player.HatRenderer.SetHat(playerData.HatId, colorId);

            UpdateName(player);

            var hatManager = HatManager.Instance;
            var skinId = playerData.SkinId;
            if (
                player.MyPhysics.Skin.skin.ProdId != hatManager.GetSkinById(skinId).ProdId
            ) SetSkin(player, skinId);

            Object.Destroy(player.CurrentPet.gameObject);

            var pet = player.CurrentPet = Object.Instantiate(hatManager.GetPetById(playerData.PetId));
            pet.transform.position = player.transform.position;
            pet.Source = player;
            pet.Visible = player.Visible;

            PlayerControl.SetPlayerMaterialColors(colorId, player.CurrentPet.rend);
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var data = player.Data;
                if (data.IsDead || data.Disconnected) continue;
                player.nameText.text = "";
                PlayerControl.SetPlayerMaterialColors(Color.grey, player.myRend);
                player.HatRenderer.SetHat(0, 0);
                SetSkin(player, 0);

                Object.Destroy(player.CurrentPet.gameObject);

                var pet = player.CurrentPet = Object.Instantiate(
                    HatManager.Instance.GetPetById(0)
                );
                pet.transform.position = player.transform.position;
                pet.Source = player;
                pet.Visible = player.Visible;
            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var data = player.Data;
                if (data.IsDead || data.Disconnected) continue;
                Unmorph(player);
            }
        }

        public static void AddUnique<T>(
            this Il2CppSystem.Collections.Generic.List<T> self, T item
        ) where T : IDisconnectHandler
        {
            if (!self.Contains(item)) self.Add(item);
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player?.Data == null) return RoleEnum.None;

            var role = Role.GetRole(player);
            if (role == null)
                return player.Data.IsImpostor ? RoleEnum.Impostor : RoleEnum.Crewmate;

            return role.RoleType;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;

            return null;
        }

        public static List<PlayerControl> GetCrewmates(IEnumerable<GameData.PlayerInfo> infected)
        {
            var list = new List<PlayerControl>(PlayerControl.AllPlayerControls.ToArray());

            foreach (var impostor in infected)
            {
                list.Remove(impostor.Object);
            }

            return list;
        }

        public static List<PlayerControl> GetImpostors(IEnumerable<GameData.PlayerInfo> infected) =>
            infected.Select(x => x.Object).ToList();

        public static PlayerControl GetClosestPlayer(
            PlayerControl refPlayer,
            List<PlayerControl> AllPlayers,
            float maxDistance = float.NaN)
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (
                    player.Data.IsDead ||
                    player.PlayerId == refPlayer.PlayerId ||
                    !player.Collider.enabled
                ) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < maxDistance;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized,
                    vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                maxDistance = distBetweenPlayers;
                result = player;
            }

            return result;
        }

        public static PlayerControl getClosestPlayer(
            PlayerControl refplayer, float maxDistance = float.NaN
        ) => GetClosestPlayer(refplayer, PlayerControl.AllPlayerControls.ToArray().ToList(), maxDistance);
        
        public static void SetTarget(
            ref PlayerControl closestPlayer,
            KillButtonManager button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null)
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, maxDistance, targets)
            );
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null)
        {
            var player = GetClosestPlayer(
                PlayerControl.LocalPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList(),
                maxDistance
            );
            return closestPlayer = player;
        }


        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }


        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.BypassKill, SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target)
        {
            var targetData = target.Data;
            if (targetData == null || targetData.IsDead) return;

            if (killer.AmOwner)
                SoundManager.Instance.PlaySound(killer.KillSfx, false, 0.8f);

            var killerData = killer.Data;
            target.gameObject.layer = LayerMask.NameToLayer("Ghost");
            if (target.AmOwner)
            {
                try
                {
                    if (Minigame.Instance != null)
                    {
                        Minigame.Instance.Close();
                        Minigame.Instance.Close();
                    }

                    if (MapBehaviour.Instance != null)
                    {
                        MapBehaviour.Instance.Close();
                        MapBehaviour.Instance.Close();
                    }
                }
                catch { }

                var hudManager = HudManager.Instance;
                hudManager.KillOverlay.ShowKillAnimation(killerData, targetData);
                hudManager.ShadowQuad.gameObject.SetActive(false);
                target.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                target.RpcSetScanner(false);
                var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (PlayerControl.GameOptions.GhostsDoTasks)
                {
                    importantTextTask.Text = TranslationController.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }
                else
                {
                    foreach (var playerTask in target.myTasks)
                    {
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    target.myTasks.Clear();
                    importantTextTask.Text = TranslationController.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                target.myTasks.Insert(0, importantTextTask);
            }

            killer.MyPhysics.StartCoroutine(
                killer.KillAnimations.Random().CoPerformKill(killer, target)
            );
            var deadBody = new DeadPlayer
            {
                PlayerId = target.PlayerId,
                KillerId = killer.PlayerId,
                KillTime = DateTime.UtcNow
            };

            Murder.KilledPlayers.Add(deadBody);
            var killerRole = Role.GetRole(killer);
            if (
                killerRole.RoleType != RoleEnum.Arsonist &&
                killerRole.RoleType != RoleEnum.Glitch
            )
                CrewmateRoles.ChildMod.Murder.CheckChild(target);

            if (target.Is(ModifierEnum.Diseased))
            {
                if (killerRole.RoleType == RoleEnum.Glitch)
                {
                    var cooldown = CustomGameOptions.GlitchKillCooldown;
                    ((Glitch)killerRole).LastKill = DateTime.UtcNow.AddSeconds(
                        cooldown * 2
                    );
                    killer.SetKillTimer(cooldown * 3);
                }
                else if (killerData.IsImpostor)
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * 3);
                
                return;
            }

            if (killerData.IsImpostor)
                killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
        {
            color.a = alpha;
            var fullscreen = HudManager.Instance.FullScreen;
            var oldcolour = fullscreen.color;
            fullscreen.enabled = true;
            fullscreen.color = color;
            yield return new WaitForSeconds(waitfor);
            fullscreen.enabled = false;
            fullscreen.color = oldcolour;
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second) =>
            first.Zip(second, (x, y) => (x, y));

        public static void DestroyAll(this IEnumerable<Component> components)
        {
            foreach (var item in components)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) continue;
                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false) =>
            ShipStatus.RpcEndGame(reason, false);
    }
}
