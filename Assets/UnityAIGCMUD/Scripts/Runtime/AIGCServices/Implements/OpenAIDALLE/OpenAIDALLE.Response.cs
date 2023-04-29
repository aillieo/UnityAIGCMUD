namespace AillieoUtils.AIGC.Implements
{
    using System;
    using UnityEngine;

    public partial class OpenAIDALLE
    {
        [Serializable]
        private class Response : IImageResponse
        {
            [Serializable]
            public class ImageData
            {
                public string url;
                public string b64_json;
            }

            public ImageData[] data;

            public void GetImage(TextureProperty textureProperty)
            {
                Texture2D texture = Utils.LoadImageFromBase64(this.data[0].b64_json);
                textureProperty.Value = texture;
            }
        }
    }
}
