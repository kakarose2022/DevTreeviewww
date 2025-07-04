
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

            Blocks.InsertChild(new BlockTreeView("Image Source 1",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                      new BlockTreeView() { Name = "OutputImage 2",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("ColImageFileTool1 3",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                        new BlockTreeView() { Name = "InputImage 4" ,BlockType= BlockType.Input},
                        new BlockTreeView() { Name = "OutputImage 5",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogPMAlignTool1 6",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                        new BlockTreeView() { Name = "InputImage 7" , BlockType = BlockType.Input},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose() 8" ,BlockType = BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationX 9" ,BlockType= BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationX 10" ,BlockType= BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].GetPose().Rotation 11" ,BlockType= BlockType.Output},
                        new BlockTreeView() { Name = "Results.Item[0].Score 12",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogPaInspectTool1 13",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                        new BlockTreeView() { Name = "InputImage 14",BlockType= BlockType.Input },
                        new BlockTreeView() { Name = "Pose 15",BlockType= BlockType.Input },
                        new BlockTreeView() { Name = "Pattern.TrainImage 16",BlockType= BlockType.Input },
                        new BlockTreeView() { Name = "Results.Origin 17" ,BlockType= BlockType.Input},
                        new BlockTreeView() { Name = "Results.GetDifferenceImage(Absolute) 18" ,BlockType= BlockType.Output},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogToolBlock1 19",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                         new BlockTreeView() { Name = "OutputImage 20" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_Score 21" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_Score1 22" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_GetPose_Rotation 23" ,BlockType= BlockType.Input},
                         new BlockTreeView() { Name = "Results_Item_0_GetPose_TranslationX 24" ,BlockType= BlockType.Input},
                                 new BlockTreeView() { Name = "OutPutImage1 25" ,BlockType= BlockType.Input},
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
