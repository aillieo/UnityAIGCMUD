namespace AillieoUtils.AIGC.Implements
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public partial class OpenAIDALLE : Text2ImageService
    {
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

        public override async Task<IImageResponse> RequestImageAsync(string prompt, IText2ImageConext conext)
        {
            var request = new Request();
            request.prompt = prompt;
            Response response = await Utils.PostAsync<Request, Response>(this.url, request, new Dictionary<string, string>() { { "Authorization", $"Bearer {this.apiKey}" } });
            return response;
        }
    }
}
