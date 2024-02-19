﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiColorAreaSeries.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
//   Copyright (c) 2024 Taogya
// </copyright>
// <summary>
//   Represents a multi-color area series.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MultiColorAreaSeriesSample.OxyPlot.Series
{
    /// <summary>
    /// Represents a multi-color line series.
    /// </summary>
    public class MultiColorAreaSeries : LineSeries
    {
        public class FillWithLimit
        {
            public double LimitLo { get; set; }
            public double LimitHi { get; set; }
            public OxyColor Color { get; set; }

            public FillWithLimit() : this(null, null, OxyColors.Automatic) { }
            public FillWithLimit(FillWithLimit fwl) : this(fwl.LimitLo, fwl.LimitHi, fwl.Color) { }
            public FillWithLimit(double? LimitLo, double? LimitHi, OxyColor Color)
            {
                this.LimitLo = LimitLo ?? double.MinValue;
                this.LimitHi = LimitHi ?? double.MaxValue;
                this.Color = Color;
            }
            public bool IsInLimitRange(double value) => LimitLo <= value && value < LimitHi;
        }

        /// <summary>
        /// The default color.
        /// </summary>
        private readonly List<FillWithLimit> defaultFills;


        /// <summary>
        /// The second list of points.
        /// </summary>
        private readonly List<DataPoint> points2 = new();

        /// <summary>
        /// The secondary data points from the <see cref="P:ItemsSource" /> collection.
        /// </summary>
        private readonly List<DataPoint> itemsSourcePoints2 = new();

        /// <summary>
        /// The secondary data points from the <see cref="P:Points2" /> list.
        /// </summary>
        private List<DataPoint> actualPoints2 = new();

        /// <summary>
        /// Initializes a new instance of the <see cref = "MultiColorAreaSeries" /> class.
        /// </summary>
        public MultiColorAreaSeries(int Size)
        {
            if (Size < 1)
            {
                throw new ArgumentException("Size must be 1 or greater.");
            }
            this.Size = Size;
            this.Reverse2 = false;
            this.Color2 = OxyColors.Automatic;
            this.defaultFills = Enumerable.Repeat(new FillWithLimit(), Size).ToList();
            this.Fills = this.defaultFills.Select(fill => new FillWithLimit(fill)).ToList();
        }

        public int Size { get; private set; }

        public OxyColor AreaStrokeColor { get; set; }

        public OxyColor ActualAreaStrokeColor { get => this.AreaStrokeColor.GetActualColor(OxyColors.Undefined); }

        public double AreaStrokeThickness { get; set; }

        public double ActualAreaStrokeThickness { get => this.AreaStrokeThickness; }

        /// <summary>
        /// Gets or sets a constant value for the area definition.
        /// This is used if DataFieldBase and BaselineValues are <c>null</c>.
        /// </summary>
        /// <value>The baseline.</value>
        /// <remarks><see cref="P:ConstantY2" /> is used if <see cref="P:ItemsSource" /> is set 
        /// and <see cref="P:DataFieldX2" /> or <see cref="P:DataFieldY2" /> are <c>null</c>, 
        /// or if <see cref="P:ItemsSource" /> is <c>null</c> and <see cref="P:Points2" /> is empty.</remarks>
        public double ConstantY2 { get; set; }

        /// <summary>
        /// Gets or sets the data field to use for the X-coordinates of the second data set.
        /// </summary>
        /// <remarks>This property is used if <see cref="P:ItemsSource" /> is set.</remarks>
        public string DataFieldX2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data field to use for the Y-coordinates of the second data set.
        /// </summary>
        /// <remarks>This property is used if <see cref="P:ItemsSource" /> is set.</remarks>
        public string DataFieldY2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color of the line for the second data set.
        /// </summary>
        /// <value>The color.</value>
        public OxyColor Color2 { get; set; }

        /// <summary>
        /// Gets the actual color of the line for the second data set.
        /// </summary>
        /// <value>The actual color.</value>
        public virtual OxyColor ActualColor2
        {
            get
            {
                return this.Color2.GetActualColor(this.ActualColor);
            }
        }

        /// <summary>
        /// Gets or sets the fill color of the area.
        /// </summary>
        /// <value>The fill color.</value>
        public List<FillWithLimit> Fills { get; set; }

        /// <summary>
        /// Gets the actual fill color of the area.
        /// </summary>
        /// <value>The actual fill color.</value>
        public List<FillWithLimit> ActualFills
        {
            get
            {
                return this.Fills.Select((fill, n) => new FillWithLimit(fill.LimitLo, fill.LimitHi, fill.Color.GetActualColor(OxyColor.FromAColor(100, this.defaultFills[n].Color)))).ToList();
            }
        }

        /// <summary>
        /// Gets the second list of points.
        /// </summary>
        /// <value>The second list of points.</value>
        /// <remarks>This property is not used if <see cref="P:ItemsSource" /> is set.</remarks>
        public List<DataPoint> Points2
        {
            get
            {
                return this.points2;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the second data collection should be reversed.
        /// </summary>
        /// <value><c>true</c> if the second data set should be reversed; otherwise, <c>false</c>.</value>
        /// <remarks>The first dataset is not reversed, and normally
        /// the second dataset should be reversed to get a
        /// closed polygon.</remarks>
        public bool Reverse2 { get; set; }

        /// <summary>
        /// Gets the actual points of the second data set.
        /// </summary>
        /// <value>A list of data points.</value>
        protected List<DataPoint> ActualPoints2
        {
            get
            {
                return this.ItemsSource != null ? this.itemsSourcePoints2 : this.actualPoints2;
            }
        }

        /// <summary>
        /// Gets or sets the last visible window start position in second data points collection.
        /// </summary>
        protected int WindowStartIndex2 { get; set; }

        /// <summary>
        /// Gets a value indicating whether Points2 collection was defined by user.
        /// </summary>
        protected bool IsPoints2Defined { get; private set; }

        /// <summary>
        /// Gets the nearest point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="interpolate">interpolate if set to <c>true</c> .</param>
        /// <returns>A TrackerHitResult for the current hit.</returns>
        public override TrackerHitResult? GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            var xy = this.InverseTransform(point);
            var targetX = xy.X;
            int startIdx = this.IsXMonotonic
                ? this.FindWindowStartIndex(this.ActualPoints, p => p.X, targetX, this.WindowStartIndex)
                : 0;
            int startIdx2 = this.IsXMonotonic
                ? this.FindWindowStartIndex(this.ActualPoints2, p => p.X, targetX, this.WindowStartIndex2)
                : 0;

            TrackerHitResult result1, result2;
            if (interpolate && this.CanTrackerInterpolatePoints)
            {
                result1 = this.GetNearestInterpolatedPointInternal(this.ActualPoints, startIdx, point);
                result2 = this.GetNearestInterpolatedPointInternal(this.ActualPoints2, startIdx2, point);
            }
            else
            {
                result1 = this.GetNearestPointInternal(this.ActualPoints, startIdx, point);
                result2 = this.GetNearestPointInternal(this.ActualPoints2, startIdx2, point);
            }

            TrackerHitResult? result;
            if (result1 != null && result2 != null)
            {
                double dist1 = result1.Position.DistanceTo(point);
                double dist2 = result2.Position.DistanceTo(point);
                result = dist1 < dist2 ? result1 : result2;
            }
            else
            {
                result = result1 ?? result2;
            }

            if (result != null)
            {
                result.Text = StringHelper.Format(
                    this.ActualCulture,
                    this.TrackerFormatString,
                    result.Item,
                    this.Title,
                    this.XAxis.Title ?? XYAxisSeries.DefaultXAxisTitle,
                    this.XAxis.GetValue(result.DataPoint.X),
                    this.YAxis.Title ?? XYAxisSeries.DefaultYAxisTitle,
                    this.YAxis.GetValue(result.DataPoint.Y));
            }

            return result;
        }

        /// <summary>
        /// Gets interpolated X coordinate for given Y on a straight line
        /// between two points.
        /// </summary>
        /// <param name="a">First point.</param>
        /// <param name="b">Second point.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>Corresponding X coordinate.</returns>
        private double GetInterpolatedX(DataPoint a, DataPoint b, double y)
        {
            return (((y - a.Y) / (b.Y - a.Y)) * (b.X - a.X)) + a.X;
        }

        /// <summary>
        /// Renders the series on the specified rendering context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        public override void Render(IRenderContext rc)
        {
            this.VerifyAxes();

            var actualPoints = this.ActualPoints;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            var actualPoints2 = this.ActualPoints2;
            if (actualPoints2 == null || actualPoints2.Count == 0)
            {
                return;
            }

            int startIdx = 0;
            int startIdx2 = 0;
            double xmax = double.MaxValue;

            if (this.IsXMonotonic)
            {
                // determine render range
                var xmin = this.XAxis.ClipMinimum;
                xmax = this.XAxis.ClipMaximum;
                this.WindowStartIndex = this.UpdateWindowStartIndex(actualPoints, point => point.X, xmin, this.WindowStartIndex);
                this.WindowStartIndex2 = this.UpdateWindowStartIndex(actualPoints2, point => point.X, xmin, this.WindowStartIndex2);

                startIdx = this.WindowStartIndex;
                startIdx2 = this.WindowStartIndex2;
            }

            double minDistSquared = this.MinimumSegmentLength * this.MinimumSegmentLength;

            var areaContext = new AreaRenderContext()
            {
                Points = actualPoints,
                WindowStartIndex = startIdx,
                XMax = xmax,
                MinDistSquared = minDistSquared,
                RenderContext = rc,
                Reverse = false,
                Color = this.ActualColor,
                DashArray = this.ActualDashArray,
            };

            var chunksOfPoints = this.RenderChunkedPoints(areaContext);

            areaContext.Points = actualPoints2;
            areaContext.WindowStartIndex = startIdx2;
            areaContext.Reverse = this.Reverse2;
            areaContext.Color = this.ActualColor2;

            var chunksOfPoints2 = this.RenderChunkedPoints(areaContext);

            if (chunksOfPoints.Count != chunksOfPoints2.Count)
            {
                return;
            }

            // Draw the fill
            for (int chunkIndex = 0; chunkIndex < chunksOfPoints.Count; chunkIndex++)
            {
                var pts = chunksOfPoints[chunkIndex];
                var pts2 = chunksOfPoints2[chunkIndex];

                if (pts.Count != pts2.Count && pts.Count == 0)
                {
                    continue;
                }

                ScreenPoint lastPoint = pts[0];
                ScreenPoint lastPoint2 = pts2[0];
                var dp = this.InverseTransform(lastPoint);
                var lastFill = this.Fills.Where(fill => fill.IsInLimitRange(dp.Y)).FirstOrDefault();
                var allPts = new List<ScreenPoint>();
                var ptsChunk = new List<ScreenPoint>() { pts[0] };
                var ptsChunk2 = new List<ScreenPoint>() { pts2[0] };
                for (var n = 1; n < pts.Count; n++)
                {
                    var sp = pts[n];
                    var sp2 = pts2[n];

                    // Judgement Now Range
                    dp = InverseTransform(sp);
                    var fill = this.Fills.Where(fill => fill.IsInLimitRange(dp.Y)).FirstOrDefault();
                    // Changed Fill Color
                    if (lastFill != fill && fill != null)
                    {
                        var limit = dp.Y < lastFill.LimitLo ? lastFill.LimitLo : lastFill.LimitHi;
                        var boundarySp = ptsChunk.Last();
                        var boundarySp2 = ptsChunk2.Last();
                        if (limit != double.MinValue && limit != double.MaxValue)
                        {
                            var lastDp = InverseTransform(boundarySp);
                            var lastDp2 = InverseTransform(boundarySp2);
                            var boundaryDp = new DataPoint(this.GetInterpolatedX(lastDp, dp, limit), limit);
                            boundarySp = this.Transform(boundaryDp.X, boundaryDp.Y);
                            boundarySp2 = this.Transform(boundaryDp.X, lastDp2.Y);
                            // combine the two lines and draw the clipped area
                            ptsChunk.Add(boundarySp);
                            ptsChunk2.Add(boundarySp2);
                        }
                        ptsChunk2.Reverse();
                        allPts.AddRange(ptsChunk2);
                        allPts.AddRange(ptsChunk);
                        rc.DrawReducedPolygon(
                            allPts,
                            minDistSquared,
                            this.GetSelectableFillColor(lastFill?.Color ?? OxyColors.White),
                            this.ActualAreaStrokeColor,
                            this.ActualAreaStrokeThickness,
                            this.EdgeRenderingMode);

                        lastFill = fill;
                        lastPoint = sp;
                        lastPoint2 = sp2;
                        allPts = new List<ScreenPoint>();
                        ptsChunk = new List<ScreenPoint>() { boundarySp };
                        ptsChunk2 = new List<ScreenPoint>() { boundarySp2 };
                    }
                    // Add Point
                    ptsChunk.Add(sp);
                    ptsChunk2.Add(sp2);
                }
                // Draw Last Area
                ptsChunk2.Reverse();
                allPts.AddRange(ptsChunk2);
                allPts.AddRange(ptsChunk);
                rc.DrawReducedPolygon(
                    allPts,
                    minDistSquared,
                    this.GetSelectableFillColor(lastFill?.Color ?? OxyColors.White),
                            this.ActualAreaStrokeColor,
                            this.ActualAreaStrokeThickness,
                    this.EdgeRenderingMode);

                var markerSizes = new[] { this.MarkerSize };

                // draw the markers on top
                rc.DrawMarkers(
                    pts,
                    this.MarkerType,
                    null,
                    markerSizes,
                    this.ActualMarkerFill,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    1);
                rc.DrawMarkers(
                    pts2,
                    this.MarkerType,
                    null,
                    markerSizes,
                    this.MarkerFill,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    1);
            }
        }

        /// <summary>
        /// Renders the legend symbol for the line series on the
        /// specified rendering context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        /// <param name="legendBox">The bounding rectangle of the legend box.</param>
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double y0 = (legendBox.Top * 0.2) + (legendBox.Bottom * 0.8);
            double y1 = (legendBox.Top * 0.4) + (legendBox.Bottom * 0.6);
            double y2 = (legendBox.Top * 0.8) + (legendBox.Bottom * 0.2);

            var pts0 = new[] { new ScreenPoint(legendBox.Left, y0), new ScreenPoint(legendBox.Right, y0) };
            var pts1 = new[] { new ScreenPoint(legendBox.Right, y2), new ScreenPoint(legendBox.Left, y1) };
            var pts = new List<ScreenPoint>();
            pts.AddRange(pts0);
            pts.AddRange(pts1);

            if (this.StrokeThickness > 0 && this.ActualLineStyle != LineStyle.None)
            {
                rc.DrawLine(pts0, this.GetSelectableColor(this.ActualColor), this.StrokeThickness, this.EdgeRenderingMode, this.ActualLineStyle.GetDashArray());
                rc.DrawLine(pts1, this.GetSelectableColor(this.ActualColor2), this.StrokeThickness, this.EdgeRenderingMode, this.ActualLineStyle.GetDashArray());
            }
            rc.DrawPolygon(pts, this.GetSelectableFillColor(this.ActualFills[0].Color), OxyColors.Undefined, 0, this.EdgeRenderingMode);
        }

        /// <summary>
        /// The update data.
        /// </summary>
        protected override void UpdateData()
        {
            base.UpdateData();

            if (this.ItemsSource == null)
            {
                this.IsPoints2Defined = this.points2.Count > 0;

                if (this.IsPoints2Defined)
                {
                    this.actualPoints2 = this.points2;
                }
                else
                {
                    this.actualPoints2 = this.GetConstantPoints2().ToList();
                }

                return;
            }

            this.itemsSourcePoints2.Clear();

            // TODO: make it consistent with DataPointSeries.UpdateItemsSourcePoints
            // Using reflection on DataFieldX2 and DataFieldY2
            this.IsPoints2Defined = this.DataFieldX2 != null && this.DataFieldY2 != null;

            if (this.IsPoints2Defined)
            {
                var filler = new ListBuilder<DataPoint>();
                filler.Add(this.DataFieldX2!, double.NaN);
                filler.Add(this.DataFieldY2!, double.NaN);
                filler.Fill(this.itemsSourcePoints2, this.ItemsSource, args => new DataPoint(Axis.ToDouble(args[0]), Axis.ToDouble(args[1])));
            }
            else
            {
                this.itemsSourcePoints2.AddRange(this.GetConstantPoints2());
            }
        }

        /// <summary>
        /// Updates the maximum and minimum values of the series.
        /// </summary>
        protected override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            this.InternalUpdateMaxMin(this.ActualPoints2);
        }

        /// <summary>
        /// Renders data points skipping NaN values.
        /// </summary>
        /// <param name="context">Area rendering context.</param>
        /// <returns>The list of chunks.</returns>
        protected List<List<ScreenPoint>> RenderChunkedPoints(AreaRenderContext context)
        {
            var result = new List<List<ScreenPoint>>();
            var screenPoints = new List<ScreenPoint>();

            int clipCount = 0;
            var actualPoints = context.Points;
            for (int i = context.WindowStartIndex; i < actualPoints.Count; i++)
            {
                var point = actualPoints[i];

                if (double.IsNaN(point.Y))
                {
                    if (screenPoints.Count == 0)
                    {
                        continue;
                    }

                    result.Add(this.RenderScreenPoints(context, screenPoints));
                    screenPoints = new List<ScreenPoint>();
                }
                else
                {
                    var sp = this.Transform(point.X, point.Y);
                    screenPoints.Add(sp);
                }

                // We break after two points were seen beyond xMax to ensure glitch-free rendering.
                clipCount += point.X > context.XMax ? 1 : 0;
                if (clipCount > 1)
                {
                    break;
                }
            }

            if (screenPoints.Count > 0)
            {
                result.Add(this.RenderScreenPoints(context, screenPoints));
            }

            return result;
        }

        /// <summary>
        /// Renders a chunk of points on the screen.
        /// </summary>
        /// <param name="context">Render context.</param>
        /// <param name="points">Screen points.</param>
        /// <returns>The list of resampled points.</returns>
        protected virtual List<ScreenPoint> RenderScreenPoints(AreaRenderContext context, List<ScreenPoint> points)
        {
            var final = points;

            if (context.Reverse)
            {
                final.Reverse();
            }

            if (this.InterpolationAlgorithm != null)
            {
                var resampled = ScreenPointHelper.ResamplePoints(final, this.MinimumSegmentLength);
                final = this.InterpolationAlgorithm.CreateSpline(resampled, false, 0.25);
            }

            context.RenderContext.DrawReducedLine(
                final,
                context.MinDistSquared,
                this.GetSelectableColor(context.Color),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                context.DashArray,
                this.LineJoin);

            return final;
        }

        /// <summary>
        /// Gets the x coordinate of a DataPoint.
        /// </summary>
        /// <param name="point">Data point.</param>
        /// <returns>X coordinate.</returns>
        protected double GetPointX(DataPoint point)
        {
            return point.X;
        }

        /// <summary>
        /// Gets the points when <see cref="P:ConstantY2" /> is used.
        /// </summary>
        /// <returns>A sequence of <see cref="T:DataPoint"/>.</returns>
        private IEnumerable<DataPoint> GetConstantPoints2()
        {
            var actualPoints = this.ActualPoints;
            if (!double.IsNaN(this.ConstantY2) && actualPoints.Count > 0)
            {
                // Use ConstantY2
                var x0 = actualPoints[0].X;
                var x1 = actualPoints[actualPoints.Count - 1].X;
                yield return new DataPoint(x0, this.ConstantY2);
                yield return new DataPoint(x1, this.ConstantY2);
            }
        }

        /// <summary>
        /// Holds parameters for point rendering.
        /// </summary>
        protected class AreaRenderContext
        {
            /// <summary>
            /// Gets or sets source data points.
            /// </summary>
            public List<DataPoint> Points { get; set; }

            /// <summary>
            /// Gets or sets start index of a visible window.
            /// </summary>
            public int WindowStartIndex { get; set; }

            /// <summary>
            /// Gets or sets maximum visible X coordinate.
            /// </summary>
            public double XMax { get; set; }

            /// <summary>
            /// Gets or sets render context.
            /// </summary>
            public IRenderContext RenderContext { get; set; }

            /// <summary>
            /// Gets or sets minimum squared distance between points.
            /// </summary>
            public double MinDistSquared { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to reverse the points.
            /// </summary>
            public bool Reverse { get; set; }

            /// <summary>
            /// Gets or sets line color.
            /// </summary>
            public OxyColor Color { get; set; }

            /// <summary>
            /// Gets or sets line dash array.
            /// </summary>
            public double[] DashArray { get; set; }
        }
    }
}
