using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiColorAreaSeriesSample
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        PlotModel plotModel = new();

        public MainWindowViewModel()
        {
            #region Series
            var startDateTime = new DateTime(2024, 2, 16, 14, 0, 0);
            var series = new TwoColorAreaSeries()
            {
                Color = OxyColor.Parse("#55ff0000"),
                Color2 = OxyColor.Parse("#550000ff"),
                Fill = OxyColor.Parse("#5500ff00"),
                Fill2 = OxyColor.Parse("#5500ffff"),
                Limit =1.0,
            };
            var f = 1.0 / 60; // Hz
            var fs = 1.0; // Hz
            var nMax = 60.0 / fs; // 1 min
            for (var n = 0; n < nMax; n++)
            {
                var x = DateTimeAxis.ToDouble(startDateTime.AddSeconds(n / fs));
                var y = Math.Sin(2 * Math.PI * f * n / fs) + 1;
                series.Points.Add(new(x, y));
            }
            plotModel.Series.Add(series);
            #endregion

            #region X Axis
            var xMin = series.Points.Select(p => p.X).Min();
            var xMax = series.Points.Select(p => p.X).Max();
            var xStep = (xMax - xMin) / 10;
            plotModel.Axes.Add(new DateTimeAxis()
            {
                Title = "DateTime",
                StringFormat = "yyyy/MM/dd HH:mm:ss",
                Angle = -80,
                Maximum = xMax, 
                Minimum = xMin,
                MajorStep = xStep,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 1,
                MinorStep = xStep / 2,
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineThickness = 1,

            });
            #endregion

            #region Y Axis
            var yMin = series.Points.Select(p => p.Y).Min();
            var yMax = series.Points.Select(p => p.Y).Max();
            var yStep = (yMax - yMin) / 10;
            plotModel.Axes.Add(new LinearAxis()
            {
                Title = "Value",
                Maximum = yMax,
                Minimum = yMin,
                MajorStep = yStep,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 1,
                MinorStep = yStep / 2,
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineThickness = 1,
            });
            #endregion
        }
    }

}
