namespace Async
{
  using System;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;


  /// <summary>
  ///   <p>
  ///     When `SynchronizationContext.Current` is null, there is no guarantee that the continuation of `await`
  ///     will be executed on the same thread. Normally, this will only happen when the task that is being awaited
  ///     has completed before the `await` statement was invoked. Then the execution just continues on the same
  ///     thread without interruption.
  ///   </p>
  ///   <p>
  ///     There is a way to ensure that the continuation will execute on the same thread. In WinForms, for example,
  ///     the UI thread sets up its own synchronization context. In this example, we are going to do the same.
  ///   </p>
  /// </summary>
  public class Program
  {
    public static void Main()
    {
      //
      // This is the standard way to execute a task. `SynchronizationContext.Current` is null, which means that the
      // continuation will use the same TaskScheduler, meaning that a thread pool will be used and continuation
      // will finish on another thread.

      Console.WriteLine("No synchronization context");
      Console.WriteLine("--------------------------");

      GetPageSourceAsync().Wait();

      //
      // `AsyncPump` is a simple class that will temporarily set up its own synchronization context - go ahead and
      // have a look at its implementation. It ensures that the continuation of `await` executes on the same thread.

      Console.WriteLine();
      Console.WriteLine("Single-threaded synchronization context");
      Console.WriteLine("---------------------------------------");

      AsyncPump.Run(async () => { await GetPageSourceAsync(); });
    }    public static async Task<string> GetPageSourceAsync()
    {
      using (var client = new HttpClient())
      {
        Console.WriteLine("Current thread ID: " + Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine("Executing an asynchronus operation...");

        Task<string> task = client.GetStringAsync("http://google.com");
        string result = await task;

        Console.WriteLine("Current thread ID: " + Thread.CurrentThread.ManagedThreadId);

        return result;
      }
    }
  }
}
