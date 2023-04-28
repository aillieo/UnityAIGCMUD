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

        public readonly SectionData model = new SectionData();

        public event Action onGameStart;

        public event Action onBeginRequest;

        public event Action onEndRequest;

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

        public GameConfig.MUDGameSettings mudSettings { get { return this.config.mudSettings; } }

        public void RequestGameStart()
        {
            model.text.Value = null;
            model.text.Value += config.mudSettings.introText;
            model.choices.Clear().Add(config.mudSettings.gameStartText);
            model.image.Value = null;

            this.onGameStart?.Invoke();
        }

        public async Task Send(string optionText)
        {
            onBeginRequest?.Invoke();

            ITextResponse textResponse = await this.config.text2Text.RequestTextAsync(optionText, this.text2TextConext);

            model.text.Value = string.Empty;
            textResponse.GetDescription(model.text);

            model.choices.Clear();
            textResponse.GetChoices(model.choices);

            textResponse.GetImagePrompt(model.imagePrompt);
            if (!string.IsNullOrEmpty(model.imagePrompt.Value))
            {
                IImageResponse imageResponse = await this.config.text2Image.RequestImageAsync(model.imagePrompt.Value);
                imageResponse.GetImage(model.image);
            }
            else
            {
                model.image.Value = null;
            }

            onEndRequest?.Invoke();
        }
    }
}
