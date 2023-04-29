namespace AillieoUtils.AIGC.Implements
{
    using System;

    public partial class StableDiffusion
    {
        [Serializable]
        private class Request : IRequest
        {
            public string image_dimensions = "512x512";
            public int num_outputs = 1;
            public string prompt;
            public int steps = 20;
        }
    }
}
