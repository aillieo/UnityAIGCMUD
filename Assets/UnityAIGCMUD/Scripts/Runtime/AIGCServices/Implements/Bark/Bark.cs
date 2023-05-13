namespace AillieoUtils.AIGC.Implements
{
    using System.Threading.Tasks;
    using UnityEngine;

    public partial class Bark : Text2AudioService
    {
        [SerializeField]
        private string url = "localhost:8000/bark";

        [SerializeField]
        [Range(0, 1)]
        private float temp = 0.7f;

        public override Task<IText2AudioConext> Initialize()
        {
            return Task.FromResult((IText2AudioConext)default);
        }

        public async override Task<IAudioResponse> RequestAudioAsync(string prompt, IText2AudioConext conext)
        {
            var request = new Request();
            request.prompt = prompt;
            request.temp = this.temp;
            return await Utils.PostAsync<Request, Response>(this.url, request);
        }

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }
    }
}
