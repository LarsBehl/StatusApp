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
using StatusApp.Extensions;

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
        private static readonly int TEXT_SIZE = 12;
        private static readonly int TOUCH_POINT_RADIUS = 15;
        private static readonly int FLOATING_LABEL_BORDER_WIDTH = 2;
        private static readonly int TRIANGLE_BASE_WIDTH = 10;
        private static readonly int FLOATING_LABEL_OFFSET = 15;
        #endregion

        #region private fields
        private SKPoint[] _points;
        private SKPoint[] _drawnPoints;
        private SKPaint _graphPaint;
        private SKPaint _circlePaint;
        private SKPaint _axisPaint;
        private SKPaint _xLabelPaint;
        private SKPaint _yLabelPaint;
        private SKPaint _floatingLabelBackgroundPaint;
        private SKPaint _floatingLabelBorderPaint;
        private bool _isInitialized;
        private DisplayRotation _rotation;
        private Stopwatch _stopwatch;
        private bool _animationDone;
        private int _touchedPointIndex;
        #endregion

        #region bindable properties
        public static readonly BindableProperty GraphDataProperty = BindableProperty.Create(
            propertyName: nameof(GraphData),
            returnType: typeof(List<AbstractDataPoint>),
            declaringType: typeof(LineGraphComponent),
            defaultBindingMode: BindingMode.OneWay,
            defaultValue: default,
            propertyChanged: GraphDataChanged
        );

        public List<AbstractDataPoint> GraphData { get; set; }

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
                Style = SKPaintStyle.Fill
            };

            this._axisPaint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill
            };

            this._yLabelPaint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Right,
                TextSize = TEXT_SIZE
            };

            this._xLabelPaint = new SKPaint()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = TEXT_SIZE
            };

            this._floatingLabelBorderPaint = new SKPaint()
            {
                Color = SKColors.Gray,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = FLOATING_LABEL_BORDER_WIDTH
            };

            this._floatingLabelBackgroundPaint = new SKPaint()
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = FLOATING_LABEL_BORDER_WIDTH
            };

            this._isInitialized = false;
            this._animationDone = false;
            this._touchedPointIndex = -1;
            this._stopwatch = new Stopwatch();
# if __MOBILE__
            this._rotation = DeviceDisplay.MainDisplayInfo.Rotation;
            DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;
