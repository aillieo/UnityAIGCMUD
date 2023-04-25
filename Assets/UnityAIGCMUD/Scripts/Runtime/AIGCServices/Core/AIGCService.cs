namespace AillieoUtils.AIGC
{
    using System.Threading.Tasks;
    using UnityEngine;

    public abstract class AIGCService : ScriptableObject
    {
        public abstract Task<bool> Validate();
    }
}
