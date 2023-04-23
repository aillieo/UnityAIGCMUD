namespace AillieoUtils.AIGC
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class Utils
    {
        public static async Task<string> PostAsync(string url, string jsonData)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonData);
            var unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST, new DownloadHandlerBuffer(), new UploadHandlerRaw(bytes));
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            await unityWebRequest.SendWebRequestAsync();
            return unityWebRequest.downloadHandler.text;
        }

        public static Texture2D BytesToTexture2D(byte[] bytes)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.LoadImage(bytes);
            texture.Apply();
            return texture;
        }

        public static Texture2D LoadImageFromBase64(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return BytesToTexture2D(bytes);
        }

        public static async Task<Texture2D> LoadImageFromURL(string url, int timeout = 60)
        {
            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url);
            unityWebRequest.timeout = timeout;
            await unityWebRequest.SendWebRequestAsync();
            var downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
            return downloadHandlerTexture.texture;
        }
    }
}
