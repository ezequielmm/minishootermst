using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using System.Collections.Concurrent;

namespace MiniShooter
{
    public class ObservableCharacterEditor : ObservableBaseDictionary<string, CharacterEditorPacket>
    {
        public ObservableCharacterEditor(ushort key) : base(key) { }

        public ObservableCharacterEditor(ushort key, ConcurrentDictionary<string, CharacterEditorPacket> defaultValues) : base(key, defaultValues) { }

        public override void Deserialize(string value) { }

        public override string Serialize()
        {
            return string.Empty;
        }

        protected override string ReadKey(EndianBinaryReader reader)
        {
            return reader.ReadString();
        }

        protected override CharacterEditorPacket ReadValue(EndianBinaryReader reader)
        {
            return reader.ReadPacket(new CharacterEditorPacket());
        }

        protected override void WriteKey(string key, EndianBinaryWriter writer)
        {
            writer.Write(key);
        }

        protected override void WriteValue(CharacterEditorPacket value, EndianBinaryWriter writer)
        {
            writer.Write(value);
        }
    }
}
