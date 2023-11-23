using MasterServerToolkit.Bridges;
using MasterServerToolkit.Networking;
using System.Collections.Generic;

namespace MiniShooter
{
    public class CharacterEditorPacket : SerializablePacket
    {
        public string Category { get; set; } = string.Empty;
        public string PartId { get; set; } = string.Empty;
        public List<ColorPacket> Colors { get; set; } = new List<ColorPacket>();

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            Category = reader.ReadString();
            PartId = reader.ReadString();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Colors.Add(reader.ReadPacket(new ColorPacket()));
        }

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(Category);
            writer.Write(PartId);
            writer.Write(Colors.Count);

            foreach (var color in Colors)
                writer.Write(color);
        }
    }
}