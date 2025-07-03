using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace DevTreeview.Core
{
    public interface ITreeView<TNode>
    {
        bool IsExpandedValue { get; set; }
        BlockType BlockType { get; set; }
        void InsertChild(TNode child);

        IEnumerable<TNode> GetChildren(TNode node);
    }

    /// <summary>
    ///  计算 Link所需要的 元素
    /// </summary>
    public interface TreeViewLink
    {
        int TreeViewDeep { get; set; }
        Guid TreeViewGuid { get; set; }
        double ActualWidth { get; set; }
        double ActualHeight { get; set; }
        bool IsVisibility { get; set; }
        //LinkCollection LinkCollection { get; set; }
    }
}
