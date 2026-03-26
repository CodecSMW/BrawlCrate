using BrawlLib.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlLib.SSBB.ResourceNodes.Moveset.Enemies
{
    public unsafe class MoveDefEnmTriggerNode : MoveDefEntryNode
    {
        internal bint* Start => (bint*)WorkingUncompressed.Address;
        public List<MoveDefSectionParamNode> rangeList;
        public List<MoveDefEnmTrigger> triggerList;

        public override bool OnInitialize()
        {
            rangeList = new List<MoveDefSectionParamNode>();
            triggerList = new List<MoveDefEnmTrigger>();
            return true;
        }
        public override void OnPopulate()
        {
            bint* addr = Start;
            MoveDefSectionParamNode tempData;
            for (int i = 0; i < 60; i++)
            {
                bint* offsetInfo = addr + 0x10 * i; //0x10 words = 0x40 bytes
                if (offsetInfo[0] != 0xB)
                {
                    tempData = new MoveDefSectionParamNode { _name = "Range " + i };
                    tempData.Initialize(this, offsetInfo, 0x40);
                    rangeList.Add(tempData);
                }
                else
                {
                    tempData = new MoveDefSectionParamNode { _name = "Range " + i + " - Camera" };
                    tempData.Initialize(this, offsetInfo, 0x24);
                    rangeList.Add(tempData);

                    offsetInfo += 0x24 / 4;
                    i++;

                    tempData = new MoveDefSectionParamNode { _name = "Range " + i + " - TargetSearchUnit" };
                    tempData.Initialize(this, offsetInfo, 0x20);
                    rangeList.Add(tempData);

                    offsetInfo += 0x20 / 4;


                    offsetInfo += GetParamCount(tempData, offsetInfo);

                    //TODO: More
                    tempData = new MoveDefSectionParamNode { _name = "Randomization Table" }; //0x10 blocks
                    tempData.Initialize(this, offsetInfo, 0x10 * 4); //4 possibilities to use as ranges
                    rangeList.Add(tempData);

                    offsetInfo += 0x10;
                    for (int j = 0, k = GetTriggerCount(); j < k; j++)
                    {
                        MoveDefEnmTrigger trigger = new MoveDefEnmTrigger { _name = "Trigger Node " + j };
                        trigger.Initialize(this,offsetInfo,0xC);
                        triggerList.Add(trigger);
                        offsetInfo += 3; //0xC bytes, 3 words
                    }

                    break;



                    //TODO: sweaponanmcmddata set following format
                    /*
                     0x8 bytes - pointer, sub count (int) Maybe always 3?

                     0x8 * subCount bytes -
                        - subacton
                        - 4
                        - subaction
                        - 4
                        - subaction
                        - 4

                        then followed by Data Table for boss (seen in Brawl Item Editor)
                        then relocation offsets
                    */
                }
            }
        }

        #region EnemySpecific

        public int GetTriggerCount()
        {
            int triggerCount = 0;
            switch(RootNode.Name)
            {
                case "EnmRayquaza":
                    triggerCount = 17; break;
            }
            return triggerCount;
        }
        public int GetParamCount(MoveDefSectionParamNode tempData, bint* offsetInfo)
        {
            int paramCount = 0;
            switch (RootNode.Name)
            {
                case "EnmFalconflyer":
                    paramCount = 1; goto case "setup";
                //EnmFalconflyer: 1 int
                case "EnmBossPackun":
                    paramCount = 4; goto case "setup";
                //EnmBossPackun: 1 float, 3 ints
                case "EnmRayquaza":
                case "EnmDuon":
                case "EnmGalleom":
                    paramCount = 6; goto case "setup";
                //EnmRayquaza: 5 floats, 1 int
                //EnmDuon: 3 floats, 3 ints
                //EnmGalleom: 4 floats, 2 ints
                case "EnmRidley":
                    paramCount = 7; goto case "setup";
                //EnmRidley: 2 floats, 5 ints
                case "EnmMetaridley":
                case "EnmTaboo":
                    paramCount = 11; goto case "setup";
                //EnmMetaRidley: 7 floats, 4 ints 
                //EnmTaboo: 9 floats, 2 ints
                case "EnmMasterhand":
                    paramCount = 21; goto case "setup";
                //EnmMasterhand: 14 floats, 7 ints 
                case "EnmCrazyhand":
                    paramCount = 14; goto case "setup";
                //EnmCrazyhand: 12 floats, 2 ints 
                case "EnmPorky":
                    paramCount = 9; goto case "setup";
                //EnmPorky: 5 floats, 4 ints
                case "setup":
                    tempData = new MoveDefSectionParamNode { _name = "Param" };
                    tempData.Initialize(this, offsetInfo, paramCount * 4);
                    rangeList.Add(tempData);
                    break;
                default:
                    break;
            }
            return paramCount;
        }
        #endregion

    }

    public unsafe class MoveDefEnmTrigger : MoveDefEntryNode
    {
        internal VoidPtr Start => WorkingUncompressed.Address;
        //TODO: add support for adding actions to UI
        [Category("Data Offsets")] public int Odds => odds;
        [Category("Data Offsets")] public float Value => value;

        int odds;
        float value;
        public MoveDefActionNode triggerAction;

        public override bool OnInitialize()
        {
            return true;
        }
        public override void OnPopulate()
        {
            bint* addr = (bint*)Start;
            bfloat* addr2 = (bfloat*)Start;
            odds = addr[0];
            value = addr2[1];
            if (addr[2] != 0) //if blank, no action attached
            {
                triggerAction = new MoveDefActionNode("Trigger", false, this);
                triggerAction.Initialize(this, new DataSource(BaseAddress + addr[2], 0));
            }
        }
    }

}
