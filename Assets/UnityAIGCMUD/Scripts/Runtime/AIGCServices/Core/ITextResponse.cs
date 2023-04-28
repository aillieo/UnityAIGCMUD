namespace AillieoUtils.AIGC
{
    public interface ITextResponse
    {
        void GetDescription(TextProperty textProperty);

        void GetChoices(ArrayProperty<string> arrayProperty);

        void GetImagePrompt(TextProperty textProperty);
    }
}
