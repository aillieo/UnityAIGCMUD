namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;

    public partial class LocalText2TextService : Text2TextService
    {
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
