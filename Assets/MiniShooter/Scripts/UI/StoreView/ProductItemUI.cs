using MasterServerToolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniShooter
{
    public class ProductItemUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private Image iconImage;
        [SerializeField]
        private TMP_Text nameLable;
        [SerializeField]
        private UIButton buyButton;

        public UnityEvent<InventoryItem> OnBuyClickEvent;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Set(InventoryItem item)
        {
            iconImage.sprite = null;
            iconImage.sprite = item.ItemIcon;
            nameLable.text = item.ItemTitle;
            buyButton.SetLable($"Buy ${item.BuyPrice}");
            buyButton.AddOnClickListener(() =>
            {
                OnBuyClickEvent?.Invoke(item);
            });
        }
    }
}