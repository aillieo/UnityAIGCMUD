namespace AillieoUtils.AIGC
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class Utils
    {
        public static async Task<string> PostAsync(string url, string jsonData, IReadOnlyDictionary<string, string> headers = null)
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
            var result = unityWebRequest.downloadHandler.text;
            Debug.Log(result);
            return result;
        }

        public static async Task<TRes> PostAsync<TReq, TRes>(string url, TReq reqest, IReadOnlyDictionary<string, string> headers = null)
        {
            var jsonData = JsonUtility.ToJson(reqest);
            var result = await PostAsync(url, jsonData, headers);
            return JsonUtility.FromJson<TRes>(result);
        }

        public static Task StreamPostAsync(string url, string jsonData, Action<byte[], int> onDataReceived, IReadOnlyDictionary<string, string> headers = null)
        {
            Debug.Log(jsonData);
            var bytes = Encoding.UTF8.GetBytes(jsonData);
            var unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST, new DownloadHandlerCustom(onDataReceived), new UploadHandlerRaw(bytes));
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    unityWebRequest.SetRequestHeader(pair.Key, pair.Value);
                }
            }

            return unityWebRequest.SendWebRequestAsync();
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
            var pattern = $"<{tag}>(.*?)<\\/{tag}>";
            MatchCollection matches = Regex.Matches(rawText, pattern);

            var result = new string[matches.Count];
            for (var i = 0; i < matches.Count; ++i)
            {
                result[i] = matches[i].Groups[1].Value;
            }

            return result;
        }

        public static bool FindTag(string rawText, out string tag, out int index)
        {
            var pattern = "<(\\w+)>";
            Match match = Regex.Match(rawText, pattern);
            if (match.Success)
            {
                tag = match.Groups[1].Value;
                index = match.Groups[0].Index;
                return true;
            }

            tag = default;
            index = default;
            return false;
        }

        public static bool MatchClosingTag(string rawText, string tag, out int index)
        {
            var pattern = $"</{tag}>";
            Match match = Regex.Match(rawText, pattern);
            if (match.Success)
            {
                index = match.Groups[0].Index;
                return true;
            }

            index = default;
            return false;
        }

        public static bool MatchIncompleteClosingTag(string rawText, string tag)
        {
            var index = rawText.LastIndexOf("</", StringComparison.Ordinal);
            if (index < 0)
            {
                return false;
            }

            var potentialTagLength = rawText.Length - index - 3;

            if (potentialTagLength + 1 > tag.Length)
            {
                return false;
            }

            var length = Math.Min(potentialTagLength, tag.Length);

            for (var i = 0; i < length; ++i)
            {
                if (rawText[index + 2 + i] != tag[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static string TrimIncompleteUtf8Chars(string rawString)
        {
            var length = rawString.Length;
            while (length > 0 && IsIncompleteUtf8Char(rawString[length - 1]))
            {
                length--;
            }

            return rawString.Substring(0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsIncompleteUtf8Char(char c)
        {
            if ((c & 0b10000000) == 0b00000000)
            {
                return false;
            }

            if ((c & 0b11100000) == 0b11000000)
            {
                return true;
            }

            if ((c & 0b11110000) == 0b11100000)
            {
                return true;
            }

            if ((c & 0b11111000) == 0b11110000)
            {
                return true;
            }

            return false;
        }

        public static AudioClip BytesToAudioClip(byte[] bytes)
        {
            int numChannels = BitConverter.ToInt16(bytes, 22);
            var sampleRate = BitConverter.ToInt32(bytes, 24);
            var byteRate = BitConverter.ToInt32(bytes, 28);
            int blockAlign = BitConverter.ToInt16(bytes, 32);
            int bitsPerSample = BitConverter.ToInt16(bytes, 34);

            var headLength = 44;
            var clip = AudioClip.Create("WAV", bytes.Length - headLength, numChannels, sampleRate, false);

            var audioSamples = new float[(bytes.Length - headLength) / 2];
            for (var i = headLength; i < audioSamples.Length; i++)
            {
                // two bytes => one short => one float
                var sample = (short)((bytes[(i * 2) + 1] << 8) | bytes[i * 2]);
                audioSamples[i] = (float)sample / short.MaxValue;
            }

            clip.SetData(audioSamples, 0);

            return clip;
        }
    }
}
