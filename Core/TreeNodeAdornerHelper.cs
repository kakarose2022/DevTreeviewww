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
            //Trace.WriteLine(TreeViewRowControlHelper.GetRowControlContent(expanderChildren?.FirstOrDefault()));
            //Trace.WriteLine(TreeViewRowControlHelper.GetRowControlContent(expanderChildren?.LastOrDefault()));

            //var startIndex = TreeViewRowControlHelper.GetLogicalRowIndex(treeViewControl, expanderChildren?.FirstOrDefault());
            //var endIndex = TreeViewRowControlHelper.GetLogicalRowIndex(treeViewControl, expanderChildren?.LastOrDefault());

            //foreach (var adorner in adorners)
            //{
            //    if (adorner is TreeNodeAdorner treeNodeAdorner)
            //    {
            //        var startRootControl = TreeViewRowControlHelper.GetRootRowControl(treeNodeAdorner.startRowControl.RowControl, treeViewControl);
            //        var endRootControl = TreeViewRowControlHelper.GetRootRowControl(treeNodeAdorner.endRowControl.RowControl, treeViewControl);

            //        var a = startRootControl.ToControlContent();
            //        var b = endRootControl.ToControlContent();


            //        // 装饰器  完全包含 折叠项
            //        if (treeNodeAdorner.StartRowControlIndex < startIndex
            //            && treeNodeAdorner.EndRowControlIndex > endIndex)
            //        {
            //            treeNodeAdorner.ReDraw(null, isExpandar ? null : endRootControl);
            //        }
            //        // 折叠项包含 装饰器尾部
            //        else if (treeNodeAdorner.EndRowControlIndex >= startIndex
            //            && treeNodeAdorner.EndRowControlIndex <= endIndex)
            //        {

            //            if (treeNodeAdorner.StartRowControlIndex >= startIndex
            //            && treeNodeAdorner.StartRowControlIndex <= endIndex)
            //            {
            //                treeNodeAdorner.ReDrawEmpty(isExpandar);
            //            }
            //            else
            //            {
            //                treeNodeAdorner.ReDraw(null, isExpandar ? null : endRootControl);
            //            }
            //        }
            //        // 折叠项包含 装饰器头部
            //        else if (treeNodeAdorner.StartRowControlIndex >= startIndex
            //            && treeNodeAdorner.StartRowControlIndex <= endIndex
            //            )
            //        {

            //            treeNodeAdorner.ReDraw(isExpandar ? null : startRootControl, null);
            //        }
            //    }
            //}


            //return;



            var startExpandarNode = TreeViewRowControlHelper.GetNodeByRowControl(expanderChildren?.FirstOrDefault());
            var endExpandarNode = TreeViewRowControlHelper.GetNodeByRowControl(expanderChildren?.LastOrDefault());

            foreach (var treeNodeAdorner in adorners)
            {
                if (treeNodeAdorner is TreeNodeAdorner adorner)
                {
                    var startRow = adorner.startRowControl.RowControl;
                    var endRow = adorner.endRowControl.RowControl;
                    var aa = startRow.ToControlContent();
                    var bb = endRow.ToControlContent();


                    var startNode = TreeViewRowControlHelper.GetNodeByRowControl(startRow);
                    var endNode = TreeViewRowControlHelper.GetNodeByRowControl(endRow);
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
                    var startRowControlNew = TreeViewRowControlHelper.GetRowControlByNode(treeViewControl, startFirstUnexpanded);
                    var endRowControlNew = TreeViewRowControlHelper.GetRowControlByNode(treeViewControl, endFirstUnexpanded);

                    // UpperPartContain
                    if (startIndex < expandStartIndex
                        && endIndex >= expandStartIndex
                        && endIndex <= expandEndIndex)
                    {
                        adorner.ReDraw(startRowControlNew, endRowControlNew);
                    }
                    //FullContain
                    //else if (startIndex > expandStartIndex
                    //    && TreeViewElementFinder.IsSmall(treeviewEx, endExpandar, adorner.EndTreeViewExItem, false))
                    //{
                    //    adorner.RedrawByExpandar(
                    //                              startFirstNoExpandarItem,
                    //                              endFirstNoExpandarItem
                    //                            );
                    //}
                    //// LowerPartContain
                    else if (startIndex >= expandStartIndex && startIndex <= expandEndIndex && endIndex > expandEndIndex)
                    {
                        var fallbackStart = isExpandar
                            ? startRowControlNew
                            : TreeViewRowControlHelper.GetRowControlByNode(treeViewControl, startExpandarNode);

                        adorner.ReDraw(fallbackStart, endRowControlNew);
                    }
                    // FullContained
                    else if (startIndex >= expandStartIndex && endIndex <= expandEndIndex)
                    {
                        adorner.ReDraw(startRowControlNew, endRowControlNew);
                    }
                    else
                    {
                        adorner.ReDraw(null, null);
                    }
                }
            }




        }

    }
}
