namespace AillieoUtils.AIGC
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class Utils
    {
        public static async Task<string> PostAsync(string url, string jsonData, IDictionary<string, string> headers = null)
        {
            Debug.Log(jsonData);
            var bytes = Encoding.UTF8.GetBytes(jsonData);
            var unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST, new DownloadHandlerBuffer(), new UploadHandlerRaw(bytes));
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    unityWebRequest.SetRequestHeader(pair.Key, pair.Value);
                }
            }

            await unityWebRequest.SendWebRequestAsync();
            string result = unityWebRequest.downloadHandler.text;
            Debug.Log(result);
            return result;
        }

        public static async Task<TRes> PostAsync<TReq, TRes>(string url, TReq reqest, IDictionary<string, string> headers = null)
        {
            string jsonData = JsonUtility.ToJson(reqest);
            string result = await PostAsync(url, jsonData, headers);
            return JsonUtility.FromJson<TRes>(result);
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

        public static string[] ExtractWithTag(string rawText, string tag)
        {
            string pattern = $"<{tag}>(.*?)<\\/{tag}>";
            MatchCollection matches = Regex.Matches(rawText, pattern);

            string[] result = new string[matches.Count];
            for (int i = 0; i < matches.Count; ++i)
            {
                result[i] = matches[i].Groups[1].Value;
            }

            return result;
        }
    }
}
