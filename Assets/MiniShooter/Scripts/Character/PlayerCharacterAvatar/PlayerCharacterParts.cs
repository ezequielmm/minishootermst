using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniShooter
{
    public delegate void CharacterPartDelegate(string partCat, string partId);

    public class PlayerCharacterParts : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private PlayerCharacterPartsPreset[] presets;

        #endregion

        /// <summary>
        /// List of all parts
        /// </summary>
        public Dictionary<string, List<PlayerCharacterPart>> Parts { get; private set; } = new Dictionary<string, List<PlayerCharacterPart>>();

        /// <summary>
        /// List of presets
        /// </summary>
        public PlayerCharacterPartsPreset[] Preset => presets;

        /// <summary>
        /// List of active parts
        /// </summary>
        public List<PlayerCharacterPart> ActiveParts
        {
            get
            {
                List<PlayerCharacterPart> activePartsList = new List<PlayerCharacterPart>();

                foreach (var partsList in Parts.Values)
                {
                    foreach (var part in partsList.Where(x => x.gameObject.activeSelf))
                    {
                        activePartsList.Add(part);
                    }
                }

                return activePartsList;
            }
        }

        private void Awake()
        {
            FindAllParts();
        }

        void Start()
        {
            ApplyPreset(0);
        }

        private void FindAllParts()
        {
            Parts.Clear();

            foreach (var part in GetComponentsInChildren<PlayerCharacterPart>(true))
            {
                if (!Parts.ContainsKey(part.Category))
                {
                    Parts[part.Category] = new List<PlayerCharacterPart>()
                    {
                        part
                    };
                }
                else
                {
                    Parts[part.Category].Add(part);
                }
            }
        }

        /// <summary>
        /// Applies preset from <see cref="presets"/> by its index
        /// </summary>
        /// <param name="presetIndex"></param>
        public void ApplyPreset(int presetIndex, CharacterPartDelegate applyCallback = null)
        {
            if (presets != null && presets.Length > presetIndex)
            {
                foreach (var presetPart in presets[presetIndex].presetParts)
                {
                    SetPartActive(presetPart.part.Category, presetPart.part.Id);

                    for (int i = 0; i < presetPart.colors.Length; i++)
                    {
                        Color color = presetPart.colors[i];
                        SetPartColor(presetPart.part.Category, presetPart.part.Id, i, color);
                    }

                    applyCallback?.Invoke(presetPart.part.Category, presetPart.part.Id);
                }
            }
        }

        /// <summary>
        /// Sets part active by its <paramref name="partId"/> and by its <paramref name="partCat"/>
        /// </summary>
        /// <param name="partId"></param>
        public void SetPartActive(string partCat, string partId)
        {
            if (Parts.TryGetValue(partCat, out List<PlayerCharacterPart> parts) && parts != null)
            {
                foreach (var part in parts)
                    part.gameObject.SetActive(part.Id == partId);
            }
        }

        /// <summary>
        /// Sets a given <paramref name="color"/> by <paramref name="colorIndex"/> of active part by its <paramref name="partId"/> and by its <paramref name="partCat"/>
        /// </summary>
        /// <param name="partCat"></param>
        /// <param name="partId"></param>
        /// <param name="colorIndex"></param>
        /// <param name="color"></param>
        public void SetPartColor(string partCat, string partId, int colorIndex, Color color)
        {
            if (Parts.TryGetValue(partCat, out List<PlayerCharacterPart> parts) && parts != null)
            {
                foreach (var part in parts)
                    if (part.Id == partId)
                        part.Materials[colorIndex].color = color;
            }
        }
    }
}