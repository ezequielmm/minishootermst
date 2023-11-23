using System;
using UnityEngine;

namespace MiniShooter
{
    [Serializable]
    public struct InventoryItemUseKey
    {
        public InventoryItem item;
        public KeyCode keyCode;
        public string keyCodeName;
    }
}