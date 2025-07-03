using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevTreeview.Core;
using DevTreeview.Extenstions;
using Newtonsoft.Json;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace DevTreeview.Adorner
{
    public class TreeNodeAdorner : System.Windows.Documents.Adorner, IDisposable
    {
        public Guid AdornerGuid;
        private LineElement lineElement;
        private double OffSetX = 20;
        private double LineSpace = 9;
        private bool ReDrawing = false;
        private RowControlProperty startRowControl;
        private RowControlProperty endRowControl;

        [JsonProperty]
        public double LinkMaxWidth;
        private TreeViewControl treeViewControl;

        public List<LineElement> lineElements { get; }

        public TreeNodeAdorner(RowControlProperty startNode, RowControlProperty endNode, TreeViewControl treeview) : base(startNode.RowControl)
        {
            startRowControl = startNode;
            endRowControl = endNode;
            treeViewControl = treeview;
            lineElements = new List<LineElement>();
            AdornerGuid = Guid.NewGuid();
            ClearTemp();
            CalAndDrawLinePoint();
            InvalidateVisual();
        }

        private void CalAndDrawLinePoint()
        {
            var pointElements = new List<PointElement>();
            Point adornedElementPosition = new Point();
            Point startPoint = new Point();
            Point endPoint = new Point();

            GetTreeViewItemEndPoint(startRowControl.RowControl, ref startPoint);
            GetTreeViewItemEndPoint(endRowControl.RowControl, ref endPoint);

            Point rightPoint;
            StartToEndRelativeMaxPoint(startRowControl, endRowControl, ref rightPoint);
            LinkMaxWidth = new List<Double> { startPoint.X, endPoint.X, rightPoint.X }.Max();

            var startLink = LinkMaxWidth + OffSetX;

            if (AdornedElementToTreeViewPoint(ref adornedElementPosition))
            {
                //开始横线
                pointElements.Add(new PointElement()
                {
                    StartPoint = new Point(startPoint.X, startPoint.Y).ToRelativePostion(adornedElementPosition),
                    EndPoint = new Point(startLink, startPoint.Y).ToRelativePostion(adornedElementPosition),
                    //isTemp = IsTemp,

                });

                //结束横线
                pointElements.Add(new PointElement()
                {
                    StartPoint = new Point(endPoint.X, endPoint.Y).ToRelativePostion(adornedElementPosition),
                    EndPoint = new Point(startLink, endPoint.Y).ToRelativePostion(adornedElementPosition),
                    //isTemp = IsTemp,
                    IsArrow = true
                });

                //竖线
                pointElements.Add(new PointElement()
                {
                    StartPoint = new Point(startLink, startPoint.Y).ToRelativePostion(adornedElementPosition),
                    EndPoint = new Point(startLink, endPoint.Y).ToRelativePostion(adornedElementPosition),
                    //isTemp = IsTemp,
                });
            }

            DrawLineElements(pointElements);
        }

        private void StartToEndRelativeMaxPoint(RowControlProperty start, RowControlProperty end, ref Point maxPoint)
        {
            var rowLists = TreeViewRowControlHelper.FindRowControlsBetween(treeViewControl, start.RowControl, end.RowControl, true);

            double maxX = 0;
            Point current = new Point();

            foreach (var row in rowLists)
            {
                if (GetTreeViewItemEndPoint(row, ref current))
                {
                    if (current.X > maxX)
                    {
                        maxX = current.X;
                        maxPoint = current;
                    }
                }
            }
        }


        /// <summary>
        /// RowControl里面文本最右中心点坐标
        /// </summary>
        /// <param name="rowControl"></param>
        /// <param name="Point"></param>
        /// <returns></returns>
        private bool GetTreeViewItemEndPoint(RowControl rowControl, ref Point point)
        {
            var textBlock = BlockTreeHelper.FindChild<TextBlock>(rowControl);
            if (textBlock == null)
                return false;

            // 确保控件已布局过
            if (textBlock.ActualWidth == 0 || textBlock.ActualHeight == 0)
            {
                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            double width = textBlock.ActualWidth;
            double height = textBlock.ActualHeight;
            Point rightMiddlePoint = new Point(width, height / 2);

            // 转换到 treeViewControl 的坐标系
            var transform = textBlock.TransformToAncestor(treeViewControl);
            point = transform.Transform(rightMiddlePoint);
            return true;
        }


        /// <summary>
        ///  装饰器 相对于TreeView 坐标转换
        /// </summary>
        /// <param name="adornedElementPosition"></param>
        /// <returns></returns>
        private bool AdornedElementToTreeViewPoint(ref Point adornedElementPosition)
        {
            if (AdornedElement == null) return false;
            //var treeViewControl = BlockTreeHelper.FindAncestor<TreeViewControl>(AdornedElement);
            //if (treeViewControl == null) return false;

            GeneralTransform transform = AdornedElement.TransformToAncestor(treeViewControl);
            adornedElementPosition = transform.Transform(new Point(0, 0));
            return true;
        }


        private bool GetTreeViewItemEndPoint(BlockTreeView BlockTreeView, RowControl rowControl, ref Point Point)
        {
            //var textBlock = BlockTreeHelper.FindChild<TextBlock>(rowControl);
            //var height = textBlock.ActualHeight;
            //var width = textBlock.ActualWidth;

            var height = rowControl.ActualHeight;
            var width = rowControl.ActualWidth;

            Point rightMiddlePoint = new Point(width, height / 2);
            var generalTransform = rowControl.TransformToAncestor(treeViewControl);
            Point = generalTransform.Transform(rightMiddlePoint);
            return true;
        }




        public void DrawLineElements(IEnumerable<PointElement> points)
        {
            List<PointElement> PointElements = new List<PointElement>();
            lineElement = new LineElement(points, this);
            lineElement.DisposeAdorner -= DisposeLineElement;
            lineElement.DisposeAdorner += DisposeLineElement;
            lineElements.Add(lineElement);

            //if (StartTreeView != null)
            //{
            //    if (ReDrawing)
            //    {
            //        var existLinklineElement = StartTreeView.LinkCollection.FirstOrDefault(o => o.AdornerGuid == this.AdornerGuid);
            //        if (existLinklineElement != null)
            //        {
            //            int index = StartTreeView.LinkCollection.IndexOf(existLinklineElement);
            //            if (index >= 0)
            //            {
            //                StartTreeView.LinkCollection[index] = lineElement;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        StartTreeView.LinkCollection.AddRange(lineElements);
            //    }
            //}
            AddVisualChild(lineElement);
        }

        public void DisposeLineElement(LineElement lineElement)
        {
            lineElement.ClearElement();
            this.Dispose();
            TreeNodeAdornerHelper.RemoveAdorner(treeViewControl,this);
        }

        public void ClearTemp()
        {
            for (int i = lineElements.Count - 1; i >= 0; i--)
            {
                if (lineElements[i].isTemplate)
                {
                    lineElements.RemoveAt(i);
                }
            }
        }

        public void Dispose()
        {
            ClearLines();
        }

        public void ClearLines()
        {
            foreach (var lineElement in lineElements)
            {
                RemoveVisualChild(lineElement);
            }

            lineElements.Clear();
            InvalidateVisual();
        }


        #region override
        protected override int VisualChildrenCount => lineElements.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index >= 0 && index < lineElements.Count)
            {
                return lineElements[index];
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var lineElement in lineElements)
            {
                lineElement.Arrange(new Rect(new Point(0, 0), finalSize)); // 布局每条线
            }
            return finalSize;
        }

        public override string ToString()
        {
            return $"{startRowControl.RowContent} --->{endRowControl.RowContent}";
        }
        #endregion
    }

    public class PointElement
    {
        [JsonProperty("StartPoint")]
        public Point StartPoint { get; set; }
        [JsonProperty("EndPoint")]
        public Point EndPoint { get; set; }
        [JsonProperty("IsArrow")]
        public bool IsArrow { get; set; }
        [JsonProperty("isTemp")]
        public bool isTemp { get; set; }
    }

    public class RowControlProperty
    {
        public RowControl RowControl { get; set; }
        public double TextWidth { get; set; }
        public double TextHeight { get; set; }
        public String RowContent{ get; set; }


        public RowControlProperty(RowControl rowControl,string rowContent ,double textWidht, double textHeight)
        {
            RowControl = rowControl;
            RowContent = rowContent;
            TextWidth = textWidht;
            TextHeight = textHeight;
        }

        public override string? ToString()
        {
            return $"{RowContent} Height: {TextHeight}  Width: {TextWidth}";
        }
    }





}
