using MasterServerToolkit.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniShooter
{
    public class WeaponUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private TMP_Text weaponNameLable;
        [SerializeField]
        private Image weaponIcon;
        [SerializeField]
        private TMP_Text ammoQuantityLable;
        [SerializeField]
        private UIProperty reloadingProgress;

        #endregion

        private float totalReloadingProcessTime = 0;
        private float currentReloadingProcessTime = 0;

        private PlayerCharacterWeapon choosenWeapon;

        private void Awake()
        {
            reloadingProgress.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (choosenWeapon != null)
            {
                ammoQuantityLable.text = $"{choosenWeapon.CurrentAmmo}/{choosenWeapon.TotalAmmo}";
                weaponNameLable.text = $"{choosenWeapon.Title}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void StartReloadingProgress(PlayerCharacterWeapon weapon)
        {
            totalReloadingProcessTime = weapon.ReloadTime;
            currentReloadingProcessTime = 0f;

            reloadingProgress.gameObject.SetActive(true);
            reloadingProgress.SetMin(currentReloadingProcessTime);
            reloadingProgress.SetMax(totalReloadingProcessTime);

            StopAllCoroutines();
            StartCoroutine(StartReloadingProgressCoroutine());
        }

        private IEnumerator StartReloadingProgressCoroutine()
        {
            while (currentReloadingProcessTime < totalReloadingProcessTime)
            {
                float delta = 1f / totalReloadingProcessTime;

                currentReloadingProcessTime += delta;
                reloadingProgress.SetValue(currentReloadingProcessTime);
                yield return new WaitForSecondsRealtime(delta);
            }

            reloadingProgress.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        public void ChangeWeapon(PlayerCharacterWeapon weapon)
        {
            choosenWeapon = weapon;
            weaponIcon.sprite = weapon.Icon;
        }
    }
}
