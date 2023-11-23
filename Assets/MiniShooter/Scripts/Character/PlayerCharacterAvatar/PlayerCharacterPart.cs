using UnityEngine;

namespace MiniShooter
{
    public class PlayerCharacterPart : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private string id;
        [SerializeField]
        private string title;
        [SerializeField]
        private string category;

        #endregion

        private Material[] materials;

        public string Id => id;
        public string Title => title;
        public string Category => category;
        public Material[] Materials
        {
            get
            {
                if (materials == null)
                {
                    if (TryGetComponent(out Renderer render))
                        materials = render.materials;
                    else
                        materials = new Material[0];
                }

                return materials;
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = name;

            if (string.IsNullOrEmpty(title))
                title = name;
        }
    }
}