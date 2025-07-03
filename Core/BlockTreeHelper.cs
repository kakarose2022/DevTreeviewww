using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using FlowDirection = System.Windows.FlowDirection;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using Size = System.Windows.Size;

namespace DevTreeview.Core
{
    public class BlockTreeHelper
    {
        public static ReadOnlyObservableCollection<BlockTreeView> BlockTreeViewItems { get; set; }
        public static bool BlockTreeCanLink<T>(T startNode, T endNode) where T : ITreeView<T>
        {
            if(startNode == null || endNode == null)
            {
                Trace.WriteLine("BOOL~~~");
                return false;
            }

            //if (ReferenceEquals(startNode.Patient,endNode.Patient))
            //{
            //    return false;
            //}
            if (startNode.BlockType == BlockType.Input && endNode.BlockType == BlockType.Output
               || startNode.BlockType == BlockType.Output && endNode.BlockType == BlockType.Input)
            {
                return true;
            }
            return false;
        }

        public static Size MeasureString(string text, double fontSize, FontFamily fontFamily)
        {
            var typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1.0
            );

            return new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
        }

        #region  查找节点之间的所有的节点
        //public static List<TreeViewExItem> FindNodesBetween(TreeViewEx treeViewEx, TreeViewExItem startNode, TreeViewExItem endNode)
        //{
        //    // 存储结果的列表
        //    var result = new List<TreeViewExItem>();
        //    bool isBetween = false;

        //    foreach (var item in treeViewEx.Items)
        //    {
        //        if (item is TreeViewExItem treeViewItem)
        //        {
        //            FindNodesBetweenRecursive(treeViewItem, startNode, endNode, ref isBetween, result); // 初始深度为 0
        //        }
        //    }

        //    return result;
        //}

        //private static void FindNodesBetweenRecursive(TreeViewExItem currentNode, TreeViewExItem startNode, TreeViewExItem endNode, ref bool isBetween, List<TreeViewExItem> result)
        //{
        //    if (currentNode == startNode)
        //    {
        //        isBetween = true; // 开始记录
        //    }

        //    // 如果在起点和终点之间，记录当前节点
        //    if (isBetween)
        //    {
        //        if (currentNode != startNode && currentNode != endNode)
        //        {
        //            result.Add(currentNode);
        //        }
        //    }

        //    if (currentNode == endNode)
        //    {
        //        isBetween = false; // 停止记录
        //        return;
        //    }

        //    // 递归遍历子节点
        //    foreach (var child in currentNode.Items)
        //    {
        //        if (child is TreeViewExItem childNode)
        //        {
        //            FindNodesBetweenRecursive(childNode, startNode, endNode, ref isBetween, result);
        //        }
        //    }
        //}

        public static List<BlockTreeView> FindNodesBetween(IEnumerable<BlockTreeView> treeChildren, BlockTreeView startNode, BlockTreeView endNode)
        {
            // 存储结果的列表
            var result = new List<BlockTreeView>();
            bool isBetween = false;

            foreach (var item in treeChildren)
            {
                if (item is BlockTreeView treeViewItem)
                {
                    FindNodesBetweenRecursive(treeViewItem, startNode, endNode, ref isBetween, result, 0); // 初始深度为 0
                }
            }

            return result;
        }

        private static void FindNodesBetweenRecursive(
            BlockTreeView currentNode,
            BlockTreeView startNode,
            BlockTreeView endNode,
            ref bool isBetween,
            List<BlockTreeView> result,
            int currentDepth)
        {
            if (currentNode == startNode)
            {
                isBetween = true; // 开始记录
            }

            // 如果在起点和终点之间，记录当前节点
            if (isBetween)
            {
                if (currentNode != startNode && currentNode != endNode)
                {
                    currentNode.TreeViewDeep = currentDepth; // 设置节点深度
                    result.Add(currentNode);
                }
            }

            if (currentNode == endNode)
            {
                isBetween = false; // 停止记录
                return;
            }

            // 递归遍历子节点，深度增加 1
            foreach (var child in currentNode.Children)
            {
                if (child is BlockTreeView childNode)
                {
                    FindNodesBetweenRecursive(childNode, startNode, endNode, ref isBetween, result, currentDepth);
                }
            }
            currentDepth++;
        }

