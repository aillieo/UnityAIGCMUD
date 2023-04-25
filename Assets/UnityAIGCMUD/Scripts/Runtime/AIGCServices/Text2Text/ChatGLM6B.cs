namespace AillieoUtils.AIGC.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class ChatGLM6B : Text2TextService
    {
        private class ChatConext : IText2TextConext
        {
            public List<string> history;
        }

        [SerializeField]
        private string url = "localhost:8000";

        [Serializable]
        private class Request : IRequest
        {
            public string prompt;
            public string[] history;
        }

        [Serializable]
        private class Response : ITextResponse
        {
            public string response;
            public string[] history;

            public string GetDescription()
            {
                return response;
            }

            public string[] GetChoices()
            {
                string rawText = this.response;
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
                return null;
            }
        }

        public override Task<bool> Validate()
        {
            return Task.FromResult(true);
        }

        public override async Task<ITextResponse> RequestTextAsync(string prompt, IText2TextConext context)
        {
            ChatConext chatConext = context as ChatConext;

            Request request = new Request()
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
            ChatConext chatConext = new ChatConext();
            chatConext.history = new List<string>();

            return Task.FromResult((IText2TextConext)chatConext);
        }
    }
}
