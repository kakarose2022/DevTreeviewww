
using DevTreeview.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRealBlock.ViewModel;

namespace DevTreeview
{
    public class MainWindowViewModel:BindableBase
    {


        private BlockTreeView block;
        public BlockTreeView Blocks
        {
            get { return block; }
            set
            {
                SetProperty(ref block, value);
            }
        }


        public MainWindowViewModel()
        {

            Blocks = new BlockTreeView(true, true, true)
            {
            };

            Blocks.InsertChild(new BlockTreeView("Image Source",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                      new BlockTreeView() { Name = "OutputImage",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("ColImageFileTool1",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                        new BlockTreeView() { Name = "InputImage" ,BlockType= BlockType.Input},
                        new BlockTreeView() { Name = "OutputImage",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogPMAlignTool1",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                        new BlockTreeView() { Name = "InputImage" , BlockType = BlockType.Input},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose()" ,BlockType = BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationX" ,BlockType= BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationX" ,BlockType= BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose().Rotation" ,BlockType= BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].Score",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogPaInspectTool1",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                        new BlockTreeView() { Name = "InputImage",BlockType= BlockType.Input },
                        new BlockTreeView() { Name = "Pose",BlockType= BlockType.Input },
                        new BlockTreeView() { Name = "Pattern.TrainImage",BlockType= BlockType.Input },
                        new BlockTreeView() { Name = "Results.Origin" ,BlockType= BlockType.Input},
                        new BlockTreeView() { Name = "Results.GetDifferenceImage(Absolute)" ,BlockType= BlockType.Output},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogToolBlock1",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                         new BlockTreeView() { Name = "OutputImage" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_Score" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_Score1" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_GetPose_Rotation" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_GetPose_TranslationX" ,BlockType= BlockType.Input},
                                 new BlockTreeView() { Name = "OutPutImage1" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

        }
    }


    public class TreeItem
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }

        public ObservableCollection<TreeItem> Children { get; set; } = new ObservableCollection<TreeItem>();

        public override string? ToString()
        {
            return $@"{Name}";
        }
    }
}
