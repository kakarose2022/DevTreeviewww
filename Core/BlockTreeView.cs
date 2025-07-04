
using DevTreeview.Adorner;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using TestRealBlock.ViewModel;


namespace DevTreeview.Core
{
    public class BlockTreeView: BindableBase, ITreeView<BlockTreeView>, TreeViewLink
    {
        public Guid BlockGuid { get; set; }

        private string imageSource;
		public string ImageSource
        {
			get { return imageSource; }
			set 
			{ 
				SetProperty(ref imageSource, value);
			}
		}

        private bool allDrop = true;
        public bool AllowDrop
        {
            get { return BlockType == BlockType.BlockItem ? false: true; }
        }

        private BlockType blockType = BlockType.BlockItem;
        public BlockType BlockType
        {
            get { return blockType; }
            set
            {
                SetProperty(ref blockType, value);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
            }
        }

        /// <summary>
        /// 当前UI相对于TreeViewEx 的相对变换
        /// </summary>
        public GeneralTransform GeneralTransform { get; set; }

        private double elementActualHeight;
        public double ElementActualHeight
        {
            get { return elementActualHeight; }
            set 
            {
                SetProperty(ref elementActualHeight, value); 
            }
        }

        private bool isExpandedValue = true;
        public bool IsExpandedValue
        {
            get { return isExpandedValue; }
            set 
            { 
                SetProperty(ref isExpandedValue, value); 
            }
        }

        [JsonIgnore]
        public BlockTreeView Patient { set; get; }

        private ObservableCollection<BlockTreeView> children;
        public ObservableCollection<BlockTreeView> Children
        {
            get { return children; }
            set
            {
                SetProperty(ref children, value);
            }
        }

        #region TreeViewLink
        [JsonIgnore]
        public Guid TreeViewGuid { get ; set ; }
        public double ActualWidth { get; set ; }
        public double ActualHeight { get ; set; }
        public bool IsVisibility { get; set; } 
        public int TreeViewDeep { get; set; }
        public ObservableCollection<LineElement> LinkCollection { get; set ; }
        #endregion

        public delegate void SelectTreeViewAdornerEvent();
        public event SelectTreeViewAdornerEvent SelectTreeViewAdorner;

        public BlockTreeView()
        {
            Children = new ObservableCollection<BlockTreeView>();
            LinkCollection = new ObservableCollection<LineElement>();
            BlockGuid = Guid.NewGuid();
        }

        public BlockTreeView(bool initData, bool needInput = true,bool needOutput = true, string name ="")
        {
            Children = new ObservableCollection<BlockTreeView>();
            LinkCollection = new ObservableCollection<LineElement>();
            Name = name;
            if (needInput)
            {
                Children.Add(new BlockTreeView()
                {
                    Name = "输入 0",
                    BlockType = BlockType.Input,
                });
            }

            if (needOutput)
            {
                Children.Add(new BlockTreeView()
                {
                    Name = "输出 26",
                    BlockType = BlockType.Output
                });
            }
            BlockGuid = Guid.NewGuid();
        }

        public BlockTreeView(string name, ObservableCollection<BlockTreeView> blocks )
        {
            Name = name;
            Children = new ObservableCollection<BlockTreeView>();
            foreach (var block in blocks)
            {
                Children.Add(block);
            }
            BlockGuid = Guid.NewGuid();
        }

        #region Insert
        public void InsertChild(BlockTreeView block)
        {
            if (Children.Count() == 0)
            {
                Children.Add(block);
            }
            else
            {
                Children.Insert(Children.Count-1, block);
            }       
        }

        public void InsertItems(IEnumerable<BlockTreeView> blocks)
        {
            if (Children.Count() == 0)
            {
                foreach (var block in blocks)
                {
                    Children.Add(block);
                }
            }
            else
            {
                foreach (var block in blocks)
                {
                    Children.Insert(Children.Count - 1, block);
                }
            }
        }

        public void InsertByIndex(BlockTreeView block,int index)
        {
            if (index == -1)
            {
                return;
            }

            if(index >= Children.Count)
            {
                return;
            }
            Children.Insert(index, block);
        }
        #endregion

        //public static bool operator >(BlockTreeView blockTreeView1, BlockTreeView blockTreeView2)
        //{
        //    return BlockTreeViewItems.IndexOf(blockTreeView1) > BlockTreeViewItems.IndexOf(blockTreeView2);
        //}

        //public static bool operator <(BlockTreeView blockTreeView1, BlockTreeView blockTreeView2)
        //{
        //    return sources.IndexOf(blockTreeView1) > sources.IndexOf(blockTreeView2);
        //}

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<BlockTreeView> GetChildren(BlockTreeView node)
        {
            return node.children;
        }
    }

    public enum BlockType 
    {
       Input,
       Output,
       BlockItem
    }
}
