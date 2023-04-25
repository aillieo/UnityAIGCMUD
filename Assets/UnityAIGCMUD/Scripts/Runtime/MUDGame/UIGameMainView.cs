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
        private Text descriptionText;

        [SerializeField]
        private UIButton buttonPrefab;

        [SerializeField]
        private Transform buttonLayout;

        //[SerializeField]
        //private InputField inputField;

        private List<UIButton> choicesButtons = new List<UIButton>();

        private void OnEnable()
        {
            MUDGameManager.instance.onGameStart += this.OnGameStart;
            MUDGameManager.instance.onBeginRequest += this.OnBeginRequest;
            MUDGameManager.instance.onEndRequest += this.OnEndRequest;
            MUDGameManager.instance.onGameOver += this.OnGameOver;
        }

        private void OnDisable()
        {
            MUDGameManager.instance.onGameStart -= this.OnGameStart;
            MUDGameManager.instance.onBeginRequest -= this.OnBeginRequest;
            MUDGameManager.instance.onEndRequest -= this.OnEndRequest;
            MUDGameManager.instance.onGameOver -= this.OnGameOver;
        }

        private void OnGameStart(SectionData sectionData)
        {
            this.ReloadView(sectionData);
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

        private void OnEndRequest(SectionData sectionData)
        {
            this.ReloadView(sectionData);
        }

        private void ReloadView(SectionData sectionData)
        {
            int choiceCount = sectionData.choices.Length;

            while (this.choicesButtons.Count < choiceCount)
            {
                this.choicesButtons.Add(Instantiate(this.buttonPrefab, this.buttonLayout));
            }

            while (this.choicesButtons.Count > choiceCount)
            {
                int lastIndex = this.choicesButtons.Count - 1;
                UIButton last = this.choicesButtons[lastIndex];
                this.choicesButtons.RemoveAt(lastIndex);
                Destroy(last.gameObject);
            }

            for (int i = 0; i < choiceCount; ++i)
            {
                this.choicesButtons[i].SetInteractable(true);
                this.choicesButtons[i].SetTextContent(sectionData.choices[i]);
            }

            this.descriptionText.text = sectionData.text;
            this.sceneImage.texture = sectionData.texture;
        }
    }
}
