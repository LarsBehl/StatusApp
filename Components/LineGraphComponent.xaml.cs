using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Essentials;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using StatusApp.Domain.Model.Local;

namespace StatusApp.Components
{
    public partial class LineGraphComponent : ContentView
    {
        #region constants
        private static readonly int CIRCLE_RADIUS = 5;
        private static readonly int ANIMATION_DURATION = 500;
        private static readonly int VERTICAL_OFFSET = 50;
        private static readonly int HORIZONTAL_OFFSET = 50;
        private static readonly int GRAPH_WIDTH = 3;
        private static readonly int AXIS_WIDTH = 3;
        private static readonly int LABEL_LENGTH = 15;
        private static readonly int LABEL_STRING_X_OFFSET = 5;
        private static readonly int LABEL_STRING_Y_OFFSET = 3;
        private static readonly int TEXT_SIZE = 15;
        #endregion

        #region private fields
        private SKPoint[] _points;
        private SKPoint[] _drawnPoints;
        private SKPaint _graphPaint;
        private SKPaint _circlePaint;
        private SKPaint _axisPaint;
        private SKPaint _labelPaint;
        private bool _isInitialized;
        private DisplayRotation _rotation;
        private Stopwatch _stopwatch;
        private bool _animationDone;
        #endregion

        #region bindable properties
        public static readonly BindableProperty DataPointsProperty = BindableProperty.Create(
            propertyName: nameof(DataPoints),
            returnType: typeof(List<DataPoint>),
            declaringType: typeof(LineGraphComponent),
            defaultBindingMode: BindingMode.OneWay,
            defaultValue: default,
            propertyChanged: DataChanged
        );

        // TODO add custom object to prevent duplicate unit information
        public List<DataPoint> DataPoints { get; set; }

        public static readonly BindableProperty ShowAnimationProperty = BindableProperty.Create(
            propertyName: nameof(ShowAnimation),
            returnType: typeof(bool),
            declaringType: typeof(LineGraphComponent),
            defaultBindingMode: BindingMode.OneWay,
            defaultValue: default,
            propertyChanged: ShowAnimationChanged
        );

        public bool ShowAnimation { get; set; }
        #endregion

        public LineGraphComponent()
        {
            InitializeComponent();
            this._graphPaint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = GRAPH_WIDTH
            };

            this._circlePaint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill
            };

