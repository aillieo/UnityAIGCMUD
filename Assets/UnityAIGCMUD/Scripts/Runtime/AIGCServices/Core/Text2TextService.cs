namespace AillieoUtils.AIGC
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class Text2TextService : AIGCService
    {
        public abstract Task<TextResponse> RequestTextAsync(string prompt, IEnumerable<string> history);
    }
}
