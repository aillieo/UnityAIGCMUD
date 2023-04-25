namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;

    public class LocalText2TextService : Text2TextService
    {
        private class Response : ITextResponse
        {
            public string[] GetChoices()
            {
                return new string[] { "继续", "放弃" };
            }

            public string GetDescription()
            {
                return "一句描述而已";
            }

            public string GetImagePrompt()
            {
                return "场景图";
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
