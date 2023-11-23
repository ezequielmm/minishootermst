using System.Collections;
using UnityEngine;

namespace MiniShooter
{
    public class WeaponShotFlash : MonoBehaviour
    {
        private SpriteRenderer[] renderers;
        private Light[] lights;

        private void Awake()
        {
            lights = GetComponentsInChildren<Light>();
            renderers = GetComponentsInChildren<SpriteRenderer>();
            SetRenderersActive(false);
        }

        private void OnEnable()
        {
            SetRenderersActive(false);
        }

        public void Flash()
        {
            if (isActiveAndEnabled)
                StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            SetRenderersActive(true);
            yield return new WaitForSecondsRealtime(0.05f);
            SetRenderersActive(false);
        }

        private void SetRenderersActive(bool value)
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = value;
            }

            foreach (var light in lights)
            {
                light.enabled = value;
            }
        }
    }
}