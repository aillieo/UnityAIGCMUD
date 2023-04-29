namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class OpenAIDALLE
    {
        [Serializable]
        private class Request : IRequest
        {
            public string response_format = "b64_json";
            public string size = "512x512";
            public int n = 1;
            public string prompt;
        }
    }
}
