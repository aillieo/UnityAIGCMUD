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
            public int num_inference_steps = 20;
        }

        [Serializable]
        private class Response : IImageResponse
        {
            [Serializable]
            public class ImageData
            {
                public string url;
            }

            public ImageData[] items;

            public Texture2D GetImage()
            {
                return null;
            }
        }

        [SerializeField]
        private string url = "http://localhost:7860/run/predict";

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
