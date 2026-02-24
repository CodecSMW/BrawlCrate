using BrawlLib.Internal;
using BrawlLib.SSBB.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Channels;

namespace BrawlLib.SSBB.ResourceNodes
{

public class MoveDefSubActionGroupNode : MoveDefEntryNode
{
 public override ResourceType ResourceFileType => ResourceType.MDefSubActionGroup;
 public override bool AllowDuplicateNames => true;

 public override string ToString()
 {
     return Name;
 }

 public AnimationFlags _flags;
 public byte _inTransTime;

 [Category("Sub Action")]
 public AnimationFlags Flags
 {
     get => _flags;
     set
     {
         _flags = value;
         SignalPropertyChange();
     }
 }

 [Category("Sub Action")]
 public byte InTranslationTime
 {
     get => _inTransTime;
     set
     {
         _inTransTime = value;
         SignalPropertyChange();
     }
 }

 [Category("Sub Action")] public int ID => Index;
}

public class MoveDefActionGroupNode : MoveDefEntryNode
{
 public override ResourceType ResourceFileType => ResourceType.MDefActionGroup;

 public override string ToString()
 {
     return Name;
 }

 [Category("Action")] public int ID => Index;
}

public unsafe class MoveDefActionNode : MoveDefExternalNode
{
 public override ResourceType ResourceFileType => ResourceType.MDefAction;

 internal FDefEvent* Header => (FDefEvent*) WorkingUncompressed.Address;

     public override string ToString()
     {
         if (Name.StartsWith("SubRoutine"))
         {
             return Name;
         }

         return base.ToString();
     }
        public override void Remove()
        {
            MoveDefNode RootCopy = Root; //Needed because this object is about to not exist!
            foreach (MoveDefEventNode e in ActionRefs)
            {
                e.EventID = 0x00020000; //Force to be No Operation!
                e.NewChildren();
            }
            base.Remove();
            RootCopy.flushEventPointers(); //Lists need to know that their contents have been re-ordered!
        }
        public override void OnMoved()
        {
            base.OnMoved();
            Root.flushEventPointers(); //We want to update all of the lists!
        }

    public override string Name
    {
        get => base.Name;
        set
            { //TODO: DataTable needs to be a more normal string table!

                Root._externalSections.Remove(this);
                Root.sections.Children.Remove(this);
                if (nameKey != null)
                    Root.sections.DataTable.Remove(nameKey);
                base.Name = value;
                if (Children.Count == 0) //Don't try to add an override if it is empty. This will do bad things!
                    return;
                if (nameKey == null)
                {
                    nameKey = new NameSizeGroup(_name, Size);
                    stringInfo = new FDefStringEntry() { _dataOffset = _offset, _stringOffset = -1 };

                }
            if (_changed &&
                    ((Parent is MoveDefActionGroupNode) && !(_name == "Entry" || _name == "Exit")) || //Actions
                    Parent is MoveDefSectionNode || //Externals 
                    Parent is MoveDefActionOverrideEntryNode || // Action Overrides
                    (Parent is MoveDefSubRoutineListNode && !_name.StartsWith("SubRoutine"))) //Subroutines
                {

                    Root._externalSections.Add(this);
                    _extOverride = true;
                    _extNode = Root.IsExternal(_offset);
                    Root.sections.Children.Add(this);
                    Root.sections._sectionList.Add(this);
                    nameKey.Name = _name;
                    nameKey.Size = Size;
                    stringInfo._dataOffset = _offset;
                    stringInfo._stringOffset = -1;
                    Root.sections.DataTable.Add(nameKey,stringInfo);
                    // }

                }
            else
            {
                    Root._externalSections.Remove(this);
                    Root.sections.Children.Remove(this);
                    Root.sections.DataTable.Remove(nameKey);
                    _extOverride = false;
            }
            
        }

    }

public bool _isBlank;
public bool _build;
NameSizeGroup nameKey;
FDefStringEntry stringInfo;

 [Category("Script")]
 public bool ForceWrite
 {
     get => _build;
     set => _build = value;
 }

 public List<ResourceNode> _actionRefs = new List<ResourceNode>();
 public ResourceNode[] ActionRefs => _actionRefs.ToArray();


