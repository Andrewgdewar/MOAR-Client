using LiteNetLib.Utils;
using System;

namespace MOAR.Networking
{
    public class PresetSyncPacket : INetSerializable
    {
        public string PresetName;
        public string PresetLabel;

        public PresetSyncPacket() { }

        public PresetSyncPacket(string name, string label)
        {
            PresetName = name;
            PresetLabel = label;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PresetName);
            writer.Put(PresetLabel);
        }

        public void Deserialize(NetDataReader reader)
        {
            PresetName = reader.GetString();
            PresetLabel = reader.GetString();
        }
    }
}
