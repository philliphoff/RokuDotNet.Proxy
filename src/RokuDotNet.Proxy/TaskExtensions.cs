using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    internal static class TaskExtensions
    {
        public static Task<object> ToTaskObject(this Task task)
        {
            // TODO: Does this honor failures?
            return task.ContinueWith<object>(t => new object());
        }

        public static Task<object> ToTaskObject<T>(this Task<T> task)
        {
            return task.ContinueWith<object>(t => t.Result);
        }
    }
}