 public MoveDefActionNode(string name, bool blank, ResourceNode parent)
 {
     _name = name;
     _isBlank = blank;
     _build = false;
     if (_isBlank) //Initialize will not be called, because there is no data to read
     {
         _parent = parent;

         if (_name == null)
         {
             if (Parent.Parent.Name == "Action Scripts")
             {
                 if (Index == 0)
                 {
                     _name = "Entry";
                 }
                 else if (Index == 1)
                 {
                     _name = "Exit";
                 }
             }
             else if (Parent.Parent.Name == "SubAction Scripts")
             {
                 if (Index == 0)
                 {
                     _name = "Main";
                 }
                 else if (Index == 1)
                 {
                     _name = "GFX";
                 }
                 else if (Index == 2)
                 {
                     _name = "SFX";
                 }
                 else if (Index == 3)
                 {
                     _name = "Other";
                 }
             }

             if (_name == null)
             {
                 _name = "Action " + Convert.ToString(Index,16);
             }
         }
     }
 }

 public override bool OnInitialize()
 {
     _offsets.Add(_offset);
     _build = true;
     if (_offset > Root.dataSize)
     {
         return false;
     }

     if (_name == null)
     {
         if (Parent.Parent.Name == "Action Scripts")
         {
             if (Index == 0)
             {
                 _name = "Entry";
             }
             else if (Index == 1)
             {
                 _name = "Exit";
             }
         }
         else if (Parent.Parent.Name == "SubAction Scripts")
         {
             if (Index == 0)
             {
                 _name = "Main";
             }
             else if (Index == 1)
             {
                 _name = "GFX";
             }
             else if (Index == 2)
             {
                 _name = "SFX";
             }
             else if (Index == 3)
             {
                 _name = "Other";
             }
         }

         if (_name == null)
         {
             _name = "Action " + Convert.ToString(Index, 16);
         }

     }

    Dictionary<NameSizeGroup, FDefStringEntry> dTbl = Root.sections.DataTable;
    foreach (KeyValuePair<NameSizeGroup, FDefStringEntry> data in dTbl)
    {
        if (data.Value._dataOffset == _offset)
        {
            _name = data.Key.Name; //Works for statusAnimCmd. 
        }
                /*
                //TODO: effectAnimCmd_
                //          BatSwing4Common, HarisenSwing4HoldCommon, LipStickSwing4HoldCommon
                //          ScopeAirFireCommon, ScopeAirRapidCommon, ScopeAirStartCommon, ScopeFireUpperCommon
                //              ScoperRapidUpperCommon, ScopeStartUpperCommon
                //          SmashThrowBCommon, SmashThrowFCommon, SmashThrowHiCommon, SmashThrowLwCommon
                //          StarRodSwing4HoldCommon, Swing4Impact, SwordSwing4HoldCommon

                //      gameAnimCmd_
                //          BatSwing1Common, BatSwing3Common, BatSwing4Common
                //              BatSwingDashCommon, BatSwingDashCommon2, BatSwingDashCommon3
                //          CaptureCutCommon
                //          DamageFlyIndirectCommon, DamageFlyIndirectNoReaction # These two are fine and register as subroutines
                //          HarisenSwing1Common, HarisenSwing3Common, HarisenSwing4Common
                //              HarisenSwingDashCommon, HarisenSwingDashCommon2, HarisenSwingDashCommon3
                //          LipStickSwing1Common, LipStickSwing3Common, LipStickSwing4Common
                //              LipStickSwingDashCommon, LipStickSwingDashCommon2, LipStickSwingDashCommon3
                //          ScrewCommon
                //          StarRodSwing1Common, StarRodSwing3Common, StarRodSwing4Common
                //              StarRodSwingDashCommon, StarRodSwingDashCommon2, StarRodSwingDashCommon3
                //              StarShotCommon
                //          SwordSwing1Common, SwordSwing3Common, SwordSwing4Common
                //              SwordSwingDashCommon, SwordSwingDashCommon2, SwordSwingDashCommon3

                //          Falcon lacks Swing4Common and SwingDashCommon3 variants off gameAnimCmd???

                //      itemSwingData, patternPowerMul # Already addressed

                // statsAnimCmdExit_ (starting at [54])
                //      AirLasso [7F], AirLassoFailure [83], AirLassoHang [81], AirLassoReach [80], AirLassoRewind [82]
                //      Appeal [10C & 10D], AttackAir [33]
                //      CatchPull [35 & 37], CatchTurn (?), CatchWait [39]
                //      Guard [1B], GuardDamage [1D], GuardOn [1A]
                //      SmashAppeal (?)

                // statusAnimCmdGroup_ ([68])
                //      CHK_AIR_ATTACK, CHK_AIR_CLIFF, CHK_AIR_ESCAPE, CHK_AIR_ITEM_THROW
                //      CHK_AIR_JUMP_AERIAL, CHK_AIR_LANDING, CHK_AIR_LASSO, CHK_AIR_SPECIAL
                //      CHK_AIR_TREAD_JUMP, CHK_AIR_WALL_JUMP
                //      CHK_GROUND, CHK_GROUND_ATTACK, CHK_GROUND_CATCH, CHK_GROUND_ESCAPE
                //      CHK_GROUND_GUARD, CHK_GROUND_ITEM, CHK_GROUND_JUMP, CHK_GROUND_SPECIAL
                //      MAIN, TEMPORARY

                //statusAnimCmdPre_ ([88])
                //      Bind, Bury, DamageSongStart, EscapeFb, Fall, Fly, ItemShoot
                //      ItemSwingS4Start, Jump, JumpAerial, Landing, Run, SpecialN*, SpecialNCommon
                //      TreadJump, Wait, Walk

                //statusAnimCmd_ ([105-194]) *= subroutine already, number = action ID, ! = not accounted for
                //      AIRChkAttack, AIRChkCliff, AIRChkCliffForce, AIRChkEscape, AIRChkFallCommon*
                //      AIRChkItemThrow, AIRChkJumpAerial, AIRChkLanding, AIRChkStopCeil*, AIRChkTreadJump
                //      AIRChkWallJump
                //      AirLasso [7F], AirLassoFailure [83], AirLassoHang [81], AirLassoReach [80], AirLassoRewind [82]
                //      Appeal [10C], AttackAir [33], AttackAirCommon*, AttackAirUniq*
                //      AttackHi4 [32], AttackHi4Hold [31], AttackHi4Start [30]
                //      AttackLw3 [29], AttackLw4 [2F], AttackLw4Hold [2E], AttackLw4Start [2D]
                //      AttackS4 [2C], AttackS4Hold [2B], AttackS4Start [2A]
                //      Catch [34], CatchDash [36], CatchPull [35], CatchPullCommon*, CatchTurn [38], CatchWait [39]
                //      CliffCatch [74], CliffCatchMove [73]
                //      DamageFall [49], Dead [BD], Entry [10E], EscapeAirCommon*
                //      FALLCommonUniq*, Fall [E], FallAerial [F], FallSpecial [10]
                //      GROUNDChkEscape, GimmickCatapult [C1], Glide [85], GlideAttack [87], GlideEnd [88]
                //      Guard [1B], GuardCont*, GuardDamageUniq*, GuardOnUniq*, GuardUniq*, 
                //      ItemShootAir_Common*, ItemShootDashB_Common*, ItemShootDashF_Common*
                //      ItemShootJumpSquat_Common*, ItemShootLanding_Common*, ItemShootWait_Common*
                //      ItemShootWalkBBrake_Common*, ItemShootWalkB_Common*
                //      ItemShootWalkFBrake_Common*, ItemShootWalkF_Common*
                //      ItemSwingS4 [A1], ItemSwingS4Hold [A2], ItemSwingS4Start [A0], ItemSwingS4Uniq*
                //      ItemThrow [9B], ItemThrowHeavyCommon*
                //      Jump [B], JumpAerial [C], JumpSquat [A]
                //      Landing [16], LandingAttackAir [18], LandingFallSpecial [19], LandingLight [17]
                //      Lose [110], MissFoot [7E], RebirthCommon*
                //      SpecialJumpAttack, SpecialJumpAttackCommon
                //      Throw [3C], ThrowLogCommon*
                //      TurnRunUniq*, Wait [0], WaitCommon*, Win [10F]

                //      Notes for prep of customizable Fighter.pac:
                //          Manually modify Fighter.pac Action 37 Exit to point to Action 35 Exit
                //              Same with 3A Exit to 39 Exit, 10D Exit to 10C Exit

                //      statusAnimCmdExit_: all accouunted for besides SmashAppeal and CatchTurn?
                //      All need generating:
                //          statusAnimCmdGroup_, effectAnimCmd_
                //      statusAnimCmdPre_ needs all except for SpecialN (not SpecialNCommon?!)
                //      gameAnimCmd_ needs all except DamageFlyIndirectNoReactionCommon and DamageFlyIndirectCommon
                //     
                //      unaccounted for from statusAnimCmd_:
                //                   
                //      AIRChkCliffForce
                //      SpecialJumpAttack, SpecialJumpAttackCommon
                */
            }

            base.OnInitialize();

     //Root._paths[_offset] = TreePath;

     return Header->_nameSpace != 0 || Header->_id != 0;
 }

