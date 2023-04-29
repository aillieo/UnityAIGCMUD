namespace AillieoUtils.MUD
{
    using System;
    using System.Threading.Tasks;
    using AillieoUtils.AIGC;

    public class MUDGameManager
    {
        public static readonly MUDGameManager instance = new MUDGameManager();

        public readonly SectionData model = new SectionData();

        private IText2TextConext text2TextConext;
        private IText2ImageConext text2ImageConext;

        private GameConfig config;

        private bool gameStartMessage = true;

        private MUDGameManager()
        {
        }

        public event Action onGameStart;

        public event Action onBeginRequest;

        public event Action onEndRequest;

        public event Action onGameOver;

        public GameConfig.MUDGameSettings mudSettings
        {
            get { return this.config.mudSettings; }
        }

        public async Task<bool> Initialize(GameConfig config)
        {
            this.config = config;
            this.text2TextConext = await this.config.text2Text.Initialize();
            this.text2ImageConext = await this.config.text2Image.Initialize();
            return true;
        }

        public void RequestGameStart()
        {
            this.model.text.Value = this.config.mudSettings.introText;
            this.model.choices.Clear().Add(this.config.mudSettings.gameStartText);
            this.model.image.Value = null;

            this.onGameStart?.Invoke();
        }

        public async Task SendOption(string optionText)
        {
            this.onBeginRequest?.Invoke();

            var sendText = optionText;

            if (this.gameStartMessage)
            {
                this.gameStartMessage = false;
                sendText = string.Join("\n", this.mudSettings.backgroundText, this.mudSettings.introText, optionText);
            }

            ITextResponse textResponse = await this.config.text2Text.RequestTextAsync(sendText, this.text2TextConext);

            this.model.text.Value = string.Empty;
            textResponse.GetDescription(this.model.text);

            this.model.choices.Clear();
            textResponse.GetChoices(this.model.choices);

            this.model.imagePrompt.Value = string.Empty;
            textResponse.GetImagePrompt(this.model.imagePrompt);

            this.onEndRequest?.Invoke();
        }

        public async Task RequestImage()
        {
            IImageResponse imageResponse = await this.config.text2Image.RequestImageAsync(this.model.imagePrompt.Value, this.text2ImageConext);
            imageResponse.GetImage(this.model.image);
        }
    }
}
