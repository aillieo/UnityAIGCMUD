namespace AillieoUtils.AIGC
{
    using System.Threading.Tasks;

    public abstract class Text2ImageService : AIGCService
    {
        public abstract Task<ImageResponse> RequestImageAsync(string prompt);
    }
}
