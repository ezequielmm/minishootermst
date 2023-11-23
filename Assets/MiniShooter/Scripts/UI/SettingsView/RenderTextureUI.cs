using UnityEngine;
using UnityEngine.UI;

namespace MiniShooter
{
    public class RenderTextureUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private RawImage renderImage;
        [SerializeField]
        private Camera renderCamera;

        #endregion

        private RenderTexture rt;
        private RectTransform rect;
        private bool isChanged = false;
        private Vector2 lastRectSize = Vector2.zero;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            if (rect && renderImage && renderCamera && isChanged)
            {
                CreateTexture();
                lastRectSize = rect.rect.size;
            }

            if (rect)
                isChanged = lastRectSize != rect.rect.size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public void CreateTexture()
        {
            rt = new RenderTexture(Mathf.RoundToInt(rect.rect.size.x), Mathf.RoundToInt(rect.rect.size.y), 16, RenderTextureFormat.ARGB32);
            rt.Create();

            renderImage.texture = rt;
            renderCamera.targetTexture = rt;
            renderCamera.Render();
        }
    }
}