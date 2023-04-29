namespace AillieoUtils.AIGC.Implements
{
    using System.Collections.Generic;

    public partial class OpenAIChatGPT
    {
        private class ChatConext : IText2TextConext
        {
            public ChatPrompt system;
            public LinkedList<ChatPrompt> conversation;
        }
    }
}
