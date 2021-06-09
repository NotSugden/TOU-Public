﻿// This folder is a Stripped down version of Reactor-Essentials
// Please use https://github.com/DorCoMaNdO/Reactor-Essentials because it is more updated and less buggy

using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor;

namespace TownOfUs.CustomOption
{
    public static class Rpc
    {
        public static void SendRpc(CustomOption optionn = null)
        {
            List<CustomOption> options;
            if (optionn != null)
            {
                options = new List<CustomOption> { optionn };
            }
            else
            {
                options = CustomOption.AllOptions;
            }

            var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SyncCustomSettings, SendOption.Reliable);
            foreach (var option in options)
            {
                writer.Write(option.ID);
                if (option.Type == CustomOptionType.Toggle) writer.Write((bool)option.Value);
                else if (option.Type == CustomOptionType.Number) writer.Write((float)option.Value);
                else if (option.Type == CustomOptionType.String) writer.Write((int)option.Value);

            }
            writer.EndMessage();

        }

        public static void ReceiveRpc(MessageReader reader)
        {
            while (reader.BytesRemaining > 0)
            {

                int id = reader.ReadInt32();
                CustomOption customOption = CustomOption.AllOptions.FirstOrDefault(option => option.ID == id); // Works but may need to change to gameObject.name check
                var type = customOption?.Type;
                object value = null;
                if (type == CustomOptionType.Toggle) value = reader.ReadBoolean();
                else if (type == CustomOptionType.Number) value = reader.ReadSingle();
                else if (type == CustomOptionType.String) value = reader.ReadInt32();

                customOption?.Set(value);

            }
        }
    }
}
