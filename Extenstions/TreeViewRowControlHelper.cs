using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeView;
using DevTreeview.Adorner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

public static class TreeViewRowControlHelper
{
    private static readonly Dictionary<TreeViewControl, List<RowControl>> _rowControlCache = new();


    public static List<RowControl> GetTreeViewControlRowControls(TreeViewControl treeViewControl)
    {
        if (treeViewControl == null)
            return new List<RowControl>();

        var found = FindVisualChildren<RowControl>(treeViewControl).ToList();

        _rowControlCache[treeViewControl] = found;
        return found;
    }

    public static List<RowControl> GetCachedRowControls(TreeViewControl treeViewControl)
    {
        if (treeViewControl == null)
            return new List<RowControl>();

        var found = FindVisualChildren<RowControl>(treeViewControl).ToList();

        _rowControlCache[treeViewControl] = found;
        return found;
    }

    public static void ClearRowControlsCache(TreeViewControl treeViewControl)
    {
        if (treeViewControl != null)
            _rowControlCache.Remove(treeViewControl);
    }

    public static void ClearAllCache()
    {
        _rowControlCache.Clear();
    }

    // ---------- 查找、转换 ----------
    public static RowControl? GetRowControlByNode(TreeViewControl treeViewControl, TreeListNode node)
    {
        var rowControls = GetCachedRowControls(treeViewControl);
 
        return rowControls.FirstOrDefault(r =>
        {
            var rowData = r.DataContext as TreeViewRowData;
            return rowData?.Node == node;
        });
    }

    public static TreeListNode? GetNodeByRowControl(RowControl rowControl)
    {
        return (rowControl.DataContext as TreeViewRowData)?.Node;
    }


    public static List<TreeListNode> FlattenTree(TreeViewControl treeView)
    {
        var result = new List<TreeListNode>();

        void Traverse(TreeListNode node)
        {
            result.Add(node);
            foreach (var child in node.Nodes)
                Traverse(child);
        }

        foreach (var root in treeView.Nodes)
            Traverse(root);

        return result;
    }

    public static int GetLogicalRowIndex(TreeViewControl treeViewControl, RowControl rowControl)
    {
        if (rowControl.DataContext is not TreeViewRowData rowData)
            return -1;

        var node = rowData.Node;
        var flatList = FlattenTree(treeViewControl);

        return flatList.IndexOf(node); // 这个 index 是稳定的
    }

    public static int GetStartEndSkip(TreeViewControl treeViewControl, TreeNodeAdorner treeNodeAdorner)
    {
        var startIndex = treeNodeAdorner.StartRowControlIndex;
        var endIndex = treeNodeAdorner.EndRowControlIndex;

        return Math.Abs(startIndex - endIndex);
    }



    //public static int GetRowControlIndex(TreeViewControl treeViewControl,RowControl rowControl)
    //{
    //    var cached = GetCachedRowControls(treeViewControl);
    //    var index = cached.IndexOf(rowControl);

    //    return index;
    //}

    public static RowControl? GetRowControlByIndex(int index, TreeViewControl treeViewControl)
    {
        var cached = GetCachedRowControls(treeViewControl);

        if (index >= 0 && index < cached.Count)
            return cached[index];
        return null;
    }

    public static List<TreeListNode> GetAllChildNodes(TreeListNode node)
    {
        var result = new List<TreeListNode>();
        foreach (var child in node.Nodes)
        {
            result.Add(child);
            result.AddRange(GetAllChildNodes(child));
        }
        return result;
    }

    public static List<RowControl> GetAllChildRowControls(TreeViewControl treeViewControl, TreeListNode parentNode)
    {
        var result = new List<RowControl>();
        var allChildNodes = GetAllChildNodes(parentNode);
        var allRowControls = GetCachedRowControls(treeViewControl);

        foreach (var node in allChildNodes)
        {
            var match = allRowControls.FirstOrDefault(r =>
            {
                var rowData = r.DataContext as TreeViewRowData;
                return rowData?.Node == node;
            });

            if (match != null)
                result.Add(match);
        }

        return result;
    }



    public static RowControl? GetRootRowControl(RowControl rowControl, TreeViewControl treeViewControl)
    {
        if (rowControl.DataContext is not TreeViewRowData rowData)
            return null;

        TreeListNode currentNode = rowData.Node;

        // 向上找直到根节点
        while (currentNode.ParentNode != null)
            currentNode = currentNode.ParentNode;

        // 找到根节点后，再找对应的 RowControl
        return GetRowControlByNode(treeViewControl, currentNode);
    }

    /// <summary>
    /// 查找包含目标节点的第一个未展开的父节点（从根开始）
    /// </summary>
    /// <param name="treeView">TreeViewControl</param>
    /// <param name="targetNode">目标节点</param>
    /// <returns>第一个未展开且包含目标节点的父节点</returns>
    public static TreeListNode? FindFirstUnexpandedParentNode(TreeViewControl treeView, TreeListNode targetNode)
    {
        foreach (var root in treeView.Nodes)
        {
            var result = FindInNode(root, targetNode);
            if (result != null)
                return result;
        }

        return null;
    }

    private static TreeListNode? FindInNode(TreeListNode currentNode, TreeListNode targetNode)
    {
        if (ContainsNode(currentNode, targetNode))
        {
            if (!currentNode.IsExpanded)
                return currentNode;

            foreach (var child in currentNode.Nodes)
            {
                var result = FindInNode(child, targetNode);
                if (result != null)
                    return result;
            }
        }

        return null;
    }

    private static bool ContainsNode(TreeListNode parent, TreeListNode target)
    {
        if (parent == null || target == null)
            return false;

        foreach (var child in parent.Nodes)
        {
            if (child == target)
                return true;

            if (ContainsNode(child, target))
                return true;
        }

        return false;
    }



    public static string ToControlContent(this RowControl rowControl)
    {
       if(rowControl == null)
        {
            return string.Empty;
        }
       var dc= rowControl.DataContext as TreeViewRowData;
        return dc.Node.Content.ToString();
    }


    public static IEnumerable<RowControl> FindRowControlsBetween(
        TreeViewControl treeViewControl,
        RowControl startRowControl,
        RowControl endRowControl,
        bool visibleOnly)
    {
        var rowControls = GetCachedRowControls(treeViewControl);

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
