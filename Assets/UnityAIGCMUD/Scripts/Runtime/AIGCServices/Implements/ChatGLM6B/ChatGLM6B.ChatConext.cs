namespace AillieoUtils.AIGC.Implements
{
    using System.Collections.Generic;

    public partial class ChatGLM6B
    {
        private class ChatConext : IText2TextConext
        {
            public List<string> history;
        }
    }
}
