namespace AillieoUtils.AIGC
{
    using UnityEngine;

    public abstract class AIGCService : ScriptableObject
    {
        public abstract bool Validate(out string error);
    }
}
