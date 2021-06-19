/*using HarmonyLib;
using System;
using TownOfUs.Roles;
using System.Text;
using UnityEngine;
namespace TownOfUs
{
    public static class FreeplayLaptopPatch
    {
        public class RoleSetterButton : TaskAddButton
        {
            public Type RoleClass;
            public CustomRPC Rpc;
            public RoleEnum RoleType;
            public RoleSetterButton(Type T, CustomRPC rpc, RoleEnum roleType) : base() {
                RoleClass = T;
                Rpc = rpc;
                RoleType = roleType;
                ImpostorTask = true;
            }

            [HarmonyPatch(typeof(TaskAddButton), nameof(TaskAddButton.AddTask))]
            public static class TaskAddButton_AddTask
            {
                public static bool Prefix(TaskAddButton __instance)
                {
                    var instance = __instance.TryCast<RoleSetterButton>();
                    if (instance == null) return true;
                    var player = PlayerControl.LocalPlayer;
                    var playerId = player.PlayerId;
                    var existingRole = Role.RoleDictionary.ContainsKey(playerId) ? Role.RoleDictionary[playerId] : null;
                    var hasSameRole = existingRole?.RoleType == instance.RoleType;
                    Role.RoleDictionary.Remove(player.PlayerId);
                    if (hasSameRole)
                        Role.Gen(typeof(Crewmate), player, CustomRPC.SetCrewmate);
                    else
                        Role.Gen(instance.RoleClass, player, instance.Rpc);
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.Begin))]
        public static class TaskAdderGame_Begin
        {
            public static void Postfix(TaskAdderGame __instance)
            {
                var folder = new TaskFolder();
                folder.FolderName = "Roles";
                __instance.Root.SubFolders.Add(folder);
            }
        }

        [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
        public static class TaskAdderGame_ShowFolder
        {
            public static bool Prefix(TaskAdderGame __instance, [HarmonyArgument(0)] TaskFolder folder)
            {
                if (folder.FolderName != "Roles") return true;
                var builder = new StringBuilder(64);
                __instance.Heirarchy.Add(folder);
                foreach (var _folder in __instance.Heirarchy)
                {
                    builder.Append(_folder.FolderName);
                    builder.Append("\\");
                }
                __instance.PathText.text = builder.ToString();
                foreach (var item in __instance.ActiveItems)
                {
                    UnityEngine.Object.Destroy(item.gameObject);
                }
                __instance.ActiveItems.Clear();

                float x = 0, y = 0, maxHeight = 0;
                void AddItem(Type T, string name)
                {
                    var rpc = (CustomRPC)Enum.Parse(typeof(CustomRPC), $"Set{name}");
                    var roleType = (RoleEnum)Enum.Parse(typeof(RoleEnum), name);
                    var button = new RoleSetterButton(T, rpc, roleType);
                    button.Text.text = $"Become_{name}.exe";
                    __instance.AddFileAsChild(folder, button, ref x, ref y, ref maxHeight);
                }
                #region Crewmate Roles
                AddItem(typeof(Mayor), "Mayor");
                AddItem(typeof(Sheriff), "Sheriff");
                AddItem(typeof(Engineer), "Engineer");
                AddItem(typeof(Swapper), "Swapper");
                if (PlayerControl.GameOptions.MapId != 4)
                    AddItem(typeof(Investigator), "Investigator");
                AddItem(typeof(TimeLord), "TimeLord");
                AddItem(typeof(Medic), "Medic");
                AddItem(typeof(Seer), "Seer");
                AddItem(typeof(Spy), "Spy");
                AddItem(typeof(Altruist), "Altruist");
                #endregion
                #region Neutral Roles
                AddItem(typeof(Jester), "Jester");
                AddItem(typeof(Arsonist), "Arsonist");
                AddItem(typeof(Shifter), "Shifter");
                AddItem(typeof(Executioner), "Executioner");
                AddItem(typeof(Glitch), "Glitch");
                #endregion
                #region Impostor Roles
                AddItem(typeof(Morphling), "Morphling");
                AddItem(typeof(Assassin), "Assassin");
                AddItem(typeof(Miner), "Miner");
                AddItem(typeof(Swooper), "Swooper");
                AddItem(typeof(Janitor), "Janitor");
                #endregion
                return false;
            }
        }
    }
}
*/
