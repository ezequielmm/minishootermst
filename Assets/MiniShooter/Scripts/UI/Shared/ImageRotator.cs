using UnityEngine;
using UnityEngine.UI;

namespace MiniShooter
{
    public class ImageRotator : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private Image effectImage;
        [SerializeField]
        private float speed = 100;
        [SerializeField]
        private bool useScale = true;
        [SerializeField, Range(0f, 1f)]
        private float startScaleFrom = 0f;

        #endregion

        void Update()
        {
            if (effectImage)
            {
                effectImage.transform.Rotate(0, 0, speed * Time.deltaTime);

                if (useScale)
                    effectImage.transform.localScale = Mathf.PingPong(Time.time + startScaleFrom, 1f) * Vector3.one;
            }
        }
    }
}