namespace AillieoUtils.MUD
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIGameMainView : MonoBehaviour
    {
        [SerializeField]
        private RawImage sceneImage;

        [SerializeField]
        private RawImage sceneImageBack;

        [SerializeField]
        private Text descriptionText;

        [SerializeField]
        private UIButton buttonPrefab;

        [SerializeField]
        private Transform buttonLayout;

        [SerializeField]
        private InputField inputField;

        private List<UIButton> choicesButtons = new List<UIButton>();

        private bool imageRequestingFlag = false;

        private void OnEnable()
        {
            MUDGameManager.instance.onGameStart += this.OnGameStart;
            MUDGameManager.instance.onBeginRequest += this.OnBeginRequest;
            MUDGameManager.instance.onEndRequest += this.OnEndRequest;
            MUDGameManager.instance.onGameOver += this.OnGameOver;

            MUDGameManager.instance.model.text.onValueChanged += this.OnDescriptionTextChanged;
            MUDGameManager.instance.model.choices.onLengthChanged += this.OnChoiceCountChanged;
            MUDGameManager.instance.model.choices.onValueChanged += this.OnChoiceContentChanged;
            MUDGameManager.instance.model.imagePrompt.onValueChanged += this.OnImagePromptChanged;
            MUDGameManager.instance.model.image.onValueChanged += this.OnImageChanged;

            this.imageRequestingFlag = false;
        }

        private void OnDisable()
        {
            MUDGameManager.instance.onGameStart -= this.OnGameStart;
            MUDGameManager.instance.onBeginRequest -= this.OnBeginRequest;
            MUDGameManager.instance.onEndRequest -= this.OnEndRequest;
            MUDGameManager.instance.onGameOver -= this.OnGameOver;

            MUDGameManager.instance.model.text.onValueChanged -= this.OnDescriptionTextChanged;
            MUDGameManager.instance.model.choices.onLengthChanged -= this.OnChoiceCountChanged;
            MUDGameManager.instance.model.choices.onValueChanged -= this.OnChoiceContentChanged;
            MUDGameManager.instance.model.imagePrompt.onValueChanged -= this.OnImagePromptChanged;
            MUDGameManager.instance.model.image.onValueChanged -= this.OnImageChanged;
        }

        private void OnGameStart()
        {
        }

        private void OnGameOver()
        {
        }

        private void OnBeginRequest()
        {
            foreach (var button in this.choicesButtons)
            {
                button.SetInteractable(false);
            }
        }

        private void OnEndRequest()
        {
            foreach (var button in this.choicesButtons)
            {
                button.SetInteractable(true);
            }
        }

        private void OnDescriptionTextChanged()
        {
            SectionData sectionData = MUDGameManager.instance.model;
            this.descriptionText.text = sectionData.text.Value;
        }

        private void OnChoiceCountChanged()
        {
            SectionData sectionData = MUDGameManager.instance.model;

            var choiceCount = sectionData.choices.Length;

            while (this.choicesButtons.Count < choiceCount)
            {
                this.choicesButtons.Add(Instantiate(this.buttonPrefab, this.buttonLayout));
            }

            while (this.choicesButtons.Count > choiceCount)
            {
                var lastIndex = this.choicesButtons.Count - 1;
                UIButton last = this.choicesButtons[lastIndex];
                this.choicesButtons.RemoveAt(lastIndex);
                Destroy(last.gameObject);
            }
        }

        private void OnChoiceContentChanged(int index)
        {
            SectionData sectionData = MUDGameManager.instance.model;
            var text = sectionData.choices[index];
            if (string.IsNullOrEmpty(text))
            {
                text = MUDGameManager.instance.mudSettings.defaultOption;
            }

            this.choicesButtons[index].SetTextContent(text);
        }

        private async void OnImagePromptChanged()
        {
            if (this.imageRequestingFlag)
            {
                return;
            }

            if (!string.IsNullOrEmpty(MUDGameManager.instance.model.imagePrompt.Value))
            {
                this.imageRequestingFlag = true;
                await MUDGameManager.instance.RequestImage();
                this.imageRequestingFlag = false;
            }
            else
            {
                MUDGameManager.instance.model.image.Value = null;
            }
        }

        private void OnImageChanged()
        {
            SectionData sectionData = MUDGameManager.instance.model;

            this.sceneImage.enabled = sectionData.image.Value != null;
            this.sceneImage.texture = sectionData.image.Value;
            this.sceneImageBack.texture = sectionData.image.Value;
        }
    }
}