            this._axisPaint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill
            };

            this._labelPaint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Right,
                TextSize = TEXT_SIZE
            };

            this._isInitialized = false;
            this._animationDone = false;
            this._stopwatch = new Stopwatch();
            this._rotation = DeviceDisplay.MainDisplayInfo.Rotation;
            DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;
        }

        #region event handlers
        void OnDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            if (this._rotation != e.DisplayInfo.Rotation)
            {
                this._rotation = e.DisplayInfo.Rotation;
                this.TransformDataPoints();

                for (int i = 0; i < this._points.Length; i++)
                    this._drawnPoints[i] = this._points[i];
                this.SkiaView.InvalidateSurface();
            }
        }

        void OnCanvasViewPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (!this._isInitialized)
            {
                this.TransformDataPoints();
                this._isInitialized = true;

                if (!this.ShowAnimation)
                {
                    for (int i = 0; i < this._points[0].Length; i++)
                        this._drawnPoints[i] = this._points[i];
                }
                else
                {
                    for (int i = 0; i < this._points.Length; i++)
                        this._drawnPoints[i] = new SKPoint(this._points[i].X, this.SkiaView.CanvasSize.Height - VERTICAL_OFFSET);
                }
            }

            List<SKPoint> renderedPoints = this.CalculateAssistPoints();
            this.DrawGraph(canvas, renderedPoints);
            this.DrawPoints(canvas);
            this.DrawAxis(canvas);
            this.DrawAxisLabels(canvas);

            if (!this._animationDone && this.ShowAnimation)
                this.Animate();

        }
        #endregion

        #region render helpers
        private void DrawGraph(SKCanvas canvas, List<SKPoint> points)
        {
            for (int i = 0; i < points.Count; i += 3)
            {
                using (SKPath path = new SKPath())
                {
                    path.MoveTo(points[i]);
                    path.CubicTo(points[i + 1], points[i + 2], points[i + 3]);
                    canvas.DrawPath(path, this._graphPaint);
                }
            }
        }

        private void DrawPoints(SKCanvas canvas)
        {
            foreach (SKPoint point in this._drawnPoints)
                canvas.DrawCircle(point, CIRCLE_RADIUS, this._circlePaint);
        }

        private void DrawAxis(SKCanvas canvas)
        {
            float halfAxisWidth = AXIS_WIDTH / 2f;
            float xAxisEnd = this.SkiaView.CanvasSize.Width - 2 * HORIZONTAL_OFFSET + halfAxisWidth;
            float axisYStart = this.SkiaView.CanvasSize.Height - VERTICAL_OFFSET;
            float axisXStart = HORIZONTAL_OFFSET - halfAxisWidth;
            float yAxisEnd = this.SkiaView.CanvasSize.Height - 2 * VERTICAL_OFFSET;

            canvas.DrawRect(axisXStart, axisYStart, xAxisEnd, AXIS_WIDTH, this._axisPaint);
            canvas.DrawRect(axisXStart, VERTICAL_OFFSET, AXIS_WIDTH, yAxisEnd, this._axisPaint);
        }

        private void DrawAxisLabels(SKCanvas canvas)
        {
            float yStart = this.SkiaView.CanvasSize.Height - VERTICAL_OFFSET;

            float maxHeight = float.MaxValue;
            int maxHeightIndex = -1;

            for (int i = 0; i < this._points.Length; i++)
            {
                if (this._points[i].Y < maxHeight)
                {
                    maxHeight = this._points[i].Y;
                    maxHeightIndex = i;
                }

                canvas.DrawRect(this._drawnPoints[i].X - (AXIS_WIDTH / 2f), yStart, AXIS_WIDTH, LABEL_LENGTH, this._axisPaint);
                canvas.DrawText(this.DataPoints[i].Label, this._drawnPoints[i].X - LABEL_STRING_X_OFFSET, yStart + TEXT_SIZE + LABEL_STRING_Y_OFFSET, this._labelPaint);
            }

            float xStartPosition = HORIZONTAL_OFFSET - LABEL_LENGTH;
            int maxValue = this.DataPoints[maxHeightIndex].Value;
            float halfYPos = this.SkiaView.CanvasSize.Height / 2;

            canvas.DrawRect(xStartPosition, maxHeight, LABEL_LENGTH, AXIS_WIDTH, this._axisPaint);
            canvas.DrawText($"{maxValue} {this.DataPoints[maxHeightIndex].Unit}", HORIZONTAL_OFFSET - LABEL_STRING_X_OFFSET, maxHeight + TEXT_SIZE + LABEL_STRING_Y_OFFSET, this._labelPaint);
            canvas.DrawRect(xStartPosition, halfYPos, LABEL_LENGTH, AXIS_WIDTH, this._axisPaint);
            canvas.DrawText($"{(maxValue / 2)} {this.DataPoints[maxHeightIndex].Unit}", HORIZONTAL_OFFSET - LABEL_STRING_X_OFFSET, halfYPos + TEXT_SIZE + LABEL_STRING_Y_OFFSET, this._labelPaint);
        }

        private void Animate()
        {
            long elapsedMillis = this._stopwatch.ElapsedMilliseconds;
            elapsedMillis = elapsedMillis.Clamp(0, 500);
            if (elapsedMillis == ANIMATION_DURATION)
            {
                this._animationDone = true;
                this._stopwatch.Stop();
            }

            float t = (float)elapsedMillis / ANIMATION_DURATION;
            t = 1 / (1 + MathF.Pow(MathF.E, -10 * t * 5));

            for (int i = 0; i < this._points.Length; i++)
                this._drawnPoints[i] = new SKPoint(
                    this._points[i].X,
                    this.SkiaView.CanvasSize.Height - (this.SkiaView.CanvasSize.Height - this._points[i].Y * t) - VERTICAL_OFFSET * (1 - t)
                );

            this.SkiaView.InvalidateSurface();
        }
        #endregion

        #region data transformation
        private List<SKPoint> CalculateAssistPoints()
        {
            List<SKPoint> result = new List<SKPoint>();

            for (int i = 0; i < this._drawnPoints.Length; i++)
            {
                SKPoint current = this._drawnPoints[i];
                SKPoint nextAssist;
                SKPoint prevAssist;

                if (i == 0)
                {
                    nextAssist = this.GetAssistPoint(current, this._drawnPoints[i + 1]);
                    result.Add(current);
                    result.Add(nextAssist);
                    continue;
                }

                if (i == this._drawnPoints.Length - 1)
                {
                    prevAssist = this.GetAssistPoint(current, this._drawnPoints[i - 1]);
                    result.Add(prevAssist);
                    result.Add(current);
                    break;
                }

                SKPoint next = this._drawnPoints[i + 1];
                SKPoint prev = this._drawnPoints[i - 1];
                prevAssist = this.GetAssistPoint(current, prev);
                nextAssist = this.GetAssistPoint(current, next);

                result.Add(prevAssist);
                result.Add(current);
                result.Add(nextAssist);
            }

            return result;
        }

        private void TransformDataPoints()
        {
            int maxValue = this.DataPoints.Max(dp => dp.Value);
            float canvasHeight = this.SkiaView.CanvasSize.Height - 2 * VERTICAL_OFFSET;
            float horizontalStepSize = (this.SkiaView.CanvasSize.Width - 2 * HORIZONTAL_OFFSET) / (this.DataPoints.Count - 1);

            for (int i = 0; i < this.DataPoints.Count; i++)
            {
                float verticalPercentile = this.DataPoints[i].Value / (float)maxValue;
                this._points[i] = new SKPoint(HORIZONTAL_OFFSET + i + horizontalStepSize, this.SkiaView.CanvasSize.Height - (canvasHeight * verticalPercentile) - VERTICAL_OFFSET);
            }
        }

        private SKPoint GetAssistPoint(SKPoint current, SKPoint point)
        {
            SKPoint distVec = point - current;

            return new SKPoint(current.X + (distVec.X * 0.5f), current.Y);
        }
        #endregion

        #region bindable property setters
        private static void DataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            LineGraphComponent component = bindable as LineGraphComponent;
            if (newValue is null)
                return;
            component.DataPoints = newValue as List<DataPoint>;
            component._points = new SKPoint[component.DataPoints.Count];
            component._drawnPoints = new SKPoint[component.DataPoints.Count];

            if (component._isInitialized)
            {
                component._isInitialized = false;
                component._animationDone = false;
                component._stopwatch.Reset();
            }

            component.SkiaView.InvalidateSurface();
        }

        private static void ShowAnimationChanged(BindableObject bindable, object oldValue, object newValue)
        {
            LineGraphComponent component = bindable as LineGraphComponent;
            component.ShowAnimation = (bool)newValue;
        }
        #endregion
    }
}