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
  public abstract class BusinessLogicAbstractAPI : IDisposable
  {
    #region Layer Factory

    /// <summary>
    /// Returns the singleton instance. Two calls return the same object.
    /// </summary>
    public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
    {
      return modelInstance.Value;
    }

    /// <summary>
    /// Creates a brand-new BusinessLogicImplementation instance.
    /// Use this after Dispose() when you want to restart the simulation (e.g. Stop then Start).
    /// </summary>
    public static BusinessLogicAbstractAPI CreateNewBusinessLogicLayer()
    {
      return new BusinessLogicImplementation();
    }

    #endregion Layer Factory

    #region Layer API

    public static readonly Dimensions GetDimensions = new(20.0, 420.0, 400.0);

    public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #endregion Layer API

    #region private

    private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());

    #endregion private
  }

  /// <summary>
  /// Immutable type representing table dimensions
  /// </summary>
  /// <param name="BallDimension">Diameter of a ball in pixels</param>
  /// <param name="TableHeight">Height of the table in pixels</param>
  /// <param name="TableWidth">Width of the table in pixels</param>
  public record Dimensions(double BallDimension, double TableHeight, double TableWidth);

  public interface IPosition
  {
    double x { get; init; }
    double y { get; init; }
  }

  public interface IBall
  {
    event EventHandler<IPosition> NewPositionNotification;

    /// <summary>
    /// Current velocity of the ball in px/s
    /// </summary>
    IPosition Velocity { get; }
  }
}