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


        public static void AddLine(TreeViewControl treeViewControl, RowControlProperty startRowControl, RowControlProperty endRowControl)
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

            // 保存回附加属性
            SetTreeNodeAdorner(treeViewControl, adorners);
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


        public static void RedrawAdorners
            (
            TreeViewControl treeViewControl,
            IEnumerable<RowControl> expanderChildren ,
            RowControl patientRowControl,
            bool isExpandar
            )
        {
            var adorners =  GetTreeNodeAdorner(treeViewControl);
            if(adorners == null || !adorners.Any()) return;

            // 折叠的项目
            var startExpandarNode = TreeViewRowControlHelper.GetNodeByRowControl(expanderChildren?.FirstOrDefault());
            var endExpandarNode = TreeViewRowControlHelper.GetNodeByRowControl(expanderChildren?.LastOrDefault());

            foreach (var treeNodeAdorner in adorners)
            {
                if (treeNodeAdorner is TreeNodeAdorner adorner)
                {
                    //var startRow = adorner.startRowControl.RowControl;
                    //var endRow = adorner.endRowControl.RowControl;
                    //var startNode = TreeViewRowControlHelper.GetNodeByRowControl(startRow);
                    //var endNode = TreeViewRowControlHelper.GetNodeByRowControl(endRow);

                    var startNode = adorner.startRowControl.TreeListNode;
                    var endNode= adorner.endRowControl.TreeListNode;

                    if (startNode == null || endNode == null)
                        continue;

                    var flatList = TreeViewRowControlHelper.FlattenTree(treeViewControl);
                    int startIndex = flatList.IndexOf(startNode);
                    int endIndex = flatList.IndexOf(endNode);
                    int expandStartIndex = flatList.IndexOf(startExpandarNode);
                    int expandEndIndex = flatList.IndexOf(endExpandarNode);

                    if (startIndex == -1 || endIndex == -1 || expandStartIndex == -1 || expandEndIndex == -1)
                        continue;

                    var startFirstUnexpanded = TreeViewRowControlHelper.FindFirstUnexpandedParentNode(treeViewControl, startNode) ?? startNode;
                    var endFirstUnexpanded = TreeViewRowControlHelper.FindFirstUnexpandedParentNode(treeViewControl, endNode) ?? endNode;
                    //var startRowControlNew = TreeViewRowControlHelper.GetRowControlByNode(treeViewControl, startFirstUnexpanded);
                    //var endRowControlNew = TreeViewRowControlHelper.GetRowControlByNode(treeViewControl, endFirstUnexpanded);

                    adorner.ReDrawByNode(startFirstUnexpanded, endFirstUnexpanded);
                   
                    //// UpperPartContain
                    //if (startIndex < expandStartIndex
                    //    && endIndex >= expandStartIndex
                    //    && endIndex <= expandEndIndex)
                    //{
                    //    adorner.ReDraw(startFirstUnexpanded, endFirstUnexpanded);
                    //}
                    ////FullContain
                    ////else if (startIndex > expandStartIndex
                    ////    && TreeViewElementFinder.IsSmall(treeviewEx, endExpandar, adorner.EndTreeViewExItem, false))
                    ////{
                    ////    adorner.RedrawByExpandar(
                    ////                              startFirstNoExpandarItem,
                    ////                              endFirstNoExpandarItem
                    ////                            );
                    ////}
                    ////// LowerPartContain
                    //else if (startIndex >= expandStartIndex && startIndex <= expandEndIndex && endIndex > expandEndIndex)
                    //{
                    //    var fallbackStart = !isExpandar
                    //        ? startFirstUnexpanded
                    //        : startExpandarNode;

                    //    adorner.ReDraw(fallbackStart, endFirstUnexpanded);
                    //}
                    //// FullContained
                    //else if (startIndex > expandStartIndex
                    //    && endIndex < expandEndIndex)
                    //{
                    //    adorner.ReDraw(startFirstUnexpanded, endFirstUnexpanded);
                    //}
                    //else
                    //{
                    //    adorner.ReDraw(startFirstUnexpanded, endFirstUnexpanded);
                    //}
                }
            }
        }
    }
}
