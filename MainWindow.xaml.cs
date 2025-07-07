using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeView;
using DevExpress.XtraTreeList;
using DevTreeview.Adorner;
using DevTreeview.Core;
using System.Diagnostics;
using System.Windows;
using Point = System.Windows.Point;

namespace DevTreeview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        #region Dev DragAndDrop
        object startItem;
        private void treeList_StartRecordDrag(object sender, DevExpress.Xpf.Core.StartRecordDragEventArgs e)
        {
            var aa = treeList.GetNodeByRowHandle(0);
            startItem = e.Records[0];
            //Trace.WriteLine("Start item" + startItem.ToString());
        }

        private void treeList_DragRecordOver(object sender, DevExpress.Xpf.Core.DragRecordOverEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.All;
        }

        object endItem;
        private void treeList_CompleteRecordDragDrop(object sender, DevExpress.Xpf.Core.CompleteRecordDragDropEventArgs e)
        {
            //endItem = e.Records[0];
            //Trace.WriteLine("End item" + endItem.ToString());
        }

        private void treeList_DropRecord(object sender, DevExpress.Xpf.Core.DropRecordEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.None;
            endItem = e.TargetRecord;
            e.Handled = true;

            var rowControls = TreeViewRowControlHelper.GetCachedRowControls(treeList).ToList();
            int startIndex = rowControls.FindIndex(row =>
            {
                if (row.DataContext is TreeViewRowData rowData)
                    return Equals(rowData.Row, startItem);
                return false;
            });

            int targetIndex = rowControls.FindIndex(row =>
            {
                if (row.DataContext is TreeViewRowData rowData)
                    return Equals(rowData.Row, endItem);
                return false;
            });

            if (startIndex != -1 && targetIndex != -1)
            {
                var startRowControl = rowControls[startIndex];
                var endRowControl = rowControls[targetIndex];
                var startSize = BlockTreeHelper.MeasureString(startItem.ToString(), startRowControl.FontSize,startRowControl.FontFamily);
                var endSize = BlockTreeHelper.MeasureString(endItem.ToString(), endRowControl.FontSize, endRowControl.FontFamily);

                var startRowControlProperty = new RowControlProperty(startRowControl, startItem.ToString(), startSize.Width, startSize.Height);
                var endRowControlProperty = new RowControlProperty(endRowControl, endItem.ToString(), endSize.Width, endSize.Height);
                TreeNodeAdornerHelper.AddLine(treeList, startRowControlProperty, endRowControlProperty);
                //Trace.WriteLine("End item" + endItem.ToString());
            } 
        }
        #endregion
        private void treeList_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandAllNodes(treeList.Nodes);




        }

        private void ExpandAllNodes(TreeListNodeCollection nodes)
        {
            foreach (var node in nodes)
            {
                node.IsExpanded = true;
                ExpandAllNodes(node.Nodes); // 递归展开子节点
            }
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point position = e.GetPosition(this);
            Postion.Text = $"X:{position.X}  Y: {position.Y}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TreeNodeAdornerHelper.ClearAllAdorners(treeList);
        }

        private void treeList_NodeCollapsed(object sender, DevExpress.Xpf.Grid.TreeList.TreeViewNodeEventArgs e)
        {
            treeList.Dispatcher.BeginInvoke(new Action(() =>
            {
                Redraw(e.Node,false);

            }), System.Windows.Threading.DispatcherPriority.Background);

        }

        private void treeList_NodeExpanded(object sender, DevExpress.Xpf.Grid.TreeList.TreeViewNodeEventArgs e)
        {
            treeList.Dispatcher.BeginInvoke(new Action(() =>
            {
                Redraw(e.Node,true);

            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void Redraw(TreeListNode node,bool isExpanded)
        {
            var allRowControls = TreeViewRowControlHelper.GetCachedRowControls(treeList);
            var expanderRowControl = allRowControls.FirstOrDefault(r =>
            {
                var rowData = r.DataContext as TreeViewRowData;
                return rowData?.Node == node;
            });
            var exPanderChildNodes = TreeViewRowControlHelper.GetAllChildRowControls(treeList, node);

            foreach (var item in exPanderChildNodes)
            {
                Trace.WriteLine("Redraw  " + item.ToControlContent());
            }
            TreeNodeAdornerHelper.RedrawAdorners(treeList, exPanderChildNodes, expanderRowControl, isExpanded);
        }



    }
}