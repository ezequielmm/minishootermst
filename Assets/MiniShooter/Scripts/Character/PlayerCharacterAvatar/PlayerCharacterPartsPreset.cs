using System;

namespace MiniShooter
{
    [Serializable]
    public struct PlayerCharacterPartsPreset
    {
        public string presetName;
        public PlayerCharacterPartPreset[] presetParts;
    }
}