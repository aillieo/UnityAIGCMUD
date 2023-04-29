namespace AillieoUtils.AIGC.Implements
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public partial class ChatGLM6B : Text2TextService
    {
        [SerializeField]
        private string url = "localhost:8000";

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override async Task<ITextResponse> RequestTextAsync(string prompt, IText2TextConext context)
        {
            var chatConext = context as ChatConext;

            var request = new Request()
            {
                prompt = prompt,
                history = chatConext.history.ToArray(),
            };

            Response response = await Utils.PostAsync<Request, Response>(this.url, request);
            chatConext.history.Clear();
            chatConext.history.AddRange(response.history);
            chatConext.history.Add(response.response);
            return response;
        }

        public override Task<IText2TextConext> Initialize()
        {
            var chatConext = new ChatConext();
            chatConext.history = new List<string>();

            return Task.FromResult((IText2TextConext)chatConext);
        }
    }
}
