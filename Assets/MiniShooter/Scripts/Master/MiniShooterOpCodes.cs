using MasterServerToolkit.Extensions;

namespace MiniShooter
{
    public struct MiniShooterOpCodes
    {
        public static ushort UpdateCharacterEditorInfo = nameof(UpdateCharacterEditorInfo).ToUint16Hash();

        public static ushort GetFreeMoney = nameof(GetFreeMoney).ToUint16Hash();
        public static ushort LevelUp = nameof(LevelUp).ToUint16Hash();
    }
}