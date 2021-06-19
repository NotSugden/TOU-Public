using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using TownOfUs.MedicMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;

        public static string ColorText(Color32 color, string text) => ColorText(
            string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { color.r, color.g, color.b, color.a }),
            text
        );
        public static string ColorText(string hex, string text) => $"<color=#{hex}>{text}</color>";

        public static void SendRpc(
            CustomRPC method, byte[] parameters = null, int targetClient = -1
        )
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
              PlayerControl.LocalPlayer.NetId,
              (byte)method,
              SendOption.Reliable,
              targetClient
            );

            if (parameters != null)
            {
                foreach (byte parameter in parameters)
                {
                    writer.Write(parameter);
                }
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static void SetSkin(PlayerControl Player, uint skin)
        {
            Player.MyPhysics.SetSkin(skin);
        }

        public static void Morph(PlayerControl Player, PlayerControl MorphedPlayer, bool resetAnim = false)
        {
            if (CamouflageMod.CamouflageUnCamouflage.IsCamoed)
            {
                return;
            }

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                Player.nameText.text = MorphedPlayer.Data.PlayerName;
            }

            var colorId = MorphedPlayer.Data.ColorId;
            PlayerControl.SetPlayerMaterialColors(colorId, Player.myRend);
            Player.HatRenderer.SetHat(MorphedPlayer.Data.HatId, colorId);
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                Player.Data.HatId == 0U ? 0.7f :
                CustomHats.HatCreation.TallIds.Contains(Player.Data.HatId) ? 1.2f : 1.05f,
                -0.5f
            );

            if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                .AllSkins.ToArray()[(int)MorphedPlayer.Data.SkinId].ProdId)
            {
                SetSkin(Player, MorphedPlayer.Data.SkinId);
            }

            if (Player.CurrentPet == null || Player.CurrentPet.ProdId !=
                DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int)MorphedPlayer.Data.PetId].ProdId)
            {

                if (Player.CurrentPet != null)
                {
                    Object.Destroy(Player.CurrentPet.gameObject);
                }

                Player.CurrentPet =
                    Object.Instantiate(
                        DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int)MorphedPlayer.Data.PetId]);
                Player.CurrentPet.transform.position = Player.transform.position;
                Player.CurrentPet.Source = Player;
                Player.CurrentPet.Visible = Player.Visible;
            }

            PlayerControl.SetPlayerMaterialColors(colorId, Player.CurrentPet.rend);
            /*if (resetAnim && !Player.inVent)
            {
                Player.MyPhysics.ResetAnim();
            }*/
        }

        public static void Unmorph(PlayerControl Player)
        {
            var colorId = Player.Data.ColorId;
            Player.nameText.text = Player.Data.PlayerName;
            PlayerControl.SetPlayerMaterialColors(colorId, Player.myRend);
            Player.HatRenderer.SetHat(Player.Data.HatId, colorId);
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                Player.Data.HatId == 0U ? 0.7f :
                CustomHats.HatCreation.TallIds.Contains(Player.Data.HatId) ? 1.2f : 1.05f,
                -0.5f
            );

            if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                .AllSkins.ToArray()[(int)Player.Data.SkinId].ProdId)
            {
                SetSkin(Player, Player.Data.SkinId);
            }


            if (Player.CurrentPet != null)
            {
                Object.Destroy(Player.CurrentPet.gameObject);
            }

            Player.CurrentPet =
                Object.Instantiate(
                    DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int)Player.Data.PetId]);
            Player.CurrentPet.transform.position = Player.transform.position;
            Player.CurrentPet.Source = Player;
            Player.CurrentPet.Visible = Player.Visible;


            PlayerControl.SetPlayerMaterialColors(colorId, Player.CurrentPet.rend);

            /*if (!Player.inVent)
            {
                Player.MyPhysics.ResetAnim();
            }*/
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                player.nameText.text = "";
                PlayerControl.SetPlayerMaterialColors(Color.grey, player.myRend);
                player.HatRenderer.SetHat(0, 0);
                if (player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                    .AllSkins.ToArray()[0].ProdId)
                {
                    SetSkin(player, 0);
                }

                if (player.CurrentPet != null)
                {
                    Object.Destroy(player.CurrentPet.gameObject);
                }
                player.CurrentPet =
                    Object.Instantiate(
                        DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0]);
                player.CurrentPet.transform.position = player.transform.position;
                player.CurrentPet.Source = player;
                player.CurrentPet.Visible = player.Visible;

            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Unmorph(player);
            }
        }


        public static Dictionary<PlayerControl, Color> oldColors = new Dictionary<PlayerControl, Color>();

        public static List<WinningPlayerData> potentialWinners = new List<WinningPlayerData>();


        public static bool IsCrewmate(this PlayerControl player)
        {
            return player.Is(RoleEnum.Crewmate);
        }


        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item) where T : IDisconnectHandler
        {
            if (!self.Contains(item))
            {
                self.Add(item);
            }
        }

        public static bool isLover(this PlayerControl player)
        {
            return player.Is(new RoleEnum[] { RoleEnum.Lover, RoleEnum.LoverImpostor });
        }

        public static bool Is(this PlayerControl player, RoleEnum[] roleTypes)
        {
            return roleTypes.Any(roleType => player.Is(roleType));
        }
        public static bool Is(this PlayerControl player, ModifierEnum[] roleTypes)
        {
            return roleTypes.Any(roleType => player.Is(roleType));
        }
        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Roles.Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return Modifier.GetModifier(player)?.ModifierType == modifierType;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player?.Data == null) return RoleEnum.None;

            var role = Role.GetRole(player);
            if (role != null)
            {
                return role.RoleType;
            }

            return player.Data.IsImpostor ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }

            return null;
        }

        public static List<PlayerControl> getCrewmates(IEnumerable<GameData.PlayerInfo> infection)
        {
            var list = new List<PlayerControl>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var isImpostor = false;
                foreach (var impostor in infection)
                {
                    if (player.PlayerId == impostor.Object.PlayerId)
                    {
                        isImpostor = true;
                    }
                }

                if (!isImpostor)
                {
                    list.Add(player);
                }

            }

            return list;
        }

        public static bool isShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Roles.Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static Medic getMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Roles.Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as Medic;
        }

        public static List<PlayerControl> getImpostors(IEnumerable<GameData.PlayerInfo> infection)
        {
            var list = new List<PlayerControl>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var isImpostor = false;
                foreach (var impostor in infection)
                {
                    if (player.PlayerId == impostor.Object.PlayerId)
                    {
                        isImpostor = true;
                    }
                }

                if (isImpostor)
                {
                    list.Add(player);
                }

            }

            return list;
        }

        public static PlayerControl getClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }

        public static PlayerControl getClosestPlayer(PlayerControl refplayer)
        {
            return getClosestPlayer(refplayer, PlayerControl.AllPlayerControls.ToArray().ToList());
        }

        public static double getDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcKillDuringMeeting(PlayerVoteArea voteArea, PlayerControl player)
        {
            KillDuringMeeting(voteArea, player);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.KillDuringMeeting, SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void KillDuringMeeting(PlayerControl player)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            KillDuringMeeting(voteArea, player);
        }
        public static void KillDuringMeeting(PlayerVoteArea voteArea, PlayerControl player)
        {
            SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f);
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!PlayerControl.GameOptions.GhostsDoTasks)
                {
                    for (int i = 0;i < player.myTasks.Count;i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        UnityEngine.Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(0)
                    );
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);

                if (player.Is(RoleEnum.Swapper))
                {
                    var buttons = Role.GetRole<Swapper>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    }
                }
            }
            player.Die(DeathReason.Kill);
            var meetingHud = MeetingHud.Instance;
            if (amOwner)
            {
                meetingHud.SetForegroundForDead();
                ShowRoleNamePatch.Patch(true);
            }
            var deadBody = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadBody);
            if (voteArea == null) return;
            if (voteArea.DidVote) voteArea.UnsetVote();
            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;
            var amHost = AmongUsClient.Instance.AmHost;
            foreach (var playerVoteArea in meetingHud.playerStates)
            {
                if (!playerVoteArea.DidVote || playerVoteArea.VotedFor != player.PlayerId) return;
                if (amHost)
                {
                    meetingHud.RpcClearVote(playerVoteArea.TargetPlayerId);
                }
                playerVoteArea.UnsetVote();
                var voteAreaPlayer = PlayerById(playerVoteArea.TargetPlayerId);
                if (!voteAreaPlayer.AmOwner) continue;
                var skipButton = meetingHud.SkipVoteButton;
                skipButton.gameObject.SetActive(true);
                skipButton.SetEnabled();
                skipButton.voteComplete = false;
                foreach (var playerVoteArea2 in meetingHud.playerStates)
                {
                    playerVoteArea2.SetEnabled();
                    playerVoteArea2.voteComplete = false;
                }
                meetingHud.state = MeetingHud.VoteStates.NotVoted;
            }
            if (!amHost) return;
            meetingHud.CheckForEndVoting();
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, bool showBody = true)
        {
            MurderPlayer(killer, target, showBody);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.BypassKill, Hazel.SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write(showBody);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showBody = true)
        {
            GameData.PlayerInfo data = target.Data;
            if (data != null && !data.IsDead)
            {
                if (killer == PlayerControl.LocalPlayer)
                {
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
                }

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }
                    var hudManager = HudManager.Instance;
                    hudManager.KillOverlay.ShowKillAnimation(killer.Data, data);
                    hudManager.ShadowQuad.gameObject.SetActive(false);
                    target.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                    if (!PlayerControl.GameOptions.GhostsDoTasks)
                    {
                        for (int i = 0;i < target.myTasks.Count;i++)
                        {
                            PlayerTask playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            UnityEngine.Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks,
                            new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks,
                            new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                if (showBody)
                {
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random<KillAnimation>()
                        .CoPerformKill(killer, target));
                }
                else
                {
                    target.Die(DeathReason.Kill);
                    target.PlayAnimation((byte)KillAnimType.Stab);
                }
                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow,
                };

                Murder.KilledPlayers.Add(deadBody);
                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Roles.Role.GetRole<Roles.Glitch>(killer);
                    glitch.KillTimer = PlayerControl.GameOptions.KillCooldown * 3;
                    glitch.Player.SetKillTimer(PlayerControl.GameOptions.KillCooldown * 3);
                }
            }
        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
        {

            color.a = alpha;
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            var oldcolour = fullscreen.color;
            fullscreen.enabled = true;
            fullscreen.color = color;
            yield return new WaitForSeconds(waitfor);
            fullscreen.enabled = false;
            fullscreen.color = oldcolour;
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return Enumerable.Zip(first, second, (x, y) => (x, y));
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) return;
                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.HumansByVote, bool showAds = false)
        {
            ShipStatus.RpcEndGame(reason, showAds);
        }
    }
}
