using BrawlLib.Internal;
using BrawlLib.SSBB.ResourceNodes.Moveset.Converters;
using BrawlLib.SSBB.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class MoveDefAnimParamNode : MoveDefEntryNode
    {
        internal AnimParamHeader* Header => (AnimParamHeader*)WorkingUncompressed.Address;

        public override ResourceType ResourceFileType => ResourceType.MDefAnimParam;
        public List<SpecialOffset> specialOffsets = new List<SpecialOffset>();
        internal uint DataLen;
        private int unk14, unk14b;

        [Category("Data Offsets")] public int SubactionFlags => Header->SubactionFlagsStart;

        [Category("Data Offsets")] public int SubactionFlagsCount => Header->SubactionCount;

        [Category("Data Offsets")] public int ActionFlags => Header->ActionFlagsStart;

        [Category("Data Offsets")] public int ActionFlagsCount => Header->ActionCount;

        [Category("Data Offsets")] public int Unk4 => Header->Unknown4;

        [Category("Data Offsets")] public int Unk5 => Header->Unknown5;

        [Category("Data Offsets")] public int Unk6 => Header->Unknown6;

        [Category("Data Offsets")] public int Unk7 => Header->Unknown7;

        [Category("Data Offsets")] public int Unk8 => Header->Unknown8;

        [Category("Data Offsets")] public int Unk9 => Header->Unknown9;

        [Category("Data Offsets")] public int Unk10 => Header->Unknown10;

        [Category("Data Offsets")] public int Unk11 => Header->Unknown11;

        [Category("Data Offsets")] public int HitData => Header->Unknown12;

        [Category("Data Offsets")] public int Unk13 => Header->Unknown13;

        [Category("Data Offsets")] public int CollisionData => Header->Unknown14;

        [Category("Data Offsets")] public int Unk15 => Header->Unknown15;


        public MoveDefFlagsNode _animFlags;
        public MoveDefActionFlagsNode actionFlags;
        public MoveDefSectionParamNode physNode, effNode;
        public MoveDefMiscHurtBoxesNode hurtboxes, shieldboxes;
        public MoveDefActionListNode subActions, actions, hiddenActions;
        public CollisionDataNode collision;

        public MoveDefAnimParamNode(uint dataLen, string name)
        {
            DataLen = dataLen;
            _name = name;
        }

        public FDefSubActionStringTable subActionTable;
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

        public override bool OnInitialize()
        {
            //base.OnInitialize();
            bint* current = (bint*)Header;
            for (int i = 0; i < 16; i++)
            {
                specialOffsets.Add(new SpecialOffset { Index = i, Offset = *current++ });
            }
            //CalculateDataLen();
            return true;
        }


        public override void OnPopulate()
        {
            //Accounted for: 0-3, 5-15
            //Unaccounted for: 4

            //4 in N/A?
            //12 in WarioBike (size 0x0C "0xD18"), RobotGyro ("0xB78"), Metroid ("0xCDC"), Starfy ("0x15F0"), Sonans ("0x818")
            //13 in N/A?
            //14 in WarioBike (unk "0xD24")
            //15 in WarioBike/SnakeGrenade/HammerBros/Jugem/Lati@s/Tingle ("0x40")
            //          PeachDaikon ("0x43"), LinkBomb/ToonLinkBomb/Kusudama ("0x108"), SmashBall/Nyarth ("0x204")
            //           Robin/Excitebike/Soldier/Tank 3, Manaphy 0x30C, Andross 0x7108. Figure 0x148


            #region Populate
            bint* actionOffset;
            List<int> ActionOffsets;

            subActions = new MoveDefActionListNode { _name = "SubAction Scripts", _parent = this };
            actions = new MoveDefActionListNode { _name = "Action Scripts", _parent = this };
            hiddenActions = new MoveDefActionListNode { _name = "Hidden Action Scripts", _parent = this };
            #region actions
            for (int i = 10; i < 12; i++)
            {
                actionOffset = (bint*)(BaseAddress + specialOffsets[10].Offset);
                ActionOffsets = new List<int>();
                for (int x = 0; x < (i == 10 ? ActionFlagsCount : 4); x++)
                {
                    ActionOffsets.Add(actionOffset[x]);
                }
                if (i == 10)
                    actions.ActionOffsets.Add(ActionOffsets);
                else
                    hiddenActions.ActionOffsets.Add(ActionOffsets);
            }

            if (ActionFlagsCount != 0)
            {
                int count = ActionFlagsCount;

                //Initialize using first offset so the node is sorted correctly
                actions.Initialize(this, BaseAddress + specialOffsets[10].Offset, 0);
                hiddenActions.Initialize(this, BaseAddress + specialOffsets[11].Offset, 0);

                if (specialOffsets[2] != null)
                {
                    (actionFlags = new MoveDefActionFlagsNode("Item Action Flags", ActionFlagsCount)
                    { offsetID = 2 }).Initialize(this,
                        new DataSource(BaseAddress + specialOffsets[2].Offset, ActionFlagsCount * 16));
                }

                //Set up groups
                for (int i = 0; i < count; i++)
                {
                    actions.AddChild(new MoveDefActionGroupNode { _name = "Action " + Convert.ToString(i, 16).ToUpper(), offsetID = i },
                        false);
                }
                for (int i = 0; i < 4; i++)
                {
                    hiddenActions.AddChild(new MoveDefActionGroupNode { _name = "Hidden Action " + Convert.ToString(i, 16), offsetID = i },
                        false);
                }
                //Add children
                if (ActionFlagsCount != 0)
                {
                    PopulateActionGroup(actions, actions.ActionOffsets[0], false, 0);
                    PopulateActionGroup(hiddenActions, hiddenActions.ActionOffsets[0], false, 0);
                }

                //Add to children (because the parent was set before initialization)
                Children.Add(actions);
                Children.Add(hiddenActions);
            }
            #endregion
            #region subactions
            if (SubactionFlagsCount != 0)
            {
                (_animFlags = new MoveDefFlagsNode { offsetID = 0, _parent = this }).Initialize(this,
                    BaseAddress + specialOffsets[0].Offset, SubactionFlagsCount * 8);
            }
            for (int i = 7; i < 10; i++)
            {
                actionOffset = (bint*)(BaseAddress + specialOffsets[i].Offset);
                ActionOffsets = new List<int>();
                for (int x = 0; x < SubactionFlagsCount; x++)
                {
                    ActionOffsets.Add(actionOffset[x]);
                }

                subActions.ActionOffsets.Add(ActionOffsets);
            }

            if (SubactionFlagsCount != 0)
            {
                string name;
                int count = SubactionFlagsCount;


                subActions.Initialize(this, BaseAddress + specialOffsets[7].Offset, 0);

                //Set up groups
                for (int i = 0; i < count; i++)
                {
                    if (_animFlags._names.Count > i && _animFlags._flags[i]._stringOffset > 0)
                    {
                        name = _animFlags._names[i];
                    }
                    else
                    {
                        name = "<null>";
                    }

                    subActions.AddChild(
                        new MoveDefSubActionGroupNode
                        {
                            _name = name,
                            _flags = _animFlags._flags[i]._Flags,
                            _inTransTime = _animFlags._flags[i]._InTranslationTime
                        }, false);
                }

                //Add children
                for (int i = 0; i < 3; i++)
                {
                    if (SubactionFlagsCount != 0)
                    {
                        PopulateActionGroup(subActions, subActions.ActionOffsets[i], true, i);
                    }
                }

                //Add to children (because the parent was set before initialization)
                Children.Add(subActions);
                Root._subActions = subActions;
            }
            #endregion

            if (specialOffsets[5].Offset != 0)
            {
                physNode = new MoveDefSectionParamNode { _name = "Physics Init Data" };
                physNode.Initialize(this, BaseAddress + specialOffsets[5].Offset, 32);

            }
            if (specialOffsets[6].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[6].Offset);

                effNode = new MoveDefSectionParamNode { _name = "Effect Base Node" };
                effNode.Initialize(this, BaseAddress + addr[0], addr[1] * 4);
            }

            if (HitData != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[12].Offset);
                hurtboxes = new MoveDefMiscHurtBoxesNode(addr[1]);
                hurtboxes.Initialize(this, BaseAddress + addr[0], addr[1] * 0x20);
                hurtboxes._name = "Override Hurtbox List";
            }
            if (specialOffsets[13].Offset != 0) //Reflect/Shield Type
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[13].Offset);
                shieldboxes = new MoveDefMiscHurtBoxesNode(1);
                shieldboxes.Initialize(this, BaseAddress + addr[0], 0x20);
                shieldboxes._name = "Override Reflectbox List";
            }
            if (specialOffsets[14].Offset != 0) //ECB Type
            {
                collision = new CollisionDataNode();
                collision.Initialize(this, BaseAddress + specialOffsets[14].Offset, 0);
                collision._name = "Override Collision Info";
            }
            #endregion

            Root._actions = actions;
            Root._hiddenActions = hiddenActions;
            //SortChildren();
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

        public override int OnCalculateSize(bool force)
        {
            _entryLength = 124;// 124;
            _childLength = MovesetItemConverter.CalcDataSize(this);
            return _entryLength + _childLength;
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            MovesetItemConverter.BuildData(this, (AnimParamHeader*)dataHeaderAddr, address, length, force);
        }

    }

    public unsafe class MoveDefItemParamNode : MoveDefEntryNode
    {
        internal VoidPtr* Header => (VoidPtr*)WorkingUncompressed.Address;
        internal uint DataLen;
        public ItmParamEntryNode itemEntry;
        //public override ResourceType ResourceFileType => ResourceType.NoEditFolder;
        public MoveDefItemParamNode(uint dataLen, string name)
        {
            DataLen = dataLen;
            _name = name;
        }
        public override bool OnInitialize()
        {
            return true;
        }

        public override void OnPopulate()
        {
            itemEntry = new ItmParamEntryNode() { _name = "Entry" };
            itemEntry.Initialize(this, Header, ItmParamEntry.Size);
        }
        public override int OnCalculateSize(bool force)
        {
            return ItmParamEntry.Size;
        }
        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            _entryOffset = address;
            itemEntry.Rebuild(address, ItmParamEntry.Size, force);
        }
    }
}