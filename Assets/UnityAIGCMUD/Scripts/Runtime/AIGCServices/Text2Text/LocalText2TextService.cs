namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;

    public class LocalText2TextService : Text2TextService
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

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override Task<IText2TextConext> Initialize()
        {
            return Task.FromResult((IText2TextConext)default);
        }

        public override Task<ITextResponse> RequestTextAsync(string prompt, IText2TextConext context)
        {
            return Task.FromResult((ITextResponse)new Response());
        }
    }
}
