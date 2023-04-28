namespace AillieoUtils.AIGC.Implements
{
    using System;
    using System.Threading.Tasks;
    using UnityEngine;

    public class StableDiffusion : Text2ImageService
    {
        [Serializable]
        private class Request : IRequest
        {
            public string image_dimensions = "512x512";
            public int num_outputs = 1;
            public string prompt;
            public int steps = 20;
        }

        [Serializable]
        private class Response : IImageResponse
        {
            public string[] images;

            public void GetImage(TextureProperty textureProperty)
            {
                textureProperty.Value = Utils.LoadImageFromBase64(images[0]);
            }
        }

        [SerializeField]
        private string url = "http://localhost:7861/sdapi/v1/txt2img";

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
            Response response = await Utils.PostAsync<Request, Response>(this.url, request);
            return response;
        }
    }
}
