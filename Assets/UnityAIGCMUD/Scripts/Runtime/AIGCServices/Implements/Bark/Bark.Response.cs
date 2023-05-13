namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class Bark
    {
        [Serializable]
        private class Response : IAudioResponse
        {
            public string audio;

            public void GetAudio(AudioClipProperty audioClipProperty)
            {
                var bytes = Convert.FromBase64String(this.audio);
                audioClipProperty.Value = Utils.BytesToAudioClip(bytes);
            }
        }
    }
}
