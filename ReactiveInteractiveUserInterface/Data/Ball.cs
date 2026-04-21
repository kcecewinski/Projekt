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
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      velocity = initialVelocity;
      thread = new Thread(Run) { IsBackground = true };
      thread.Start();
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity
    {
      get => velocity;
      set => velocity = new Vector(value.x, value.y);
    }

    #endregion IBall

    #region internal API

    internal void Stop()
    {
      running = false;
    }

    /// <summary>
    /// Moves the ball manually by the given delta vector.
    /// Available only in DEBUG — used exclusively by unit tests.
    /// In production, movement is driven by the internal thread via Run().
    /// </summary>
    [Conditional("DEBUG")]
    internal void Move(Vector delta)
    {
      MoveInternal(delta);
    }

    [Conditional("DEBUG")]
    internal void GetThreadId(Action<int> callback)
    {
      callback(thread.ManagedThreadId);
    }
    
    #endregion internal API

    #region private

    private Vector Position;
    private Vector velocity;
    private volatile bool running = true;
    private readonly Thread thread;
    private const int PeriodMs = 10; // stały okres pętli [ms]

    private void Run()
    {
      while (running)
      {
        Stopwatch sw = Stopwatch.StartNew();

        // przesunięcie = prędkość [px/s] * czas trwania okresu [s]
        double dt = PeriodMs / 1000.0;
        MoveInternal(new Vector(velocity.x * dt, velocity.y * dt));

        sw.Stop();
        int toWait = PeriodMs - (int)sw.ElapsedMilliseconds;
        if (toWait > 0)
          Thread.Sleep(toWait);
      }
    }

    private void MoveInternal(Vector delta)
    {
      Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      NewPositionNotification?.Invoke(this, Position);
    }

    #endregion private
  }
}