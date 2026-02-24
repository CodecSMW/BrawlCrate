using BrawlLib.Internal;
using BrawlLib.SSBB.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class MoveDefSubParamNode : MoveDefEntryNode
    {
        internal SubParamHeader* Header => (SubParamHeader*)WorkingUncompressed.Address;
        public List<SpecialOffset> specialOffsets = new List<SpecialOffset>();
        internal uint DataLen;
        MoveDefSectionParamNode unk2, unk4;
        private int unk0, unk1;

        [Category("Data Offsets")]
        public int initLABasic7
        {
            get => unk0;
            set
            {
                unk0 = value;
                SignalPropertyChange();
            }
        }
        [Category("Data Offsets")] public int UnusedUnk1 => unk1;
        public MoveDefSubParamNode(uint dataLen, string name)
        {
            DataLen = dataLen;
            _name = name;
        }
        public override bool OnInitialize()
        {
            //base.OnInitialize();
            bint* current = (bint*)Header;
            for (int i = 0; i < 6; i++)
            {
                specialOffsets.Add(new SpecialOffset { Index = i, Offset = *current++ });
            }
            unk0 = Header->Unknown0;
            unk1 = Header->Unknown1;
            return true;
        }
        public override void OnPopulate()
        {
            unk2 = new MoveDefSectionParamNode { _name = "Unk 1" };
            unk2.Initialize(this, BaseAddress + specialOffsets[2].Offset, (specialOffsets[3].Offset + 1) * 4);
            unk4 = new MoveDefSectionParamNode { _name = "Unk 2" };
            unk4.Initialize(this, BaseAddress + specialOffsets[4].Offset, (specialOffsets[5].Offset + 1) * 4);
        }
        public override int OnCalculateSize(bool force)
        {
            _lookupCount = 2; // one each for unk1 and unk2
            return 24 + ((specialOffsets[3].Offset + 2 + specialOffsets[5].Offset) * 4);
        }
        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            _entryOffset = address;
            bint* data = (bint*)address;
            VoidPtr dataAddress = (address + 0x10);
            VoidPtr baseAddress = _rebuildBase;
            MoveDefNode RootNode = Parent.Parent as MoveDefNode;
            SubParamHeader* header = (SubParamHeader*)address;

            header->Unknown0 = initLABasic7;
            header->Unknown1 = UnusedUnk1;
            header->Unknown2 = Rebuild(RootNode, unk2, ref dataAddress, baseAddress);
            header->Unknown3 = Header->Unknown3; //nodecount for above + 1
            header->Unknown4 = Rebuild(RootNode, unk4, ref dataAddress, baseAddress);
            header->Unknown5 = Header->Unknown5; //nodecount for above + 1
            MoveDefNode._lookupOffsets.Add(header->Unknown2);
            MoveDefNode._lookupOffsets.Add(header->Unknown4);
        }

        public static int Rebuild(MoveDefNode root, MoveDefEntryNode node, ref VoidPtr dataAddress, VoidPtr baseAddress)
        {
            if (node != null)
            {
                if (!(node.External && !(node._extNode is MoveDefReferenceEntryNode)))
                {
                    node.Rebuild(dataAddress, node._calcSize, true);
                    dataAddress += node._calcSize;

                    if (node._lookupOffsets.Count != node._lookupCount && !(node is MoveDefActionNode))
                    {
                        Console.WriteLine(node.TreePath + (node._lookupCount - node._lookupOffsets.Count));
                    }

                    MoveDefNode._lookupOffsets.AddRange(node._lookupOffsets.ToArray());
                }

                MoveDefEntryNode next = node;
            Top:


                return node._rebuildOffset;
            }
            else
            {
                return 0;
            }
        }
    }
}