namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;

    public class StableDiffusion : Text2ImageService
    {
        public override bool Validate(out string error)
        {
            throw new System.NotImplementedException();
        }

        public override Task<ImageResponse> RequestImageAsync(string prompt)
        {
            throw new System.NotImplementedException();
        }
    }
}
