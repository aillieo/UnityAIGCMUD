namespace AillieoUtils.AIGC.Implements
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;

    public partial class OpenAIChatGPT : Text2TextService
    {
        [SerializeField]
        private string url = "https://api.openai.com/v1/chat/completions";

        [SerializeField]
        private string apiKey = "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        [SerializeField]
        [Range(0, 1)]
        private float temperature = 1f;

        [SerializeField]
        private string model = "gpt-3.5-turbo";

        [SerializeField]
        private bool stream = true;

        [SerializeField]
        [TextArea]
        private string systemPrompt;

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override Task<IText2TextConext> Initialize()
        {
            var chatConext = new ChatConext();
            chatConext.system = new ChatPrompt()
            {
                role = "system",
                content = this.systemPrompt,
            };

            chatConext.conversation = new LinkedList<ChatPrompt>();
            chatConext.conversation.AddLast(chatConext.system);

            return Task.FromResult((IText2TextConext)chatConext);
        }

        public override async Task<ITextResponse> RequestTextAsync(string prompt, IText2TextConext context)
        {
            var chatConext = context as ChatConext;

            var chat = new ChatPrompt() { content = prompt, role = "user" };
            chatConext.conversation.AddLast(chat);

            var request = new Request();
            request.stream = this.stream;
            request.model = this.model;
            request.messages = chatConext.conversation.ToArray();
            request.temperature = this.temperature;

            if (request.stream)
            {
                var response = new StreamResponse();
                Utils.StreamPostAsync(
                    this.url,
                    JsonUtility.ToJson(request),
                    response.Append,
                    new Dictionary<string, string>() { { "Authorization", $"Bearer {this.apiKey}" } })
                    .AwaitAndCheck();

                response.onComplete += () => chatConext.conversation.AddLast(response.AsHistoryChatPrompt());

                return response;
            }
            else
            {
                Response response = await Utils.PostAsync<Request, Response>(this.url, request, new Dictionary<string, string>() { { "Authorization", $"Bearer {this.apiKey}" } });

                chatConext.conversation.AddLast(response.choices[0].message);
                return response;
            }
        }
    }
}
