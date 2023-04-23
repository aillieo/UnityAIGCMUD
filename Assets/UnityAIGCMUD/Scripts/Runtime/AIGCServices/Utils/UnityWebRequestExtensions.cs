namespace AillieoUtils.AIGC
{
    using System;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class UnityWebRequestExtensions
    {
        public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest unityWebRequest)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            UnityWebRequestAsyncOperation operation = unityWebRequest.SendWebRequest();
            operation.completed += _ =>
            {
                if (unityWebRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(unityWebRequest.error);
                    tcs.SetException(new Exception(unityWebRequest.error));
                }
                else
                {
                    try
                    {
                        tcs.SetResult(unityWebRequest);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        tcs.SetException(e);
                    }
                }
            };

            return tcs.Task;
        }
    }
}
