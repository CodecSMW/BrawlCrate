using BrawlLib.Internal;
using BrawlLib.SSBB.Types;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using static BrawlLib.SSBB.Types.hkClassMember;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class MovesetCmnConverter : MovesetConverter
    {
        //Wow this format is messy.

        //Huge thanks to: 

        //PhantomWings, for the PSA source code and its text files.
        //Dantarion, for the OpenSA3/Tabuu source code.
        //Kryal, for Brawlbox and starting to make structs for this format.
        //Bero, for starting to parse the format for Brawlbox.
        //Toomai, for the hitbox/hurtbox flags and rendering code.
        //The Project M team, for their moveset text files to use in the internal event dictionary.

        //Data order:

        //Header
        //Attributes
        //SSE Attributes
        //Common Action Flags
        //Unknown 7
        //Subaction Other Data - includes articles (at end)
        //Subaction GFX Data - includes articles (at end)
        //Misc Bone Section
        //Subaction SFX Data - includes articles (at end)
        //Sound Lists Final Children Entries
        //Subaction Main Data - includes articles (at end)
        //Misc Section 1
        //Hurtboxes
        //Misc Unk Section 9 Final Children Entries
        //Ledgegrabs
        //Crawl
        //Tether
        //Multijump
        //Glide
        //Actions Data Part1/Part2 alternating
        //Subroutines
        //Article Actions
        //Action Flags
        //Action Entry Offsets
        //Action Exit Offsets
        //Action Pre
        //Subaction Flags
        //Subaction Offsets Main/GFX/SFX/Other
        //Model Visibility
        //Misc Item Bones
        //Hand Bones
        //Unknown Section 9
        //Unk24
        //Unknown Section 12
        //Unk22
        //Extra Params
        //Static Articles
        //Entry Article
        //Attributed Articles
        //Misc Section 2
        //Action Interrupts
        //Bone Floats 1
        //Bone Floats 2
        //Bone Floats 3
        //Bone References Main
        //Bone References Misc
        //Misc Sound Lists
        //Misc Section Header
        //Sections Data (includes data header)
        //Lookup Offsets
        //Sections Offsets
        //References Offsets
        //Sections/References String Table

        public static int CalcDataSize2()
        {
            int size = 0;
            foreach (MoveDefEntryNode e in MoveDefNode.nodeDictionary.Values)
            {
                if (e.External && !(e._extNode is MoveDefReferenceEntryNode))
                {
                    continue;
                }

                size += e.CalculateSize(true);
            }

            return size;
        }

        public static void BuildData2(MoveDefDataNode node, MovesetHeader* header, VoidPtr address, int length,
                                      bool force)
        {
            VoidPtr addr = address;
            foreach (MoveDefEntryNode e in MoveDefNode.nodeDictionary.Values)
            {
                if (e.External && !(e._extNode is MoveDefReferenceEntryNode))
                {
                    continue;
                }

                e.Rebuild(addr, e._calcSize, true);
            }
        }
        /*
        public static int GetSize(MoveDefEntryNode node, ref int lookupCount)
        {
            int size = 0;
            return size;
        };*/
        public static int CalcDataSize(MoveDefDataCommonNode node)
        {
            int
                lookupCount = 7, //The seven for IC-Basics
                part1Len = 0,
                part2Len = 0,
                part3Len = 0,
                part4Len = 0,
                part5Len = 0,
                part6Len = 0,
                part7Len = 0;
            MoveDefNode RootNode = node.Parent.Parent as MoveDefNode;

            RootNode.sections._namedFeatures.Clear();

            FDefSubActionStringTable subActionTable = new FDefSubActionStringTable();

            #region Part 1: Basic Parameter Tables
            //23 is wrong? Returns 0x58?????
            //part1Len += node.ambigNode[23].CalculateSize(true);
            part1Len += GetSize(node.ambigNode[23], ref lookupCount); //size 0xA8 at the start "Unk23". Not actually referenced????
            //part1Len += 0xA0; 
                // There is a block of 0xA0 in Unk7 entries here, don't be mistaken by vanilla Fighter.pac's arrangement!
            part1Len += GetSize(node.ambigNode[8], ref lookupCount); //Size 0x1A4. Params8
            part1Len += GetSize(node.ambigNode[10], ref lookupCount); //Size 0x10. Params10
            part1Len += GetSize(node.ambigNode[16], ref lookupCount); //Size 0x48. Params16
            part1Len += GetSize(node.ambigNode[18], ref lookupCount); //Size 0x10. Params18
            part1Len += GetSize(node.ambigNode[0], ref lookupCount); //Size 0xBC. Global IC-Basics
            part1Len += GetSize(node.ambigNode[2], ref lookupCount); //Size 0x89C. IC-Basics
            part1Len += GetSize(node.ambigNode[12], ref lookupCount); //Size 0x80. Params12
            part1Len += GetSize(node.ambigNode[13], ref lookupCount); //Size 0x80. Params13
            part1Len += GetSize(node.ambigNode[14], ref lookupCount); //Size 0x40. Params14
            part1Len += GetSize(node.ambigNode[15], ref lookupCount); //Size 0x24. Params15
            part1Len += GetSize(node.ambigNode[1], ref lookupCount); //Size 0xBC. SSE Global IC-Basics
            part1Len += GetSize(node.ambigNode[3], ref lookupCount); //Size 0x8A0. SSE IC-Basics
            part1Len += GetSize(node.ambigNode[9], ref lookupCount); //Size 0x64. itemSwingData
            part1Len += GetSize(node.ambigNode[17], ref lookupCount); //Size 0x2A8. patternPowerMul

            //add objects that need to keep their names
            RootNode.sections._namedFeatures.Add(node.ambigNode[9]); //itemSwingData
            RootNode.sections._namedFeatures.Add(node.ambigNode[17]); //patternPowerMul
            #endregion

            #region Part 2: Rudimentary Offset Nodes

            part2Len += GetSize(node.node_7, ref lookupCount);
            part2Len += GetSize(node.node_11, ref lookupCount); //Size 0x64.
            part2Len += GetSize(node.node_21, ref lookupCount); //Size 0x70. Leg Bones.
            part2Len += GetSize(node.node_22, ref lookupCount); //Size 0x1A0.

            #endregion

            #region Part 3: Subroutines

            for (int i = 0; i < RootNode._subRoutineList.Count; i++)
            {
                if ((RootNode._subRoutineList[i] as MoveDefActionNode)._actionRefs.Count > 0) //Normally 0, debugging with -1
                {
                    part3Len += GetSize(RootNode._subRoutineList[i] as MoveDefActionNode, ref lookupCount);
                    AddNamedAction(RootNode.sections._namedFeatures, RootNode._subRoutineList[i] as MoveDefActionNode);
                }
            }

            #endregion

            #region Part 4: Simple command data

            //should probably add something to here.....
            
            #endregion
            #region Part 5: Action Entry and Exit info


            foreach (MoveDefActionGroupNode a in RootNode._actions.Children)
            {
                if (a.Children[0].Children.Count > 0 || (a.Children[0] as MoveDefActionNode)._actionRefs.Count > 0 ||
                    (a.Children[0] as MoveDefActionNode)._build) //Entry
                {
                    part5Len += GetSize(a.Children[0] as MoveDefActionNode, ref lookupCount);
                    lookupCount++;
                }

                if (a.Children[1].Children.Count > 0 || (a.Children[1] as MoveDefActionNode)._actionRefs.Count > 0 ||
                    (a.Children[1] as MoveDefActionNode)._build) //Exit
                {
                    part5Len += GetSize(a.Children[1] as MoveDefActionNode, ref lookupCount);
                    lookupCount++;
                }
                AddNamedAction(RootNode.sections._namedFeatures, a.Children[0] as MoveDefActionNode); //Entry
                AddNamedAction(RootNode.sections._namedFeatures, a.Children[1] as MoveDefActionNode); //Exit
            }
            #endregion

            #region Part 6: Unorthodox Action Lists

            foreach (MoveDefCommonActionNode a in RootNode.dataCommon._flashOverlay.Children) //19 "Hit Overlay Actions"
            {
                if (a.Children.Count > 0 || a._actionRefs.Count > 0 || a._build)
                {
                    part6Len += GetSize(a, ref lookupCount);
                    lookupCount+=2;
                }
            }
            foreach (MoveDefCommonActionNode a in RootNode.dataCommon._screenTint.Children) //20 "Screen Tint Actions"
            {
                if (a.Children.Count > 0 || a._actionRefs.Count > 0 || a._build)
                {
                    part6Len += GetSize(a, ref lookupCount);
                    lookupCount++;
                }
            }
            /*
            foreach (MoveDefCommonActionNode a in RootNode.dataCommon._hitOverlay.Children) //6 "Hit Overlay Actions"
            {
                if (a.Children.Count > 0 || a._actionRefs.Count > 0 || a._build)
                {
                    part6Len += GetSize(a, ref lookupCount);
                    lookupCount++;
                }
            }
            */
            #endregion

            #region Part 7: Pointers to Action Tables

            //Actions part 1 and 2 offsets
            lookupCount += 5; //offset to the lists
            part7Len += RootNode._actions.Children.Count * 8;

            part7Len += node._flashOverlay.Children.Count * 12; //8 per for section 19, 4 for 6. Shared
            part7Len += node._screenTint.Children.Count * 8;

            //Misc Section?

            #endregion


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
 
        public static int GetSize(MoveDefEntryNode node, ref int lookupCount)
        {
            if (node != null)
            {
                int size = 0;
                if (!(node.External && !(node._extNode is MoveDefReferenceEntryNode)))
                {
                    node._lookupOffsets = new List<int>();

                    if ((node.Parent is MoveDefDataNode || node.Parent is MoveDefMiscNode) && !node.isExtra)
                    {
                        lookupCount++;
                    }

                    size = node.CalculateSize(true);
                    lookupCount += node._lookupCount;
                }
                /*
                MoveDefEntryNode next = node;
                Top:
                //Check for random params around the file
                if (next.Parent is MoveDefDataNode) //doesn't seem to trigger????
                {
                    if (next.Parent.Children.Count > next.Index + 1)
                    {
                        if (
                            (next = next.Parent.Children[next.Index + 1] as MoveDefEntryNode) is
                            MoveDefCharSpecificNode || next is MoveDefRawDataNode && next.Children.Count > 0 &&
                            next.Children[0] is MoveDefSectionParamNode)
                        {
                            if (!(next is MoveDefRawDataNode))
                            {
                                size += next.CalculateSize(true);
                                lookupCount += next._lookupCount;
                            }
                            else
                            {
                                foreach (MoveDefSectionParamNode p in next.Children)
                                {
                                    size += p.CalculateSize(true);
                                }
                            }

                            goto Top;
                        }
                    }
                }
                */
                return size;
            }

            return 0;
        }

        internal static void BuildData(MoveDefDataCommonNode node, CommonMovesetHeader* header, VoidPtr address, int length,
                                       bool force)
        {
            MoveDefNode RootNode = node.Parent.Parent as MoveDefNode;

            VoidPtr dataAddress = address;
            VoidPtr baseAddress = node._rebuildBase;

            node._entryOffset = header;

            bint* action1Offsets = (bint*)(dataAddress + node.part1Len + node.part2Len + node.part3Len
                + node.part4Len + node.part5Len + node.part6Len);
            bint* action2Offsets = action1Offsets + RootNode._actions.Children.Count;
            bint* flashOverlayOffsets = action2Offsets + RootNode._actions.Children.Count;
            bint* screenOverlayOffsets = flashOverlayOffsets + RootNode.dataCommon._flashOverlay.Children.Count * 2;
            bint* hitOverlayOffsets = screenOverlayOffsets + RootNode.dataCommon._screenTint.Children.Count * 2;


            header->ActionsStart = (int)action1Offsets - (int)baseAddress;
            header->Actions2Start = (int)action2Offsets - (int)baseAddress;
            header->Unknown6  = (int)hitOverlayOffsets - (int)baseAddress; //node._hitOverlay; //TODO
            header->Unknown19 = (int)flashOverlayOffsets - (int)baseAddress;  //node._flashOverlay
            header->Unknown20 = (int)screenOverlayOffsets - (int)baseAddress;  //node._screenTint

            #region Part 1

            if ((int)dataAddress - (int)baseAddress != 0)
            {
                Console.WriteLine("p1");
            }
            //part1


            Rebuild(RootNode, node.ambigNode[23], ref dataAddress, baseAddress); // block of unreferenced data at start
            header->Unknown8 = Rebuild(RootNode, node.ambigNode[8], ref dataAddress, baseAddress);
            header->Unknown10 = Rebuild(RootNode, node.ambigNode[10], ref dataAddress, baseAddress);
            header->Unknown16 = Rebuild(RootNode, node.ambigNode[16], ref dataAddress, baseAddress);
            header->Unknown18 = Rebuild(RootNode, node.ambigNode[18], ref dataAddress, baseAddress);
            header->Unknown0 = Rebuild(RootNode, node.ambigNode[0], ref dataAddress, baseAddress); //Global IC-Basics
            header->Unknown2 = Rebuild(RootNode, node.ambigNode[2], ref dataAddress, baseAddress); //IC-Basics
            header->Unknown12 = Rebuild(RootNode, node.ambigNode[12], ref dataAddress, baseAddress);
            header->Unknown13 = Rebuild(RootNode, node.ambigNode[13], ref dataAddress, baseAddress);
            header->Unknown14 = Rebuild(RootNode, node.ambigNode[14], ref dataAddress, baseAddress);
            header->Unknown15 = Rebuild(RootNode, node.ambigNode[15], ref dataAddress, baseAddress);
            header->Unknown1 = Rebuild(RootNode, node.ambigNode[1], ref dataAddress, baseAddress); //SSE Global IC-Basics
            header->Unknown3 = Rebuild(RootNode, node.ambigNode[3], ref dataAddress, baseAddress); //SSE IC-Basics
            header->Unknown9 = Rebuild(RootNode, node.ambigNode[9], ref dataAddress, baseAddress); //itemSwingData
            header->Unknown17 = Rebuild(RootNode, node.ambigNode[17], ref dataAddress, baseAddress); //patternPowerMul

            //These relate to rotation configurations within ambigNode[23]
            MoveDefNode._lookupOffsets.Add(header->Unknown0 + 0x64); // Global IC-Basics
            MoveDefNode._lookupOffsets.Add(header->Unknown1 + 0x64);
            MoveDefNode._lookupOffsets.Add(header->Unknown2 + 0x72C);// IC-Basics
            MoveDefNode._lookupOffsets.Add(header->Unknown2 + 0x768);
            MoveDefNode._lookupOffsets.Add(header->Unknown2 + 0x7DC);
            MoveDefNode._lookupOffsets.Add(header->Unknown3 + 0x72C);
            MoveDefNode._lookupOffsets.Add(header->Unknown3 + 0x768);
            MoveDefNode._lookupOffsets.Add(header->Unknown3 + 0x7DC);
            #endregion

            #region Part 2

            if ((int)dataAddress - (int)baseAddress != node.part1Len)
            {
                Console.WriteLine("p2");
            }

            //part2
            header->Unknown7 = Rebuild(RootNode, node.node_7, ref dataAddress, baseAddress);
            header->Unknown11 = Rebuild(RootNode, node.node_11, ref dataAddress, baseAddress);
            header->Unknown21 = Rebuild(RootNode, node.node_21, ref dataAddress, baseAddress);
            header->Unknown22 = Rebuild(RootNode, node.node_22, ref dataAddress, baseAddress);

            #endregion
            #region Part 3

            if ((int)dataAddress - (int)baseAddress != node.part1Len + node.part2Len)
            {
                Console.WriteLine("p3");
            }
            //part3
            for (int i = 0; i < RootNode._subRoutineList.Count; i++)
            {
                if ((RootNode._subRoutineList[i] as MoveDefActionNode)._actionRefs.Count > 0) //if not referenced, discards subroutine!
                {
                    Rebuild(RootNode, RootNode._subRoutineList[i] as MoveDefActionNode, ref dataAddress, baseAddress);
                }
            }

            #endregion

            #region Part 4
            if ((int)dataAddress - (int)baseAddress != node.part1Len + node.part2Len + node.part3Len)
            {
                Console.WriteLine("p4");
            }
            //part4

            //empty for now

            #endregion

            #region Part 5

            if ((int)dataAddress - (int)baseAddress != node.part1Len + node.part2Len + node.part3Len + node.part4Len)
            {
                Console.WriteLine("p5");
            }
            //part4
            foreach (MoveDefActionGroupNode grp in RootNode._actions.Children)
            {
                if (grp.Children[0].Children.Count > 0 ||
                    (grp.Children[0] as MoveDefActionNode)._actionRefs.Count > 0 ||
                    (grp.Children[0] as MoveDefActionNode)._build) //Entry
                {
                    action1Offsets[grp.Index] = Rebuild(RootNode, grp.Children[0] as MoveDefActionNode, ref dataAddress,
                        baseAddress);
                    MoveDefNode._lookupOffsets.Add((int)&action1Offsets[grp.Index] - (int)baseAddress);
                }

                if (grp.Children[1].Children.Count > 0 ||
                    (grp.Children[1] as MoveDefActionNode)._actionRefs.Count > 0 ||
                    (grp.Children[1] as MoveDefActionNode)._build) //Exit
                {
                    action2Offsets[grp.Index] = Rebuild(RootNode, grp.Children[1] as MoveDefActionNode, ref dataAddress,
                        baseAddress);
                    MoveDefNode._lookupOffsets.Add((int)&action2Offsets[grp.Index] - (int)baseAddress);
                }
            }

            #endregion


            //part6 TODO: We need to split 4 and 5! Also need to do 9 itemSwingData and 17 patternPowerMul
            #region Part 6

            if ((int)dataAddress - (int)baseAddress !=
                node.part1Len + node.part2Len + node.part3Len + node.part4Len + node.part5Len)
            {
                Console.WriteLine("p6");
            }
            //header->Unknown6 = *flashOverlayOffsets;
            foreach (MoveDefCommonActionNode grp in RootNode.dataCommon._flashOverlay.Children)
            {
                if (grp.Children.Count > 0 ||
                    grp._actionRefs.Count > 0 ||
                    grp._build) //Entry
                {
                    bint value = (grp._unk1 << 24) +
                        (grp._unk2 << 16) +
                        (grp._unk3 << 8) +
                        (grp._unk4);

                    flashOverlayOffsets[grp.Index * 2] = Rebuild(RootNode, grp, ref dataAddress,
                        baseAddress);
                    flashOverlayOffsets[grp.Index * 2 + 1] = value;
                    MoveDefNode._lookupOffsets.Add((int)&flashOverlayOffsets[grp.Index * 2] - (int)baseAddress);
                    hitOverlayOffsets[grp.Index] = flashOverlayOffsets[grp.Index * 2];
                    MoveDefNode._lookupOffsets.Add((int)&hitOverlayOffsets[grp.Index] - (int)baseAddress);
                }
            }
            //header->Unknown19 = *screenOverlayOffsets;
            foreach (MoveDefCommonActionNode grp in RootNode.dataCommon._screenTint.Children)
            {
                if (grp.Children.Count > 0 ||
                    grp._actionRefs.Count > 0 ||
                    grp._build) //Entry
                {
                    bint value = (grp._unk1 << 24) +
                        (grp._unk2 << 16) +
                        (grp._unk3 << 8) +
                        (grp._unk4);

                    screenOverlayOffsets[grp.Index * 2] = Rebuild(RootNode, grp, ref dataAddress,
                        baseAddress);
                    screenOverlayOffsets[grp.Index * 2 + 1] = value;
                    MoveDefNode._lookupOffsets.Add((int)&screenOverlayOffsets[grp.Index * 2] - (int)baseAddress);
                }
            }
            /*
            foreach (MoveDefCommonActionNode grp in RootNode.dataCommon._hitOverlay.Children)
            {
                if (grp.Children.Count > 0 ||
                    grp._actionRefs.Count > 0 ||
                    grp._build) //Entry
                {
                    hitOverlayOffsets[grp.Index] = Rebuild(RootNode, grp, ref dataAddress,
                        baseAddress);
                    MoveDefNode._lookupOffsets.Add((int)&hitOverlayOffsets[grp.Index] - (int)baseAddress);
                }
            }
            */
            //MoveDefNode._lookupOffsets.Add
            #endregion

            #region Part 7

            if ((int)dataAddress - (int)baseAddress != node.part1Len + node.part2Len + node.part3Len + node.part4Len +
                node.part5Len + node.part6Len)
            {
                Console.WriteLine("p7");
            }
            //part7

            //Actions part 1 and 2 already written
            dataAddress += RootNode._actions.Children.Count * 8;
            dataAddress += RootNode.dataCommon._flashOverlay.Children.Count * 12;
            dataAddress += RootNode.dataCommon._screenTint.Children.Count * 8;
            //dataAddress += RootNode.dataCommon._hitOverlay.Children.Count * 4;

            #endregion

            bint* offsets = (bint*)header;
            for (int i = 0; i < 23; i++) //Data Offset table goes from 00-to-26
            {
                if (offsets[i] > 0)
                {
                    MoveDefNode._lookupOffsets.Add((int)&offsets[i] - (int)baseAddress);
                }
            }
            //dataAddress += 0x110; //suspect way to deal with corruption?
            foreach (MoveDefEntryNode entry in RootNode._postProcessNodes)
            {
                entry.PostProcess();
            }
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
                //Check for random params around the file
                /*
                if (next.Parent is MoveDefDataNode)
                {
                    if (next.Parent.Children.Count > next.Index + 1)
                    {
                        if (
                            (next = next.Parent.Children[next.Index + 1] as MoveDefEntryNode) is
                            MoveDefCharSpecificNode || next is MoveDefRawDataNode && next.Children.Count > 0 &&
                            next.Children[0] is MoveDefSectionParamNode)
                        {
                            if (!(next is MoveDefRawDataNode))
                            {
                                next.Rebuild(dataAddress, next._calcSize, true);
                                MoveDefNode._lookupOffsets.AddRange(next._lookupOffsets.ToArray());
                                dataAddress += next._calcSize;

                                if (next._lookupCount != next._lookupOffsets.Count)
                                {
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                next._entryOffset = dataAddress;
                                foreach (MoveDefSectionParamNode p in next.Children)
                                {
                                    p.Rebuild(dataAddress, p.AttributeBuffer.Length, true);
                                    dataAddress += p.AttributeBuffer.Length;
                                }
                            }

                            goto Top;
                        }
                    }
                }
                */

                return node._rebuildOffset;
            }
            else
            {
                return 0;
            }
        }

        public static int CalcSizeArticleActions(MoveDefDataNode node, ref int lookupCount, bool subactions, int index)
        {
            int size = 0;
            if (node.staticArticles != null && node.staticArticles.Children.Count > 0)
            {
                foreach (MoveDefArticleNode d in node.staticArticles.Children)
                {
                    if (!subactions)
                    {
                        if (d.actions != null)
                        {
                            foreach (MoveDefActionNode a in d.actions.Children)
                            {
                                if (a.Children.Count > 0)
                                {
                                    size += GetSize(a, ref lookupCount);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (d.subActions != null)
                        {
                            foreach (MoveDefSubActionGroupNode grp in d.subActions.Children)
                            {
                                if (grp.Children[index].Children.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._actionRefs.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._build)
                                {
                                    size += GetSize(grp.Children[index] as MoveDefActionNode, ref lookupCount);
                                }
                            }
                        }
                    }
                }
            }

            if (node.entryArticle != null)
            {
                if (!subactions)
                {
                    if (node.entryArticle.actions != null)
                    {
                        foreach (MoveDefActionNode a in node.entryArticle.actions.Children)
                        {
                            if (a.Children.Count > 0)
                            {
                                size += GetSize(a, ref lookupCount);
                            }
                        }
                    }
                }
                else
                {
                    if (node.entryArticle.subActions != null)
                    {
                        foreach (MoveDefSubActionGroupNode grp in node.entryArticle.subActions.Children)
                        {
                            if (grp.Children[index].Children.Count > 0 ||
                                (grp.Children[index] as MoveDefActionNode)._actionRefs.Count > 0 ||
                                (grp.Children[index] as MoveDefActionNode)._build)
                            {
                                size += GetSize(grp.Children[index] as MoveDefActionNode, ref lookupCount);
                            }
                        }
                    }
                }
            }

            foreach (MoveDefArticleNode d in node._articles.Values)
            {
                if (!subactions)
                {
                    if (d.actions != null)
                    {
                        if (d.pikmin)
                        {
                            foreach (MoveDefActionGroupNode grp in d.actions.Children)
                            {
                                foreach (MoveDefActionNode a in grp.Children)
                                {
                                    if (a.Children.Count > 0)
                                    {
                                        size += GetSize(a, ref lookupCount);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (MoveDefActionNode a in d.actions.Children)
                            {
                                if (a.Children.Count > 0)
                                {
                                    size += GetSize(a, ref lookupCount);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (d.subActions != null)
                    {
                        MoveDefEntryNode e = d.subActions;
                        int populateCount = 1;
                        bool children = false;
                        if (d.subActions.Children[0] is MoveDefActionListNode)
                        {
                            populateCount = d.subActions.Children.Count;
                            children = true;
                        }

                        for (int i = 0; i < populateCount; i++)
                        {
                            if (children)
                            {
                                e = d.subActions.Children[i] as MoveDefEntryNode;
                            }

                            foreach (MoveDefSubActionGroupNode grp in e.Children)
                            {
                                if (grp.Children[index].Children.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._actionRefs.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._build)
                                {
                                    size += GetSize(grp.Children[index] as MoveDefActionNode, ref lookupCount);
                                }
                            }
                        }
                    }
                }
            }

            return size;
        }

        public static void RebuildArticleActions(MoveDefNode RootNode, MoveDefDataNode node, ref VoidPtr dataAddress,
                                                 VoidPtr baseAddress, bool subactions, int index)
        {
            if (node.staticArticles != null && node.staticArticles.Children.Count > 0)
            {
                foreach (MoveDefArticleNode d in node.staticArticles.Children)
                {
                    if (!subactions)
                    {
                        if (d.actions != null)
                        {
                            foreach (MoveDefActionNode a in d.actions.Children)
                            {
                                if (a.Children.Count > 0)
                                {
                                    Rebuild(RootNode, a, ref dataAddress, baseAddress);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (d.subActions != null)
                        {
                            foreach (MoveDefSubActionGroupNode grp in d.subActions.Children)
                            {
                                if (grp.Children[index].Children.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._actionRefs.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._build)
                                {
                                    Rebuild(RootNode, grp.Children[index] as MoveDefActionNode, ref dataAddress,
                                        baseAddress);
                                }
                            }
                        }
                    }
                }
            }

            if (node.entryArticle != null)
            {
                if (!subactions)
                {
                    if (node.entryArticle.actions != null)
                    {
                        foreach (MoveDefActionNode a in node.entryArticle.actions.Children)
                        {
                            if (a.Children.Count > 0)
                            {
                                Rebuild(RootNode, a, ref dataAddress, baseAddress);
                            }
                        }
                    }
                }
                else
                {
                    if (node.entryArticle.subActions != null)
                    {
                        foreach (MoveDefSubActionGroupNode grp in node.entryArticle.subActions.Children)
                        {
                            if (grp.Children[index].Children.Count > 0 ||
                                (grp.Children[index] as MoveDefActionNode)._actionRefs.Count > 0 ||
                                (grp.Children[index] as MoveDefActionNode)._build)
                            {
                                Rebuild(RootNode, grp.Children[index] as MoveDefActionNode, ref dataAddress,
                                    baseAddress);
                            }
                        }
                    }
                }
            }

            foreach (MoveDefArticleNode d in node._articles.Values)
            {
                if (!subactions)
                {
                    if (d.actions != null)
                    {
                        if (d.pikmin)
                        {
                            foreach (MoveDefActionGroupNode grp in d.actions.Children)
                            {
                                foreach (MoveDefActionNode a in grp.Children)
                                {
                                    if (a.Children.Count > 0)
                                    {
                                        Rebuild(RootNode, a, ref dataAddress, baseAddress);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (MoveDefActionNode a in d.actions.Children)
                            {
                                if (a.Children.Count > 0)
                                {
                                    Rebuild(RootNode, a, ref dataAddress, baseAddress);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (d.subActions != null)
                    {
                        MoveDefEntryNode e = d.subActions;
                        int populateCount = 1;
                        bool children = false;
                        if (d.subActions.Children[0] is MoveDefActionListNode)
                        {
                            populateCount = d.subActions.Children.Count;
                            children = true;
                        }

                        for (int i = 0; i < populateCount; i++)
                        {
                            if (children)
                            {
                                e = d.subActions.Children[i] as MoveDefEntryNode;
                            }

                            foreach (MoveDefSubActionGroupNode grp in e.Children)
                            {
                                if (grp.Children[index].Children.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._actionRefs.Count > 0 ||
                                    (grp.Children[index] as MoveDefActionNode)._build)
                                {
                                    Rebuild(RootNode, grp.Children[index] as MoveDefActionNode, ref dataAddress,
                                        baseAddress);
                                }
                            }
                        }
                    }
                }
            }
        }

        //dataCommon:

        //Unknown7 entries
        //Params8
        //Params10
        //Params16
        //Params18
        //Global IC-Basics
        //Unknown23
        //IC-Basics
        //Params24
        //Params12
        //Params13
        //Params14
        //Params15        
        //SSE Global IC-Basics
        //SSE IC-Basics
        //Flash Overlay Actions
        //patternPowerMul parameters
        //Flash Overlay Action Offsets
        //Screen Tint Actions
        //Screen Tint Action Offsets
        //Unknown22 entries
        //Entry/Exit actions alternating
        //Subroutines
        //Unknown7 Data entries
        //Unknown11
        //Leg bones
        //Unknown22 header
        //patternPowerMul header
        //patternPowerMul events
        //Sections data
        //dataCommon header

        public static int CalcDataCommonSize(MoveDefDataCommonNode node)
        {
            return 0;
        }

        internal static void BuildDataCommon(MoveDefDataNode node, CommonMovesetHeader* header, VoidPtr address, int length,
                                       bool force)
        {
            MoveDefNode RootNode = node.Parent.Parent as MoveDefNode;

            VoidPtr dataAddress = address;
            VoidPtr baseAddress = node._rebuildBase;

            node._entryOffset = header;


        }
    }
}