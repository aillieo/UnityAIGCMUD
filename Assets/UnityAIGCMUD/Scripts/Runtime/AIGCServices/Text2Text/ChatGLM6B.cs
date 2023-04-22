namespace AillieoUtils.AIGC.Implements
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ChatGLM6B : Text2TextService
    {
        public override bool Validate(out string error)
        {
            throw new System.NotImplementedException();
        }

        public override Task<TextResponse> RequestTextAsync(string prompt, IEnumerable<string> history)
        {
            throw new System.NotImplementedException();
        }
    }
}
