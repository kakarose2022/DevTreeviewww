using DevExpress.Xpf.Grid;
using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DevTreeview.Extenstions
{
    public class TreeViewRowControlHelper
    {

        public static IEnumerable<RowControl> FindRowControlsBetween(
            TreeViewControl treeViewControl,
            RowControl endRowControl,
            RowControl startRowControl,
            bool visibleOnly)
        {
            if (treeViewControl == null || startRowControl == null || endRowControl == null)
                yield break;

            var rowControls = FindVisualChildren<RowControl>(treeViewControl).ToList();

            // 排除不可见行（根据需要）
            if (visibleOnly)
                rowControls = rowControls.Where(r => r.IsVisible).ToList();

            int startIndex = rowControls.IndexOf(startRowControl);
            int endIndex = rowControls.IndexOf(endRowControl);

            if (startIndex == -1 || endIndex == -1)
                yield break;

            if (startIndex > endIndex)
            {
                var temp = startIndex;
                startIndex = endIndex;
                endIndex = temp;
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                yield return rowControls[i];
            }
        }



        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }





    }
}
