namespace AillieoUtils.MUD
{
    using System;
    using AillieoUtils.AIGC;
    using UnityEngine;

    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private Text2TextService text2TextService;

        [SerializeField]
        private Text2ImageService text2ImageService;

        public Text2TextService text2Text { get { return this.text2TextService; } }

        public Text2ImageService text2Image { get { return this.text2ImageService; } }

        [SerializeField]
        private MUDGameSettings settings;

        public MUDGameSettings mudSettings { get { return this.settings; } }

        [Serializable]
        public class MUDGameSettings
        {
            [TextArea]
            public string backgroundText;

            [TextArea]
            public string introText;

            [TextArea]
            public string gameStartText;

            [TextArea]
            public string defaultOption;
        }
    }
}
