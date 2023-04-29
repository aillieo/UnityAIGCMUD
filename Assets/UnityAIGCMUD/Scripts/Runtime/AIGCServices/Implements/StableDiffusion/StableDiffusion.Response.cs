namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class StableDiffusion
    {
        [Serializable]
        private class Response : IImageResponse
        {
            public string[] images;

            public void GetImage(TextureProperty textureProperty)
            {
                textureProperty.Value = Utils.LoadImageFromBase64(this.images[0]);
            }
        }
    }
}
