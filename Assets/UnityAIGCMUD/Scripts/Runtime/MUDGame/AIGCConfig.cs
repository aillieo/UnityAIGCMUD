namespace AillieoUtils.MUD
{
    using AillieoUtils.AIGC;
    using UnityEngine;

    public class AIGCConfig : ScriptableObject
    {
        [SerializeField]
        private Text2ImageService text2ImageService;

        [SerializeField]
        private Text2TextService text2TextService;
    }
}
