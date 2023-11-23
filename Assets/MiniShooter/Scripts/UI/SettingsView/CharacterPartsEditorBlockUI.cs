using MasterServerToolkit.Bridges;
using MasterServerToolkit.Extensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static TMPro.TMP_Dropdown;

namespace MiniShooter
{
    public class CharacterPartsEditorBlockUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private TMP_Text headerTitle;
        [SerializeField]
        private RectTransform componentsContainer;

        [Header("Prefabs"), SerializeField]
        private ColorPalette colorsPalettePrefab;
        [SerializeField]
        private TMP_Dropdown dropdownPrefab;
        [SerializeField]
        private TMP_Text lablePrefab;

        public UnityEvent<string, string> OnPartChangeEvent;
        public UnityEvent<string, string, int, Color> OnPartColorChangeEvent;

        #endregion

        private TMP_Dropdown dropdownInstance;
        private readonly Dictionary<string, ColorPalette> colorPalettes = new Dictionary<string, ColorPalette>();
        private readonly Dictionary<string, TMP_Text> lables = new Dictionary<string, TMP_Text>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="parts"></param>
        public void Set(string title, List<PlayerCharacterPart> parts)
        {
            if (parts == null || parts.Count == 0)
                throw new Exception("The number of parts in list must be more than 0");

            headerTitle.text = title;

            componentsContainer.RemoveChildren();

            if (parts.Count > 1)
            {
                DrawDropDown(parts);
                DrawColorPalettes(parts);
            }
            else
            {
                DrawColorPalettes(parts);
            }

            SetActiveColorPalettesByName(parts[0].Id);
            SetActiveLablesPalettesByName(parts[0].Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SelectDropdownItem(string value)
        {
            if (dropdownInstance)
            {
                int index = dropdownInstance.options.FindIndex(v => v.text == value);

                if (index >= 0)
                    SelectDropdownItem(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SelectDropdownItem(int index)
        {
            if (dropdownInstance)
                dropdownInstance.value = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SelectColorInPalette(string partId, int paletteIndex, Color color)
        {
            if (colorPalettes.TryGetValue($"color_{partId}_{paletteIndex}", out ColorPalette palette) && palette != null)
                palette.SelectColor(color);
        }

        private void DrawDropDown(List<PlayerCharacterPart> parts)
        {
            dropdownInstance = Instantiate(dropdownPrefab, componentsContainer, false);
            dropdownInstance.name = "dropdown";

            var options = new List<OptionData>();

            foreach (var part in parts)
            {
                options.Add(new OptionData()
                {
                    text = part.Title
                });
            }

            dropdownInstance.options = options;
            dropdownInstance.onValueChanged.AddListener((index) =>
            {
                OnPartChangeEvent.Invoke(parts[index].Category, parts[index].Id);
                SetActiveColorPalettesByName(parts[index].Id);
                SetActiveLablesPalettesByName(parts[index].Id);
            });
        }

        private void DrawColorPalettes(List<PlayerCharacterPart> parts)
        {
            lables.Clear();
            colorPalettes.Clear();

            foreach (var part in parts)
            {
                int index = 0;

                foreach (var material in part.Materials)
                {
                    var lableInatance = Instantiate(lablePrefab, componentsContainer, false);
                    lableInatance.name = $"lable_{part.Id}_{index}";
                    lableInatance.text = $"Color {index}";

                    lables.Add(lableInatance.name, lableInatance);

                    var colorsPaletteInatance = Instantiate(colorsPalettePrefab, componentsContainer, false);
                    colorsPaletteInatance.name = $"color_{part.Id}_{index}";

                    int colorIndex = index;

                    colorsPaletteInatance.OnColorChangeEvent.AddListener((color) =>
                    {
                        OnPartColorChangeEvent?.Invoke(part.Category, part.Id, colorIndex, color);
                    });

                    colorPalettes.Add(colorsPaletteInatance.name, colorsPaletteInatance);

                    index++;
                }
            }
        }

        private void SetActiveLablesPalettesByName(string id)
        {
            foreach (var lable in lables)
                lable.Value.gameObject.SetActive(lable.Key.Contains(id));
        }

        private void SetActiveColorPalettesByName(string id)
        {
            foreach (var palette in colorPalettes)
                palette.Value.gameObject.SetActive(palette.Key.Contains(id));
        }
    }
}