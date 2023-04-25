namespace AillieoUtils.AIGC
{
    using System.Threading.Tasks;

    public abstract class Text2TextService : AIGCService
    {
        public abstract Task<IText2TextConext> Initialize();

        public abstract Task<ITextResponse> RequestTextAsync(string prompt, IText2TextConext context);
    }
}
