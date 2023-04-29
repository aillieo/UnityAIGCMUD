namespace AillieoUtils.AIGC.Implements
{
    using System;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public partial class OpenAIChatGPT
    {
        [Serializable]
        private class StreamResponse : ITextResponse
        {
            private static readonly string lineHeader = "data: ";
            private static readonly string endingFlag = "[DONE]";

            private TextProperty description = new TextProperty();
            private ArrayProperty<string> choices = new ArrayProperty<string>();
            private TextProperty imagePrompt = new TextProperty();
            private StringBuilder imagePromptBuilder = new StringBuilder();

            private MemoryStream rawStream;
            private StreamReader rawStreamReader;
            private long rawStreamPosition;
            private string textBuffer;
            private TextBlock textBlock = TextBlock.Unknown;
            private StringBuilder debug = new StringBuilder();
            private bool endingFlagReceived = false;

            internal event Action onComplete;

            private enum TextBlock
            {
                Unknown,
                ImagePrompt,
                Description,
                Choices,
                End,
            }

            public void GetDescription(TextProperty textProperty)
            {
                textProperty.Value = this.description.Value;
                this.description.onValueChanged += () => textProperty.Value = this.description.Value;
            }

            public void GetChoices(ArrayProperty<string> arrayProperty)
            {
                arrayProperty.Length = this.choices.Length;
                this.choices.onLengthChanged += () => arrayProperty.Length = this.choices.Length;

                for (var i = 0; i < arrayProperty.Length; ++i)
                {
                    arrayProperty[i] = this.choices[i];
                }

                this.choices.onValueChanged += index => arrayProperty[index] = this.choices[index];
            }

            public void GetImagePrompt(TextProperty textProperty)
            {
                textProperty.Value = this.imagePrompt.Value;
                this.imagePrompt.onValueChanged += () => textProperty.Value = this.imagePrompt.Value;
            }

            public void Append(byte[] bytes, int byteCount)
            {
                if (this.rawStream == null)
                {
                    this.rawStream = new MemoryStream();
                    this.rawStreamReader = new StreamReader(this.rawStream, Encoding.UTF8);
                }

                // net buffer -> rawStream
                this.rawStream.Write(bytes, 0, byteCount);

                this.rawStream.Position = this.rawStreamPosition;

                while (true)
                {
                    // rawStream -> textStream
                    var line = this.rawStreamReader.ReadLine();

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
                        var trimmed = line.Substring(lineHeader.Length);
                        if (string.CompareOrdinal(trimmed, endingFlag) == 0)
                        {
                            this.endingFlagReceived = true;
                            break;
                        }
                        else
                        {
                            StreamResponseDelta responseDelta = JsonUtility.FromJson<StreamResponseDelta>(trimmed);
                            var content = responseDelta.choices[0].delta.content;
                            if (!string.IsNullOrEmpty(content))
                            {
                                this.debug.Append(content);

                                switch (this.textBlock)
                                {
                                    case TextBlock.Unknown:
                                    case TextBlock.ImagePrompt:
                                    case TextBlock.Description:
                                    case TextBlock.Choices:
                                        this.textBuffer += content;
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

                this.rawStreamPosition = this.rawStream.Position;

                while (!string.IsNullOrEmpty(this.textBuffer))
                {
                    if (this.textBlock == TextBlock.Unknown)
                    {
                        if (Utils.FindTag(this.textBuffer, out var tag, out var index))
                        {
                            switch (tag)
                            {
                                case "description":
                                    this.textBlock = TextBlock.Description;
                                    break;
                                case "keyword":
                                    this.textBlock = TextBlock.ImagePrompt;
                                    break;
                                case "option":
                                    this.textBlock = TextBlock.Choices;
                                    this.choices.Add(null);
                                    break;
                            }

                            var start = index + tag.Length + 2; // <xxx>
                            this.textBuffer = this.textBuffer.Substring(start);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (this.textBlock != TextBlock.Unknown)
                    {
                        var content = this.textBuffer;

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

                        if (Utils.MatchIncompleteClosingTag(this.textBuffer, tag))
                        {
                            break;
                        }

                        var hasClosing = Utils.MatchClosingTag(this.textBuffer, tag, out var index);
                        if (hasClosing)
                        {
                            var start = index;
                            content = this.textBuffer.Substring(0, start);
                            var nextStart = index + tag.Length + 3; // </xxx>
                            this.textBuffer = this.textBuffer.Substring(nextStart);
                        }
                        else
                        {
                            this.textBuffer = string.Empty;
                        }

                        switch (this.textBlock)
                        {
                            case TextBlock.ImagePrompt:
                                this.imagePromptBuilder.Append(content);
                                break;
                            case TextBlock.Description:
                                this.description.Value += content;
                                break;
                            case TextBlock.Choices:
                                var last = this.choices[this.choices.Length - 1];
                                last += content;
                                this.choices[this.choices.Length - 1] = last;
                                break;
                        }

                        if (hasClosing)
                        {
                            if (this.textBlock == TextBlock.ImagePrompt)
                            {
                                this.imagePrompt.Value = this.imagePromptBuilder.ToString();
                            }

                            this.textBlock = TextBlock.Unknown;
                        }
                    }
                }

                if (this.endingFlagReceived)
                {
                    Debug.Log(this.debug);
                    this.onComplete?.Invoke();
                }
            }

            public ChatPrompt AsHistoryChatPrompt()
            {
                var chatPrompt = new ChatPrompt();
                chatPrompt.role = "assistant";
                chatPrompt.content = this.description.Value;
                return chatPrompt;
            }

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
        }
    }
}
