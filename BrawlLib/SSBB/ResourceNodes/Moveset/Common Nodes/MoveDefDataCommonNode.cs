using BrawlLib.Internal;
using BrawlLib.SSBB.Types;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class MoveDefDataCommonNode : MoveDefEntryNode
    {
        internal CommonMovesetHeader* Header => (CommonMovesetHeader*) WorkingUncompressed.Address;

        public List<SpecialOffset> specialOffsets = new List<SpecialOffset>();
        internal uint DataLen;

        [Category("Data Offsets")] public int GlobalICBasics => Header->Unknown0;

        [Category("Data Offsets")] public int GlobalICBasicsSSE => Header->Unknown1;

        [Category("Data Offsets")] public int ICBasics => Header->Unknown2;

        [Category("Data Offsets")] public int ICBasicsSSE => Header->Unknown3;

        [Category("Data Offsets")] public int EntryActions => Header->ActionsStart;

        [Category("Data Offsets")] public int ExitActions => Header->Actions2Start;

        [Category("Data Offsets")] public int FlashOverlaysList => Header->Unknown6;

        [Category("Data Offsets")] public int Unk7 => Header->Unknown7;

        [Category("Data Offsets")] public int Unk8 => Header->Unknown8;

        [Category("Data Offsets")] public int Unk9 => Header->Unknown9;

        [Category("Data Offsets")] public int Unk10 => Header->Unknown10;

        [Category("Data Offsets")] public int Unk11 => Header->Unknown11;

        [Category("Data Offsets")] public int Unk12 => Header->Unknown12;

        [Category("Data Offsets")] public int Unk13 => Header->Unknown13;

        [Category("Data Offsets")] public int Unk14 => Header->Unknown14;

        [Category("Data Offsets")] public int Unk15 => Header->Unknown15;

        [Category("Data Offsets")] public int Unk16 => Header->Unknown16;

        [Category("Data Offsets")] public int Unk17 => Header->Unknown17;

        [Category("Data Offsets")] public int RGBAColor => Header->Unknown18;

        [Category("Data Offsets")] public int FlashOverlays => Header->Unknown19;

        [Category("Data Offsets")] public int ScreenTints => Header->Unknown20;

        [Category("Data Offsets")] public int LegBoneNames => Header->Unknown21;

        [Category("Data Offsets")] public int Unk22 => Header->Unknown22;

        /*[Category("Data Offsets")] public int Unk23 => Header->Unknown23;

        [Category("Data Offsets")] public int Unk24 => Header->Unknown24;

        [Category("Data Offsets")] public int Unk25 => Header->Unknown25;*/

        [Category("Special Offsets Node")] public SpecialOffset[] Offsets => specialOffsets.ToArray();

        public MoveDefActionListNode actions;
        public MoveDefCommonUnk7ListNode node_7;
        public MoveDefUnk11Node node_11;
        public MoveDefCommonUnk21Node node_21;
        public MoveDefParamListNode node_22;
        public MoveDefSectionParamNode[] ambigNode;
        public MoveDefDataCommonNode(uint dataLen, string name)
        {
            DataLen = dataLen;
            _name = name;
        }

        public MoveDefActionsSkipNode _hitOverlay, _flashOverlay, _screenTint;

        public override bool OnInitialize()
        {
            base.OnInitialize();
            bint* current = (bint*) Header;
            for (int i = 0; i < 23; i++)
            {
                specialOffsets.Add(new SpecialOffset {Index = i, Offset = *current++});
            }
            specialOffsets.Add(new SpecialOffset { Index = 23, Offset = 0 });


            CalculateDataLen();

            return true;
        }

        public VoidPtr dataHeaderAddr;

        public int
            part1Len = 0,
            part2Len = 0,
            part3Len = 0,
            part4Len = 0,
            part5Len = 0,
            part6Len = 0,
            part7Len = 0,
            part8Len = 0;

        public override void OnPopulate()
        {
            #region Populate

            //if (RootNode is ARCNode && (RootNode as ARCNode).IsFighter)
            if (Name == "dataCommon")
            {
                List<int> ActionOffsets;

                actions = new MoveDefActionListNode {_name = "Action Scripts", _parent = this};

                bint* actionOffset;

                //Parse offsets first
                for (int i = 4; i < 6; i++)
                {
                    actionOffset = (bint*) (BaseAddress + specialOffsets[i].Offset);
                    ActionOffsets = new List<int>();
                    for (int x = 0; x < specialOffsets[i].Size / 4; x++)
                    {
                        ActionOffsets.Add(actionOffset[x]);
                    }

                    actions.ActionOffsets.Add(ActionOffsets);
                }
                
                //unk18 == ColorF4

                int r = 0;
                ambigNode = new MoveDefSectionParamNode[24]; //Won't use all of these but do not care.
                foreach (SpecialOffset s in specialOffsets)
                {
                    //if (r != 4 && r != 5 && r != 6 && r != 7 && 
                    //    r != 11 && r != 17 && 
                    //   r != 19 && r != 20 && r != 21 && r != 22)
                    if (r < 4 || r == 23 || (r < 19 && r > 7 && r != 11))
                    {
                        string name = "Params " + r;
                        ambigNode[r] = new MoveDefSectionParamNode { _name = name, offsetID = r };
                        if (r < 4)
                        {
                            name = (r == 0 || r == 2 ? "" : "SSE ") + (r < 2 ? "Global " : "") + "IC-Basics";
                            ambigNode[r]._name = name;
                            ambigNode[r].offsetID = r;
                        }
                        ambigNode[r].Initialize(this, BaseAddress + s.Offset, s.Size);
                        if (r == 9 || r == 17) // itemSwingData (9), patternPowerMul (17)
                        {
                            Dictionary<NameSizeGroup, FDefStringEntry> dTbl = (Parent as MoveDefSectionNode).DataTable;
                            foreach (KeyValuePair<NameSizeGroup, FDefStringEntry> data in dTbl)
                            {
                                if (data.Value._dataOffset == s.Offset)
                                {
                                    ambigNode[r]._name = data.Key.Name; //Tested. Confirmed functional. 
                                }
                            }
                            ambigNode[r].RefreshSectionDesc(); //Allows BrawlCrate to show descriptions for these two.
                        }
                    }

                    r++;
                }

                if (specialOffsets[7].Size != 0)
                {
                    node_7 = new MoveDefCommonUnk7ListNode { _name = "Unknown7", offsetID = 7 };
                    node_7.Initialize(this, BaseAddress + specialOffsets[7].Offset, 48); //TODO: This one's size is bruteforced, we still don't know what Unk7 is for!
                }

                if (specialOffsets[11].Size != 0)
                {
                    node_11 = new MoveDefUnk11Node { _name = "Unknown11", offsetID = 11 };
                    node_11.Initialize(this, BaseAddress + specialOffsets[11].Offset, 0);
                }

                if (specialOffsets[19].Size != 0)
                {
                    (_flashOverlay = new MoveDefActionsSkipNode("Flash Overlay Actions") {offsetID = 19}).Initialize(
                        this, BaseAddress + specialOffsets[19].Offset, 0);
                }

                if (specialOffsets[20].Size != 0)
                {
                    (_screenTint = new MoveDefActionsSkipNode("Screen Tint Actions") {offsetID = 20}).Initialize(this,
                        BaseAddress + specialOffsets[20].Offset, 0);
                }
                /*
                 //Test viewer to observe table. Normally uses same pointers as 19 so okay to keep hidden. Compilation accounts for this.
                if (specialOffsets[6].Size != 0)
                {
                    MoveDefRawDataNode hitOver;
                    (hitOver = new MoveDefRawDataNode("Hit Overlay Actions") { offsetID = 6 }).Initialize(
                        this, BaseAddress + specialOffsets[6].Offset, 168); //TODO: Make it so 0 naturally works instead of forcing 21 * 8!
                }
                */
                if (specialOffsets[21].Size != 0)
                {
                    node_21 = new MoveDefCommonUnk21Node { offsetID = 21 };
                    node_21.Initialize(this, BaseAddress + specialOffsets[21].Offset, 16);
                }

                if (specialOffsets[22].Size != 0)
                {
                    node_22 = new MoveDefParamListNode { _name = "Unknown22", offsetID = 22 };
                    node_22.Initialize(this, BaseAddress + specialOffsets[22].Offset, 0);
                }
                if (specialOffsets[4].Size != 0 || specialOffsets[5].Size != 0)
                {
                    int count;
                    if (specialOffsets[4].Size == 0)
                    {
                        count = specialOffsets[5].Size / 4;
                    }
                    else
                    {
                        count = specialOffsets[4].Size / 4;
                    }

                    //Initialize using first offset so the node is sorted correctly
                    actions.Initialize(this, BaseAddress + specialOffsets[4].Offset, 0);

                    //Set up groups
                    for (int i = 0; i < count; i++)
                    {
                        actions.AddChild(new MoveDefActionGroupNode {_name = "Action " + System.Convert.ToString(i, 16).ToUpper()}, false);
                    }

                    //Add children
                    for (int i = 0; i < 2; i++)
                    {
                        if (specialOffsets[i + 4].Size != 0)
                        {
                            //TODO: Fix Reading!
                            PopulateActionGroup(actions, actions.ActionOffsets[i], false, i);
                        }
                    }

                    //Add to children (because the parent was set before initialization)

                    //TODO

                    //MovesetCmnConverter.CalcDataSize(this);

                    Children.Add(actions);

                    Root._actions = actions;
                }
            }

            #endregion

            SortChildren();
        }

        private void CalculateDataLen()
        {
            List<SpecialOffset> sorted = specialOffsets.OrderBy(x => x.Offset).ToList();
            for (int i = 0; i <= 23; i++) //24 and 25 are within others!
            { //TODO: Gut out specialOffsets 24 and 25, they're lookup offsets!
                //TODO: figure out how to  calculate size of 22!
                //TODO: Force size of 0xA8 for 0
                if (i < sorted.Count - 1)
                {
                    sorted[i].Size = (int) (sorted[i + 1].Offset - sorted[i].Offset);
                }
                else
                {
                    sorted[i].Size = (int) (DataLen - sorted[i].Offset);
                }

                //Console.WriteLine(sorted[i].ToString());
            }
            //TODO: offset 23 needs to have parts of unk7 removed from calculations!
            specialOffsets[23].Size = 0xA8;
            specialOffsets[17].Size = 0x28;
            specialOffsets[3].Size = 0x8A0;
            if (specialOffsets[6].Size > 0x800)
                specialOffsets[6].Size = 148;
        }
        //TODO: FIX COMPILATION OF FIGHTER.PAC!!!!

        public override int OnCalculateSize(bool force)
        {
            _entryLength = 124;// 124;
            _childLength = MovesetCmnConverter.CalcDataSize(this);
            return _entryLength + _childLength;
        } //TODO: Make accurate!
        
        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            MovesetCmnConverter.BuildData(this, (CommonMovesetHeader*)dataHeaderAddr, address, length, force);
        }
        

        public void PopulateActionGroup(ResourceNode g, List<int> ActionOffsets, bool subactions, int index)
        {
            string innerName = "";
            if (subactions)
            {
                if (index == 0)
                {
                    innerName = "Main";
                }
                else if (index == 1)
                {
                    innerName = "GFX";
                }
                else if (index == 2)
                {
                    innerName = "SFX";
                }
                else if (index == 3)
                {
                    innerName = "Other";
                }
                else
                {
                    return;
                }
            }
            else if (index == 0)
            {
                innerName = "Entry";
            }
            else if (index == 1)
            {
                innerName = "Exit";
            }

            int i = 0;
            foreach (int offset in ActionOffsets)
            {
                //if (i >= g.Children.Count)
                //    if (subactions)
                //        g.Children.Add(new MoveDefSubActionGroupNode() { _name = "Extra" + i, _flags = new AnimationFlags(), _inTransTime = 0, _parent = g });
                //    else
                //        g.Children.Add(new MoveDefGroupNode() { _name = "Extra" + i, _parent = g });

                if (offset > 0)
                {
                    new MoveDefActionNode(innerName, false, g.Children[i]).Initialize(g.Children[i],
                        new DataSource(BaseAddress + offset, 0));
                }
                else
                {
                    g.Children[i].Children.Add(new MoveDefActionNode(innerName, true, g.Children[i]));
                }

                i++;
            }
        }
    }

    public unsafe class MoveDefCommonUnk21Node : MoveDefEntryNode
    {
        internal FDefListOffset* Header => (FDefListOffset*) WorkingUncompressed.Address;
        internal int i = 0;

        [Category("List Offset")] public int DataOffset1 => Header[0]._startOffset;

        [Category("List Offset")] public int Count1 => Header[0]._listCount;

        [Category("List Offset")] public int DataOffset2 => Header[1]._startOffset;

        [Category("List Offset")] public int Count2 => Header[1]._listCount;

        public override bool OnInitialize()
        {
            _name = "IK Leg Bones";
            base.OnInitialize();
            return DataOffset1 > 0 || DataOffset2 > 0;
        }

        public override void OnPopulate()
        {
            if (DataOffset1 > 0)
            {
                new MoveDefRawDataNode("Left") {offsetID = 0}.Initialize(this, BaseAddress + DataOffset1, 16);
            }

            if (DataOffset2 > 0)
            {
                new MoveDefRawDataNode("Right") {offsetID = 1}.Initialize(this, BaseAddress + DataOffset2, 16);
            }
            
            foreach (MoveDefRawDataNode d in Children)
            {
                bint* addr = (bint*) d.Header;
                for (int i = 0; i < d.Size / 4; i++)
                {
                    new MoveDefRawDataNode(new string((sbyte*) (BaseAddress + addr[i]))).Initialize(d, d.Header + i * 4,
                        4);
                }
                foreach (MoveDefRawDataNode e in d.Children)
                {
                    bint* addr2 = (bint*)e.Header;
                    new MoveDefRawDataNode("data").Initialize(e, BaseAddress + addr2->Value, 8);
                }
            }
        }
        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            _entryOffset = address;
            int* data = (int*)address;
            int* addr = data;
            int* tableAddr = data + 12;
            byte* tableChr = (byte*)(address + 0x30);
            FDefCommonUnk7Entry* header = (FDefCommonUnk7Entry*)address;
            //Header
            *data++ = (bint)(address - _rebuildBase + 0x10).Reverse(); //4 ints so 0x10 # !! = Needs Lookup
            _lookupOffsets.Add(address - (int)_rebuildBase); //4 lookups
            *data++ = (bint)Count1.Reverse();
            *data++ = (bint)(address - _rebuildBase + 0x20).Reverse(); //8 ints so 0x20 # !! Needs Lookup
            _lookupOffsets.Add(address - (int)_rebuildBase + 8); //4 lookups
            *data++ = (bint)Count2.Reverse();
            //Left Leg
            for (int i = 0; i < 8; i++) // All need lookups
            {
                *data++ = (bint)(address - _rebuildBase + i * 8 + 0x30).Reverse();
                _lookupOffsets.Add(address - (int)(_rebuildBase) + i * 4 + 0x10); //4 lookups
            }
            for (int i = 0; i < 2; i++) // I probably should just make a table of the expected strings..... but this works so oh well.
                for (int j = 0; j < 4; j++)
                    foreach (MoveDefRawDataNode raw in Children[i].Children[j].Children)
                        for (int k = 0; k < 8; k++) //L/R LegJ, KneeJ, FootJ, ToeN
                            *tableChr++ = raw.data[k];

            

            
        }

        public override int OnCalculateSize(bool force)
        {
            _lookupCount = Children.Count;
            int size = 0x10;
            foreach (MoveDefRawDataNode p in Children) // Left & Right
            {
                size += 0x10;
                foreach (MoveDefRawDataNode q in p.Children) //LegJ, KneeJ, FootJ, ToeN 
                {
                    _lookupCount++;
                    size += 12; //4 for pointer, 8 for string
                }
            }

            return size;
        }
    }
    public unsafe class MoveDefCommomnUnk21EntryNode : MoveDefEntryNode
    {


    }

    public unsafe class MoveDefCommonUnk7ListNode : MoveDefEntryNode
    {
        internal FDefCommonUnk7Entry* First => (FDefCommonUnk7Entry*) WorkingUncompressed.Address;

        public override bool OnInitialize()
        {
            base.OnInitialize();
            return Size / 12 > 0;
        }

        public override void OnPopulate()
        {
            for (int i = 0; i < 4; i++) //Only 4 entries. Please keep it that way.
            {
                new MoveDefCommonUnk7EntryNode {_extOverride = true}.Initialize(this, First + i, 12);
            }
        }

        public override int OnCalculateSize(bool force)
        {
            _lookupCount = 0;
            int size = 0;
            foreach (MoveDefCommonUnk7EntryNode h in Children)
            {
                size += h.OnCalculateSize(true);
                _lookupCount += h._lookupCount;
            };
            return _entryLength = size;
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            _entryOffset = address;
            FDefCommonUnk7Entry* data = (FDefCommonUnk7Entry*) address;
            float* tblData = (float*)(address + 0x30);
            foreach (MoveDefCommonUnk7EntryNode h in Children)
            {
                data->_list._startOffset = tblData - (Parent as MoveDefDataCommonNode)._rebuildBase;
                _lookupOffsets.Add((int)data->_list._startOffset.Address - (int)_rebuildBase); //4 lookups
                h.Rebuild(data++, 12, true);
                foreach (MoveDefCommonUnk7EntryListEntryNode d in h.Children)
                {
                    bfloat temp = 0;
                    temp = d.unk1._data;
                    *tblData++ = temp;
                    temp = d.unk2._data;
                    *tblData++ = temp;
                }
            }
        }
    }

    public unsafe class MoveDefCommonUnk7EntryNode : MoveDefEntryNode
    {
        internal FDefCommonUnk7Entry* Header => (FDefCommonUnk7Entry*) WorkingUncompressed.Address;
        public override ResourceType ResourceFileType => ResourceType.Unknown;

        public int unk1, unk2;
        public short unk3, unk4;

        [Category("Unknown 7 Entry")] public int DataOffset => unk1;

        [Category("Unknown 7 Entry")] public int Count => unk2;

        [Category("Unknown 7 Entry")]
        public short Unknown1
        {
            get => unk3;
            set
            {
                unk3 = value;
                SignalPropertyChange();
            }
        }

        [Category("Unknown 7 Entry")]
        public short Unknown2
        {
            get => unk4;
            set
            {
                unk4 = value;
                SignalPropertyChange();
            }
        }

        public override bool OnInitialize()
        {
            base.OnInitialize();

            if (_name == null)
            {
                _name = "Data" + Index;
            }

            unk1 = Header->_list._startOffset;
            unk2 = Header->_list._listCount;
            unk3 = Header->_unk3;
            unk4 = Header->_unk4;
            return DataOffset > 0 && Count > 0;
        }

        public override void OnPopulate()
        {
            for (int i = 0; i < Count; i++)
            {
                new MoveDefCommonUnk7EntryListEntryNode {_name = "Entry" + i}.Initialize(this,
                   BaseAddress + DataOffset + i * 8, 8);
            }
        }

        public override int OnCalculateSize(bool force)
        {
            _lookupCount = 1; //1 for each of the 4 entries
            int size = 12; // base
            foreach (MoveDefSectionParamNode e in Children)
            {
                size += 8;
            }
            return size;
        }

        public override void Rebuild(VoidPtr address, int length, bool force)
        {
            _entryOffset = address;
            FDefCommonUnk7Entry* data = (FDefCommonUnk7Entry*) address; 
            data->_list._listCount = Children.Count;
            data->_unk3 = unk3;
            data->_unk4 = unk4;
        }
    }

    public unsafe class MoveDefCommonUnk7EntryListEntryNode : MoveDefSectionParamNode
    {
        internal FDefCommonUnk7EntryListEntry* Header => (FDefCommonUnk7EntryListEntry*) WorkingUncompressed.Address;
        public override ResourceType ResourceFileType => ResourceType.Unknown;

        public bfloat unk1, unk2;

        [Category("Unknown 7 Entry")]
        public float Unknown1
        {
            get => unk1;
            set
            {
                unk1 = value;
                SignalPropertyChange();
            }
        }

        [Category("Unknown 7 Entry")]
        public float Unknown2
        {
            get => unk2;
            set
            {
                unk2 = value;
                SignalPropertyChange();
            }
        }

        public override bool OnInitialize()
        {
            base.OnInitialize();

            if (_name == null)
            {
                _name = "Entry" + Index;
            }

            unk1 = Header->_unk1;
            unk2 = Header->_unk2;
            return false;
        }

        public override int OnCalculateSize(bool force)
        {
            _lookupCount = 0;
            return 12;
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            _entryOffset = address;
            FDefCommonUnk7EntryListEntry* data = (FDefCommonUnk7EntryListEntry*) address;
            data->_unk1 = unk1;
            data->_unk2 = unk2;
        }
    }

    public unsafe class MoveDefUnk11Node : MoveDefEntryNode
    {
        internal FDefListOffset* Header => (FDefListOffset*) WorkingUncompressed.Address;
        internal int i = 0;

        [Category("List Offset")] public int DataOffset => Header->_startOffset;

        [Category("List Offset")] public int Count => Header->_listCount;

        public override bool OnInitialize()
        {
            base.OnInitialize();
            return Count > 0;
        }

        public override void OnPopulate()
        {
            for (int i = 0; i < Count; i++)
            {
                new MoveDefUnk11EntryNode {_name = "Entry" + i}.Initialize(this, BaseAddress + DataOffset + i * 12,
                    12);
            }
        }

        public override int OnCalculateSize(bool force)
        {
            _lookupCount = Children.Count > 0 ? 1 : 0;
            _entryLength = 8;
            _childLength = 0;
            foreach (MoveDefUnk11EntryNode p in Children)
            {
                _childLength += p.CalculateSize(true);
                _lookupCount += p._lookupCount;
            }

            return _entryLength + _childLength;
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            VoidPtr addr = address;

            _entryOffset = addr;
            FDefListOffset* header = (FDefListOffset*) addr;
            header->_listCount = Children.Count;
            if (Children.Count > 0)
            {
                header->_startOffset = (int)( address - _rebuildBase + 8);
                _lookupOffsets.Add(address - (int) _rebuildBase); //1 + 2
                int* data = (int*)(address + 8);
                float* tblData = (float*)(address + 0x20); //2 sets of 12-byte parameters before the tables
                
                foreach (MoveDefUnk11EntryNode e in Children)
                {
                    _lookupOffsets.Add((int)(data - _rebuildBase + 4)); //It's the second word, not first!
                    *data++ = (bint)e.Unknown.Reverse();
                    *data++ = (bint)(tblData - _rebuildBase).Reverse(); //We will set up the table immediately after the two sets of entries
                    *data++ = (bint)e.Count.Reverse();
                    e.Children[0].Rebuild(tblData, e.Count * 4, true); //Section Parameter
                    tblData += e.Count;
                }
            }

            

            _entryOffset = address;

        }
    }

    public unsafe class MoveDefUnk11EntryNode : MoveDefEntryNode
    {
        internal FDefCommonUnk11Entry* Header => (FDefCommonUnk11Entry*) WorkingUncompressed.Address;
        internal int unk;

        [Category("List Offset")]
        public int Unknown
        {
            get => unk;
            set
            {
                unk = value;
                SignalPropertyChange();
            }
        }

        [Category("List Offset")] public int DataOffset => Header->_list._startOffset;

        [Category("List Offset")] public int Count => Header->_list._listCount;

        public override bool OnInitialize()
        {
            base.OnInitialize();
            unk = Header->_unk1;
            return Count > 0;
        }

        public override void OnPopulate()
        {
            /*
            for (int i = 0; i < Count; i++)
            {
                new MoveDefIndexNode {_name = "Index" + i}.Initialize(this, BaseAddress + DataOffset + i * 4, 4);
            }
            */
            new MoveDefSectionParamNode { _name = "Data" }.Initialize(this, BaseAddress + DataOffset, Count * 4);
        }
        public override int OnCalculateSize(bool force)
        {
            _lookupCount = 1;
            _entryLength = 12;
            _childLength = 0;
            foreach (MoveDefSectionParamNode p in Children)
            {
                _childLength += p.CalculateSize(true);
                _lookupCount += p._lookupCount;
            }

            return _entryLength + _childLength;
        }
    }

    public unsafe class MoveDefActionsSkipNode : MoveDefEntryNode
    {
        internal buint* Header => (buint*) WorkingUncompressed.Address;

        internal List<uint> ActionOffsets = new List<uint>();
        internal List<uint> Flags = new List<uint>();

        public MoveDefActionsSkipNode(string name)
        {
            _name = name;
        }

        public override bool OnInitialize()
        {
            base.OnInitialize();
            if (Name != "Hit Overlay Actions")
            {
                for (int i = 0; i < WorkingUncompressed.Length / 8; i++)
                {
                    ActionOffsets.Add(Header[i * 2]);
                    Flags.Add(Header[i * 2 + 1]);
                }
            }
            else
            {
                for (int i = 0; i < WorkingUncompressed.Length / 4; i++)
                {
                    ActionOffsets.Add(Header[i]);
                    Flags.Add(Header[i]);
                }
            }


            return true;
        }

        public override void OnPopulate()
        {
            int i = 0;
            foreach (int offset in ActionOffsets)
            {
                if (offset > 0)
                {
                    new MoveDefCommonActionNode("Action" + i, false, this, Flags[i]).Initialize(this,
                        new DataSource(BaseAddress + offset, 0));
                }
                else
                {
                    Children.Add(new MoveDefCommonActionNode("Action" + i, true, this, Flags[i]));
                }

                i++;
            }
        }
        public override int OnCalculateSize(bool force)
        {
            int size = Children.Count * 8;
            _lookupCount = Children.Count;
            if (force)  //toggle for command data to be included for ease of debugging
            {
                foreach (MoveDefCommonActionNode c in Children)
                {
                    size += c.OnCalculateSize(true);
                    _lookupCount += c._lookupCount;
                }
            }

            return size;
        }
    }

    public unsafe class MoveDefParamsOffsetNode : MoveDefCharSpecificNode
    {
        internal bint* Header => (bint*) WorkingUncompressed.Address;
        internal int i = 0;

        [Category("List Offset")] public int DataOffset => Header[0];

        public override bool OnInitialize()
        {
            base.OnInitialize();
            return true;
        }

        public override void OnPopulate()
        {
            new MoveDefSectionParamNode {_name = "Data"}.Initialize(this, BaseAddress + DataOffset, 168);
        }
    }

    public class MoveDefCommonActionNode : MoveDefActionNode
    {
        public byte Unk1
        {
            get => _unk1;
            set
            {
                _unk1 = value;
                SignalPropertyChange();
            }
        }

        [TypeConverter(typeof(Bin8StringConverter))]
        public Bin8 Flags
        {
            get => new Bin8(_unk2);
            set
            {
                _unk2 = (byte) value._data;
                SignalPropertyChange();
            }
        }

        public byte Unk3
        {
            get => _unk3;
            set
            {
                _unk3 = value;
                SignalPropertyChange();
            }
        }

        public byte Unk4
        {
            get => _unk4;
            set
            {
                _unk4 = value;
                SignalPropertyChange();
            }
        }

        public byte _unk1, _unk2, _unk3, _unk4;

        public MoveDefCommonActionNode(string name, bool blank, ResourceNode parent, uint flags) : base(name, blank,
            parent)
        {
            _unk1 = (byte) ((flags >> 24) & 0xFF);
            _unk2 = (byte) ((flags >> 16) & 0xFF);
            _unk3 = (byte) ((flags >> 8) & 0xFF);
            _unk4 = (byte) ((flags >> 0) & 0xFF);
        }
        /*
         
        Notes:
            Flash Overlay Actions:
                Unk1 is 0 for all
                Flags:
                    Action 0
                        0001 1110
                    Action 1-19, 21-22, 24-25, 33-35, 37-38
                        0110 0100
                    Action 20
                        0000 1100
                    Action 23
                        1001 0110
                    Action 26, 39-40
                        0000 1010
                    Action 27-30, 36
                        0011 1100
                    Action 31
                        0000 1111
                    Action 32
                        0000 1011
                Unk3:
                   Action 0-27, 30-32, 35-40
                        0
                   Action 28-29, 33-34
                        1
                Unk4:
                   Action 0-25, 27-36
                        0
                   Action 26, 37-40
                        1
            
            Hit Overlay Actions:
                Unk1, Flags are 0 for all
                Unk2, Unk3 vary wildly
            Screen Tint Actions:
                Unk1, Unk3 and Unk4 are 0 for all
                Flags:
                    Action 0-7
                        0011 0010
                    Action 8, 10-12
                        0111 1111
                    Action 9
                        0111 1110
          
        */
    }
}