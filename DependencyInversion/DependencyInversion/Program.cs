namespace DependencyInversion
{
  using System;
  using System.Collections.Generic;


  public class Program
  {
    public static void Main()
    {
      // Instead of creating all the musicians inside the Orchestra, we create them in the composition root and pass
      // them into the Orchestra as dependency.
      // Composition points are also called seams.

      var musicians = new IMusician[]
      {
        new Drummer(),
        new Pianist(),
      };

      var orchestra = new Orchestra(musicians);
      orchestra.Perform();
    }
  }



  public class Orchestra
  {
    private readonly IEnumerable<IMusician> musicians;

    public Orchestra(IEnumerable<IMusician> musicians)
    {
      this.musicians = musicians;
    }

    public void Perform()
    {
      foreach (var musician in musicians)
      {
        musician.Play();
      }
    }
  }



  public interface IMusician
  {
    void Play();
  }


  public class Pianist : IMusician
  {
    public void Play()
    {
      Console.WriteLine("Do Re Mi Fa Sol");
    }
  }


  public class Drummer : IMusician
  {
    public void Play()
    {
      Console.WriteLine("Ba DUM tssss");
    }
  }
}
