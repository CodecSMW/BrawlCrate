using BrawlLib.Internal;
using BrawlLib.SSBB.ResourceNodes.Moveset.Enemies;
using BrawlLib.SSBB.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Numerics;
using System.Security.RightsManagement;
using System.Xml.Linq;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class MoveDefEnmWpnNode : MoveDefEntryNode
    {
        internal EnemyMovesetHeader* Header => (EnemyMovesetHeader*)WorkingUncompressed.Address;
        internal uint DataLen;

        internal int entryCount;
        internal int subOffset;

        public List<MoveDefSubActionGroupNode> subList;

        public FDefSubActionFlag _animFlags;

        public MoveDefEnmWpnNode(uint dataLen, string name)
        {
            DataLen = dataLen;
            _name = name;
        }

        public VoidPtr dataHeaderAddr;

        public override bool OnInitialize()
        {
            //base.OnInitialize();
            bint* current = (bint*)Header;
            subOffset = current[0];
            entryCount = current[1];
            return true;
        }
        public override void OnPopulate()
        {
            subList = new List<MoveDefSubActionGroupNode>();
            for (int i = 0; i < entryCount; i++)
            {
                VoidPtr* ptr = (VoidPtr*)(BaseAddress + subOffset + (i*8));
                bint* addr = (bint*)(ptr);
                byte* flagInfo = (byte*)(ptr+1);

                MoveDefSubActionGroupNode test = new MoveDefSubActionGroupNode();
                test._name = "Weapon Subaction " + i;
                test._flags = (AnimationFlags)flagInfo[3];
                test._inTransTime = flagInfo[2];
                subList.Add(test);
                AddChild(test);
                if (addr[0] > 0)
                {
                    new MoveDefActionNode("<null>", false, test).Initialize(test,
                        new DataSource(BaseAddress + addr[0], 0));
                }

            }       
            
        }
    }
    public unsafe class MoveDefEnmDataNode : MoveDefEntryNode
    {
        internal EnemyMovesetHeader* Header => (EnemyMovesetHeader*)WorkingUncompressed.Address;

        public List<SpecialOffset> specialOffsets = new List<SpecialOffset>();
        internal uint DataLen;
        private int unk14, unk14b;

        [Category("Data Offsets")] public int SubactionFlags => Header->Unknown0;

        [Category("Data Offsets")] public int HitDataSet => Header->Unknown1;

        [Category("Data Offsets")] public int HitWeaknessDataSet => Header->Unknown2;

        [Category("Data Offsets")] public int UnkParam3 => Header->Unknown3;

        [Category("Data Offsets")] public int UnkCount => Header->Unknown4;

        [Category("Data Offsets")] public int Attributes1 => Header->Unknown5;

        [Category("Data Offsets")] public int Attributes2 => Header->Unknown6;

        [Category("Data Offsets")] public int Unk7 => Header->Unknown7;

        [Category("Data Offsets")] public int Unk8 => Header->Unknown8;

        [Category("Data Offsets")] public int Unk9 => Header->Unknown9;

        [Category("Data Offsets")] public int Unk10 => Header->Unknown10;

        [Category("Data Offsets")] public int Unk11 => Header->Unknown11;

        [Category("Data Offsets")] public int HitData => Header->Unknown12;

        [Category("Data Offsets")] public int Unk13 => Header->Unknown13;

        [Category("Data Offsets")] public int CollisionData => Header->Unknown14;

        [Category("Data Offsets")] public int Unk15 => Header->Unknown15;

        [Category("Data Offsets")] public int CollGenType => unk14;
        [Category("Data Offsets")] public int CollGenTypeCnt => unk14b;

        public MoveDefFlagsNode _animFlags;
        public MoveDefActionFlagsNode actionFlags;
        public MoveDefSectionParamNode weaknessData, param3Data, settableData, camRangeData, camSphereData;// grColData; 2, 3, 5, 25, 26
        public MoveDefSectionParamNode attributes1, attributes2; //6 7
        public MoveDefMiscHurtBoxesNode hurtboxes, shieldboxes;
        public List<MoveDefRawDataNode> triggerList;
        public CollDataType0 collisionData;
        public MoveDefBoneIndicesNode grColData;
        public MoveDefActionNode enmHiddenAction;

        public MoveDefEnmTriggerNode enmTrigger;
        public MoveDefEnmDataNode(uint dataLen, string name)
        {
            DataLen = dataLen;
            _name = name;
        }

        public override bool OnInitialize()
        {
            //base.OnInitialize();
            bint* current = (bint*)Header;
            for (int i = 0; i < 28; i++)
            {
                specialOffsets.Add(new SpecialOffset { Index = i, Offset = *current++});
            }
            return true;
        }

        public VoidPtr dataHeaderAddr;

        public override void OnPopulate()
        {
            //Accounted for: 0-3, 5-11
            //Unaccounted for: 4, 12-15

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
            int count, subCount;
            triggerList = new List<MoveDefRawDataNode>();

            MoveDefActionListNode subActions =
                    new MoveDefActionListNode { _name = "SubAction Scripts", _parent = this },
                actions = new MoveDefActionListNode { _name = "Action Scripts", _parent = this },
                hiddenActions = new MoveDefActionListNode { _name = "Hidden Action Scripts", _parent = this };

            count = (specialOffsets[20].Offset - specialOffsets[19].Offset) / 4;
            subCount = (specialOffsets[11].Offset - specialOffsets[10].Offset) / 4;
            int ActionFlagsCount = count;
            int SubactionFlagsCount = subCount;

            for (int i = 19; i < 22; i++) //3 instead of the 2 for fighters. Enter, Exit and Init.
            {
                actionOffset = (bint*)(BaseAddress + specialOffsets[i].Offset);
                ActionOffsets = new List<int>();
                for (int x = 0; x < count; x++)
                {
                    ActionOffsets.Add(actionOffset[x]);
                }

                actions.ActionOffsets.Add(ActionOffsets);
            }
            for (int i = 10; i < 19; i++) //12 instead of 3 or 4. 3x Main, GFX and SFX
            {
                actionOffset = (bint*)(BaseAddress + specialOffsets[i].Offset);
                ActionOffsets = new List<int>();
                for (int x = 0; x < subCount; x++)
                {
                    ActionOffsets.Add(actionOffset[x]);
                }

                subActions.ActionOffsets.Add(ActionOffsets);
            }
            //Initialize using first offset so the node is sorted correctly
            actions.Initialize(this, BaseAddress + specialOffsets[19].Offset, 0);
            //TODO: hiddenActions at 22 with unique formating
            //hiddenActions.Initialize(this, BaseAddress + specialOffsets[24].Offset, 0);

            //Set up groups
            for (int i = 0; i < count; i++)
            {
                actions.AddChild(new MoveDefActionGroupNode { _name = "Custom Action " + Convert.ToString(i, 16).ToUpper(), offsetID = i },
                    false);
            }


            #endregion

            //suspected known: 0, 
            //complete: 23, 25-27

            /*
            TODO:

             
            */


            #region Named sections and Collisions
            /*
            TODO: 
            8 - Unknown8
            22 - UnknownDatas22
            DONE:
            0 - Subaction flags
            1 - Hurtboxes
            2 - WeaknessDataSet
            3 - Parameter3 
            4 - Parameter3 Entry Count
            5 - SetableNodeData
            6-7 Attributes
            9 - Action Flags
            10-18 Subactions
            19-21 Actions
            23 - Misc Collision Data
            24 - Hidden Enemy Action
            25 - cameraRangeSet
            26 - CameraClipSphereData
            27 - grColDataSet
            */
            if (specialOffsets[1].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[1].Offset);
                hurtboxes = new MoveDefMiscHurtBoxesNode(addr[1]);
                hurtboxes.Initialize(this, BaseAddress + addr[0], addr[1] * 0x20);
                hurtboxes._name = "Override Hurtbox List";
            }
            if (specialOffsets[23].Offset != 0)
            {
                collisionData = new CollDataType0 { _name = "Misc Collision Data" }; //not named
                collisionData.Initialize(this, new DataSource(BaseAddress + specialOffsets[23].Offset, 0)); //typically size 8
            }

            if (specialOffsets[2].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[2].Offset);

                weaknessData = new MoveDefSectionParamNode { _name = "HitWeaknessDataSet" };
                weaknessData.Initialize(this, BaseAddress + addr[0], addr[1] * 4); // word entries
            }
            if (specialOffsets[3].Offset != 0 && specialOffsets[4].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[3].Offset);

                weaknessData = new MoveDefSectionParamNode { _name = "Paramater3" }; //Note, parameter may be more complex than anticipated. EnmGyraan has duplicate data follow?
                weaknessData.Initialize(this, addr, specialOffsets[4].Offset * 4); // word entries. Not a pointer for some reason and directly points to block?
            }
            if (specialOffsets[5].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[5].Offset);

                settableData = new MoveDefSectionParamNode { _name = "SettableNodeDataSet" };
                settableData.Initialize(this, BaseAddress + addr[0], addr[1] * 4); // word entries
            }       
            if (specialOffsets[7].Offset != 0)
            {
                //specialOffsets[6] will be the start of the file so is expected to be offset 0

                bint* addr = (bint*)(BaseAddress + specialOffsets[6].Offset);
                attributes1 = new MoveDefSectionParamNode { _name = "Attributes1" };
                attributes1.Initialize(this, addr, 0xB0); // fixed size
                
                addr = (bint*)(BaseAddress + specialOffsets[7].Offset);
                attributes2 = new MoveDefSectionParamNode { _name = "Attributes2" };
                attributes2.Initialize(this, addr, 0xB8); // fixed size
            }
            if (specialOffsets[25].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[25].Offset);

                camRangeData = new MoveDefSectionParamNode { _name = "cameraRangeSet" }; 
                camRangeData.Initialize(this, BaseAddress + addr[0], addr[1] * 0x14); //size 0x14
            }
            if (specialOffsets[26].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[26].Offset);

                camSphereData = new MoveDefSectionParamNode { _name = "CameraClipSphereData" };
                camSphereData.Initialize(this, BaseAddress + addr[0], addr[1] * 0x14); //size 0x14
            }
            if (specialOffsets[27].Offset != 0)
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[27].Offset);

                grColData = new MoveDefBoneIndicesNode("grColDataSet", addr[1] * 2);
                grColData.Initialize(this, BaseAddress + addr[0], addr[1] * 8); // bone indexes?
            }

            if (specialOffsets[22].Offset != 0) //TODO: Trigger sets that have action overrides. ActionInit?
            {
                bint* addr = (bint*)(BaseAddress + specialOffsets[22].Offset);
                MoveDefRawDataNode tempData;
                for (int i = 0; i < (specialOffsets[8].Offset - specialOffsets[22].Offset) / 4; i++)
                {
                    tempData = new MoveDefRawDataNode("Trigger Action " + i); //TODO: Always internal "custom" actions + external actions
                    int tempOff = addr[i];
                    if (addr[i] >= 0x200000)
                    {
                        break;
                        //EnmPorky: 33 pointers to blocks and then "Run" from subaction names follows
                        //EnmDuon: 24 pointers to blocks and then "Enter" subaction name
                    }
                    tempData.Initialize(this, BaseAddress + addr[i], 0x18);
                    triggerList.Add(tempData);
                }
            }
            if (specialOffsets[8].Offset != 0) //TODO: Trigger sets that have action overrides. ActionInit?
            {
                enmTrigger = new MoveDefEnmTriggerNode { _name = "Trigger Section" };
                enmTrigger.Initialize(this, BaseAddress + specialOffsets[8].Offset, 0);
            }

            #endregion
            #region Actions

            //Add children
            if (count != 0) //ActionflagsCount
            {
                for (int i = 0; i < 3; i++)
                {
                    //if (specialOffsets[i + 19].Size != 0)
                    PopulateActionGroup(actions, actions.ActionOffsets[i], false, i);
                }
                //PopulateActionGroup(hiddenActions, hiddenActions.ActionOffsets[0], false, 0);
            }
            Children.Add(actions);
            if (specialOffsets[9].Offset != 0)
            {
                (actionFlags = new MoveDefActionFlagsNode("Enemy Action Flags", ActionFlagsCount)
                { offsetID = 2 }).Initialize(this,
                    new DataSource(BaseAddress + specialOffsets[9].Offset, ActionFlagsCount * 16));
            }
            if (specialOffsets[24].Offset > 0)
            {
                actionOffset = (bint*)(BaseAddress + specialOffsets[24].Offset);
                enmHiddenAction = new MoveDefActionNode("Hidden Enemy Action", false, this);
                enmHiddenAction.Initialize(this, new DataSource(actionOffset, 0));
            }
            Root._actions = actions;
            #endregion
            #region subactions
            if (subCount != 0) //SubactionFlagsCount
            {
                (_animFlags = new MoveDefFlagsNode { offsetID = 0, _parent = this }).Initialize(this,
                    BaseAddress + specialOffsets[0].Offset, subCount * 8);
            }
            //Initialize using first offset so the node is sorted correctly
            subActions.Initialize(this, BaseAddress + specialOffsets[10].Offset, 0);

            //Set up groups
            for (int i = 0; i < subCount; i++)
            {
                string name;
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
            for (int i = 0; i < 9; i++)
            {
                if (subCount != 0)
                {
                    PopulateActionGroup(subActions, subActions.ActionOffsets[i], true, i);
                }
            }
            
            //Add to children (because the parent was set before initialization)
            Children.Add(subActions);

            Root._subActions = subActions;
            #endregion

            //SortChildren();
        }
        
        public void PopulateActionGroup(ResourceNode g, List<int> ActionOffsets, bool subactions, int index)
        {
            string innerName = "";
            if (subactions)
            {
                switch(index)
                {
                    case 0: case 1: case 2:
                        innerName = "Main " + Convert.ToString(index + 1); break;
                    case 3: case 4: case 5:
                        innerName = "GFX " + Convert.ToString(index - 2); break;
                    case 6: case 7: case 8:
                        innerName = "SFX " + Convert.ToString(index - 5); break;
                    default:
                        innerName = "Other " + Convert.ToString(index - 8); break;
                }
            }
            else
            {
                switch(index)
                {
                    case 0:
                        innerName = "Entry"; break;
                    case 1:
                        innerName = "Init"; break;
                    default:
                        innerName = "Exit"; break;
                }
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
}

