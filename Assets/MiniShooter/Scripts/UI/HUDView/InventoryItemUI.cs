using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniShooter
{
    public class InventoryItemUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private Image iconImage;
        [SerializeField]
        private TMP_Text quantityLable;
        [SerializeField]
        private TMP_Text keyCodeLable;

        #endregion

        public void SetKeyCode(string keyCode)
        {
            keyCodeLable.text = keyCode;
        }

        public void SetQuantity(int quantity)
        {
            quantityLable.text = $"{quantity}";
        }

        public void SetIcon(Sprite icon)
        {
            iconImage.sprite = icon;
        }
    }
}