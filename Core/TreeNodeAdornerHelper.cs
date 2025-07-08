using DevExpress.Utils.Design;
using DevExpress.Xpf.ExpressionEditor.Native;
using DevExpress.Xpf.Grid;
using DevExpress.XtraTreeList;
using DevTreeview.Adorner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DevTreeview.Core
{
    public static class TreeNodeAdornerHelper
    {
        private const int MiniAdornerInterval = 10;

        public static List<System.Windows.Documents.Adorner> GetTreeNodeAdorner(DependencyObject obj)
        {
            return (List<System.Windows.Documents.Adorner>)obj.GetValue(TreeNodeAdornerProperty);
        }

        public static void SetTreeNodeAdorner(DependencyObject obj, List<System.Windows.Documents.Adorner> value)
        {
            obj.SetValue(TreeNodeAdornerProperty, value);
        }

        // Using a DependencyProperty as the backing store for TreeNodeAdorner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeNodeAdornerProperty =
            DependencyProperty.RegisterAttached("TreeNodeAdorner", typeof(List<System.Windows.Documents.Adorner>), typeof(TreeNodeAdornerHelper), new PropertyMetadata(null));


        public static void AddAdorner(TreeViewControl treeViewControl, RowControlProperty startRowControl, RowControlProperty endRowControl,UIElement uIElement)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(treeViewControl);
            if (adornerLayer == null) return;
            var treeNodeAdorner = new TreeNodeAdorner
            (
               startRowControl,
               endRowControl,
               treeViewControl
            );
            adornerLayer.Add(treeNodeAdorner);

            // 先取已有列表
            var adorners = GetTreeNodeAdorner(treeViewControl);
            if (adorners == null)
            {
                adorners = new List<System.Windows.Documents.Adorner>();
            }
            adorners.Add(treeNodeAdorner);
            SetTreeNodeAdorner(treeViewControl, adorners);
            RedrawAdorners(treeViewControl, false);
        }

        public static void RemoveAdorner(TreeViewControl treeViewControl, TreeNodeAdorner adornerToRemove)
        {
            if (treeViewControl == null || adornerToRemove == null) return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(treeViewControl);
            if (adornerLayer == null) return;

            var adorners = GetTreeNodeAdorner(treeViewControl);
            if (adorners == null) return;

            if (adorners.Remove(adornerToRemove))
            {
                adornerLayer.Remove(adornerToRemove);
                RedrawAdorners(treeViewControl,false);
                SetTreeNodeAdorner(treeViewControl, adorners);
            }
        }

        public static void ClearAllAdorners(TreeViewControl treeViewControl)
        {
            if (treeViewControl == null) return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(treeViewControl);
            if (adornerLayer == null) return;

            var adorners = GetTreeNodeAdorner(treeViewControl);
            if (adorners == null) return;

            foreach (var adorner in adorners)
            {
                adornerLayer.Remove(adorner);
            }

            adorners.Clear();
            // 清空附加属性
            SetTreeNodeAdorner(treeViewControl, null);
        }

        public static bool HasSameStartAdorners(TreeViewControl treeViewControl, RowControlProperty startRowControl)
        {
            var adorners = GetTreeNodeAdorner(treeViewControl);
            if (adorners == null || !adorners.Any()) return false;

            foreach (var treeNodeAdorner in adorners)
            {
                if (treeNodeAdorner is TreeNodeAdorner adorner)
                {
                    if (adorner.startRowControl.TreeListNode.Equals(startRowControl.TreeListNode))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static double ReDrawSameStartAdorners(TreeViewControl treeViewControl, RowControlProperty startRowControl, double LinkEndPointX)
        {
            var adorners = GetTreeNodeAdorner(treeViewControl);
            if (adorners == null || !adorners.Any()) return 0;
            List<double> pointX = new List<double>();
            pointX.Add(LinkEndPointX);

            foreach (var treeNodeAdorner in adorners)
            {
                if (treeNodeAdorner is TreeNodeAdorner adorner)
                {
                    if (!adorner.startRowControl.TreeListNode.Equals(startRowControl.TreeListNode))
                    {
                        continue;
                    }
                    pointX.Add(adorner.LinkEndPointX);
                }
            }

            var calvalue = pointX.Max();
            foreach (var treeNodeAdorner in adorners)
            {
                if (treeNodeAdorner is TreeNodeAdorner adorner)
                {
                    if (!adorner.startRowControl.TreeListNode.Equals(startRowControl.TreeListNode))
                    {
                        continue;
                    }

                    RedrawAdorner(treeViewControl, adorner, calvalue);
                }
            }
            return calvalue;
        }


        public static void RedrawAdorners(TreeViewControl treeViewControl,bool isByExpandar)
        {
            var adorners =  GetTreeNodeAdorner(treeViewControl);
            if(adorners == null || !adorners.Any()) return;

            if (isByExpandar)
            {
                foreach (var treeNodeAdorner in adorners)
                {
                    if (treeNodeAdorner is TreeNodeAdorner adorner)
                    {
                        adorner.ReCalLinkMaxWidth();
                    }
                }
            }

            var maxXByStartNode = adorners
            .OfType<TreeNodeAdorner>()
            .GroupBy(a => a.startRowControl.TreeListNode)
            .ToDictionary(
                g => g.Key, // TreeListNode
                g => {
                    var maxX = g.Max(a => a.LinkEndPointX);
                    var skip = TreeViewRowControlHelper.GetStartEndSkip(treeViewControl,g.First());
                    return new AdornerDroupDrawInfo(maxX, skip);
                });

            AdjustAdornerDrawInfoSpacing(maxXByStartNode, MiniAdornerInterval);

            foreach (var treeNodeAdorner in adorners)
            {
                if (treeNodeAdorner is TreeNodeAdorner adorner)
                {
                    maxXByStartNode.TryGetValue(adorner.startRowControl.TreeListNode, out AdornerDroupDrawInfo adornerDroupDrawInfo);
                    RedrawAdorner(treeViewControl, adorner, adornerDroupDrawInfo.MaxLinkEndPointX);
                }
            }
        }

        public static void AdjustAdornerDrawInfoSpacing(
                Dictionary<TreeListNode, AdornerDroupDrawInfo> infoDict,
                double minSpacing = MiniAdornerInterval)
        {
            // Step 1: 按 Skip 值从小到大排序
            var sorted = infoDict
                .OrderBy(kv => kv.Value.StartEndSkip)
                .ToList();

            double currentX = 0;

            for (int i = 0; i < sorted.Count; i++)
            {
                var node = sorted[i].Key;
                var info = sorted[i].Value;

                if (i == 0)
                {
                    currentX = info.MaxLinkEndPointX;
                }
                else
                {
                    if (info.MaxLinkEndPointX - currentX < minSpacing)
                    {
                        info.MaxLinkEndPointX = currentX + minSpacing;
                    }

                    currentX = info.MaxLinkEndPointX;
                }

                // 更新到原字典
                infoDict[node] = info;
            }
        }


        public static void RedrawAdorner(TreeViewControl treeViewControl, TreeNodeAdorner adorner, double calLinkEndPointX = 0)
        {
            var startNode = adorner.startRowControl.TreeListNode;
            var endNode = adorner.endRowControl.TreeListNode;
            var flatList = TreeViewRowControlHelper.FlattenTree(treeViewControl);
            int startIndex = flatList.IndexOf(startNode);
            int endIndex = flatList.IndexOf(endNode);

            if (startIndex == -1 || endIndex == -1)
            {
                return;
            }

            var startFirstUnexpanded = TreeViewRowControlHelper.FindFirstUnexpandedParentNode(treeViewControl, startNode) ?? startNode;
            var endFirstUnexpanded = TreeViewRowControlHelper.FindFirstUnexpandedParentNode(treeViewControl, endNode) ?? endNode;
            adorner.ReDrawByNode(startFirstUnexpanded, endFirstUnexpanded, calLinkEndPointX);
        }
    }

    public class AdornerDroupDrawInfo
    {
        public double MaxLinkEndPointX { get; set; }
        public int StartEndSkip { get; set; }

        public AdornerDroupDrawInfo(double maxLinkEndPointX, int startEndSkip)
        {
            MaxLinkEndPointX = maxLinkEndPointX;
            StartEndSkip = startEndSkip;
        }
    }


}
