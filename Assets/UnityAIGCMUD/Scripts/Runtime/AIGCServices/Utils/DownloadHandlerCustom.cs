namespace AillieoUtils.AIGC
{
    using System;
    using UnityEngine.Networking;

    public class DownloadHandlerCustom : DownloadHandlerScript
    {
        private Action<byte[], int> onDataReceived;
        private Action onDownloadCompleted;

        public ulong totalLength { get; private set; }

        public ulong receivedLength { get; private set; }

        public DownloadHandlerCustom(Action<byte[], int> onDataReceived, Action onDownloadCompleted = null)
            : base()
        {
            this.onDataReceived = onDataReceived;
            this.onDownloadCompleted = onDownloadCompleted;
        }

        public DownloadHandlerCustom(byte[] preallocatedBuffer, Action<byte[], int> onDataReceived, Action onDownloadCompleted = null)
            : base(preallocatedBuffer)
        {
            this.onDataReceived = onDataReceived;
            this.onDownloadCompleted = onDownloadCompleted;
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            totalLength = contentLength;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (dataLength <= 0)
            {
                return false;
            }

            onDataReceived?.Invoke(data, dataLength);

            receivedLength += (ulong)dataLength;

            return true;
        }

        protected override void CompleteContent()
        {
            onDownloadCompleted?.Invoke();
            base.CompleteContent();
        }
    }
}
