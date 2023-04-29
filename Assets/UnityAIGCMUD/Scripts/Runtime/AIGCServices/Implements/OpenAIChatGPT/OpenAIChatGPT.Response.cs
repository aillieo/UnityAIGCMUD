namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class OpenAIChatGPT
    {
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
                var rawText = this.choices[0].message.content;
                try
                {
                    var text = Utils.ExtractWithTag(rawText, "description")[0];
                    textProperty.Value = text;
                }
                catch
                {
                    textProperty.Value = rawText;
                }
            }

            public void GetChoices(ArrayProperty<string> arrayProperty)
            {
                var rawText = this.choices[0].message.content;
                try
                {
                    var results = Utils.ExtractWithTag(rawText, "option");
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
                var rawText = this.choices[0].message.content;
                try
                {
                    var imagePrompt = Utils.ExtractWithTag(rawText, "keyword")[0];
                    textProperty.Value = imagePrompt;
                }
                catch
                {
                    textProperty.Value = rawText;
                }
            }
        }
    }
}
