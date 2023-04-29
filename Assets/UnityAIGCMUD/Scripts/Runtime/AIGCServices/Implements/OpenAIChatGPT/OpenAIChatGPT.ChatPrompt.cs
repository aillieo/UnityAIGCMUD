namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class OpenAIChatGPT
    {
        [Serializable]
        private class ChatPrompt
        {
            public string role;
            public string content;
        }
    }
}
