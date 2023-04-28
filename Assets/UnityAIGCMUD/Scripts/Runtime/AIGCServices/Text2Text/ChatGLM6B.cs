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

            public void GetDescription(TextProperty textProperty)
            {
                textProperty.Value = response;
            }

            public void GetChoices(ArrayProperty<string> arrayProperty)
            {
                string rawText = this.response;
                try
                {
                    string[] results = Utils.ExtractWithTag(rawText, "option");
                    if (results == null || results.Length == 0)
                    {
                        arrayProperty.Clear().Add(null);
                    }
                    else
                    {
                        foreach (var r in results)
                        {
                            arrayProperty.Add(r);
                        }
                    }
                }
                catch
                {
                    arrayProperty.Clear().Add(null);
                }
            }

            public void GetImagePrompt(TextProperty textProperty)
            {
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
