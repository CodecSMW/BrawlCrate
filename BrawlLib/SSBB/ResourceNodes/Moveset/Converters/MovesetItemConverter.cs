using BrawlLib.Internal;
using BrawlLib.SSBB.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlLib.SSBB.ResourceNodes.Moveset.Converters
{
    public unsafe class MovesetItemConverter : MovesetConverter
    {
        public static int CalcDataSize(MoveDefAnimParamNode node)
        {
            int
                lookupCount = 0,
                part1Len = 0,
                part2Len = 0,
                part3Len = 0,
                part4Len = 0,
                part5Len = 0,
                part6Len = 0,
                part7Len = 0;

            MoveDefNode RootNode = node.Parent.Parent as MoveDefNode;

            FDefSubActionStringTable subActionTable = new FDefSubActionStringTable();

            #region Part 1: Action Flags, Actions and Hidden Actions
            part1Len += GetSize(node.actionFlags, ref lookupCount);
            part1Len += Calc_Actions(RootNode._actions, ref lookupCount);
            part1Len += Calc_Actions(RootNode._hiddenActions, ref lookupCount);
            part1Len += Calc_Actions(RootNode._subActions, ref lookupCount);

            for (int i = 0; i < RootNode._subRoutineList.Count; i++)
            {
                if ((RootNode._subRoutineList[i] as MoveDefActionNode)._actionRefs.Count > 0)
                {
                    part1Len += GetSize(RootNode._subRoutineList[i] as MoveDefActionNode, ref lookupCount);
                }
            }
            #endregion

            #region Part 2: Misc

            //Hurtboxes and Reflectors/Shieldboxes
            if (node.hurtboxes != null)
            {
                part2Len += GetSize(node.hurtboxes, ref lookupCount) + 8; lookupCount++;
            }
            if (node.shieldboxes != null)
            {
                part2Len += GetSize(node.shieldboxes, ref lookupCount) + 8; lookupCount++;
            }
            //Misc
            part2Len += GetSize(node.physNode, ref lookupCount);
            part2Len += GetSize(node.effNode, ref lookupCount) + 8; lookupCount++;
            part2Len += GetSize(node.collision, ref lookupCount);

            #endregion

            #region part 3

            //Actions part 1 and 2 offsets

            part3Len += RootNode._actions.Children.Count * 4;
            part3Len += RootNode._hiddenActions.Children.Count * 4;

            #endregion
            #region Part 4: Subaction Names

            //Subaction Names
            lookupCount++; //offset to the list
            foreach (MoveDefSubActionGroupNode g in RootNode._subActions.Children)
            {
                if (g.Name != "<null>")
                {
                    lookupCount++;
                    subActionTable.Add(g.Name);
                }

                part4Len += 8;
            }

            //Subaction string table
            part4Len += subActionTable.TotalSize;

            #endregion

            #region Part 5

            //Subaction offsets already written
            lookupCount += 3; //offset to the lists
            part5Len += RootNode._subActions.Children.Count * 12;

            #endregion

            node.subActionTable = subActionTable;
            node._lookupCount = lookupCount;

            return node._childLength =
                (node.part1Len = part1Len) +
                (node.part2Len = part2Len) +
                (node.part3Len = part3Len) +
                (node.part4Len = part4Len) +
                (node.part5Len = part5Len) +
                (node.part6Len = part6Len) +
                (node.part7Len = part7Len);
        }

        internal static void BuildData(MoveDefAnimParamNode node, AnimParamHeader* header, VoidPtr address, int length,
                               bool force)
        {
            MoveDefNode RootNode = node.Parent.Parent as MoveDefNode;

            VoidPtr dataAddress = address;
            VoidPtr baseAddress = node._rebuildBase;

            node._entryOffset = header;

            bint* actionOffsets = (bint*)(dataAddress + node.part1Len + node.part2Len);
            bint* hiddenActionOffsets = actionOffsets + RootNode._actions.Children.Count;

            bint* mainOffsets = (bint*)(dataAddress + (node.part1Len + node.part2Len + node.part3Len + node.part4Len));
            bint* GFXOffsets = (bint*)((VoidPtr)mainOffsets + RootNode._subActions.Children.Count * 4);
            bint* SFXOffsets = (bint*)((VoidPtr)GFXOffsets + RootNode._subActions.Children.Count * 4);

            header->Unknown7 = (int)mainOffsets - (int)baseAddress;
            header->Unknown8 = (int)GFXOffsets - (int)baseAddress;
            header->Unknown9 = (int)SFXOffsets - (int)baseAddress;
            header->Unknown10 = (int)actionOffsets - (int)baseAddress;
            header->Unknown11 = (int)hiddenActionOffsets - (int)baseAddress;

            #region Part 1: Actions


            header->ActionCount = node.actions.Children.Count;
            header->ActionFlagsStart = Rebuild(RootNode, node.actionFlags, ref dataAddress, baseAddress);

            Rebuild_Actions(ref actionOffsets, 0, RootNode._actions, RootNode, ref dataAddress, baseAddress); //Actions
            Rebuild_Actions(ref hiddenActionOffsets, 0, RootNode._hiddenActions, RootNode, ref dataAddress, baseAddress);

            Rebuild_Actions(ref mainOffsets, 0, RootNode._subActions, RootNode, ref dataAddress, baseAddress); //Main
            Rebuild_Actions(ref GFXOffsets, 1, RootNode._subActions, RootNode, ref dataAddress, baseAddress); //GFX
            Rebuild_Actions(ref SFXOffsets, 2, RootNode._subActions, RootNode, ref dataAddress, baseAddress); //SFX //Hidden Actions

            for (int i = 0; i < RootNode._subRoutineList.Count; i++)
            {
                if ((RootNode._subRoutineList[i] as MoveDefActionNode)._actionRefs.Count > 0)
                {
                    Rebuild(RootNode, RootNode._subRoutineList[i] as MoveDefActionNode, ref dataAddress, baseAddress);
                }
            }

            #endregion

            #region Part 2

            if ((int)dataAddress - (int)baseAddress != node.part1Len)
            {
                Console.WriteLine("p2");
            }

            header->Unknown12 = 0; //default to empty for both
            header->Unknown13 = 0; //the game knows what to do if 0
            if (node.hurtboxes != null && node.hurtboxes.Children.Count > 0)
            {
                bint* hurtAddress = (bint*)dataAddress;
                MoveDefNode._lookupOffsets.Add((int)hurtAddress - (int)baseAddress);
                dataAddress += sizeof(VoidPtr) * 2;
                header->Unknown12 = (int)hurtAddress - (int)baseAddress;
                hurtAddress[0] = Rebuild(RootNode, node.hurtboxes, ref dataAddress, baseAddress);
                hurtAddress[1] = node.hurtboxes.Children.Count;
            }
            if (node.shieldboxes != null && node.shieldboxes.Children.Count > 0)
            {
                bint* hurtAddress = (bint*)dataAddress;
                MoveDefNode._lookupOffsets.Add((int)hurtAddress - (int)baseAddress);
                dataAddress += sizeof(VoidPtr) * 2;
                header->Unknown13 = (int)hurtAddress - (int)baseAddress;
                hurtAddress[0] = Rebuild(RootNode, node.shieldboxes, ref dataAddress, baseAddress);
                hurtAddress[1] = node.shieldboxes.Children.Count;
            }

            header->Unknown5 = Rebuild(RootNode, node.physNode, ref dataAddress, baseAddress);

            bint* effAddress = (bint*)dataAddress;
            MoveDefNode._lookupOffsets.Add((int)effAddress - (int)baseAddress);
            dataAddress += sizeof(VoidPtr)*2;
            header->Unknown6 = (int)effAddress - (int)baseAddress;
            effAddress[0] = Rebuild(RootNode, node.effNode, ref dataAddress, baseAddress);
            effAddress[1] = node.effNode.Size / 4;

            header->Unknown14 = Rebuild(RootNode, node.collision, ref dataAddress, baseAddress); //ECB info
            header->Unknown15 = node.Unk15; //more seemingly unused flags
            header->Unknown4 = 0; //unused slot related to visibility

            #endregion

            #region Part 3

            if ((int)dataAddress - (int)baseAddress != node.part1Len + node.part2Len)
            {
                Console.WriteLine("p3");
            }


            dataAddress += RootNode._actions.Children.Count * 4;
            dataAddress += RootNode._hiddenActions.Children.Count * 4;

            #endregion

            #region Part 4

            if ((int)dataAddress - (int)baseAddress != node.part1Len + node.part2Len + node.part3Len)
            {
                Console.WriteLine("p4");
            }

            node.subActionTable.WriteTable(dataAddress);

            dataAddress += node.subActionTable.TotalSize;

            header->SubactionCount = node.subActions.Children.Count;
            header->SubactionFlagsStart = (int)dataAddress - (int)baseAddress;

            Rebuild_Sub_Names(ref dataAddress, baseAddress, RootNode._subActions, node.subActionTable);

            #endregion

            #region Part 5

            dataAddress += RootNode._subActions.Children.Count * 12;

            #endregion

            bint* offsets = (bint*)header;
            for (int i = 0; i < 16; i++) //Data Offset table goes from 00-to-15
            {
                if (i != 1 && i != 3 && i != 14) //exceptions for integers
                {
                    if (offsets[i] > 0)
                    {
                        MoveDefNode._lookupOffsets.Add((int)&offsets[i] - (int)baseAddress);
                    }
                }
            }

            Rebuild_PostProcess(RootNode);
        }
    }
}
