using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniShooter
{
    public class CharacterPartToggleUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private TMP_Text titleText;
        [SerializeField]
        private Toggle toggle;

        #endregion

        private void Start()
        {
            toggle.group = GetComponentInParent<ToggleGroup>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            titleText.text = title;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void OnToggle(UnityAction callback)
        {
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) callback?.Invoke();
            });
        }
    }
}