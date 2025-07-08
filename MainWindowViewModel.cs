
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
            Blocks = new BlockTreeView(true, true, true) { };

            Blocks.InsertChild(new BlockTreeView("Input 0",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                new BlockTreeView() { Name = "OutputImage 1",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "IpOneImage 2",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "SearchRegion 3",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogPMAlignTool 4",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                new BlockTreeView() { Name = "InputImage 5" ,BlockType= BlockType.Input},
                new BlockTreeView() { Name = "SearchRegion 6",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Item[0].GetPose() 7",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationX 8",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationY 9",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Item[0].GetPose().Rotation 10",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Item[0].Score 11",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Count 12",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogPMAlignTool1 13",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                new BlockTreeView() { Name = "InputImage 14" , BlockType = BlockType.Input},
                new BlockTreeView() { Name = "SearchRegion 15",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Item[0].GetPose() 16" ,BlockType = BlockType.Output},
                new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationX 17" ,BlockType= BlockType.Output},
                new BlockTreeView() { Name = "Results.Item[0].GetPose().TranslationY 18" ,BlockType= BlockType.Output},
                new BlockTreeView() { Name = "Results.Item[0].GetPose().Rotation 19" ,BlockType= BlockType.Output},
                new BlockTreeView() { Name = "Results.Item[0].Score 20",BlockType= BlockType.Output },
                new BlockTreeView() { Name = "Results.Count 21",BlockType= BlockType.Output },
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogFixureTool 22",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
                new BlockTreeView() { Name = "InputImage 23",BlockType= BlockType.Input },
                new BlockTreeView() { Name = "RunParams,UnTransform 24",BlockType= BlockType.Input },
                new BlockTreeView() { Name = "RunParams,UnTransform.TranslationX 25",BlockType= BlockType.Input },
                new BlockTreeView() { Name = "RunParams,UnTransform.TranslationY 26" ,BlockType= BlockType.Input},
                new BlockTreeView() { Name = "RunParams,UnTransform.Rotation 27" ,BlockType= BlockType.Output},
                new BlockTreeView() { Name = "OutputImage 28" ,BlockType= BlockType.Output},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogHistogramTool1 29",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "InputImage 30",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Mean 31",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.StandrdDeviation 32",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Variance 33" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogHistogramTool2 34",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "InputImage 35",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Mean 36",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.StandrdDeviation 37",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Variance 38" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogHistogramTool3 39",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "InputImage 40",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Mean 41",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.StandrdDeviation 42",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Variance 43" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogHistogramTool4 44",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "InputImage 45",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Mean 46",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.StandrdDeviation 47",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Variance 48" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogHistogramTool5 49",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "InputImage 50",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Mean 51",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.StandrdDeviation 52",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.Variance 53" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogPatInsprctTool 54",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "InputImage 55",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Pose 56",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Pattern.TrainImage 57",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Pattern.Origin 58" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "Result.GetDifferenceImage(Absolute) 59",BlockType= BlockType.Input },
        new BlockTreeView() { Name = "Result.GetDifferenceImage(Brighter) 60" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("CogBlobTool1 61",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "InputImage 62" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "Results.GetBlobs().Count 63" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "Results.GetBlobs().Item[0].CenterOfMessX 64" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "Results.GetBlobs().Item[0].CenterOfMessY 65" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "Results.GetBlobs().Area 66" ,BlockType= BlockType.Input},
            })
            {
                Patient = Blocks
            });

            Blocks.InsertChild(new BlockTreeView("[Outputs] 67",
            new System.Collections.ObjectModel.ObservableCollection<BlockTreeView>()
            {
        new BlockTreeView() { Name = "Count 68" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "BlobCount 69" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "Variance 70" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "StandardDeviation 71" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "FGVaMax 72" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "FGVaMin 73" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "FGStMax 74" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "FGStMin 75" ,BlockType= BlockType.Input},
        new BlockTreeView() { Name = "FanCount 76" ,BlockType= BlockType.Input},
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
