using MasterServerToolkit.Bridges;
using MasterServerToolkit.Extensions;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using MasterServerToolkit.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.TMP_Dropdown;

namespace MiniShooter
{
    public class CharacterEditorView : UIView
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private PlayerCharacterParts playerCharacterParts;
        [SerializeField]
        private RectTransform partsEditorBlocksContainer;
        [SerializeField]
        private RectTransform partTogglesContainer;
        [SerializeField]
        private TMP_Dropdown presetsDropdown;

        //[Header("Info"), SerializeField]
        //private AvatarComponent avatarUI;
        //[SerializeField]
        //private UILable usernameLable;
        //[SerializeField]
        //private UILable displayNameLable;

        //[Header("Editor"), SerializeField]
        //private TMP_InputField displayNameInputField;
        //[SerializeField]
        //private TMP_InputField avatarUrlInputField;

        [Header("Prefabs"), SerializeField]
        private CharacterPartsEditorBlockUI partsEditorBlockUiPrefab;
        [SerializeField]
        private CharacterPartToggleUI characterPartToggleUIPrefab;

        #endregion

        private string selectedEditorBlock = "";
        private ProfileLoaderBehaviour profileLoader;
        private readonly Dictionary<string, CharacterPartsEditorBlockUI> editorBlocks = new Dictionary<string, CharacterPartsEditorBlockUI>();

        protected override void Start()
        {
            base.Start();

            profileLoader = FindObjectOfType<ProfileLoaderBehaviour>();
            profileLoader.OnProfileLoadedEvent.AddListener(OnProfileLoadedEventHandler);
        }

        private void OnProfileLoadedEventHandler()
        {
            //usernameLable.Text = Mst.Client.Auth.AccountInfo.Username;

            FillPresetsDropdown();
            DrawEditorBlocks();
            UpdateCategoriesVisibility();

            foreach (var property in profileLoader.Profile.Properties)
                DrawProfileProperty(property.Key, property.Value);
        }

        private void UpdateCategoriesVisibility()
        {
            foreach (var blockKvp in editorBlocks)
            {
                if (string.IsNullOrEmpty(selectedEditorBlock))
                    selectedEditorBlock = blockKvp.Key;

                editorBlocks[blockKvp.Key].gameObject.SetActive(blockKvp.Key == selectedEditorBlock);
            }
        }

        private void DrawProfileProperty(ushort propertyCode, IObservableProperty property)
        {
            //if (propertyCode == ProfilePropertyKeys.avatarUrl)
            //{
            //    avatarUI.SetAvatarUrl(property.As<ObservableString>().Value);
            //    avatarUrlInputField.text = property.As<ObservableString>().Value;
            //}
            //else if (propertyCode == ProfilePropertyKeys.displayName)
            //{
            //    displayNameLable.Text = property.As<ObservableString>().Value;
            //    displayNameInputField.text = property.As<ObservableString>().Value;
            //}

            if (propertyCode == ProfilePropertyKeys.characterEditor)
            {
                foreach (var editor in property.As<ObservableCharacterEditor>())
                {
                    var characterEditorInfo = editor.Value;
                    playerCharacterParts.SetPartActive(characterEditorInfo.Category, characterEditorInfo.PartId);

                    for (int i = 0; i < characterEditorInfo.Colors.Count; i++)
                        playerCharacterParts.SetPartColor(characterEditorInfo.Category,
                            characterEditorInfo.PartId,
                            i, characterEditorInfo.Colors[i].Color);
                }
            }
        }

        private void FillPresetsDropdown()
        {
            presetsDropdown.options.Clear();

            foreach (var preset in playerCharacterParts.Preset)
            {
                presetsDropdown.options.Add(new OptionData(preset.presetName));
            }

            presetsDropdown.captionText.text = "Select preset";

            presetsDropdown.onValueChanged.AddListener((index) =>
            {
                playerCharacterParts.ApplyPreset(index, OnPartChangeEventHandler);
            });
        }

        private void DrawEditorBlocks()
        {
            editorBlocks.Clear();
            partsEditorBlocksContainer.RemoveChildren();
            partTogglesContainer.RemoveChildren();

            foreach (var kvp in playerCharacterParts.Parts)
            {
                var partsEditorBlockUiInstance = Instantiate(partsEditorBlockUiPrefab, partsEditorBlocksContainer, false);
                partsEditorBlockUiInstance.Set(kvp.Key, kvp.Value);

                if (profileLoader.Profile.TryGet(ProfilePropertyKeys.characterEditor, out ObservableCharacterEditor property)
                    && property.ContainsKey(kvp.Key))
                {
                    var characterEditorInfo = property[kvp.Key];
                    partsEditorBlockUiInstance.SelectDropdownItem(kvp.Value.FindIndex(p => p.Id == characterEditorInfo.PartId));

                    for (int c = 0; c < characterEditorInfo.Colors.Count; c++)
                        partsEditorBlockUiInstance.SelectColorInPalette(characterEditorInfo.PartId, c, characterEditorInfo.Colors[c].Color);
                }

                partsEditorBlockUiInstance.OnPartChangeEvent.AddListener(OnPartChangeEventHandler);
                partsEditorBlockUiInstance.OnPartColorChangeEvent.AddListener(OnPartColorChangeEventHandler);

                editorBlocks[kvp.Key] = partsEditorBlockUiInstance;
            }

            foreach (var kvp in editorBlocks)
            {
                var partToggleUIInstance = Instantiate(characterPartToggleUIPrefab, partTogglesContainer, false);
                partToggleUIInstance.SetTitle(kvp.Key);
                partToggleUIInstance.OnToggle(() =>
                {
                    selectedEditorBlock = kvp.Key;
                    UpdateCategoriesVisibility();
                });
            }
        }

        private void OnPartChangeEventHandler(string partCat, string partId)
        {
            playerCharacterParts.SetPartActive(partCat, partId);

            var characterEditorPacket = CreateEditorPacket(partCat, partId);

            if (characterEditorPacket != null)
            {
                Mst.Client.Connection.SendMessage(MiniShooterOpCodes.UpdateCharacterEditorInfo, characterEditorPacket, (status, response) =>
                {
                    if (status != ResponseStatus.Success)
                    {
                        logger.Error(response.AsString());
                        return;
                    }

                    logger.Info($"Part {partId} of category {partCat} changed");
                });
            }
        }

        private CharacterEditorPacket CreateEditorPacket(string partCat, string partId)
        {
            if (playerCharacterParts.Parts.TryGetValue(partCat, out List<PlayerCharacterPart> parts) && parts != null)
            {
                var characterEditorPacket = new CharacterEditorPacket
                {
                    Category = partCat,
                    PartId = partId
                };

                var part = parts.Find(x => x.Id == partId);

                foreach (var material in part.Materials)
                {
                    characterEditorPacket.Colors.Add(new ColorPacket(material.color));
                }

                return characterEditorPacket;
            }

            return null;
        }

        private void OnPartColorChangeEventHandler(string partCat, string partId, int index, Color color)
        {
            playerCharacterParts.SetPartColor(partCat, partId, index, color);

            var characterEditorPacket = CreateEditorPacket(partCat, partId);

            if (characterEditorPacket != null)
            {
                characterEditorPacket.Colors[index].Color = color;

                Mst.Client.Connection.SendMessage(MiniShooterOpCodes.UpdateCharacterEditorInfo, characterEditorPacket, (status, response) =>
                {
                    if (status != ResponseStatus.Success)
                    {
                        logger.Error(response.AsString());
                        return;
                    }

                    logger.Info($"Part {partId} of category {partCat} changed");
                });
            }
        }
    }
}