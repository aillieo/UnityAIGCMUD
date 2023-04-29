namespace AillieoUtils.MUD
{
    using AillieoUtils.AIGC;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Text choiceText;

        public void SetInteractable(bool interactable)
        {
            this.button.interactable = interactable;
        }

        public void SetTextContent(string textContent)
        {
            this.choiceText.text = textContent;
        }

        public void SetTextColor(Color color)
        {
            this.choiceText.color = color;
        }

        private void OnEnable()
        {
            this.button.onClick.AddListener(this.OnClick);
        }

        private void OnDisable()
        {
            this.button.onClick.RemoveListener(this.OnClick);
        }

        private void OnClick()
        {
            MUDGameManager.instance.SendOption(this.choiceText.text).AwaitAndCheck();
        }
    }
}
