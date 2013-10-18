namespace ThreadsVsTasks
{
  internal class Tree
  {
    public Tree Left { get; set; }

    public Tree Right { get; set; }

    public object Data { get; set; }



    public static Tree Create(int depth)
    {
      if (depth == 0)
      {
        return new Tree();
      }

      return new Tree
      {
        Left = Create(depth - 1),
        Right = Create(depth - 1)
      };
    }
  }
}
