namespace AillieoUtils.AIGC.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class OpenAIDALLE : Text2ImageService
    {
        [Serializable]
        private class Request : IRequest
        {
            public string response_format = "b64_json";
            public string size = "512x512";
            public int n = 1;
            public string prompt;
        }

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
                Texture2D texture = Utils.LoadImageFromBase64(data[0].b64_json);
                textureProperty.Value = texture;
            }
        }

        [SerializeField]
        private string url = "https://api.openai.com/v1/images/generations";

        [SerializeField]
        private string apiKey = "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override Task<IText2ImageConext> Initialize()
        {
            return Task.FromResult((IText2ImageConext)default);
        }

        public override async Task<IImageResponse> RequestImageAsync(string prompt)
        {
            Request request = new Request();
            request.prompt = prompt;
            Response response = await Utils.PostAsync<Request, Response>(this.url, request, new Dictionary<string, string>() { { "Authorization", $"Bearer {apiKey}" } });
            return response;
        }
    }
}
