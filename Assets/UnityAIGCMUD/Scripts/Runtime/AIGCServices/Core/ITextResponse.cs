namespace AillieoUtils.AIGC
{
    public interface ITextResponse
    {
        string GetDescription();

        string[] GetChoices();

        string GetImagePrompt();
    }
}
