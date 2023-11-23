using MasterServerToolkit.Extensions;
using MasterServerToolkit.UI;
using UnityEngine;

namespace MiniShooter
{
    public class StoreView : UIView
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private RectTransform container;
        [SerializeField]
        private ProductItemUI productUIPrefab;
        [SerializeField]
        private InventoryItemsDatabase productsDatabase;

        #endregion

        protected override void Start()
        {
            base.Start();
            DrawProducts();
        }

        private void DrawProducts()
        {
            container.RemoveChildren();

            if (productsDatabase)
            {
                foreach (var item in productsDatabase)
                {
                    var productUIInstance = Instantiate(productUIPrefab, container, false);
                    productUIInstance.Set(item);
                    productUIInstance.OnBuyClickEvent.AddListener((selectedItem) =>
                    {
                        if (isVisible)
                            OnlinePlayer.Local.Inventory.BuyItem(selectedItem.ItemId);
                    });
                }
            }
        }
    }
}