//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
  public class MainWindowViewModel : ViewModelBase, IDisposable
  {
    #region ctor

    public MainWindowViewModel() : this(null)
    { }

    internal MainWindowViewModel(ModelAbstractApi? modelLayerAPI)
    {
      ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
      Balls.CollectionChanged += OnBallsCollectionChanged;
      StartCommand = new RelayCommand(
        () => Start(NumberOfBalls),
        () => !Started
      );
      StopCommand = new RelayCommand(
        () => Stop(),
        () => Started
      );
    }

    #endregion ctor

    #region public API

    private int _numberOfBalls = 5;
    public int NumberOfBalls
    {
      get => _numberOfBalls;
      set { _numberOfBalls = value; RaisePropertyChanged(); }
    }

    public RelayCommand StartCommand { get; }

    public void Start(int numberOfBalls)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      ModelLayer.Start(numberOfBalls);
      Observer.Dispose();
      Started = true;
      StartCommand.RaiseCanExecuteChanged();
      StopCommand.RaiseCanExecuteChanged();
      RaisePropertyChanged(nameof(TotalEnergy));
      RaisePropertyChanged(nameof(TotalMomentum));
    }

    public RelayCommand StopCommand { get; }

    public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

    public double TotalEnergy =>
      Balls.Sum(b => 0.5 * (b.VelocityX * b.VelocityX + b.VelocityY * b.VelocityY));

    public double TotalMomentum =>
      Balls.Sum(b => Math.Sqrt(b.VelocityX * b.VelocityX + b.VelocityY * b.VelocityY));

    #endregion public API

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Balls.CollectionChanged -= OnBallsCollectionChanged;
          foreach (ModelIBall ball in Balls)
            ball.PropertyChanged -= OnBallPropertyChanged;
          Balls.Clear();
          Observer.Dispose();
          ModelLayer.Dispose();
        }
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null!;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;
    private bool Started = false;

    private void Stop()
    {
      Balls.CollectionChanged -= OnBallsCollectionChanged;
      foreach (ModelIBall ball in Balls)
        ball.PropertyChanged -= OnBallPropertyChanged;
      Balls.Clear();
      ModelLayer.Dispose();
      ModelLayer = ModelAbstractApi.CreateNewModel();
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
      Balls.CollectionChanged += OnBallsCollectionChanged;
      Started = false;
      StartCommand.RaiseCanExecuteChanged();
      StopCommand.RaiseCanExecuteChanged();
      RaisePropertyChanged(nameof(TotalEnergy));
      RaisePropertyChanged(nameof(TotalMomentum));
    }

    private void OnBallsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
        foreach (ModelIBall ball in e.NewItems)
          ball.PropertyChanged += OnBallPropertyChanged;

      if (e.OldItems != null)
        foreach (ModelIBall ball in e.OldItems)
          ball.PropertyChanged -= OnBallPropertyChanged;
    }

    private void OnBallPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      RaisePropertyChanged(nameof(TotalEnergy));
      RaisePropertyChanged(nameof(TotalMomentum));
    }

    #endregion private
  }
}