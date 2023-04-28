namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class LocalText2ImageService : Text2ImageService
    {
        private class Response : IImageResponse
        {
            public Texture2D texture;

            public void GetImage(TextureProperty textureProperty)
            {
                textureProperty.Value = texture;
            }
        }

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override Task<IImageResponse> RequestImageAsync(string prompt)
        {
            return Task.FromResult((IImageResponse)new Response());
        }

        public override Task<IText2ImageConext> Initialize()
        {
            return Task.FromResult((IText2ImageConext)default);
        }
    }
}