 public override void OnPopulate()
 {
     FDefEvent* ev = Header;
     if (!_isBlank)
     {
         while (ev->_nameSpace != 0 || ev->_id != 0)
         {
             new MoveDefEventNode().Initialize(this, new DataSource((VoidPtr) ev++, 8));
         }
     }

     SetSizeInternal(Children.Count * 8 + 8);
 }

 public override int OnCalculateSize(bool force)
 {
     _lookupCount = 0;
     int size = 8; //Terminator event size
     foreach (MoveDefEventNode e in Children)
     {
         if (e.EventID == 0xFADEF00D || e.EventID == 0xFADE0D8A)
         {
             continue;
         }

         size += e.CalculateSize(true);
         _lookupCount += e._lookupCount;
     }

     return size;
 }

 public override void OnRebuild(VoidPtr address, int length, bool force)
 {
     int off = 0;
     foreach (MoveDefEventNode e in Children)
     {
         off += e.Children.Count * 8;
     }

     FDefEventArgument* paramAddr = (FDefEventArgument*) address;
     FDefEvent* eventAddr = (FDefEvent*) (address + off);

     _entryOffset = eventAddr;

     foreach (MoveDefEventNode e in Children)
     {
         if (e._name == "FADEF00D" || e._name == "FADE0D8A")
         {
             continue;
         }

         e._entryOffset = eventAddr;
         *eventAddr = new FDefEvent
             {_id = e.id, _nameSpace = e.nameSpace, _numArguments = (byte) e.Children.Count, _unk1 = e.unk1};
         if (e.Children.Count > 0)
         {
             eventAddr->_argumentOffset = (uint) paramAddr - (uint) _rebuildBase;
             _lookupOffsets.Add((int) eventAddr->_argumentOffset.Address - (int) _rebuildBase);
         }
         else
         {
             eventAddr->_argumentOffset = 0;
         }

         eventAddr++;
         foreach (MoveDefEventParameterNode p in e.Children)
         {
             p._entryOffset = paramAddr;
             if (p._type != ArgVarType.Offset)
             {
                 *paramAddr = new FDefEventArgument {_type = (int) p._type, _data = p._value};
             }
             else
             {
                 Root._postProcessNodes.Add(p);
                 //if ((p as MoveDefEventOffsetNode).action != null)
                 //    _lookupOffsets.Add(0);
             }

             paramAddr++;
         }
     }

     eventAddr++; //Terminate
 }

