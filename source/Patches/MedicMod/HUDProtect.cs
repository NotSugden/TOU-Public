using HarmonyLib;

namespace TownOfUs.MedicMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDRewind
    {
        
        public static void Postfix(PlayerControl __instance)
        {
            UpdateProtectButton(__instance);
        }

        public static void UpdateProtectButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var protectButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];


            var role = Roles.Role.GetRole<Roles.Medic>(PlayerControl.LocalPlayer);



            if (isDead)
            {
                protectButton.gameObject.SetActive(false);
                protectButton.isActive = false;
            }
            else
            {
                protectButton.gameObject.SetActive(!MeetingHud.Instance);
                protectButton.isActive = !MeetingHud.Instance;
                protectButton.SetCoolDown(0f, 1f);
                var closestPlayer = role.ClosestPlayer = Utils.getClosestPlayer(PlayerControl.LocalPlayer);
                if (
                    closestPlayer == null || (
                        Utils.getDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer) < maxDistance
                    )
                )
                {
                    protectButton.SetTarget(null);
                }
                else if (__instance.enabled && !role.UsedAbility)
                {
                    protectButton.SetTarget(closestPlayer);
                    role.ClosestPlayer.myRend.material.SetColor("_OutlineColor", role.Color);
                }
            }
        }
    }
}
