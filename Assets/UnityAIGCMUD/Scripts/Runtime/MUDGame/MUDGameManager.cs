namespace AillieoUtils.MUD
{
    using System;
    using System.Threading.Tasks;
    using AillieoUtils.AIGC;

    public class MUDGameManager
    {
        public enum State
        {
            NotInitialized = 0,
        }

        public static readonly MUDGameManager instance = new MUDGameManager();

        private MUDGameManager()
        {
        }

        public event Action<SectionData> onGameStart;

        public event Action onBeginRequest;

        public event Action<SectionData> onEndRequest;

        public event Action onGameOver;

        private IText2TextConext text2TextConext;
        private IText2ImageConext text2ImageConext;

        private GameConfig config;

        public State state { get; private set; }

        public async Task<bool> Initialize(GameConfig config)
        {
            this.config = config;
            this.text2TextConext = await this.config.text2Text.Initialize();
            this.text2ImageConext = await this.config.text2Image.Initialize();
            return true;
        }

        public void RequestGameStart()
        {
            SectionData section = new SectionData();
            section.text = config.mudSettings.background;
            section.choices = new string[] { "开始游戏" };

            this.onGameStart?.Invoke(section);
        }

        public async Task<SectionData> Send(string optionText)
        {
            onBeginRequest?.Invoke();

            ITextResponse textResponse = await this.config.text2Text.RequestTextAsync(optionText, this.text2TextConext);

            SectionData sectionData = new SectionData();

            sectionData.text = textResponse.GetDescription();
            sectionData.choices = textResponse.GetChoices();

            string imagePrompt = textResponse.GetImagePrompt();
            if (!string.IsNullOrEmpty(imagePrompt))
            {
                IImageResponse imageResponse = await this.config.text2Image.RequestImageAsync(imagePrompt);
                sectionData.texture = imageResponse.GetImage();
            }

            onEndRequest?.Invoke(sectionData);

            return sectionData;
        }
    }
}
