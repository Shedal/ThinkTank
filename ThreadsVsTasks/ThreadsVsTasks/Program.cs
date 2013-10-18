namespace ThreadsVsTasks
{
  using System;
  using System.Diagnostics;
  using System.Diagnostics.CodeAnalysis;
  using System.Threading;
  using System.Threading.Tasks;



  [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
  [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:SingleLineCommentMustBePrecededByBlankLine", Justification = "Reviewed. Suppression is OK here."), SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1005:SingleLineCommentsMustBeginWithSingleSpace", Justification = "Reviewed. Suppression is OK here.")]
  public class Program
  {
    private static double staticData;

    public static void Main()
    {
      // You can restrict the system to use only one processor (IntPtr(1)), or two (IntPtr(3)), or three (IntPtr(7))... yeah, you get the point.
      // Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(3);

      Tree tree = Tree.Create(depth: 9);

      var stopwatch = new Stopwatch();
      stopwatch.Start();

      //WalkTreeSequentially(tree);
      //WalkTreeMultithreaded(tree);
      //WalkTreeWithTasks(tree);
      //WalkTreeViaThreadPool(tree);

      Console.WriteLine(stopwatch.ElapsedMilliseconds);
    }



    private static void ProcessData(object data)
    {
      for (int i = 0; i < 200000; i++)
      {
        staticData = Math.Sin(i);
      }
    }



    private static void WalkTreeSequentially(Tree tree)
    {
      if (tree == null)
      {
        return;
      }

      WalkTreeSequentially(tree.Left);
      WalkTreeSequentially(tree.Right);

      ProcessData(tree.Data);
    }



    private static void WalkTreeMultithreaded(Tree tree)
    {
      if (tree == null)
      {
        return;
      }

      var leftThread = new Thread(o => WalkTreeMultithreaded(tree.Left));
      leftThread.Start();

      var rightThread = new Thread(o => WalkTreeMultithreaded(tree.Right));
      rightThread.Start();

      leftThread.Join();
      rightThread.Join();

      ProcessData(tree.Data);
    }



    private static void WalkTreeWithTasks(Tree tree)
    {
      if (tree == null)
      {
        return;
      }

      var leftTask = new Task(() => WalkTreeWithTasks(tree.Left));
      leftTask.Start();

      var rightTask = new Task(() => WalkTreeWithTasks(tree.Right));
      rightTask.Start();

      leftTask.Wait();
      rightTask.Wait();

      ProcessData(tree.Data);
    }



    private static long activeTasks = 0;
    private static ManualResetEvent finished = new ManualResetEvent(false);

    private static void WalkTreeViaThreadPool(Tree tree)
    {
      activeTasks++;
      WalkTreeViaThreadPoolRecursive(tree);

      if (Interlocked.Decrement(ref activeTasks) > 0)
      {
        finished.WaitOne();
      }

      finished.Dispose();
    }

    private static void WalkTreeViaThreadPoolRecursive(Tree tree)
    {
      try
      {
        if (tree == null)
        {
          return;
        }

        Interlocked.Increment(ref activeTasks);
        ThreadPool.QueueUserWorkItem(x => WalkTreeViaThreadPoolRecursive(tree.Left));

        Interlocked.Increment(ref activeTasks);
        ThreadPool.QueueUserWorkItem(x => WalkTreeViaThreadPoolRecursive(tree.Right));

        ProcessData(tree.Data);
      }
      finally
      {
        if (Interlocked.Decrement(ref activeTasks) == 0)
        {
          finished.Set();
        }
      }
    }
  }
}
