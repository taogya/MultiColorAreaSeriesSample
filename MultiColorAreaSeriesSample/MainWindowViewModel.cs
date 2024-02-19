using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiColorAreaSeriesSample.OxyPlot.Series;

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
            var fills = new List<MultiColorAreaSeries.FillWithLimit>()
            {
                    new(null, -0.8, OxyColor.Parse("#77000044")), // y < -0.8 Color
                    new(-0.8, -0.6, OxyColor.Parse("#77000088")), // -0.8 <= y < -0.6 Color
                    new(-0.6, -0.4, OxyColor.Parse("#770000cc")), // -0.6 <= y < -0.4 Color
                    new(-0.4, -0.2, OxyColor.Parse("#770000ff")), // -0.4 <= y < -0.2 Color
                    new(-0.2, 0.0, OxyColor.Parse("#7700ffff")), // -0.4 <= y < -0.2 Color
                    new(0.0, 0.2, OxyColor.Parse("#77ffff00")), // 0.0 <= y < 0.2 Color
                    new(0.2, 0.4, OxyColor.Parse("#77ff0000")), // 0.2 <= y < 0.4 Color
                    new(0.4, 0.6, OxyColor.Parse("#77cc0000")), // 0.4 <= y < 0.6 Color
                    new(0.6, 0.8, OxyColor.Parse("#77880000")), // 0.6 <= y < 0.8 Color
                    new(0.8, null, OxyColor.Parse("#77440000")), // 0.8 <= y Color
            };
            var series = new MultiColorAreaSeries(fills.Count)
            {
                Color = OxyColor.FromArgb(0xff, 0xff, 0x00, 0x00), // first line color
                Color2 = OxyColor.FromArgb(0xff, 0x00, 0x00, 0xff), // second line color
                Fills = fills,
                AreaStrokeColor = OxyColors.Black,
                AreaStrokeThickness = 5.0
            };
            var f = 1.0 / 60; // Hz
            var fs = 1.0; // Hz
            var nMax = 60.0 / fs; // 1 min
            for (var n = 0; n <= nMax; n++)
            {
                var x = DateTimeAxis.ToDouble(startDateTime.AddSeconds(n / fs));
                var y = Math.Sin(2 * Math.PI * f * n / fs);
                series.Points.Add(new(x, y));
                series.Points2.Add(new(x, 0));
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
