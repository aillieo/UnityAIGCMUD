namespace AillieoUtils.AIGC.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;

    public class OpenAIChatGPT : Text2TextService
    {
        private class ChatConext : IText2TextConext
        {
            public ChatPrompt system;
            public LinkedList<ChatPrompt> conversation;
        }

        [Serializable]
        private class Request : IRequest
        {
            public string model;
            public ChatPrompt[] messages;
        }

        [Serializable]
        private class ChatPrompt
        {
            public string role;
            public string content;
        }

        [Serializable]
        private class Response : ITextResponse
        {
            [Serializable]
            public class ResponseChoice
            {
                public ChatPrompt message;
            }

            public ResponseChoice[] choices;

            public string GetDescription()
            {
                string rawText = choices[0].message.content;
                try
                {
                    return Utils.ExtractWithTag(rawText, "description")[0];
                }
                catch
                {
                    return rawText;
                }
            }

            public string[] GetChoices()
            {
                string rawText = choices[0].message.content;
                try
                {
                    string[] results = Utils.ExtractWithTag(rawText, "option");
                    if (results == null || results.Length == 0)
                    {
                        return new string[] { "继续" };
                    }
                    else
                    {
                        return results;
                    }
                }
                catch
                {
                    return new string[] { "继续" };
                }
            }

            public string GetImagePrompt()
            {
                string rawText = choices[0].message.content;
                try
                {
                    return Utils.ExtractWithTag(rawText, "keyword")[0];
                }
                catch
                {
                    return rawText;
                }
            }
        }

        [SerializeField]
        private string url = "https://api.openai.com/v1/chat/completions";

        [SerializeField]
        private string apiKey = "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        [SerializeField]
        private float temperature = 1f;

        [SerializeField]
        private string model = "gpt-3.5-turbo";

        [SerializeField][TextArea]
        private string systemPrompt;

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override Task<IText2TextConext> Initialize()
        {
            ChatConext chatConext = new ChatConext();
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
            ChatConext chatConext = context as ChatConext;

            ChatPrompt chat = new ChatPrompt() { content = prompt, role = "user" };
            chatConext.conversation.AddLast(chat);

            Request request = new Request();
            request.model = model;
            request.messages = chatConext.conversation.ToArray();
            Response response = await Utils.PostAsync<Request, Response>(this.url, request, new Dictionary<string, string>() { { "Authorization", $"Bearer {apiKey}" } });

            chatConext.conversation.AddLast(response.choices[0].message);
            return response;
        }
    }
}
