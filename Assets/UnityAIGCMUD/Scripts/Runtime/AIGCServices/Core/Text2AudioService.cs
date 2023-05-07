namespace AillieoUtils.AIGC
{
    using System.Threading.Tasks;

    public abstract class Text2AudioService : AIGCService
    {
        public abstract Task<IText2AudioConext> Initialize();

        public abstract Task<IAudioResponse> RequestAudioAsync(string prompt, IText2AudioConext conext);
    }
}
