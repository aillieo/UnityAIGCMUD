namespace AillieoUtils.AIGC.Implements
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
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
            public bool stream;
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

            public void GetDescription(TextProperty textProperty)
            {
                string rawText = choices[0].message.content;
                try
                {
                    string text = Utils.ExtractWithTag(rawText, "description")[0];
                    textProperty.Value = text;
                }
                catch
                {
                    textProperty.Value = rawText;
                }
            }

            public void GetChoices(ArrayProperty<string> arrayProperty)
            {
                string rawText = choices[0].message.content;
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
                string rawText = choices[0].message.content;
                try
                {
                    string imagePrompt = Utils.ExtractWithTag(rawText, "keyword")[0];
                    textProperty.Value = imagePrompt;
                }
                catch
                {
                    textProperty.Value = rawText;
                }
            }
        }

        [Serializable]
        private class StreamResponse : ITextResponse
        {
            private enum TextBlock
            {
                Unknown,
                ImagePrompt,
                Description,
                Choices,
                End,
            }

            private static readonly string lineHeader = "data: ";
            private static readonly string endingFlag = "[DONE]";

            private TextProperty description = new TextProperty();
            private ArrayProperty<string> choices = new ArrayProperty<string>();
            private TextProperty imagePrompt = new TextProperty();

            private MemoryStream rawStream;
            private StreamReader rawStreamReader;
            private long rawStreamPosition;
            private string textBuffer;
            private TextBlock textBlock = TextBlock.Unknown;
            private StringBuilder debug = new StringBuilder();

            [Serializable]
            public class StreamResponseDelta
            {
                public StreamResponseChoice[] choices;
            }

            [Serializable]
            public class StreamResponseChoice
            {
                public ChatPrompt delta;
            }

            public void GetDescription(TextProperty textProperty)
            {
                textProperty.Value = description.Value;
                description.onValueChanged += () => textProperty.Value = description.Value;
            }

            public void GetChoices(ArrayProperty<string> arrayProperty)
            {
                string a;

                arrayProperty.Length = choices.Length;
                choices.onLengthChanged += () => arrayProperty.Length = choices.Length;

                for (int i = 0; i < arrayProperty.Length; ++i)
                {
                    arrayProperty[i] = choices[i];
                }

                choices.onValueChanged += index => arrayProperty[index] = choices[index];
            }

            public void GetImagePrompt(TextProperty textProperty)
            {
                textProperty.Value = imagePrompt.Value;
                imagePrompt.onValueChanged += () => textProperty.Value = imagePrompt.Value;
            }

            public void Append(byte[] bytes, int byteCount)
            {
                if (rawStream == null)
                {
                    rawStream = new MemoryStream();
                    rawStreamReader = new StreamReader(rawStream, Encoding.UTF8);
                }

                // net buffer -> rawStream
                rawStream.Write(bytes, 0, byteCount);

                rawStream.Position = rawStreamPosition;

                while (true)
                {
                    // rawStream -> textStream
                    string line = rawStreamReader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (line.StartsWith(lineHeader, StringComparison.Ordinal))
                    {
                        string trimmed = line.Substring(lineHeader.Length);
                        if (string.CompareOrdinal(trimmed, endingFlag) == 0)
                        {
                            debug.Append(trimmed);
                            Debug.Log(trimmed);
                            break;
                        }
                        else
                        {
                            StreamResponseDelta responseDelta = JsonUtility.FromJson<StreamResponseDelta>(trimmed);
                            string content = responseDelta.choices[0].delta.content;
                            if (!string.IsNullOrEmpty(content))
                            {
                                debug.Append(content);
                                Debug.Log(debug);

                                switch (this.textBlock)
                                {
                                    case TextBlock.Unknown:
                                    case TextBlock.ImagePrompt:
                                    case TextBlock.Description:
                                    case TextBlock.Choices:
                                        textBuffer += content;
                                        break;
                                    case TextBlock.End:
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(line);
                    }
                }

                rawStreamPosition = rawStream.Position;

                while (!string.IsNullOrEmpty(textBuffer))
                {
                    if (textBlock == TextBlock.Unknown)
                    {
                        if (Utils.FindTag(textBuffer, out string tag, out int index))
                        {
                            switch (tag)
                            {
                                case "description":
                                    textBlock = TextBlock.Description;
                                    break;
                                case "keyword":
                                    textBlock = TextBlock.ImagePrompt;
                                    break;
                                case "option":
                                    textBlock = TextBlock.Choices;
                                    this.choices.Add(null);
                                    break;
                            }

                            int start = index + tag.Length + 2; // <xxx>
                            textBuffer = textBuffer.Substring(start);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (textBlock != TextBlock.Unknown)
                    {
                        string content = textBuffer;

                        string tag = default;
                        switch (this.textBlock)
                        {
                            case TextBlock.ImagePrompt:
                                tag = "keyword";
                                break;
                            case TextBlock.Description:
                                tag = "description";
                                break;
                            case TextBlock.Choices:
                                tag = "option";
                                break;
                        }

                        if (Utils.MatchIncompleteClosingTag(textBuffer, tag))
                        {
                            break;
                        }

                        bool hasClosing = Utils.MatchClosingTag(textBuffer, tag, out int index);
                        if (hasClosing)
                        {
                            int start = index;
                            content = textBuffer.Substring(0, start);
                            int nextStart = index + tag.Length + 3; // </xxx>
                            textBuffer = textBuffer.Substring(nextStart);
                        }
                        else
                        {
                            textBuffer = string.Empty;
                        }

                        switch (this.textBlock)
                        {
                            case TextBlock.ImagePrompt:
                                this.imagePrompt.Value += content;
                                break;
                            case TextBlock.Description:
                                this.description.Value += content;
                                break;
                            case TextBlock.Choices:
                                string last = this.choices[this.choices.Length - 1];
                                last += content;
                                this.choices[this.choices.Length - 1] = last;
                                break;
                        }

                        if (hasClosing)
                        {
                            textBlock = TextBlock.Unknown;
                        }
                    }
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
            request.stream = this.stream;
            request.model = model;
            request.messages = chatConext.conversation.ToArray();

            if (request.stream)
            {
                StreamResponse response = new StreamResponse();
                Utils.StreamPostAsync(
                    this.url,
                    JsonUtility.ToJson(request),
                    response.Append,
                    new Dictionary<string, string>() { { "Authorization", $"Bearer {apiKey}" } })
                    .AwaitAndCheck();
                return response;
            }
            else
            {
                Response response = await Utils.PostAsync<Request, Response>(this.url, request, new Dictionary<string, string>() { { "Authorization", $"Bearer {apiKey}" } });

                chatConext.conversation.AddLast(response.choices[0].message);
                return response;
            }
        }
    }
}
