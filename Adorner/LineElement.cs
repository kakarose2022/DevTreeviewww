using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using ToolTip = System.Windows.Controls.ToolTip;

namespace DevTreeview.Adorner
{

    [JsonObject(MemberSerialization.OptIn)]
    public class LineElement : FrameworkElement
    {
        private bool isSelected;
        public string toolTipContent = "LineElement Content Test!!";
        public ToolTip toolTip;
        private Pen pen => new Pen(Brushes.Black, isSelected ? 2 : 1);
        public bool isTemplate;
        public List<LineGeometry> LineGeometrys;
        public delegate void DisposeAdornerEvent(LineElement lineElement);
        public event DisposeAdornerEvent DisposeAdorner;
        private TranslateTransform _transform;
        public Guid AdornerGuid;


        [JsonProperty]
        public List<PointElement> PointElements;

        public LineElement()
        {

        }


        public LineElement(IEnumerable<PointElement> pointElements, System.Windows.Documents.Adorner adorner)
        {
            //AdornerGuid = adorner.AdornerGuid;
            PointElements = pointElements.ToList();
            LineGeometrys = new List<LineGeometry>();
            _transform = new TranslateTransform();
            toolTip = new ToolTip()
            {
                Content = adorner.ToString() ?? toolTipContent,
                HasDropShadow = true,
            };
            ToolTipService.SetToolTip(this, adorner.ToString() ?? toolTipContent);
            ToolTipService.SetShowDuration(this, 3000);
            InvalidateVisual();
        }

        #region override
        private bool _isDragging = false;
        private Point _dragStartPoint;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Trace.WriteLine("OnMouseDown...");
            Point mousePosition = e.GetPosition(this);

            var aa = LineGeometrys.Where(o => o.StrokeContains(pen, mousePosition)).ToList();
            if (LineGeometrys.Any(o => o.StrokeContains(pen, mousePosition)))
            {
                //MessageBox.Show("点击到线条!");
            }
            else
            {
                MessageBox.Show("未点击到线条");
            }

            // 如果鼠标按下时在元素内，则开始拖拽
            if (IsMouseOver)
            {
                _isDragging = true;
                _dragStartPoint = e.GetPosition(this);
                this.CaptureMouse();  // 捕获鼠标
            }
            //base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // 如果正在拖拽，更新线的位置 TODO
            if (_isDragging)
            {
                //Point currentPoint = e.GetPosition(this);
                //Vector offset = currentPoint - _dragStartPoint;

                //_transform.X = _dragStartPoint.X + offset.X;
                //_transform.Y = _dragStartPoint.Y + offset.Y;

                //Trace.WriteLine($"_startPoint...{currentPoint}");
                ////Trace.WriteLine($"_endPoint...{_endPoint}");
                //Trace.WriteLine($"_dragStartPoint...{_dragStartPoint}");

                //InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);  // 确保调用基类方法
            Trace.WriteLine("OnMouseUp...");
            if (_isDragging)
            {
                _isDragging = false;
                this.ReleaseMouseCapture();
            }
        }

        #region ToolTip
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            toolTip.IsOpen = true;
            isSelected = !isSelected;
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            toolTip.IsOpen = false;
            isSelected = false;
            InvalidateVisual();
        }
        #endregion

        #region Menu
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            ContextMenu contextMenu = new ContextMenu();
            MenuItem editMenuItem = new MenuItem { Header = "编辑" };
            editMenuItem.Click -= EditMenuItem_Click;
            editMenuItem.Click += EditMenuItem_Click;
            MenuItem deleteMenuItem = new MenuItem { Header = "删除" };
            deleteMenuItem.Click -= DeleteMenuItem_Click;
            deleteMenuItem.Click += DeleteMenuItem_Click;

            contextMenu.Items.Add(editMenuItem);
            contextMenu.Items.Add(deleteMenuItem);
            contextMenu.IsOpen = true;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DisposeAdorner?.Invoke(this);
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region 渲染
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_isDragging)
            {
                foreach (var lineGeometry in LineGeometrys)
                {
                    //drawingContext.DrawGeometry(Brushes.White, pen, lineGeometry);
                }
            }
            else
            {
                LineGeometrys.Clear();
                foreach (var pointElement in PointElements)
                {
                    var lineGeometry = new LineGeometry(pointElement.StartPoint, pointElement.EndPoint);
                    lineGeometry.Transform = _transform;
                    LineGeometrys.Add(lineGeometry);

                    //drawingContext.DrawLine(pen, pointElement.StartPoint, pointElement.EndPoint);
                    drawingContext.DrawGeometry(Brushes.White, pen, lineGeometry);

                    //TODO
                    if (pointElement.IsArrow)
                    {
                        DrawArrow(pen, pointElement.EndPoint, pointElement.StartPoint, drawingContext);
                    }
                }
            }

        }
        #endregion
        #endregion

        #region Method
        public void ClearElement()
        {
            this.PointElements.Clear();
            InvalidateVisual();
        }

        private void DrawArrow(Pen pen, Point startPoint, Point endPoint, DrawingContext drawingContext)
        {
            double arrowHeadSize = 10;
            double arrowHeadAngle = 30;

            Vector lineDirection = endPoint - startPoint;
            lineDirection.Normalize();

            double angleRad = arrowHeadAngle * Math.PI / 180;
            Vector arrowHeadLeft = new Vector
            (
                lineDirection.X * Math.Cos(angleRad) - lineDirection.Y * Math.Sin(angleRad),
                lineDirection.X * Math.Sin(angleRad) + lineDirection.Y * Math.Cos(angleRad)
            );

            Vector arrowHeadRight = new Vector
            (
                lineDirection.X * Math.Cos(-angleRad) - lineDirection.Y * Math.Sin(-angleRad),
                lineDirection.X * Math.Sin(-angleRad) + lineDirection.Y * Math.Cos(-angleRad)
            );

            Point arrowLeft = endPoint - arrowHeadLeft * arrowHeadSize;
            Point arrowRight = endPoint - arrowHeadRight * arrowHeadSize;

            drawingContext.DrawLine(pen, endPoint, arrowLeft);
            drawingContext.DrawLine(pen, endPoint, arrowRight);
        }
        #endregion

    }
}
