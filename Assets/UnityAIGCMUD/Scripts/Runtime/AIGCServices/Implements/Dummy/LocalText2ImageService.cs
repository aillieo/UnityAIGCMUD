namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;

    public partial class LocalText2ImageService : Text2ImageService
    {
        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override Task<IImageResponse> RequestImageAsync(string prompt, IText2ImageConext conext)
        {
            return Task.FromResult((IImageResponse)new Response());
        }

        public override Task<IText2ImageConext> Initialize()
        {
            return Task.FromResult((IText2ImageConext)default);
        }
    }
}
