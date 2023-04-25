namespace AillieoUtils.MUD
{
    using UnityEngine;

    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private GameConfig config;

        [SerializeField]
        private GameObject mainView;

        private async void Start()
        {
            await MUDGameManager.instance.Initialize(this.config);
            Instantiate(this.mainView);
            MUDGameManager.instance.RequestGameStart();
            Destroy(this.gameObject);
        }
    }
}
