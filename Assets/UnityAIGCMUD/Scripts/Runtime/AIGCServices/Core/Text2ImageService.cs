namespace AillieoUtils.AIGC
{
    using System.Threading.Tasks;

    public abstract class Text2ImageService : AIGCService
    {
        public abstract Task<IText2ImageConext> Initialize();

        public abstract Task<IImageResponse> RequestImageAsync(string prompt);
    }
}
