//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    { }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      for (int i = 0; i < numberOfBalls; i++)
      {
        double x = BallDiameter + RandomGenerator.NextDouble() * (TableWidth - 2 * BallDiameter);
        double y = BallDiameter + RandomGenerator.NextDouble() * (TableHeight - 2 * BallDiameter);

        double vx = (RandomGenerator.NextDouble() * 2 - 1) * 150 + 0.5;
        double vy = (RandomGenerator.NextDouble() * 2 - 1) * 150 + 0.7;
        if (Math.Abs(vx) < 20) vx = vx < 0 ? -(20.5 + RandomGenerator.NextDouble() * 5) : (20.5 + RandomGenerator.NextDouble() * 5);
        if (Math.Abs(vy) < 20) vy = vy < 0 ? -(20.5 + RandomGenerator.NextDouble() * 5) : (20.5 + RandomGenerator.NextDouble() * 5);

        Vector startPos = new Vector(x, y);
        Vector startVel = new Vector(vx, vy);

        Ball newBall = new Ball(startPos, startVel);
        newBall.NewPositionNotification += (sender, pos) => Bounce((Ball)sender!, pos);
        upperLayerHandler(startPos, newBall);
        BallsList.Add(newBall);
      }
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          foreach (Ball ball in BallsList)
            ball.Stop();
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private bool Disposed = false;
    private readonly Random RandomGenerator = new();
    private readonly List<Ball> BallsList = [];

    internal const double TableWidth = 400.0;
    internal const double TableHeight = 420.0;
    internal const double BallDiameter = 20.0;

    private void Bounce(Ball ball, IVector pos)
    {
      double newVx = ball.Velocity.x;
      double newVy = ball.Velocity.y;
      double newX = pos.x;
      double newY = pos.y;

      if (pos.x < 0) { newX = 0; newVx = Math.Abs(newVx); }
      else if (pos.x > TableWidth - BallDiameter) { newX = TableWidth - BallDiameter; newVx = -Math.Abs(newVx); }

      if (pos.y < 0) { newY = 0; newVy = Math.Abs(newVy); }
      else if (pos.y > TableHeight - BallDiameter) { newY = TableHeight - BallDiameter; newVy = -Math.Abs(newVy); }

      if (newVx != ball.Velocity.x || newVy != ball.Velocity.y)
        ball.Velocity = new Vector(newVx, newVy);
    }

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    [Conditional("DEBUG")]
    internal void CheckBallThreadIds(Action<IEnumerable<int>> returnThreadIds)
    {
      List<int> ids = new List<int>();
      foreach (Ball ball in BallsList)
        ball.GetThreadId(id => ids.Add(id));
      returnThreadIds(ids);
    }

    #endregion TestingInfrastructure
  }
}