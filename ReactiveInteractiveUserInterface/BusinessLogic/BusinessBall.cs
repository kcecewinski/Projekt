//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    #region ctor

    public Ball(Data.IBall ball)
    {
      dataBall = ball;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    /// <summary>
    /// Current velocity of the ball in px/s — read from the underlying data layer
    /// </summary>
    public IPosition Velocity => new Position(dataBall.Velocity.x, dataBall.Velocity.y);

    #endregion IBall

    #region private

    private readonly Data.IBall dataBall;

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    #endregion private
  }
}