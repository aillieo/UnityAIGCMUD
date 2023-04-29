namespace AillieoUtils.AIGC.Implements
{
    public partial class LocalText2TextService
    {
        private class Response : ITextResponse
        {
            public void GetChoices(ArrayProperty<string> arrayProperty)
            {
                arrayProperty.Clear().Add(null);
            }

            public void GetDescription(TextProperty textProperty)
            {
                textProperty.Value = null;
            }

            public void GetImagePrompt(TextProperty textProperty)
            {
                textProperty.Value = null;
            }
        }
    }
}
