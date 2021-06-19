using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Phantom : Role
    {
        public bool CompletedTasks = false;
        public bool Faded = false;
        public bool Caught = false;
        public Phantom(PlayerControl player) : base(player)
        {
            Name = "Phantom";
            ImpostorText = () => "Complete your tasks to win";
            TaskText = () => "Complete all your tasks";
            RoleType = RoleEnum.Phantom;
            Faction = Faction.Neutral;
        }

        public void Fade()
        {
            Faded = true;
            var color = new Color(1f, 1f, 1f, 0f);
			var localPlayer = PlayerControl.LocalPlayer;
			if (localPlayer.Collider == null) {
				if (localPlayer.gameObject == null) {
					System.Console.WriteLine("localplayer has no gameObject");
					return;
				}
				var collider = localPlayer.Collider = localPlayer.gameObject.AddComponent<Collider2D>();
				collider.isTrigger = true;
			}
            if (Player.Collider == null)
            {
				if (Player.gameObject == null) {
					System.Console.WriteLine("role player has no gameObject");
					return;
				}
                PhantomMod.SetPhantom.AddCollider(this);
            }
            var magnitude = (localPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;
            var num = Mathf.Max(
                0f,
                (magnitude / (ShipStatus.Instance.MaxLightRadius * PlayerControl.GameOptions.CrewLightMod)) - 1f
            );
            var magnitude2 = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + magnitude2 / Player.MyPhysics.TrueGhostSpeed * 0.13f;
            color.a = Mathf.Lerp(color.a, 0f, num);
            Player.GetComponent<SpriteRenderer>().color = color;
            Player.HatRenderer.SetHat(0U, 0);
            Player.nameText.text = "";
            if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance.AllSkins.ToArray()[0].ProdId)
                Player.MyPhysics.SetSkin(0U);

            if (Player.CurrentPet != null)
                UnityEngine.Object.Destroy(Player.CurrentPet.gameObject);

            Player.CurrentPet = UnityEngine.Object.Instantiate<PetBehaviour>(DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0]);
            Player.CurrentPet.transform.position = Player.transform.position;
            Player.CurrentPet.Source = Player;
            Player.CurrentPet.Visible = Player.Visible;
        }
        public void Loses()
        {
            Player.Data.IsImpostor = true;
        }
    }
}
