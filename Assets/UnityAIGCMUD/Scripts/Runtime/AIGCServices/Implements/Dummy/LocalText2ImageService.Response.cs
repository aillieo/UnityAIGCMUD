namespace AillieoUtils.AIGC.Implements
{
    using UnityEngine;

    public partial class LocalText2ImageService
    {
        private class Response : IImageResponse
        {
            public Texture2D texture;

            public void GetImage(TextureProperty textureProperty)
            {
                textureProperty.Value = this.texture;
            }
        }
    }
}