 /*
 #region Scripting

 public List<HitBox> catchCollisions = new List<HitBox>();
 public List<HitBox> offensiveCollisions = new List<HitBox>();
 public List<HitBox> specialOffensiveCollisions = new List<HitBox>();
 public int _eventIndex = 0, _loopCount = -1, _loopStartIndex = -1, _loopEndIndex = -1, _loopTime = 0;

 public bool _looping = false,
     _runEvents = true,
     _return = false,
     _idling = false,
     _subRoutine = false,
     _delete = false;

 public int _waitFrames = 0, _totalFrames = 0; //In frames

 public void FrameAdvance(ModelEditControl _mainWindow)
 {
     if (_eventIndex >= Children.Count)
     {
         _idling = true;
         if (_delete)
         {
             _mainWindow.pnlMoveset.selectedActionNodes.Remove(this);
             Reset(_mainWindow);
         }

         return;
     }

     string name = Parent.Name;
     string fillername = subRoutine == null ? "" : subRoutine.Parent.Name;

     if (subRoutine != null && _totalFrames >= subRoutineSetAt)
     {
         if (subRoutine == this || subRoutine == actionReferencedBy || name == fillername ||
             _subRoutine && _idling)
         {
             _return = true;
             _eventIndex = Children.Count;
         }
         else
         {
             subRoutine.FrameAdvance(_mainWindow);
             _totalFrames++;
             if (subRoutine._return || subRoutine._eventIndex == subRoutine.Children.Count)
             {
                 subRoutine.Reset(_mainWindow);
                 subRoutine = null;
                 subRoutineSetAt = 0;
             }

             return;
         }
     }

     if (_waitFrames > 0)
     {
         _waitFrames--;
         if (_waitFrames > 0)
         {
             _totalFrames++;
             if (_looping)
             {
                 _loopTime++;
             }

             return;
         }
     }

     //Progress until the next wait event
     while (_waitFrames == 0 && _eventIndex < Children.Count && subRoutine == null)
     {
         if (_looping && _loopEndIndex == _eventIndex)
         {
             _loopCount--;
             _eventIndex = _loopStartIndex;
         }

         if (_looping && _loopCount == 0)
         {
             _looping = false;
             _eventIndex = _loopEndIndex + 1;
             _loopTime = 0;
         }

         Advance(_eventIndex++, _mainWindow);
     }

     if (_looping)
     {
         _loopTime++;
     }

     _totalFrames++;
     _mainWindow.modelPanel.Invalidate();
 }

 public void SetFrame(int index, ModelEditControl _mainWindow)
 {
     Reset(_mainWindow);
     if (actionReferencedBy != null && _delete)
     {
         index -= actionReferencedBy.subRoutineSetAt;
     }

     while (_totalFrames <= index)
     {
         //Progress frame by frame

         if (_eventIndex >= Children.Count)
         {
             _idling = true;
             if (_delete)
             {
                 _mainWindow.pnlMoveset.selectedActionNodes.Remove(this);
                 Reset(_mainWindow);
             }

             return;
         }

         string name = Parent.Name;
         string fillername = subRoutine == null ? "" : subRoutine.Parent.Name;

         if (subRoutine != null && _totalFrames >= subRoutineSetAt)
         {
             if (subRoutine == this || subRoutine == actionReferencedBy || name == fillername)
             {
                 _return = true;
                 _eventIndex = Children.Count;
             }
             else
             {
                 subRoutine.SetFrame(_totalFrames - subRoutineSetAt, _mainWindow);
                 _totalFrames++;
                 if (subRoutine._return || subRoutine._eventIndex == subRoutine.Children.Count)
                 {
                     subRoutine.Reset(_mainWindow);
                     subRoutine = null;
                     subRoutineSetAt = 0;
                 }

                 continue;
             }
         }

         if (_waitFrames > 0)
         {
             _waitFrames--;
             if (_waitFrames > 0)
             {
                 _totalFrames++;
                 if (_looping)
                 {
                     _loopTime++;
                 }

                 continue;
             }
         }

         //Progress until the next wait event
         while (_waitFrames == 0 && _eventIndex < Children.Count)
         {
             if (_looping && _loopEndIndex == _eventIndex)
             {
                 _loopCount--;
                 _eventIndex = _loopStartIndex;
             }

             if (_looping && _loopCount == 0)
             {
                 _looping = false;
                 _eventIndex = _loopEndIndex + 1;
                 _loopTime = 0;
             }

             Advance(_eventIndex++, _mainWindow);
         }

         //scriptEditor1.EventList.SelectedIndices.Clear();
         //if (_eventIndex - 1 < scriptEditor1.EventList.Items.Count)
         //    scriptEditor1.EventList.SelectedIndex = _eventIndex - 1;

         if (_looping)
         {
             _loopTime++;
         }

         _totalFrames++;
         _mainWindow.modelPanel.Invalidate();
     }
 }

 public void Reset(ModelEditControl _mainWindow)
 {
     _eventIndex = 0;
     _looping = false;
     _totalFrames = 0;
     _loopCount = -1;
     _loopStartIndex = -1;
     _loopEndIndex = -1;
     _loopTime = 0;
     _waitFrames = 0;
     catchCollisions = new List<HitBox>();
     offensiveCollisions = new List<HitBox>();
     specialOffensiveCollisions = new List<HitBox>();
     _idling = false;
     _delete = false;
     _subRoutine = false;
     subRoutineSetAt = 0;
     subRoutine = null;
     actionReferencedBy = null;
     _caseIndices = null;
     _defaultCaseIndex = -1;
     _cases = null;

     //if (scriptEditor1.EventList.Items.Count > 0)
     //    scriptEditor1.EventList.SelectedIndex = 0;
 }

 public MoveDefActionNode subRoutine = null;
 public MoveDefActionNode actionReferencedBy = null;
 public int subRoutineSetAt = 0;

 public List<MoveDefEventParameterNode> _cases = null;
 public List<int> _caseIndices;
 public int _defaultCaseIndex = -1;

 public void Advance(int eventid, ModelEditControl _mainWindow)
 {
     if (eventid >= Children.Count)
     {
         return;
     }

     int list, index, type;
     MoveDefEventNode e = Children[eventid] as MoveDefEventNode;

     if (!_runEvents &&
         e._event != 0x00050000 &&
         e._event != 0x00110100 &&
         e._event != 0x00120000 &&
         e._event != 0x00130000)
     {
         return;
     }

     //Code what to do for each event here.
     switch (e._event)
     {
         case 0x00010100: //Synchronous Timer
             _waitFrames = (int) ((float) e.EventData.parameters[0]._data / 60000f);
             break;
         case 0x00020000: //No Operation
             break;
         case 0x00020100: //Asynchronous Timer
             _waitFrames = Math.Max((int) ((float) e.EventData.parameters[0]._data / 60000f) - _totalFrames, 0);
             break;
         case 0x00040100: //Set loop data
             _loopCount = (int) e.EventData.parameters[0]._data;
             _loopStartIndex = e.Index + 1;
             _runEvents = false;
             break;
         case 0x00050000: //Start looping
             _looping = true;
             _loopEndIndex = e.Index;
             _eventIndex = _loopStartIndex;
             _runEvents = true;
             break;
         case 0x01010000: //Loop Rest
             _waitFrames = 1;
             break;
         case 0x06000D00: //Offensive Collison
         case 0x062B0D00: //Thrown Collision
             HitBox bubble1 = new HitBox(e);
             bubble1.HitboxID = (int) (e.EventData.parameters[0]._data & 0xFFFF);
             bubble1.HitboxSize = (int) e.EventData.parameters[5]._data;
             offensiveCollisions.Add(bubble1);
             break;
         case 0x06050100: //Body Collision
             _mainWindow._hurtBoxType = (int) e.EventData.parameters[0]._data;
             break;
         case 0x06080200: //Bone Collision
             int id = (int) e.EventData.parameters[0]._data;
             if (Root.Model != null && Root.Model._linker.BoneCache.Length > id && id >= 0)
             {
                 MDL0BoneNode bone = Root.Model._linker.BoneCache[id] as MDL0BoneNode;
                 switch ((int) e.EventData.parameters[1]._data)
                 {
                     case 0:
                         bone._nodeColor = Color.Transparent;
                         bone._boneColor = Color.Transparent;
                         break;
                     case 1:
                         bone._nodeColor = bone._boneColor = Color.FromArgb(255, 255, 0);
                         break;
                     default:
                         bone._nodeColor = bone._boneColor = Color.FromArgb(0, 0, 255);
                         break;
                 }

                 _mainWindow.boneCollisions.Add(bone);
             }

             break;
         case 0x06060100: //Undo Bone Collision
             foreach (MDL0BoneNode bone in _mainWindow.boneCollisions)
             {
                 bone._nodeColor = bone._boneColor = Color.Transparent;
             }

             _mainWindow.boneCollisions = new List<MDL0BoneNode>();
             break;
         case 0x060A0800: //Catch Collision 1
         case 0x060A0900: //Catch Collision 2
         case 0x060A0A00: //Catch Collision 3
             HitBox bubble2 = new HitBox(e);
             bubble2.HitboxID = (int) e.EventData.parameters[0]._data;
             bubble2.HitboxSize = (int) e.EventData.parameters[2]._data;
             catchCollisions.Add(bubble2);
             break;
         case 0x060D0000: //Terminate Catch Collisions
             catchCollisions = new List<HitBox>();
             break;
         case 0x00060000: //Loop break?
             _looping = false;
             _eventIndex = _loopEndIndex + 1;
             _loopTime = 0;
             break;
         case 0x06150F00: //Special Offensive Collison
             HitBox bubble3 = new HitBox(e);
             bubble3.HitboxID = (int) (e.EventData.parameters[0]._data & 0xFFFF);
             bubble3.HitboxSize = (int) e.EventData.parameters[5]._data;
             specialOffensiveCollisions.Add(bubble3);
             break;
         case 0x06040000: //Terminate Collisions
             offensiveCollisions = new List<HitBox>();
             specialOffensiveCollisions = new List<HitBox>();
             break;
         case 0x06030100: //Delete hitbox
             foreach (HitBox ev in offensiveCollisions)
             {
                 if (ev.HitboxID == (int) e.EventData.parameters[0]._data)
                 {
                     offensiveCollisions.Remove(ev);
                     break;
                 }
             }

             foreach (HitBox ev in specialOffensiveCollisions)
             {
                 if (ev.HitboxID == (int) e.EventData.parameters[0]._data)
                 {
                     specialOffensiveCollisions.Remove(ev);
                     break;
                 }
             }

             break;
         case 0x061B0500: //Move hitbox
             foreach (HitBox ev in offensiveCollisions)
             {
                 if (ev.HitboxID == (int) e.EventData.parameters[0]._data)
                 {
                     ev.EventData.parameters[1]._data = e.EventData.parameters[1]._data;
                     ev.EventData.parameters[6]._data = e.EventData.parameters[2]._data;
                     ev.EventData.parameters[7]._data = e.EventData.parameters[3]._data;
                     ev.EventData.parameters[8]._data = e.EventData.parameters[4]._data;
                     break;
                 }
             }

             foreach (HitBox ev in specialOffensiveCollisions)
             {
                 if (ev.HitboxID == (int) e.EventData.parameters[0]._data)
                 {
                     ev.EventData.parameters[1]._data = e.EventData.parameters[1]._data;
                     ev.EventData.parameters[6]._data = e.EventData.parameters[2]._data;
                     ev.EventData.parameters[7]._data = e.EventData.parameters[3]._data;
                     ev.EventData.parameters[8]._data = e.EventData.parameters[4]._data;
                     break;
                 }
             }

             break;
         case 0x04060100: //Set anim frame - subaction timer unaffected
             _mainWindow.SetFrame((int) ((float) e.EventData.parameters[0]._data / 60000f));
             break;
         case 0x00070100: //Go to subroutine and return
             Root.GetLocation((int) e.EventData.parameters[0]._data, out list, out type, out index);
             subRoutine = Root.GetAction(list, type, index);
             if (subRoutine != null)
             {
                 subRoutineSetAt = _totalFrames;
                 subRoutine.actionReferencedBy = this;
                 subRoutine._subRoutine = true;
             }

             break;
         case 0x00080000: //Return
             _return = true;
             _eventIndex = Children.Count;
             _idling = true;
             break;
         case 0x00090100: //Go to and do not return unless called
             Root.GetLocation((int) e.EventData.parameters[0]._data, out list, out type, out index);
             MoveDefActionNode a = Root.GetAction(list, type, index);
             if (a != null)
             {
                 subRoutineSetAt = _totalFrames;
                 a.actionReferencedBy = this;
                 a._delete = true;
                 _mainWindow.pnlMoveset.selectedActionNodes.Add(a);
                 //if (_eventIndex == Children.Count)
                 _mainWindow.pnlMoveset.selectedActionNodes.Remove(this);
             }

             break;
         case 0x0B000200: //Model Changer 1
         case 0x0B010200: //Model Changer 2
             if (Root.Model._polyList == null)
             {
                 break;
             }

             if (Root.data.mdlVisibility.Children.Count == 0)
             {
                 break;
             }

             MoveDefModelVisRefNode entry =
                 Root.data.mdlVisibility.Children[(int) ((e._event >> 16) & 1)] as MoveDefModelVisRefNode;
             if (entry.Children.Count == 0 || (int) e.EventData.parameters[0]._data < 0 &&
                 (int) e.EventData.parameters[0]._data >= entry.Children.Count)
             {
                 break;
             }

             MoveDefBoneSwitchNode SwitchNode =
                 entry.Children[(int) e.EventData.parameters[0]._data] as MoveDefBoneSwitchNode;
             foreach (MoveDefModelVisGroupNode grp in SwitchNode.Children)
             {
                 foreach (MoveDefBoneIndexNode b in grp.Children)
                 {
                     if (b.BoneNode != null)
                     {
                         foreach (MDL0ObjectNode p in b.BoneNode._manPolys)
                         {
                             p._render = false;
                         }
                     }
                 }
             }

             if ((int) e.EventData.parameters[1]._data > SwitchNode.Children.Count ||
                 (int) e.EventData.parameters[1]._data < 0)
             {
                 break;
             }

             MoveDefModelVisGroupNode Group =
                 SwitchNode.Children[(int) e.EventData.parameters[1]._data] as MoveDefModelVisGroupNode;
             foreach (MoveDefBoneIndexNode b in Group.Children)
             {
                 if (b.BoneNode != null)
                 {
                     foreach (MDL0ObjectNode p in b.BoneNode._manPolys)
                     {
                         p._render = true;
                     }
                 }
             }

             break;
         case 0x0B020100:
             Root.Model._visible = e.EventData.parameters[0]._data != 0;
             break;
         case 0x00100200: //Switch
             _cases = new List<MoveDefEventParameterNode>();
             _caseIndices = new List<int>();
             _runEvents = false;
             _loopStartIndex = e.Index;
             break;
         case 0x00110100: //Case
             if (!_runEvents)
             {
                 if (_cases != null && _caseIndices != null)
                 {
                     _cases.Add(e.Children[0] as MoveDefEventParameterNode);
                     _caseIndices.Add(e.Index);
                 }
             }
             else
             {
                 _eventIndex = _loopEndIndex + 1;
                 _loopEndIndex = -1;
             }

             break;
         case 0x00120000: //Default Case
             _defaultCaseIndex = e.Index;
             break;
         case 0x00130000: //End Switch
             _runEvents = true;
             _loopEndIndex = e.Index;
             //Apply cases
             int i = 0;
             MoveDefEventParameterNode Switch =
                 Children[_loopStartIndex].Children[1] as MoveDefEventParameterNode;
             foreach (MoveDefEventParameterNode p in _cases)
             {
                 if (Switch.Compare(p, 2))
                 {
                     _eventIndex = _caseIndices[i] + 1;
                     break;
                 }

                 i++;
             }

             if (i == _cases.Count && _defaultCaseIndex != -1)
             {
                 _eventIndex = _defaultCaseIndex + 1;
             }

             _cases = null;
             _defaultCaseIndex = -1;
             _loopStartIndex = -1;
             break;
         case 0x00180000: //Break
             _eventIndex = _loopEndIndex + 1;
             _loopEndIndex = -1;
             break;
         case 10000200: //Generate Article 
             break;
         case 0x10040200: //Set Anchored Article SubAction
         case 0x10070200: //Set Remote Article SubAction
             break;
         case 0x10010200: //Set Ex-Anchored Article Action
             break;
     }
 }

 #endregion */
}

public class MoveDefActCmnNode : MoveDefActionNode
    {
        public MoveDefActCmnNode(string name, bool blank, ResourceNode parent) : base(name, blank, parent)
        {
        }
    }
}
