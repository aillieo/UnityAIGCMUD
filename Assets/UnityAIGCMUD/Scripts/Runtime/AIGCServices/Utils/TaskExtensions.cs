namespace AillieoUtils.AIGC
{
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static async void AwaitAndCheck(this Task task)
        {
            await task;
        }

        public static async void AwaitAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
        }
    }
}
