using DevExpress.Xpf.Grid;
using DevExpress.XtraTreeList;
using DevTreeview.Adorner;
using System;
using System.Collections.Generic;
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





    }
}
