namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class OpenAIChatGPT
    {
        [Serializable]
        private class Request : IRequest
        {
            public bool stream;
            public string model;
            public ChatPrompt[] messages;
            public float temperature;
        }
    }
}
