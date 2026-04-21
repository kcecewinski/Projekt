//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class DataImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        IEnumerable<IBall>? ballsList = null;
        newInstance.CheckBallsList(x => ballsList = x);
        Assert.IsNotNull(ballsList);
        int numberOfBalls = 0;
        newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
        Assert.AreEqual<int>(0, numberOfBalls);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataImplementation newInstance = new DataImplementation();
      bool newInstanceDisposed = false;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      IEnumerable<IBall>? ballsList = null;
      newInstance.CheckBallsList(x => ballsList = x);
      Assert.IsNotNull(ballsList);
      newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(0, x));
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
    }

    [TestMethod]
    public void StartTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        int numberOfCallbackInvoked = 0;
        int numberOfBalls2Create = 10;
        newInstance.Start(
          numberOfBalls2Create,
          (startingPosition, ball) =>
          {
            numberOfCallbackInvoked++;
            Assert.IsTrue(startingPosition.x >= 0);
            Assert.IsTrue(startingPosition.y >= 0);
            Assert.IsNotNull(ball);
          });
        Assert.AreEqual<int>(numberOfBalls2Create, numberOfCallbackInvoked);
        newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(10, x));
      }
    }

    [TestMethod]
    public void VelocityIsNotIntegerTest()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        List<IBall> balls = new List<IBall>();
        newInstance.Start(10, (pos, ball) => balls.Add(ball));

        foreach (IBall ball in balls)
        {
          double vx = ball.Velocity.x;
          double vy = ball.Velocity.y;
          Assert.AreNotEqual(Math.Round(vx), vx, 0.0001,
            $"VelocityX={vx} nie powinna być wartością całkowitą.");
          Assert.AreNotEqual(Math.Round(vy), vy, 0.0001,
            $"VelocityY={vy} nie powinna być wartością całkowitą.");
        }
      }
    }

    /// <summary>
    /// Sprawdza że kulka odbija się od lewej ściany — vx zmienia znak na dodatni.
    /// </summary>
    [TestMethod]
    public void LeftWallBounceTest()
    {
      Ball ball = new Ball(new Vector(1.0, 200.0), new Vector(-100.0, 0.0));
      IVector pos = new Vector(1.0, 200.0);
      ball.NewPositionNotification += (_, p) => pos = p;

      ball.Move(new Vector(ball.Velocity.x * 0.1, 0));

      double newVx = ball.Velocity.x;
      if (pos.x < 0) newVx = Math.Abs(newVx);
      ball.Velocity = new Vector(newVx, ball.Velocity.y);

      Assert.IsTrue(ball.Velocity.x > 0, "Kulka powinna odbić się od lewej ściany — vx dodatnie.");
    }

    /// <summary>
    /// Sprawdza że kulka odbija się od prawej ściany — vx zmienia znak na ujemny.
    /// </summary>
    [TestMethod]
    public void RightWallBounceTest()
    {
      double rightEdge = DataImplementation.TableWidth - DataImplementation.BallDiameter;
      Ball ball = new Ball(new Vector(rightEdge - 1.0, 200.0), new Vector(100.0, 0.0));
      IVector pos = new Vector(rightEdge - 1.0, 200.0);
      ball.NewPositionNotification += (_, p) => pos = p;

      ball.Move(new Vector(ball.Velocity.x * 0.1, 0));

      double newVx = ball.Velocity.x;
      if (pos.x > DataImplementation.TableWidth - DataImplementation.BallDiameter) newVx = -Math.Abs(newVx);
      ball.Velocity = new Vector(newVx, ball.Velocity.y);

      Assert.IsTrue(ball.Velocity.x < 0, "Kulka powinna odbić się od prawej ściany — vx ujemne.");
    }

    /// <summary>
    /// Sprawdza że kulka odbija się od górnej ściany — vy zmienia znak na dodatni.
    /// </summary>
    [TestMethod]
    public void TopWallBounceTest()
    {
      Ball ball = new Ball(new Vector(200.0, 1.0), new Vector(0.0, -100.0));
      IVector pos = new Vector(200.0, 1.0);
      ball.NewPositionNotification += (_, p) => pos = p;

      ball.Move(new Vector(0, ball.Velocity.y * 0.1));

      double newVy = ball.Velocity.y;
      if (pos.y < 0) newVy = Math.Abs(newVy);
      ball.Velocity = new Vector(ball.Velocity.x, newVy);

      Assert.IsTrue(ball.Velocity.y > 0, "Kulka powinna odbić się od górnej ściany — vy dodatnie.");
    }

    /// <summary>
    /// Sprawdza że kulka odbija się od dolnej ściany — vy zmienia znak na ujemny.
    /// </summary>
    [TestMethod]
    public void BottomWallBounceTest()
    {
      double bottomEdge = DataImplementation.TableHeight - DataImplementation.BallDiameter;
      Ball ball = new Ball(new Vector(200.0, bottomEdge - 1.0), new Vector(0.0, 100.0));
      IVector pos = new Vector(200.0, bottomEdge - 1.0);
      ball.NewPositionNotification += (_, p) => pos = p;

      ball.Move(new Vector(0, ball.Velocity.y * 0.1));

      double newVy = ball.Velocity.y;
      if (pos.y > DataImplementation.TableHeight - DataImplementation.BallDiameter) newVy = -Math.Abs(newVy);
      ball.Velocity = new Vector(ball.Velocity.x, newVy);

      Assert.IsTrue(ball.Velocity.y < 0, "Kulka powinna odbić się od dolnej ściany — vy ujemne.");
    }

    /// <summary>
    /// Każda kulka musi mieć swój własny wątek (unikalny ManagedThreadId).
    /// </summary>
    [TestMethod]
    public void EachBallHasOwnThreadTest()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        int numberOfBalls = 5;
        newInstance.Start(numberOfBalls, (pos, ball) => { });

        Thread.Sleep(50);

        List<int> threadIds = new List<int>();
        newInstance.CheckBallThreadIds(ids => threadIds.AddRange(ids));

        Assert.AreEqual(numberOfBalls, threadIds.Count, "Każda kulka powinna mieć wątek.");
        int uniqueCount = threadIds.Distinct().Count();
        Assert.AreEqual(numberOfBalls, uniqueCount, "Każda kulka musi mieć swój własny wątek.");
      }
    }

    /// <summary>
    /// Energia kinetyczna (suma v^2) musi być stała — odbicia od ściany nie zmieniają |v|.
    /// </summary>
    [TestMethod]
    public void EnergyConservationTest()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        List<IBall> balls = new List<IBall>();
        newInstance.Start(3, (pos, ball) => balls.Add(ball));

        double energyAtStart = balls.Sum(b => b.Velocity.x * b.Velocity.x + b.Velocity.y * b.Velocity.y);

        Thread.Sleep(200);

        double energyAfter = balls.Sum(b => b.Velocity.x * b.Velocity.x + b.Velocity.y * b.Velocity.y);

        Assert.AreEqual(energyAtStart, energyAfter, 0.001,
          "Suma energii kinetycznej powinna być stała — odbicia nie zmieniają |v|.");
      }
    }
  }
}