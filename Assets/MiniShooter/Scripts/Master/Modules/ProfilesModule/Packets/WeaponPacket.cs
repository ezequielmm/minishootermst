using MasterServerToolkit.Networking;

namespace MiniShooter
{
    public class WeaponPacket : SerializablePacket
    {
        public int CurrentAmmo { get; set; }
        public int TotalAmmo { get; set; }

        public override void FromBinaryReader(EndianBinaryReader reader)
        {
            CurrentAmmo = reader.ReadInt32();
            TotalAmmo = reader.ReadInt32();
        }

        public override void ToBinaryWriter(EndianBinaryWriter writer)
        {
            writer.Write(CurrentAmmo);
            writer.Write(TotalAmmo);
        }
    }
}
