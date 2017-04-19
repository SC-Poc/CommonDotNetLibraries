using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Common.Extensions
{
	public static class TaskEx
	{
		public static T RunSync<T>(this Task<T> task)
		{
			try
			{
				return Task.Run(async () => await task.ConfigureAwait(false)).Result;
			}
			catch (AggregateException ex)
			{
				if (ex.InnerExceptions.Count == 1)
					ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
				throw;
			}
		}

		public static void RunSync(this Task task)
		{
			try
			{
				Task.Run(async () => await task.ConfigureAwait(false)).Wait();
			}
			catch (AggregateException ex)
			{
				if (ex.InnerExceptions.Count == 1)
					ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
				throw;
			}
		}

	    public static async Task WithTimeout(this Task task, int delayMs)
	    {
	        if (await Task.WhenAny(task, Task.Delay(delayMs)) == task)
	            return;
	        throw new TimeoutException();
	    }

	    public static async Task<T> WithTimeout<T>(this Task<T> task, int delayMs)
	    {
	        if (await Task.WhenAny(task, Task.Delay(delayMs)) == task)
	            return task.Result;
	        throw new TimeoutException();
	    }
	}
}
}
