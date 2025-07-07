using DevExpress.Xpf.Grid;
using DevTreeview.Core;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace DevTreeview.Adorner
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DrawLineElementAdorner : System.Windows.Documents.Adorner, IDisposable
    {
        private LineElement lineElement;
        public bool IsTemp;
        public Guid AdornerGuid;
        //public TreeViewExItem StartTreeViewExItem { get; set; }
        //public TreeViewExItem EndTreeViewExItem { get; set; }
        //public TreeViewEx TreeViewEx{ get; set; }

        public UIElement StartTreeViewExItem { get; set; }
        public UIElement EndTreeViewExItem { get; set; }


        public TreeViewControl treeViewControl { get; set; }

        public BlockTreeView StartTreeView { get; set; }
        public BlockTreeView EndTreeView { get; set; }
        private double OffSetX = 20;
        private double LineSpace = 9;
        private bool ReDrawing = false;

        [JsonProperty]
        public double LinkMaxWidth;
        [JsonProperty]
        public List<LineElement> lineElements { get;  }


        public DrawLineElementAdorner(UIElement adornedElement,UIElement startItem , UIElement endItem) :base(startItem)
        {

            StartTreeViewExItem = startItem;
            EndTreeViewExItem = endItem;

            if (startItem.DataContext is BlockTreeView startTreeView)
            {
                StartTreeView = startTreeView;
            }

            if (endItem.DataContext is BlockTreeView endTreeView)
            {
                EndTreeView = endTreeView;
            }

            TreeViewEx = adornedElement as TreeViewEx;
            IsTemp = isTemp;
            lineElements = new List<LineElement>();
            AdornerGuid = Guid.NewGuid();
            ClearTemp();
            CalAndDrawLinePoint();
            InvalidateVisual();

        }

        public DrawLineElementAdorner(UIElement adornedElement, TreeViewExItem startTreeViewExItem, TreeViewExItem endTreeViewExItem,bool isTemp = false) : base(startTreeViewExItem)
        {
            StartTreeViewExItem = startTreeViewExItem;
            EndTreeViewExItem = endTreeViewExItem;
            if(startTreeViewExItem.DataContext is BlockTreeView startTreeView)
            {
                StartTreeView = startTreeView;
            }

            if (endTreeViewExItem.DataContext is BlockTreeView endTreeView)
            {
                EndTreeView = endTreeView;
            }

            TreeViewEx = adornedElement as TreeViewEx;
            IsTemp = isTemp;
            lineElements = new List<LineElement>();
            AdornerGuid = Guid.NewGuid();
            ClearTemp();
            CalAndDrawLinePoint();
            InvalidateVisual();
        }

        public DrawLineElementAdorner(UIElement adornedElement, IEnumerable<PointElement> pointElements) : base(adornedElement)
        {
            lineElements = new List<LineElement>();
            DrawLineElements(pointElements);
            InvalidateVisual();
        }

        #region Normal Draw Links

        /// <summary>
        /// 装饰器描绘
        /// </summary>
        /// <param name="startExpandarItem">重新计算的开始装饰器</param>
        /// <param name="endExpandarItem">重新计算的终点装饰器</param>
        public void CalAndDrawLinePoint
            (
            TreeViewExItem startExpandarItem = null,
            TreeViewExItem endExpandarItem = null
            )
        {
            double startWidth = StartTreeView.ActualWidth;
            double endWidth = EndTreeView.ActualWidth;
            Point startPoint = new Point();
            Point endPoint = new Point();

            if (startExpandarItem != null 
                && endExpandarItem != null 
                && startExpandarItem == endExpandarItem)
            {
                return;
            }

            var realStartTreeViewItem = startExpandarItem ?? StartTreeViewExItem;
            var realEndTreeViewItem = endExpandarItem ?? EndTreeViewExItem;

            GetTreeViewItemEndPoint(StartTreeView, realStartTreeViewItem, ref startPoint);
            GetTreeViewItemEndPoint(EndTreeView, realEndTreeViewItem, ref endPoint);

            //Trace.WriteLine($"startPoint.X {startPoint.X} startPoint.Y {startPoint.Y}");
            //Trace.WriteLine($"endPoint.X {endPoint.X} endPoint.Y {endPoint.Y}");
            //Trace.WriteLine($"=====================================");

            if (ReDrawing == false)
            {
                var startTreeviewExItem = realStartTreeViewItem;
                var endTreeviewExItem = realEndTreeViewItem;

                // 中间最大的
                double calMinUiWidth = 0;
                double sameStartUiWidth = 0;
                bool hasSameStartAdorner = true;
                UpdateExistAdornerLines(ref calMinUiWidth, ref sameStartUiWidth, ref hasSameStartAdorner, startTreeviewExItem, endTreeviewExItem);

                var notSameStartAdorners = TreeViewEx.DrawLineElementAdorners.Where(o => o.StartTreeViewExItem != StartTreeViewExItem)
                            .Select(o => o.LinkMaxWidth)
                            .Distinct()
                            .OrderBy(width => width)
                            .ToList();

                NoRepeatWithOthers(notSameStartAdorners, ref calMinUiWidth);
                LinkMaxWidth = ReDrawing ? LinkMaxWidth : (hasSameStartAdorner ? Math.Max(calMinUiWidth, sameStartUiWidth) : calMinUiWidth);
            }

            Trace.WriteLine(this.ToString());
            var startLink = LinkMaxWidth + OffSetX + startPoint.X  - startWidth;
            Point adornedElementPosition = new Point();
            if(AdornedElementToTreeViewPoint(ref adornedElementPosition))
            {
                var pointElements = new List<PointElement>();
                //横线
                pointElements.Add(new PointElement()
                {
                    StartPoint = new Point(startPoint.X, startPoint.Y).ToRelativePostion(adornedElementPosition),
                    EndPoint = new Point(startLink, startPoint.Y).ToRelativePostion(adornedElementPosition),
                    isTemp = IsTemp,
                });

                pointElements.Add(new PointElement()
                {
                    StartPoint = new Point(endPoint.X, endPoint.Y).ToRelativePostion(adornedElementPosition),
                    EndPoint = new Point(startLink, endPoint.Y).ToRelativePostion(adornedElementPosition),
                    isTemp = IsTemp,
                    IsArrow = true
                });

                //竖线
                pointElements.Add(new PointElement()
                {
                    StartPoint = new Point(startLink, startPoint.Y).ToRelativePostion(adornedElementPosition),
                    EndPoint = new Point(startLink, endPoint.Y).ToRelativePostion(adornedElementPosition),
                    isTemp = IsTemp,
                });

                DrawLineElements(pointElements);
            }
        }

        public void UpdateExistAdornerLines(ref double CalMinUiWidth , ref double SameStartWidth  ,ref bool HasSameStartAdorner,TreeViewExItem ExpandarStartTreeViewExItem = null, TreeViewExItem ExpandarEndTreeViewExItem = null)
        {
            var startTreeviewExItem = ExpandarStartTreeViewExItem != null ? ExpandarStartTreeViewExItem : StartTreeViewExItem;
            var endTreeviewExItem = ExpandarEndTreeViewExItem != null ? ExpandarEndTreeViewExItem : EndTreeViewExItem;

            // 中间最大的
            double maxWidth = 0;
            double startWidth = StartTreeView.ActualWidth;
            double endWidth = EndTreeView.ActualWidth;
            StartToEndRelativeMaxWidth(startTreeviewExItem, endTreeviewExItem, ref maxWidth);

            //刚计算的值 ?？
            // 计算出来的不和Ui重合的最小宽度。。
            CalMinUiWidth = new List<Double>() { startWidth, endWidth, maxWidth }.Max();
            var maxWidthsDict = TreeViewEx.DrawLineElementAdorners
                                .GroupBy(o => o.StartTreeViewExItem)
                                .ToDictionary(g => g.Key, g => g.Max(o => o.LinkMaxWidth));

            // 现有起始点的已有的宽度
            double sameStartMaxWidth = 0;
            var hasSameItem = maxWidthsDict.TryGetValue(StartTreeViewExItem, out sameStartMaxWidth);
            if (ReDrawing != true)
            {
                if (hasSameItem)
                {
                    SameStartWidth = Math.Max(CalMinUiWidth, sameStartMaxWidth);
                    maxWidthsDict[StartTreeViewExItem] = SameStartWidth;
                }
                else
                {
                    maxWidthsDict.Add(StartTreeViewExItem, CalMinUiWidth);
                }
            }

            //更新计算所有装饰器宽度(LinkMaxWidth) 并且重新绘制
            UpdateMaxWidths(maxWidthsDict);
            foreach (var adorner in TreeViewEx.DrawLineElementAdorners)
            //foreach (var adorner in TreeViewEx.DrawLineElementAdorners.Except(new ObservableCollection<DrawLineElementAdorner>() { this} ))
            {
                if (adorner.AdornerGuid == this.AdornerGuid)
                {
                    continue;
                }

                double adornerLinkWidth = 0f;
                var result = maxWidthsDict.TryGetValue(adorner.StartTreeViewExItem, out adornerLinkWidth);
                if (result && adorner.LinkMaxWidth != adornerLinkWidth)
                {
                   adorner.UpdateWidthAndReDraw(adornerLinkWidth);
                }
            }
        }

        public void UpdateDrawContent(TreeViewExItem endTreeViewExItem = null, TreeViewExItem expandarTreeviewItem = null)
        {
            if (endTreeViewExItem != null)
            {
                EndTreeViewExItem = endTreeViewExItem;
            }
            CalAndDrawLinePoint();
        }

        /// <summary>
        /// 计算 开始 --- 结束 这段treeview 之间最大  相对宽度
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="endItem"></param>
        /// <param name="relativeTo"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private bool StartToEndRelativeMaxWidth(TreeViewExItem startNode, TreeViewExItem endNode, ref double maxWidth)
        {
            var allNodes = TreeViewElementFinder.FindBetween(TreeViewEx,startNode,endNode,false).ToList();
            List<double> lengths = new List<double>();
            foreach (var item in allNodes)
            {
                if(item.DataContext is BlockTreeView blockTreeView && item.IsVisible == true)
                {
                    var relativePoint = GetRelativePoint(item) - GetRelativePoint(startNode);
                    lengths.Add(blockTreeView.ActualWidth + relativePoint.X);
                }
            }

            if (lengths.Count >0)
            {
                maxWidth = lengths.Max();
                return true;
            }
            return false;
        }

        private Point GetRelativePoint(TreeViewExItem item)
        {
            var textBlock = BlockTreeHelper.FindChild<TextBlock>(item);
            var generalTransform = textBlock.TransformToAncestor(TreeViewEx);
            return generalTransform.Transform(new Point(0, 0));
        }

        private bool GetTreeViewItemEndPoint(BlockTreeView BlockTreeView, TreeViewExItem treeViewExItem, ref Point Point)
        {
            var textBlock = BlockTreeHelper.FindChild<TextBlock>(treeViewExItem);
            var height = textBlock.ActualHeight;
            var width = textBlock.ActualWidth;

            Point rightMiddlePoint = new Point(width, height / 2);
            var generalTransform = textBlock.TransformToAncestor(TreeViewEx);
            Point = generalTransform.Transform(rightMiddlePoint);
            return true;
        }

        private bool GetItemRelativeWindowEndPoint(BlockTreeView BlockTreeView, TreeViewExItem treeViewExItem, ref Point Point)
        {
            var textBlock = BlockTreeHelper.FindChild<TextBlock>(treeViewExItem);
            // 获取窗口实例
            var window = Window.GetWindow(treeViewExItem);

            if (window == null)
            {
                return false; // 确保窗口存在
            }

            // 转换到窗口坐标系
            var generalTransform = textBlock.TransformToVisual(window);
            Point rightMiddlePoint = new Point(BlockTreeView.ActualWidth, BlockTreeView.ActualHeight / 2);
            Point = generalTransform.Transform(rightMiddlePoint);
            return true;
        }

        public void DrawLineElements(IEnumerable<PointElement> points)
        {
            List<PointElement> PointElements = new List<PointElement>();
            lineElement = new LineElement(points,this);
            lineElement.DisposeAdorner -= DisposeLineElement;
            lineElement.DisposeAdorner += DisposeLineElement;
            lineElements.Add(lineElement);

            if (StartTreeView != null)
            {
                if (ReDrawing)
                {
                    var existLinklineElement = StartTreeView.LinkCollection.FirstOrDefault(o => o.AdornerGuid == this.AdornerGuid);
                    if (existLinklineElement != null)
                    {
                        int index = StartTreeView.LinkCollection.IndexOf(existLinklineElement);
                        if (index >= 0)
                        {
                            StartTreeView.LinkCollection[index] = lineElement;
                        }
                    }
                }
                else
                {
                    StartTreeView.LinkCollection.AddRange(lineElements);
                }
            }
            AddVisualChild(lineElement);
        }
        #endregion

        #region Redrawing by Expandar
        public void RedrawByExpandar( TreeViewExItem startTreeViewItem, TreeViewExItem endTreeViewItem)
        {
            ClearLines();
            CalAndDrawLinePoint(startTreeViewItem, endTreeViewItem);
        }

        #endregion
        public void UpdateWidthAndReDraw(double linkMaxWidth)
        {
            this.LinkMaxWidth = linkMaxWidth;
            ReDrawing = true;
            ClearLines();
            CalAndDrawLinePoint();
            InvalidateVisual();
            ReDrawing = false;
        }

        /// <summary>
        ///  重新计算所有开始的TreeViewExItem的link宽度
        /// </summary>
        /// <param name="dict"></param>
        private void UpdateMaxWidths(Dictionary<TreeViewExItem, double> dict)
        {
            var sortedKeys = dict.Keys.OrderBy(key => dict[key]).ToList();
            for (int i = 0; i < sortedKeys.Count - 1; i++)
            {
                double currentValue = dict[sortedKeys[i]];
                double nextValue = dict[sortedKeys[i + 1]];

                if (Math.Abs(nextValue - currentValue) < 9)
                {
                    dict[sortedKeys[i + 1]] = currentValue + 9;
                }
            }
        }

        /// <summary>
        /// 计算和别的连接线终点不重合
        /// </summary>
        /// <param name="currentWidth"></param>
        /// <param name="target"></param>
        double minlinespace = 0;
        private void NoRepeatWithOthers(List<double> currentWidth, ref double target)
        {
            int index = -1;
            for (int i = 0; i < currentWidth.Count; i++)
            {
                if (Math.Abs(currentWidth[i] - target) < 1)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return; 
            }

            double newTarget = target;
            if(index == 0)
            {
                newTarget += 10;
            }

            if (index > 0)
            {
                newTarget = (currentWidth[index - 1] + currentWidth[index]) / 2;
            }

            if (index < currentWidth.Count - 1)
            {
                newTarget = (currentWidth[index] + currentWidth[index + 1]) / 2;
            }
            target = newTarget;
        }

        /// <summary>
        ///  装饰器 相对于TreeView 坐标转换
        /// </summary>
        /// <param name="adornedElementPosition"></param>
        /// <returns></returns>
        private bool AdornedElementToTreeViewPoint(ref Point adornedElementPosition)
        {
            if (AdornedElement == null) return false;
            var treeView = BlockTreeHelper.FindAncestor<TreeViewEx>(AdornedElement);
            if (treeView == null) return false;

            GeneralTransform transform = AdornedElement.TransformToAncestor(treeView);
            adornedElementPosition = transform.Transform(new Point(0, 0));
            return true;
        }

        private Point ConvertPointToAdorner(Point point, Point adornedElementPosition)
        {
            return new Point(
                point.X - adornedElementPosition.X,
                point.Y - adornedElementPosition.Y
            );
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
            return $"{StartTreeViewExItem.ParentTreeViewItem?.Header}.{StartTreeViewExItem.Header} ---> " +
                $"{EndTreeViewExItem.ParentTreeViewItem?.Header}.{EndTreeViewExItem.Header}";
        }
        #endregion

        public void ClearLines()
        {
            foreach (var lineElement in lineElements)
            {
                RemoveVisualChild(lineElement);
            }

            lineElements.Clear(); 
            InvalidateVisual();  
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

        /// <summary>
        ///  右击菜单删除
        /// </summary>
        /// <param name="lineElement"></param>
        public void DisposeLineElement(LineElement lineElement)
        {
            lineElement.ClearElement();
            this.Dispose();
            TreeViewEx.DrawLineElementAdorners.Remove(this);
        }

        public void Dispose()
        {
            ClearLines();   
            StartTreeView?.LinkCollection.Clear();
        }

        public void DisposeLast()
        {
            ClearLines();
            StartTreeView?.LinkCollection.Remove(StartTreeView?.LinkCollection?.LastOrDefault());
        }
    }

 

    public enum ExpandarType
    {
        None,
        /// <summary>
        /// 装饰器完全包含展开项目
        /// </summary>
        FullContain,
        UpperPartContain,
        LowerPartContain,
        /// <summary>
        /// 装饰器被展开项目完全包含
        /// </summary>
        FullContained
    }
}
