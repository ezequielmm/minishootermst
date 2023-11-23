using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using System.Collections.Concurrent;

namespace MiniShooter
{
    public class ObservableWeapons : ObservableBaseDictionary<string, WeaponPacket>
    {
        public ObservableWeapons(ushort key) : base(key) { }

        public ObservableWeapons(ushort key, ConcurrentDictionary<string, WeaponPacket> defaultValues) : base(key, defaultValues) { }

        public override void Deserialize(string value)
        {

        }

        public override string Serialize()
        {
            return string.Empty;
        }

        protected override string ReadKey(EndianBinaryReader reader)
        {
            return reader.ReadString();
        }

        protected override WeaponPacket ReadValue(EndianBinaryReader reader)
        {
            return reader.ReadPacket(new WeaponPacket());
        }

        protected override void WriteKey(string key, EndianBinaryWriter writer)
        {
            writer.Write(key);
        }

        protected override void WriteValue(WeaponPacket value, EndianBinaryWriter writer)
        {
            writer.Write(value);
        }
    }
}