# endif
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
                if (this.GraphData is null)
                    return;
                this.TransformDataPoints();
                this._isInitialized = true;

                if (!this.ShowAnimation)
                {
                    for (int i = 0; i < this._points.Length; i++)
                        this._drawnPoints[i] = this._points[i];
                }
                else
                {
                    for (int i = 0; i < this._points.Length; i++)
                        this._drawnPoints[i] = new SKPoint(this._points[i].X, this.SkiaView.CanvasSize.Height - VERTICAL_OFFSET);
                    this._stopwatch.Start();
                }
            }

            List<SKPoint> renderedPoints = this.CalculateAssistPoints();
            this.DrawGraph(canvas, renderedPoints);
            this.DrawPoints(canvas);
            this.DrawAxis(canvas);
            this.DrawAxisLabels(canvas);

            if (this._touchedPointIndex >= 0)
                this.DrawFloatingLabel(canvas);

            if (!this._animationDone && this.ShowAnimation)
                this.Animate();

        }

        void OnTouch(object sender, SKTouchEventArgs e)
        {
            if (!e.InContact)
                return;

            SKPoint touchPoint = e.Location;

            // check if the touch location is within touch target of a datapoint
            for (int i = 0; i < this._drawnPoints.Length; i++)
            {
                if (SKPoint.Distance(this._drawnPoints[i], touchPoint) <= TOUCH_POINT_RADIUS)
                {
                    this._touchedPointIndex = i;
                    this.SkiaView.InvalidateSurface();
                    return;
                }
            }

            // reset if tapped somewhere else in the graph
            this._touchedPointIndex = -1;
            this.SkiaView.InvalidateSurface();
        }
        #endregion

        #region render helpers
        private void DrawGraph(SKCanvas canvas, List<SKPoint> points)
        {
            for (int i = 0; i < points.Count - 1; i += 3)
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

                if (i % 2 == 0)
                {
                    canvas.DrawRect(this._drawnPoints[i].X - (AXIS_WIDTH / 2f), yStart, AXIS_WIDTH, LABEL_LENGTH, this._axisPaint);
                    canvas.DrawText(this.GraphData[i].GetXLabel(), this._drawnPoints[i].X, yStart + TEXT_SIZE + LABEL_STRING_Y_OFFSET + LABEL_LENGTH, this._xLabelPaint);
                }
            }

            float xStartPosition = HORIZONTAL_OFFSET - LABEL_LENGTH;
            float halfYPos = this.SkiaView.CanvasSize.Height / 2;

            canvas.DrawRect(xStartPosition, maxHeight, LABEL_LENGTH, AXIS_WIDTH, this._axisPaint);
            canvas.DrawText(this.GraphData[maxHeightIndex].GetYLabel(), HORIZONTAL_OFFSET - LABEL_STRING_X_OFFSET, maxHeight + TEXT_SIZE + LABEL_STRING_Y_OFFSET, this._yLabelPaint);
            canvas.DrawRect(xStartPosition, halfYPos, LABEL_LENGTH, AXIS_WIDTH, this._axisPaint);
            canvas.DrawText(this.GraphData[maxHeightIndex].GetYLabel(2), HORIZONTAL_OFFSET - LABEL_STRING_X_OFFSET, halfYPos + TEXT_SIZE + LABEL_STRING_Y_OFFSET, this._yLabelPaint);
        }

        private void DrawFloatingLabel(SKCanvas canvas)
        {
            SKPoint touchedPoint = this._drawnPoints[this._touchedPointIndex];
            AbstractDataPoint touchedDataPoint = this.GraphData[this._touchedPointIndex];
            string[] description = touchedDataPoint.GetDescription();
            float maxTextWidth = float.MinValue;

            // search for the maximum text width
            foreach (string s in description)
            {
                float textWidth = this._xLabelPaint.MeasureText(s);
                if (textWidth > maxTextWidth)
                    maxTextWidth = textWidth;
            }

            float rectHeight = description.Length * TEXT_SIZE + (description.Length + 1) * LABEL_STRING_Y_OFFSET;
            float rectWidth = maxTextWidth + 2 * LABEL_STRING_X_OFFSET;
            float rectYStart = touchedPoint.Y - FLOATING_LABEL_OFFSET - rectHeight;
            float rectXStart = touchedPoint.X - (rectWidth / 2);
            bool isAbove = true;

            if (rectYStart < 0)
            {
                rectYStart = touchedPoint.Y + FLOATING_LABEL_OFFSET;
                isAbove = false;
            }
            float rectYEnd = rectYStart + rectHeight;

            if (rectXStart < 0)
                rectXStart = 0 + LABEL_STRING_X_OFFSET;

            if (rectXStart + rectWidth > this.SkiaView.CanvasSize.Width)
                rectXStart = this.SkiaView.CanvasSize.Width - rectWidth - LABEL_STRING_X_OFFSET;

            float rectXEnd = rectXStart + rectWidth;            

            // draw the rounded rect
            SKRect rect = new SKRect(rectXStart, rectYStart, rectXEnd, rectYEnd);
            using (SKRoundRect rrect = new SKRoundRect(rect, 5))
            {
                canvas.DrawRoundRect(rrect, this._floatingLabelBackgroundPaint);
                canvas.DrawRoundRect(rrect, this._floatingLabelBorderPaint);
            }

            // draw a triangle indicating for which point the label is
            float halfBaseWidth = TRIANGLE_BASE_WIDTH / 2f;
            float triangleYPos = isAbove ? rectYEnd : rectYStart;
            float triangleTipYPos = isAbove ? touchedPoint.Y - CIRCLE_RADIUS - FLOATING_LABEL_BORDER_WIDTH : touchedPoint.Y + CIRCLE_RADIUS + FLOATING_LABEL_BORDER_WIDTH;
            using (SKPath path = new SKPath())
            {
                path.MoveTo((touchedPoint.X - halfBaseWidth).Clamp(rectXStart + TRIANGLE_BASE_WIDTH, rectXEnd - 2 * TRIANGLE_BASE_WIDTH) + 1, triangleYPos);
                path.LineTo(touchedPoint.X, triangleTipYPos);
                path.LineTo((touchedPoint.X + halfBaseWidth).Clamp(rectXStart + 2 * TRIANGLE_BASE_WIDTH, rectXEnd - TRIANGLE_BASE_WIDTH) - 1, triangleYPos);
                path.Close();
                canvas.DrawPath(path, this._floatingLabelBackgroundPaint);
            }

            using (SKPath path = new SKPath())
            {
                path.MoveTo((touchedPoint.X - halfBaseWidth).Clamp(rectXStart + TRIANGLE_BASE_WIDTH, rectXEnd - 2 * TRIANGLE_BASE_WIDTH), triangleYPos);
                path.LineTo(touchedPoint.X, triangleTipYPos);
                path.LineTo((touchedPoint.X + halfBaseWidth).Clamp(rectXStart + 2 * TRIANGLE_BASE_WIDTH, rectXEnd - TRIANGLE_BASE_WIDTH), triangleYPos);
                canvas.DrawPath(path, this._floatingLabelBorderPaint);
            }

            // draw the text
            for (int i = 0; i < description.Length; i++)
                canvas.DrawText(description[i], rectXStart + rectWidth / 2, rectYStart + (i + 1) * LABEL_STRING_Y_OFFSET + (i + 1) * TEXT_SIZE, this._xLabelPaint);
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
            if (this.GraphData is null)
                return;
            int maxValue = this.GraphData.Max(dp => dp.Value);
            float canvasHeight = this.SkiaView.CanvasSize.Height - 2 * VERTICAL_OFFSET;
            float horizontalStepSize = (this.SkiaView.CanvasSize.Width - 2 * HORIZONTAL_OFFSET) / (this.GraphData.Count - 1);

            for (int i = 0; i < this.GraphData.Count; i++)
            {
                float verticalPercentile = this.GraphData[i].Value / (float)maxValue;
                this._points[i] = new SKPoint(HORIZONTAL_OFFSET + i * horizontalStepSize, this.SkiaView.CanvasSize.Height - (canvasHeight * verticalPercentile) - VERTICAL_OFFSET);
            }
        }

        private SKPoint GetAssistPoint(SKPoint current, SKPoint point)
        {
            SKPoint distVec = point - current;

            return new SKPoint(current.X + (distVec.X * 0.5f), current.Y);
        }
        #endregion

        #region bindable property setters
        private static void GraphDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            LineGraphComponent component = bindable as LineGraphComponent;

            if (newValue is null)
                return;

            component.GraphData = newValue as List<AbstractDataPoint>;
            component._points = new SKPoint[component.GraphData.Count];
            component._drawnPoints = new SKPoint[component._points.Length];

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