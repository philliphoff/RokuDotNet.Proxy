using System.Threading.Tasks;

namespace RokuDotNet.Proxy
{
    internal static class TaskExtensions
    {
        public static Task<object> ToTaskObject<T>(this Task<T> task)
        {
            return task.ContinueWith<object>(t => t.Result);
        }
    }
}