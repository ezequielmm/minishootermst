using UnityEngine;

namespace MasterServerToolkit.Games
{
    public class LookAtCameraCanvas : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private Canvas canvas;

        #endregion

        private Vector3 defaultScale = Vector3.one;
        private float currentDistancePercentage = 0f;

        private void Awake()
        {
            defaultScale = canvas.transform.localScale;
            canvas.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            canvas.transform.rotation = Camera.main.transform.rotation;
            //currentDistancePercentage = cam.CurrentDistance / cam.MaxDistance;
            //canvas.transform.localScale = Vector3.Lerp(canvas.transform.localScale, currentDistancePercentage * defaultScale * 1f, Time.deltaTime * 3f);
        }
    }
}
