namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;
    using UnityEngine;

    public partial class StableDiffusion : Text2ImageService
    {
        [SerializeField]
        private string url = "http://localhost:7861/sdapi/v1/txt2img";

        [SerializeField]
        private int steps = 20;

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override Task<IText2ImageConext> Initialize()
        {
            return Task.FromResult((IText2ImageConext)default);
        }

        public override async Task<IImageResponse> RequestImageAsync(string prompt, IText2ImageConext conext)
        {
            var request = new Request();
            request.prompt = prompt;
            request.steps = this.steps;
            Response response = await Utils.PostAsync<Request, Response>(this.url, request);
            return response;
        }
    }
}
