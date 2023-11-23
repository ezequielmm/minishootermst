using MasterServerToolkit.MasterServer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniShooter
{
    public class LevelUpInfoUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private Image iconImage;
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI currentValueText;
        [SerializeField]
        private TextMeshProUGUI nextValueText;
        [SerializeField]
        private Button button;

        #endregion

        private ObservableFloat property;
        private LevelUpItemInfo info;

        private void OnDestroy()
        {
            if (property != null)
                property.OnDirtyEvent -= Property_OnDirtyEvent;
        }

        public void Set(LevelUpItemInfo info, IObservableProperty property)
        {
            this.info = info;
            this.property = property.As<ObservableFloat>();
            this.property.OnDirtyEvent += Property_OnDirtyEvent;

            Property_OnDirtyEvent(property);
        }

        private void Property_OnDirtyEvent(IObservableProperty property)
        {
            UpdateInfo(info);
        }

        private void UpdateInfo(LevelUpItemInfo info)
        {
            iconImage.sprite = info.icon;
            iconImage.color = info.iconColor;
            titleText.text = $"{info.title} ${info.price}";
            currentValueText.text = $"Current: {property.Value:F1}";

            if (property.Value < info.max)
                nextValueText.text = $"Next: {property.Value + info.value:F1} of {info.max:F1}";
            else
                nextValueText.text = "Max value reached";
        }

        public void OnClick(UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }
    }
}
