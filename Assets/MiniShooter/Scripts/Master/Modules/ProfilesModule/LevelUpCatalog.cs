using MasterServerToolkit.MasterServer;
using System;
using System.Linq;
using UnityEngine;

namespace MiniShooter
{
    [Serializable]
    public class LevelUpItemInfo
    {
        public string key;
        public string title;
        public Sprite icon;
        public Color iconColor = Color.white;
        public float value;
        public float max;
        public int price;
    }

    [CreateAssetMenu(menuName = MstConstants.CreateMenu + "Mini shooter/LevelUpCatalog")]
    public class LevelUpCatalog : ScriptableObject
    {
        #region INSPECTOR

        [SerializeField]
        private LevelUpItemInfo[] _items;

        #endregion

        public LevelUpItemInfo[] Items => _items;

        private void OnValidate()
        {
            for (int i = 0; i < _items.Length; i++)
                _items[i].value = Mathf.Clamp(_items[i].value, 0.01f, float.MaxValue);
        }

        public LevelUpItemInfo GetLevelUpInfo(string key)
        {
            return _items.ToList().Find(i => i.key == key);
        }

        public bool TryGetLevelUpInfo(string key, out LevelUpItemInfo info)
        {
            info = GetLevelUpInfo(key);
            return info != null;
        }
    }
}
