namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class ChatGLM6B
    {
        [Serializable]
        private class Request : IRequest
        {
            public string prompt;
            public string[] history;
        }
    }
}
