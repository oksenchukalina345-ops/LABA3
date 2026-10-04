using LABA3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace LABA3.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private double _h1 = 1.0;
        private double _h2 = 0.1;
        private bool _showLinear = true;
        private bool _showHermite = true;
        private bool _showExact = true;
        private string _errorMessage = string.Empty;

        public PointCollection OriginalPoints { get; private set; } = new();
        public PointCollection LinearPoints { get; private set; } = new();
        public PointCollection HermitePoints { get; private set; } = new();
        public PointCollection ExactPoints { get; private set; } = new();
        public PointCollection ErrorPoints { get; private set; } = new();

        public double H1
        {
            get => _h1;
            set { _h1 = value; OnPropertyChanged(); Recalculate(); }
        }

        public double H2
        {
            get => _h2;
            set { _h2 = value; OnPropertyChanged(); Recalculate(); }
        }

        public bool ShowLinear
        {
            get => _showLinear;
            set { _showLinear = value; OnPropertyChanged(); }
        }

        public bool ShowHermite
        {
            get => _showHermite;
            set { _showHermite = value; OnPropertyChanged(); }
        }

        public bool ShowExact
        {
            get => _showExact;
            set { _showExact = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand CalculateCommand { get; }

        public MainViewModel()
        {
            CalculateCommand = new RelayCommand(Recalculate);
            Recalculate();
        }

        private void Recalculate()
        {
            if (H2 >= H1 || H2 <= 0 || H1 <= 0)
            {
                ErrorMessage = "Помилка: Крок h₂ має бути додатним та меншим за h₁ (h₂ < h₁)!";
                return;
            }

            ErrorMessage = string.Empty;

            double xStart = 0;
            double xEnd = 10;

            var basePoints = new List<PointD>();
            for (double x = xStart; x <= xEnd + 1e-9; x += H1)
            {
                basePoints.Add(new PointD(x, Math.Sin(x)));
            }

            if (basePoints.Count < 2)
            {
                ErrorMessage = "Помилка: Занадто великий крок h₁, необхідно мінімум 2 точки!";
                return;
            }

            Interpolation[] interpolators = new Interpolation[]
            {
                new LinearInterpolation(),
                new HermiteInterpolation()
            };

            var linearList = new List<PointD>();
            var hermiteList = new List<PointD>();
            var exactList = new List<PointD>();
            var errorList = new List<PointD>();
            for (double x = xStart; x <= xEnd + 1e-9; x += H2)
            {
                double yLinear = interpolators[0].Calculate(basePoints, x);
                double yHermite = interpolators[1].Calculate(basePoints, x);
                double yExact = Math.Sin(x);

                linearList.Add(new PointD(x, yLinear));
                hermiteList.Add(new PointD(x, yHermite));
                exactList.Add(new PointD(x, yExact));

                double err = Math.Abs(yHermite - yExact);
                errorList.Add(new PointD(x, err));
            }
            OriginalPoints = ToCanvasCoordinates(basePoints, 0, 10, -1.5, 1.5, 800, 300);
            LinearPoints = ToCanvasCoordinates(linearList, 0, 10, -1.5, 1.5, 800, 300);
            HermitePoints = ToCanvasCoordinates(hermiteList, 0, 10, -1.5, 1.5, 800, 300);
            ExactPoints = ToCanvasCoordinates(exactList, 0, 10, -1.5, 1.5, 800, 300);
            ErrorPoints = ToCanvasCoordinates(errorList, 0, 10, 0, 0.5, 800, 130);

           
            OnPropertyChanged(nameof(OriginalPoints));
            OnPropertyChanged(nameof(LinearPoints));
            OnPropertyChanged(nameof(HermitePoints));
            OnPropertyChanged(nameof(ExactPoints));
            OnPropertyChanged(nameof(ErrorPoints));
        }

        private static PointCollection ToCanvasCoordinates(
            List<PointD> points, double minX, double maxX, double minY, double maxY, double width, double height)
        {
            var collection = new PointCollection();

           
            double rangeX = Math.Abs(maxX - minX) < 1e-12 ? 1 : (maxX - minX);
            double rangeY = Math.Abs(maxY - minY) < 1e-12 ? 1 : (maxY - minY);

            foreach (var p in points)
            {
                double px = (p.X - minX) / rangeX * width;
                double py = height - ((p.Y - minY) / rangeY * height);
                collection.Add(new System.Windows.Point(px, py));
            }
            return collection;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}