        #endregion

        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                    return ancestor;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T targetChild)
                {
                    return targetChild;
                }
                T result = FindChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static List<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> result = new List<T>();

            if (parent == null) return result;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T targetChild)
                {
                    result.Add(targetChild);
                }

                // 递归查找子节点
                result.AddRange(FindChildren<T>(child));
            }
            return result;
        }

        public static bool Bigger (BlockTreeView blockTreeView1, BlockTreeView blockTreeView2)
        {
            return BlockTreeViewItems.IndexOf(blockTreeView1) > BlockTreeViewItems.IndexOf(blockTreeView2);
        }

        public static bool Smaller(BlockTreeView blockTreeView1, BlockTreeView blockTreeView2)
        {
            return BlockTreeViewItems.IndexOf(blockTreeView1) > BlockTreeViewItems.IndexOf(blockTreeView2);
        }

        public static void SaveTreeViewToJson<T>(T treeView, string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                }
            }
            string json = JsonConvert.SerializeObject(treeView, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static T LoadTreeViewFromJson<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            T nodes = JsonConvert.DeserializeObject<T>(json);
            return nodes;
        }

        //public static TreeViewExItem ContainerFromItemRecursive(object item, ItemsControl parent)
        //{
        //    if (parent == null)
        //        return null;

        //    for (int i = 0; i < parent.Items.Count; i++)
        //    {
        //        var currentItem = parent.Items[i];

        //        var container = parent.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewExItem;

        //        if (container != null && currentItem.Equals(item))
        //        {
        //            return container;
        //        }

        //        var recursiveContainer = ContainerFromItemRecursive(item, container);
        //        if (recursiveContainer != null)
        //            return recursiveContainer;
        //    }

        //    return null; 
        //}

        //public static TreeViewExItem GetSelectedTreeViewItemParent(TreeViewExItem treeViewItemEx)
        //{
        //    var patientItemsControl =  GetSelectedTreeViewItemParent(treeViewItemEx) as TreeViewExItem;
        //    return patientItemsControl;
        //}

        //private ItemsControl GetSelectedParentItemsControl(TreeViewExItem item)
        //{
        //    DependencyObject parent = VisualTreeHelper.GetParent(item);
        //    while (!(parent is TreeViewItem || parent is TreeView))
        //    {
        //        parent = VisualTreeHelper.GetParent(parent);
        //    }
        //    return parent as ItemsControl;
        //}

        //public static List<TreeViewExItem> GetChildTreeViewItems(TreeViewExItem parent, TreeViewExItem self)
        //{
        //    if (parent == null)
        //    {
        //        return null;
        //    }

        //    var items = new List<TreeViewExItem>();
        //    foreach (var child in parent.Items)
        //    {
        //        if (parent.ItemContainerGenerator.ContainerFromItem(child) is TreeViewExItem childItem)
        //        {
        //            if (childItem == self)
        //            {
        //                items = GetChildTreeViewItems(self);
        //            }
        //            else
        //            {
        //                GetChildTreeViewItems(parent, self);
        //            }
        //        }
        //    }
        //    return items;
        //}


        //public static List<TreeViewExItem> GetChildTreeViewItems(TreeViewExItem parent)
        //{
        //    if (parent == null)
        //    {
        //        return null;
        //    }

        //    var items = new List<TreeViewExItem>();
        //    foreach (var child in parent.Items)
        //    {
        //        if (parent.ItemContainerGenerator.ContainerFromItem(child) is TreeViewExItem childItem)
        //        {
        //            items.Add(childItem);
        //            items.AddRange(GetChildTreeViewItems(childItem));
        //        }
        //    }
        //    return items;
        //}

        //public static TreeViewExItem FindItemRecursively(TreeViewExItem parentItem, TreeViewExItem targetItem)
        //{
        //    foreach (var child in parentItem.Items)
        //    {
        //        if (parentItem.ItemContainerGenerator.ContainerFromItem(child) is TreeViewExItem childItem)
        //        {
        //            if (childItem == targetItem)
        //            {
        //                return parentItem;
        //            }

        //            var foundParent = FindItemRecursively(childItem, targetItem);
        //            if (foundParent != null)
        //            {
        //                return childItem;
        //            }
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 包含targetTreeViewItem 并且没有展开的item
        ///// </summary>
        ///// <param name="expandarTreeViewItem"></param>
        ///// <param name="targetTreeViewItem"></param>
        ///// <returns></returns>
        //public static TreeViewExItem FindTargetFirstNoExpandarItem(TreeViewExItem expandarTreeViewItem, TreeViewExItem targetTreeViewItem)
        //{
        //    foreach (var child in expandarTreeViewItem.Items)
        //    {
        //        if (expandarTreeViewItem.ItemContainerGenerator.ContainerFromItem(child) is TreeViewExItem childItem)
        //        {
        //            if (GetAllChildNodes(childItem).Any(o=>o == targetTreeViewItem))
        //            {
        //                if (!childItem.IsExpanded)
        //                {
        //                    return childItem;
        //                }
        //                else
        //                {
        //                    var result = FindTargetFirstNoExpandarItem(childItem, targetTreeViewItem);
        //                    if (result != null)
        //                    {
        //                        return result;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// 包含targetTreeViewItem 并且没有展开的item
        ///// </summary>
        ///// <param name="expandarTreeViewItem"></param>
        ///// <param name="targetTreeViewItem"></param>
        ///// <returns></returns>
        //public static TreeViewExItem FindTargetFirstNoExpandarItemByTreeView(TreeViewEx treeViewEx, TreeViewExItem targetTreeViewItem)
        //{
        //    foreach (var child in treeViewEx.Items)
        //    {
        //        if (treeViewEx.ItemContainerGenerator.ContainerFromItem(child) is TreeViewExItem childItem)
        //        {
        //            if (GetAllChildNodes(childItem).Any(o => o == targetTreeViewItem))
        //            {
        //                if (!childItem.IsExpanded)
        //                {
        //                    return childItem;
        //                }
        //                else
        //                {
        //                    var result = FindTargetFirstNoExpandarItem(childItem, targetTreeViewItem);
        //                    if (result != null)
        //                    {
        //                        return result;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}


        ///// <summary>
        /////  查找所有展开/折叠的子节点
        ///// </summary>
        ///// <param name="treeView"></param>
        ///// <returns></returns>
        //public static List<TreeViewExItem> GetAllChildNodes(TreeViewExItem parentNode)
        //{
        //    List<TreeViewExItem> childNodes = new List<TreeViewExItem>();

        //    foreach (var child in parentNode.Items)
        //    {
        //        // 获取子节点的容器
        //        if (parentNode.ItemContainerGenerator.ContainerFromItem(child) is TreeViewExItem treeViewExItem)
        //        {
        //            // 添加子节点
        //            childNodes.Add(treeViewExItem);

        //            // 递归查找该子节点的所有子节点
        //            childNodes.AddRange(GetAllChildNodes(treeViewExItem));
        //        }
        //    }

        //    return childNodes;
        //}

    }
}
