namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class ChatGLM6B
    {
        [Serializable]
        private class Response : ITextResponse
        {
            public string response;
            public string[] history;

            public void GetDescription(TextProperty textProperty)
            {
                textProperty.Value = this.response;
            }

            public void GetChoices(ArrayProperty<string> arrayProperty)
            {
                var rawText = this.response;
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
            }
        }
    }
}
