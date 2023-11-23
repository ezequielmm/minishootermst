using MasterServerToolkit.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MiniShooter
{
    public class DamageView : UIView
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private Image damageScreen;

        [Header("Settings"), SerializeField]
        private Color damageColor = Color.red;
        [SerializeField]
        private float transitionTime = 1f;

        #endregion

        private float currentTransitionValue = 1f;

        protected override void Awake()
        {
            base.Awake();
            damageScreen.color = Color.clear;
            OnlinePlayerCharacter.OnLocalCharacterDamageEvent += OnlinePlayerCharacter_OnLocalCharacterDamageEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnlinePlayerCharacter.OnLocalCharacterDamageEvent -= OnlinePlayerCharacter_OnLocalCharacterDamageEvent;
        }

        private void OnlinePlayerCharacter_OnLocalCharacterDamageEvent()
        {
            Show();

            currentTransitionValue = 0;

            damageScreen.color = damageColor;
            StopCoroutine(StartTransition());
            StartCoroutine(StartTransition());
        }

        private IEnumerator StartTransition()
        {
            while (currentTransitionValue < 1f)
            {
                float time = Time.deltaTime / transitionTime;
                yield return new WaitForSeconds(time);
                damageScreen.color = Color.Lerp(damageScreen.color, Color.clear, currentTransitionValue);
                currentTransitionValue += time;
            }
        }
    }
}
