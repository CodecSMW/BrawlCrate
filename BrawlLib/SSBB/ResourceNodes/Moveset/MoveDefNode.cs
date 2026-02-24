using BrawlLib.Internal;
using BrawlLib.Internal.IO;
using BrawlLib.SSBB.Types;
using BrawlLib.SSBB.Types.Subspace;
using BrawlLib.Wii;
using BrawlLib.Wii.Compression;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;

namespace BrawlLib.SSBB.ResourceNodes
{
    public class MoveDefEntryNode : ResourceNode
    {
        //Variables specific for rebuilding
        [Browsable(false)] public VoidPtr _rebuildBase => Root._rebuildBase;

        public int _lookupCount;
        public List<int> _lookupOffsets = new List<int>();

        public VoidPtr _entryOffset = 0;
        public int _entryLength, _childLength;

        [Browsable(false)] public int _rebuildOffset => (int)_entryOffset - (int)_rebuildBase;

        [Browsable(false)] public VoidPtr Data => (VoidPtr)WorkingUncompressed.Address;

        [Browsable(false)]
        public VoidPtr BaseAddress
        {
            get
            {
                if (Root == null)
                {
                    return 0;
                }

                return Root.BaseAddress;
            }
        }

        [Browsable(false)] public MDL0Node Model => Root.Model;

        [Browsable(false)]
        public MoveDefNode Root
        {
            get
            {
                ResourceNode n = _parent;
                while (!(n is MoveDefNode) && n != null)
                {
                    n = n._parent;
                }

                return n as MoveDefNode;
            }
        }

        [Category("Moveset Entry")]
        [Browsable(false)]
        public int IntOffset => _offset;

        [Browsable(false)]
        public int _offset
        {
            get
            {
                if (Data != null)
                {
                    return (int)Data - (int)BaseAddress;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Category("Moveset Entry")]
        [Browsable(true)]
        public string HexOffset => "0x" + _offset.ToString("X");

        [Category("Moveset Entry")]
        [Browsable(true)]
        public int Size => WorkingUncompressed.Length;

        [Category("Moveset Entry")]
        [Browsable(true)]
        public bool External => _extNode != null;

        public override void Rebuild(bool force)
        {
            if (!IsDirty && !force)
            {
                return;
            }

            //Get uncompressed size
            int size = OnCalculateSize(force);

            //Create temp map
            FileMap uncompMap = FileMap.FromTempFile(size);

            //Rebuild node (uncompressed)
            Rebuild(uncompMap.Address, size, force);
            _replSrc.Map = _replUncompSrc.Map = uncompMap;

            //If compressed, compress resulting data.
            if (_compression != CompressionType.None)
            {
                //Compress node to temp file
                FileStream stream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite,
                    FileShare.None, 0x8, FileOptions.DeleteOnClose | FileOptions.SequentialScan);
                try
                {
                    Compressor.Compact(_compression, _entryOffset, _entryLength, stream, this);
                    _replSrc = new DataSource(
                        FileMap.FromStreamInternal(stream, FileMapProtect.Read, 0, (int) stream.Length), _compression);
                }
                catch (Exception x)
                {
                    stream.Dispose();
                    throw x;
                }
            }
        }

        public MoveDefExternalNode _extNode;
        public bool _extOverride = false;

        private VoidPtr data = null;
        private VoidPtr dAddr => data == null ? data = Data : data;

        public int offsetID;
        public bool isExtra = false;

        public override ResourceType ResourceFileType => ResourceType.NoEditEntry;

        public override bool OnInitialize()
        {
            if (Root == null)
            {
                return base.OnInitialize();
            }

            if (_extNode == null)
            {
                _extNode = Root.IsExternal(_offset);
                if (_extNode != null && !_extOverride)
                {
                    _name = _extNode.Name;
                    _extNode._refs.Add(this);
                }
            }

            //if (Index <= 30)
            //    Root._paths[_offset] = TreePath;
            if (!MoveDefNode.nodeDictionary.ContainsKey(_offset))
            {
                MoveDefNode.nodeDictionary.Add(_offset, this);
            }

            if (Size == 0)
            {
                int size = Root.GetSize(_offset);
                if (size > 0)
                {
                    SetSizeInternal(size);
                }
            }

            return base.OnInitialize();
        }

        public ActionEventInfo GetEventInfo(long id)
        {
            if (MoveDefNode.EventDictionary == null)
            {
                MoveDefNode.LoadEventDictionary();
            }

            if (MoveDefNode.EventDictionary.ContainsKey(id))
            {
                return MoveDefNode.EventDictionary[id];
            }

            return new ActionEventInfo(id, id.ToString("X"), "No Description Available.", null, null);
        }

        public override void SortChildren()
        {
            _children.Sort(Compare);
        }

        public static int Compare(ResourceNode n1, ResourceNode n2)
        {
            if (((MoveDefEntryNode) n1)._offset < ((MoveDefEntryNode) n2)._offset)
            {
                return -1;
            }

            if (((MoveDefEntryNode) n1)._offset > ((MoveDefEntryNode) n2)._offset)
            {
                return 1;
            }

            return 0;
        }

        public static int ActionCompare(ResourceNode n1, ResourceNode n2)
        {
            if (((MoveDefEntryNode) n1.Children[0])._offset < ((MoveDefEntryNode) n2.Children[0])._offset)
            {
                return -1;
            }

            if (((MoveDefEntryNode) n1.Children[0])._offset > ((MoveDefEntryNode) n2.Children[0])._offset)
            {
                return 1;
            }

            return 0;
        }

        public ResourceNode FindNode(int offset)
        {
            ResourceNode n;
            if (offset == _offset)
            {
                return this;
            }
            else
            {
                foreach (MoveDefEntryNode e in Children)
                {
                    if ((n = e.FindNode(offset)) != null)
                    {
                        return n;
                    }
                }
            }

            return null;
        }

        public ResourceNode GetByOffsetID(int id)
        {
            foreach (MoveDefEntryNode e in Children)
            {
                if (e.offsetID == id)
                {
                    return e;
                }
            }

            return null;
        }

        public virtual void PostProcess()
        {
        }
    }

    public abstract class MoveDefExternalNode : MoveDefEntryNode
    {
        public override ResourceType ResourceFileType => ResourceType.NoEditEntry;

        public List<int> _offsets = new List<int>();
        public List<MoveDefEntryNode> _refs = new List<MoveDefEntryNode>();

        public MoveDefEntryNode[] References => _refs.ToArray();

        public override void Remove()
        {
            foreach (MoveDefEntryNode e in _refs)
            {
                e._extNode = null;
            }

            base.Remove();
        }
    }

    public unsafe class MoveDefNode : ARCEntryNode
    {
        internal FDefHeader* Header => (FDefHeader*) WorkingUncompressed.Address;
        internal int dataSize, lookupOffset, numLookupEntries, numDataTable, numExternalSubRoutine;

        internal static ResourceNode TryParseGeneric(DataSource source, ResourceNode parent)
        {
            VoidPtr addr = source.Address;
            FDefHeader* header = (FDefHeader*) addr;

            if (header->_pad1 != 0 || header->_pad2 != 0 || header->_pad3 != 0 || header->_fileSize != source.Length ||
                header->_lookupOffset > source.Length || !Properties.Settings.Default.ParseMoveDef)
            {
                return null;
            }

            return new MoveDefNode();
        }

        public static SortedDictionary<long, ActionEventInfo> EventDictionary =
            new SortedDictionary<long, ActionEventInfo>();

        public static bool _dictionaryChanged = false;

        #region Event Dictionary

        public static void LoadEventDictionary()
        {
            EventDictionary = new SortedDictionary<long, ActionEventInfo>();
            
            //Now add on to events with user inputted data

            StreamReader sr = null;
            long idNumber = 0;
            string loc, id;

            //Read known parameters and their descriptions.
            loc = Application.StartupPath + "/MovesetData/Parameters.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        id = sr.ReadLine();
                        if (id == "" || id[0] == '*') //TODO: *OC_ID:3
                            break;
                        idNumber = Convert.ToInt32(id, 16);
                        int parameterNumber = (int)(idNumber / 0x100);
                        parameterNumber &= 0xFF;

                        if (!EventDictionary.Keys.Contains(idNumber))
                        {
                            EventDictionary.Add(idNumber, new ActionEventInfo());
                        }
                        for (int j = 0; ;j++)
                        {
                            string temp = sr.ReadLine();
                            if (temp == "")
                            {
                                break;
                            }
                            Array.Resize(ref EventDictionary[idNumber].Params, j+1);
                            Array.Resize(ref EventDictionary[idNumber].pDescs, j+1);
                            EventDictionary[idNumber].Params[j] = temp;
                            EventDictionary[idNumber].pDescs[j] = sr.ReadLine();
                        }
                    }
                }
            }

            //Read known events and their descriptions.
            loc = Application.StartupPath + "/MovesetData/Events.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        id = sr.ReadLine();
                        if (id == "" || id[0] == ' ')
                        {
                            continue;
                        }
                        idNumber = Convert.ToInt32(id, 16);

                        if (!EventDictionary.Keys.Contains(idNumber))
                        {
                            EventDictionary.Add(idNumber, new ActionEventInfo());
                            if ((idNumber & 0xFF00) >= 0x200) //will decrement through argument counts until found
                            {
                                for (long parentNumber = idNumber - 0x100; (parentNumber & 0xFF00) >= 0x100; parentNumber -= 0x100)
                                {
                                    if (EventDictionary.Keys.Contains(parentNumber)) //if found, use its parameters!
                                    {
                                        EventDictionary[idNumber].Params = EventDictionary[parentNumber].Params;
                                        EventDictionary[idNumber].pDescs = EventDictionary[parentNumber].pDescs;
                                        break;
                                    }
                                }
                            }
                        }

                        EventDictionary[idNumber].idNumber = idNumber;
                        EventDictionary[idNumber]._name = sr.ReadLine();
                        EventDictionary[idNumber]._description = sr.ReadLine();
                        EventDictionary[idNumber].SetDfltParameters(sr.ReadLine());
                        
                    }
                }
            }

            //Read the list containing the syntax to display each event with.
            loc = Application.StartupPath + "/MovesetData/EventSyntax.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        string syntax = "";
                        id = sr.ReadLine();
                        try
                        {
                            idNumber = Convert.ToInt32(id, 16);
                        }
                        catch
                        {
                            syntax = id;
                            goto AddSyntax;
                        } //Most likely syntax where the id should be

                        //Clear the existing syntax

                        if (!EventDictionary.Keys.Contains(idNumber))
                        {
                            EventDictionary.Add(idNumber, new ActionEventInfo());
                        }

                        EventDictionary[idNumber]._syntax = "";

                        syntax = sr.ReadLine();

                        EventDictionary[idNumber].idNumber = idNumber;

                        AddSyntax:
                        while (syntax != "" && syntax != null)
                        {
                            EventDictionary[idNumber]._syntax += syntax;
                            syntax = sr.ReadLine();
                        }
                    }
                }
            }
            Dictionary<long, List<int>> list = new Dictionary<long, List<int>>();
            List<string> enums = new List<string>();
            loc = Application.StartupPath + "/MovesetData/Enums.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    while (!sr.EndOfStream)
                    {
                        list = new Dictionary<long, List<int>>();
                        enums = new List<string>();
                        while (!string.IsNullOrEmpty(id = sr.ReadLine()))
                        {
                            idNumber = Convert.ToInt32(id, 16);

                            if (!list.ContainsKey(idNumber))
                            {
                                list.Add(idNumber, new List<int>());
                            }

                            string p = null;
                            while (!string.IsNullOrEmpty(p = sr.ReadLine()))
                            {
                                list[idNumber].Add(int.Parse(p));
                            }
                        }

                        string val = null;
                        while (!string.IsNullOrEmpty(val = sr.ReadLine()))
                        {
                            enums.Add(val);
                        }

                        foreach (long ev in list.Keys)
                        {
                            if (EventDictionary.ContainsKey(ev))
                            {
                                EventDictionary[ev].Enums = new Dictionary<int, List<string>>();
                                foreach (int p in list[ev])
                                {
                                    EventDictionary[ev].Enums.Add(p, enums);
                                }
                            }
                        }
                    }
                }
            }

            //CreateEventDictionary();
        }

        public void CreateEventDictionary()
        {
            string p1 = "EventDictionary.Add(";
            //idNumber
            string p2 = ", new ActionEventInfo(";
            //idNumber
            string p3 = ", \"";
            //name
            string p4 = "\",\n    \"";
            //description
            string p5 = "\",\n    new string[] {";
            //param name array
            string p6 = " },\n    new string[] {";
            //param description array
            string p7 = " },\n    \"";
            //syntax
            string p8 = "\",\n    new long[] {";
            //default params
            string p9 = " }));";

            string dic = "", idString = "";
            foreach (ActionEventInfo i in EventDictionary.Values)
            {
                idString = "0x" + i.idNumber.ToString("X").PadLeft(8, '0');
                dic += p1 + idString + p2 + idString + p3 + i._name.Replace("\"", "\\\"") + p4 +
                       i._description.Replace("\"", "\\\"") + p5;
                bool first = true;
                if (i.Params != null)
                {
                    foreach (string s in i.Params)
                    {
                        if (!first)
                        {
                            dic += ",";
                        }
                        else
                        {
                            first = false;
                        }

                        dic += " \"" + s.Replace("\"", "\\\"") + "\"";
                    }
                }
                else
                {
                    dic += " ";
                }

                dic += p6;
                first = true;
                if (i.pDescs != null)
                {
                    foreach (string s in i.pDescs)
                    {
                        if (!first)
                        {
                            dic += ",";
                        }
                        else
                        {
                            first = false;
                        }

                        dic += " \"" + s.Replace("\"", "\\\"") + "\"";
                    }
                }
                else
                {
                    dic += " ";
                }

                dic += p7 + i._syntax.Replace("\\", "\\\\") + p8;
                first = true;
                if (i.defaultParams != null)
                {
                    foreach (long s in i.defaultParams)
                    {
                        if (!first)
                        {
                            dic += ",";
                        }
                        else
                        {
                            first = false;
                        }

                        dic += " " + s;
                    }
                }
                else
                {
                    dic += " ";
                }

                dic += p9;
                Console.WriteLine(dic);
                dic = "";
            }
        }

        #endregion

        #region Attributes

        public void AddSectionDescriptions(string loc, string paramName, string keyExtra = "")
        {
            StreamReader sr;
            SectionParamInfo newParamBlock;
            List<AttributeInfo> AttributeArray = new List<AttributeInfo>();
            loc = Application.StartupPath + "/MovesetData/" + loc;
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream && i < 185; i++)
                    {
                        AttributeArray.Add(new AttributeInfo());
                        AttributeArray[i]._name = sr.ReadLine();
                        AttributeArray[i]._description = sr.ReadLine();
                        AttributeArray[i]._type = (AttributeArray[i]._name[0] == '*') ? 1 : 0;
                        /*if (AttributeArray[i]._type != 0)
                        {
                            AttributeArray[i]._name.Remove(0, 1);
                        }
                        */
                        //Considered it, but it is probably easier to keep the asterisk to be aware of integers.

                        if (AttributeArray[i]._description == "")
                        {
                            AttributeArray[i]._description = "No Description Available.";
                        }

                        sr.ReadLine();
                    }
                }
                sr.Close();
                newParamBlock = new SectionParamInfo(paramName, AttributeArray);
                Params.Add(paramName, newParamBlock);
                if (keyExtra != "")
                    Params.Add(keyExtra, newParamBlock);
            }
            
        }

        #endregion


        #region Stuff to find other stuff

        public List<ResourceNode> _externalRefs;
        public List<MoveDefExternalNode> _externalSections;

        public bool externalExists(string name)
        {
            foreach (MoveDefExternalNode e in _externalRefs)
                if (e.Name == name)
                    return true;
            foreach (MoveDefExternalNode e in _externalSections)
                if (e.Name == name)
                    return true;
            return false;
        }
        public MoveDefExternalNode IsExternal(int offset, bool IsInternal = false)
        {
            if (!IsInternal)
            {
                foreach (MoveDefExternalNode e in _externalRefs)
                {
                    foreach (int i in e._offsets)
                    {
                        if (i == offset)
                        {
                            return e;
                        }
                    }
                }
            }

            foreach (MoveDefExternalNode e in _externalSections)
            {
                foreach (int i in e._offsets)
                {
                    if (i == offset)
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        public ResourceNode FindNode(int offset)
        {
            ResourceNode n;
            if (offset == 0)
            {
                return this;
            }
            else
            {
                foreach (MoveDefEntryNode e in Children)
                {
                    if ((n = e.FindNode(offset)) != null)
                    {
                        return n;
                    }
                }
            }

            return null;
        }

        public MoveDefActionNode GetAction(int offset)
        {
            int list, type, index;
            GetLocation(offset, out list, out type, out index);
            return GetAction(list, type, index);
        }

        public MoveDefActionNode GetAction(int list, int type, int index)
        {
            if (list >= 3 && dataCommon == null || list == 4 || index == -1)
            {
                return null;
            }

            if (list >= 4 && dataCommon != null)
            {
                if (list == 5 && type >= 0 && index < dataCommon._flashOverlay.Children.Count)
                {
                    return (MoveDefActionNode) dataCommon._flashOverlay.Children[index]; //.Children[0];
                }

                if (list == 6 && type >= 0 && index < dataCommon._screenTint.Children.Count)
                {
                    return (MoveDefActionNode) dataCommon._screenTint.Children[index]; //.Children[0];
                }
            }

            if (list == 0 && type >= 0 && index < _actions.Children.Count)
            {
                return (MoveDefActionNode) _actions.Children[index].Children[type];
            }

            if (list == 1 && type >= 0 && index < _subActions.Children.Count)
            {
                return (MoveDefActionNode) _subActions.Children[index].Children[type];
            }

            if (list == 2 && _subRoutineList.Count > index)
            {
                return (MoveDefActionNode) _subRoutineList[index];
            }

            return null;
        }

        public int GetOffset(int list, int type, int index)
        {
            if (list == 4 || index == -1)
            {
                return -1;
            }

            if (list == 0 && type >= 0 && type < _actions.ActionOffsets.Count)
            {
                if (_actions.ActionOffsets[type].Count > index)
                {
                    return _actions.ActionOffsets[type][index];
                }
            }

            if (list == 1 && type >= 0 && type < _subActions.ActionOffsets.Count)
            {
                if (_subActions.ActionOffsets[type].Count > index)
                {
                    return _subActions.ActionOffsets[type][index];
                }
            }

            if (list == 2)
            {
                if (_subRoutineList.Count > index)
                {
                    return ((MoveDefEntryNode) _subRoutineList[index])._offset;
                }
            }

            if (list == 3)
            {
                if (_externalRefs.Count > index)
                {
                    return ((MoveDefEntryNode) _externalRefs[index])._offset;
                }
            }

            return -1;
        }

        public void GetLocation(int offset, out int list, out int type, out int index)
        {
            list = 0;
            type = -1;
            index = -1;

            bool done = false;
            /*
            if (dataCommon == null && data == null && dataEnm == null && animParam == null || offset <= 0)
            {
                list = 4; //Null
                done = true;
            }
            */
            //Search actions
            if (!done && _actions != null) 
            {
                for (type = 0; type < _actions.ActionOffsets.Count; type++)
                {
                    if ((index = _actions.ActionOffsets[type].IndexOf(offset)) != -1)
                    {
                        done = true;
                        break;
                    }
                }
            }
            //Search sub actions
            if (!done) 
            {
                list++;
                if (_subActions != null)
                {
                    for (type = 0; type < _subActions.ActionOffsets.Count; type++)
                    {
                        if ((index = _subActions.ActionOffsets[type].IndexOf(offset)) != -1)
                        {
                            done = true;
                            break;
                        }
                    }
                }
            }
            //Search subroutines
            if (!done) 
            {
                list++;
                MoveDefExternalNode e = IsExternal(offset,true);
                if (_subRoutines.ContainsKey(offset))
                {
                    index = _subRoutines[offset].Index;
                    string tempname = _subRoutines[offset].Name;
                    index = 0;
                    foreach (MoveDefActionNode a in _subRoutineList)
                    {
                        if (a.Name == tempname)
                        {
                            break;
                        }
                        index++;
                    }
                    /*
                    index = 0;
                    foreach (int keyOffset in _subRoutines.Keys)
                    {
                        if (_subRoutines[keyOffset]._offset == offset)
                        {
                            break;
                        }
                        index++;
                    }
                    */
                    type = -1;
                    done = true;
                }
            }
            //Search externals
            if (!done)
            {
                list++;
                MoveDefExternalNode e = IsExternal(offset);
                if (e != null)
                {
                    index = e.Index;
                    type = -1;
                    done = true;
                }
            }

            if (!done)
            {
                list++;
                type = -1;
                index = -1;
            }
            //search external lists
            if (!done && dataCommon != null && data == null && offset > 0)
            {
                if (dataCommon._screenTint != null)
                {
                    list++;
                    if ((index = dataCommon._screenTint.ActionOffsets.IndexOf((uint) offset)) != -1)
                    {
                        return;
                    }
                }

                if (dataCommon._flashOverlay != null)
                {
                    list++;
                    if ((index = dataCommon._flashOverlay.ActionOffsets.IndexOf((uint) offset)) != -1)
                    {
                        return;
                    }
                }
            }

            if (!done) //Do not remove!
            {
                list = 4; //Default if not found: Null. Will be null before subroutine pass.
            }   
        }

        #endregion

        public int GetSize(int offset)
        {
            if (_lookupSizes.ContainsKey(offset))
            {
                //_lookupSizes[offset].remove = true;
                return _lookupSizes[offset].DataSize;
            }

            return -1;
        }

        public void GetBoneIndex(ref int boneIndex)
        {
            if (RootNode.Name.StartsWith("FitWario") || RootNode.Name == "FitKirby")
            {
                if (data != null)
                {
                    if (data.warioParams8 != null)
                    {
                        MoveDefSectionParamNode p1 = data.warioParams8.Children[0] as MoveDefSectionParamNode;
                        MoveDefSectionParamNode p2 = data.warioParams8.Children[1] as MoveDefSectionParamNode;
                        bint* values = (bint*) p2.AttributeBuffer.Address;
                        int i = 0;
                        for (; i < p2.AttributeBuffer.Length / 4; i++)
                        {
                            if (values[i] == boneIndex)
                            {
                                break;
                            }
                        }

                        if (p1.AttributeBuffer.Length / 4 > i)
                        {
                            int value = -1;
                            if ((value = (int) ((bint*) p1.AttributeBuffer.Address)[i]) >= 0)
                            {
                                boneIndex = value;
                                return;
                            }
                            else
                            {
                                boneIndex -= 400;
                            }
                        }
                    }
                }
            }
        }

        public void SetBoneIndex(ref int boneIndex)
        {
            if (RootNode.Name.StartsWith("FitWario") || RootNode.Name == "FitKirby")
            {
                if (data != null)
                {
                    if (data.warioParams8 != null)
                    {
                        MoveDefSectionParamNode p1 = data.warioParams8.Children[0] as MoveDefSectionParamNode;
                        MoveDefSectionParamNode p2 = data.warioParams8.Children[1] as MoveDefSectionParamNode;
                        bint* values = (bint*) p2.AttributeBuffer.Address;
                        int i = 0;
                        for (; i < p1.AttributeBuffer.Length / 4; i++)
                        {
                            if (values[i] == boneIndex)
                            {
                                break;
                            }
                        }

                        if (p2.AttributeBuffer.Length / 4 > i)
                        {
                            int value = -1;
                            if ((value = ((bint*) p2.AttributeBuffer.Address)[i]) >= 0)
                            {
                                boneIndex = value;
                                return;
                            }
                        }
                    }
                }
            }
        }

        public bool[] StatusIDs;

        public Dictionary<uint, List<MoveDefEventNode>> _events;

        public SortedList<int, string> _paths = new SortedList<int, string>();
        public SortedList<int, string> Paths => _paths;

        public string[] iRequirements, iRequirementsUniq;
        public string[] iAirGroundStats;
        public string[] iCollisionStats;
        public string[] iGFXFiles;
        //public AttributeInfo[] AttributeArray;
        public List<AttributeInfo> AttributeArray;
        public Dictionary<string, SectionParamInfo> Params;

        public MoveDefActionListNode _subActions, _actions, _hiddenActions;

        public SortedDictionary<int, MoveDefActionNode> _subRoutines;
        public List<ResourceNode> _subRoutineList;
        public ResourceNode _subRoutineGroup;

        public MoveDefDataNode data;
        public MoveDefDataCommonNode dataCommon;
        public MoveDefAnimParamNode animParam;
        public MoveDefSubParamNode subParam;
        public ItmParamEntryNode itemParam;
        public MoveDefEnmDataNode dataEnm;
        public MoveDefEnmWpnNode enmWeapon;

        public MoveDefReferenceNode references;
        public MoveDefSectionNode sections;
        public MoveDefEntryNode internalReferences;
        public MoveDefLookupNode lookupNode;

        public CompactStringTable refTable;

        public Dictionary<int, MoveDefLookupOffsetNode> _lookupSizes;

        public MDL0Node _model = null;

        [Category("Moveset Definition")] public int LookupOffset => lookupOffset;

        [Category("Moveset Definition")] public int LookupCount => numLookupEntries;

        [Category("Moveset Definition")] public int DataTableCount => numDataTable;

        [Category("Moveset Definition")] public int ExtSubRoutines => numExternalSubRoutine;

        public MDL0Node Model => _model;

        public override ResourceType ResourceFileType => ResourceType.MDef;
        public VoidPtr BaseAddress;

        [Category("Moveset Definition")] public string DataSize => "0x" + dataSize.ToString("X");

        public SortedDictionary<int, MoveDefEntryNode> NodeDictionary => nodeDictionary;

        public static SortedDictionary<int, MoveDefEntryNode> nodeDictionary =
            new SortedDictionary<int, MoveDefEntryNode>();

        public override bool OnInitialize()
        {
            if (_name == null)
            {
                _name = "MoveDef_" + Parent.Name;
            }

            nodeDictionary = new SortedDictionary<int, MoveDefEntryNode>();

            dataSize = Header->_fileSize;
            lookupOffset = Header->_lookupOffset;
            numLookupEntries = Header->_lookupEntryCount;
            numDataTable = Header->_dataTableEntryCount;
            numExternalSubRoutine = Header->_externalSubRoutineCount;

            BaseAddress = (VoidPtr) Header + 0x20;
            return true;
        }

        //Offset - Size
        public Dictionary<int, int> _lookupEntries;

        #region Other Data

        public void LoadOtherData()
        {
            StreamReader sr = null;
            string loc;

            //Read the list of Event Requirements.
            loc = Application.StartupPath + "/MovesetData/Requirements.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        Array.Resize(ref iRequirements, i + 1);
                        iRequirements[i] = sr.ReadLine();
                    }
                }
            }
            //Special requirements distinct to different object types.
            loc = Application.StartupPath + "/MovesetData/Requirements";
            loc += "Fighter"; //TODO: switch case for special requirements for Items, Enemies! 
            loc += ".txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        Array.Resize(ref iRequirementsUniq, i + 1);
                        iRequirementsUniq[i] = sr.ReadLine();
                    }
                }
            }
            //Read the list of Air Ground Stats.
            loc = Application.StartupPath + "/MovesetData/AirGroundStats.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        Array.Resize(ref iAirGroundStats, i + 1);
                        iAirGroundStats[i] = sr.ReadLine();
                    }
                }
            }

            //Read the list of Collision Stats.
            loc = Application.StartupPath + "/MovesetData/CollisionStats.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        Array.Resize(ref iCollisionStats, i + 1);
                        iCollisionStats[i] = sr.ReadLine();
                    }
                }
            }

            //Read the list of GFX Files.
            loc = Application.StartupPath + "/MovesetData/GFXFiles.txt";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        Array.Resize(ref iGFXFiles, i + 1);
                        iGFXFiles[i] = sr.ReadLine();
                    }
                }
            }

            //string s = "iGFXFiles = new string[" + iGFXFiles.Length + "];";
            //string e = "";
            //int x = 0;
            //foreach (string v in iGFXFiles)
            //    e += "\niGFXFiles[" + x++ + "] = \"" + v + "\";";
            //Console.WriteLine(s + e);

            //string s = "iRequirements = new string[" + iRequirements.Length + "];";
            //string e = "";
            //int x = 0;
            //foreach (string v in iRequirements)
            //    e += "\niRequirements[" + x++ + "] = \"" + v + "\";";
            //Console.WriteLine(s + e);

            //s = "iAirGroundStats = new string[" + iAirGroundStats.Length + "];";
            //e = "";
            //x = 0;
            //foreach (string v in iAirGroundStats)
            //    e += "\niAirGroundStats[" + x++ + "] = \"" + v + "\";";
            //Console.WriteLine(s + e);

            //s = "iCollisionStats = new string[" + iCollisionStats.Length + "];";
            //e = "";
            //x = 0;
            //foreach (string v in iCollisionStats)
            //    e += "\niCollisionStats[" + x++ + "] = \"" + v + "\";";
            //Console.WriteLine(s + e);

            Params = new Dictionary<string, SectionParamInfo>();

            AddSectionDescriptions("Common/Attributes.txt", "Attributes", "SSE Attributes");
            AddSectionDescriptions("Common/IC-Basics.txt", "IC-Basics", "SSE IC-Basics");
            AddSectionDescriptions("Common/Global IC-Basics.txt", "Global IC-Basics", "SSE Global IC-Basics");
            AddSectionDescriptions("Common/patternPowerMul.txt", "patternPowerMul");

            //string s = "AttributeArray = new AttributeInfo[185];";
            //string e = "";
            //int x = 0;
            //foreach (AttributeInfo v in AttributeArray)
            //    e += "\nAttributeArray[" + x++ + "] = new AttributeInfo() { _name = \"" + v._name + "\", _description = \"" + (v._description == "" ? "No Description Available." : v._description) + "\", _type = " + v._type.ToString().ToLower() + " };";
            //Console.WriteLine(s + e);

            sr = null;
            loc = Application.StartupPath + "/MovesetData/CharSpecific/" + Parent.Name + ".txt";
            //loc += Parent.Name.Substring(3) + ""
            string name = "", attrName = "";
            if (File.Exists(loc))
            {
                using (sr = new StreamReader(loc))
                {
                    while (!sr.EndOfStream)
                    {
                        name = sr.ReadLine();
                        SectionParamInfo info = new SectionParamInfo();
                        info._newName = sr.ReadLine();
                        info._attributes = new List<AttributeInfo>();
                        while (true && !sr.EndOfStream)
                        {
                            if (string.IsNullOrEmpty(attrName = sr.ReadLine()))
                            {
                                break;
                            }
                            else
                            {
                                AttributeInfo i = new AttributeInfo();
                                i._name = attrName;
                                i._description = sr.ReadLine();
                                i._type = int.Parse(sr.ReadLine());
                                info._attributes.Add(i);
                                sr.ReadLine();
                            }
                        }

                        if (!Params.ContainsKey(name))
                        {
                            Params.Add(name, info);
                        }
                    }
                }
            }
        }

        #endregion

        public override void OnPopulate()
        {
            _subRoutines = new SortedDictionary<int, MoveDefActionNode>();
            _externalRefs = new List<ResourceNode>();
            _externalSections = new List<MoveDefExternalNode>();
            _lookupSizes = new Dictionary<int, MoveDefLookupOffsetNode>();
            _events = new Dictionary<uint, List<MoveDefEventNode>>();
            StatusIDs = new bool[0];
            
            LoadEventDictionary();
            LoadOtherData();

            //Parse references first but don't add to children yet
            if (numExternalSubRoutine > 0)
            {
                (references = new MoveDefReferenceNode(Header->StringTable) {_parent = this}).Initialize(this,
                    new DataSource(Header->ExternalSubRoutines, numExternalSubRoutine * 8));
                _externalRefs = references.Children;
            }

            (sections = new MoveDefSectionNode(Header->_fileSize, (VoidPtr) Header->StringTable)).Initialize(this,
                new DataSource(Header->DataTable, Header->_dataTableEntryCount * 8));
            (lookupNode = new MoveDefLookupNode(Header->_lookupEntryCount) {_parent = this}).Initialize(this,
                new DataSource(Header->LookupEntries, Header->_lookupEntryCount * 4));

            //Now add to children
            if (references != null)
            {
                Children.Add(references);
            }
            if (internalReferences != null)
            {
                Children.Add(internalReferences);
            }
            MoveDefSubRoutineListNode g = new MoveDefSubRoutineListNode() { _name = "SubRoutines", _parent = this };

            _subRoutineGroup = g;
            _subRoutineList = g.Children;

            //Load subroutines
            //if (!RootNode._origPath.Contains("Test"))
            {
                sections.Populate();
                foreach (MoveDefEntryNode p in sections._sectionList)
                {
                    if (!(p.Name == "data" || p.Name == "dataCommon" ||
                        p.Name == "animParam" || p.Name == "subParam" || p.Name == "itemParam"))
                    {
                        //if (internalReferences == null)
                        //{
                        //   internalReferences = new MoveDefReferenceNode(Header->StringTable) { _parent = sections };
                        //    internalReferences.Initialize(this, new DataSource(Header->DataTable, Header->_dataTableEntryCount * 8));
                        //    sections.Children.Add(internalReferences);
                        //}
                        //internalReferences.Children.Add(p);
                        //TODO: Identify
                        sections.Children.Add(p); //Fixes a bug where it duplicates data folders!
                        //references.Children.Add(p); //TODO: Test!
                        if (p is MoveDefActionNode)
                        {
                            //a = ResourceNode in _actions.Children
                            //b = Children[0].Name
                            //c = Children[1].Name
                            //g.Children.Add(p);
                            //_subRoutines.Add(p._offset, p as MoveDefActionNode);
                        }
                    }

                }
                sections.Populate();
            }
            g._name = "[" + g.Children.Count + "] " + g._name;

            _children.Add(g);

            //_children.Sort(MoveDefEntryNode.Compare);

            g._children.Sort(MoveDefEntryNode.Compare);

            flushEventPointers();
        }
        public void flushEventPointers()
        {
            refreshEventPointers(_subRoutineGroup);
            refreshEventPointers(_actions);
            refreshEventPointers(_subActions);

            if (sections._sectionList.Count > 0)
            {
                string name = sections._sectionList[0].Name;
                if (name == "dataCommon")
                {
                    refreshEventPointers(dataCommon._flashOverlay);
                    refreshEventPointers(dataCommon._screenTint);
                    refreshEventPointers(sections);
                }
                else if (name == "animParam")
                    refreshEventPointers(_hiddenActions);
            }
        }
        public void refreshEventPointers(ResourceNode actionList)
        {
            if (actionList == null)
                return;
            for (int j = 0; j < actionList._children.Count; j++)
            {
                if (actionList._children[j] is MoveDefActionNode)
                {
                    refreshEvents(actionList._children[j] as MoveDefActionNode);
                }
                else if (actionList._children[j] is MoveDefActionGroupNode || actionList._children[j] is MoveDefSubActionGroupNode)
                {
                    for (int k = 0; k < actionList._children[j]._children.Count; k++)
                    {
                        MoveDefActionNode refreshNode = actionList._children[j]._children[k] as MoveDefActionNode;
                        if (refreshNode != null)
                        {
                            refreshEvents(refreshNode);
                        }
                    }
                }
            }
        }
        public void refreshEvents(MoveDefActionNode EventList)
        {
            foreach (MoveDefEventNode d in EventList.Children)
            {
                int eventCount = d.Children.Count;
                for (int i = 0; i < eventCount; i++)
                {
                    if (d.Children[i] is MoveDefEventOffsetNode)
                    {
                        MoveDefEventOffsetNode c = d.Children[i] as MoveDefEventOffsetNode;
                        if (!c.External || !externalExists(c._extNode.Name)) //if not external and the external wasn't removed
                            GetLocation(c._value, out c.list, out c.type, out c.index);
                    }
                }
            }
        }

        public List<MoveDefEntryNode> _postProcessNodes;
        public VoidPtr _rebuildBase;
        public static LookupManager _lookupOffsets;
        public int lookupCount, lookupLen;
        
        public void refCalculate(ref CompactStringTable Table, ref int refCount, ref int size, ref MoveDefReferenceNode references)
        {
            foreach (MoveDefExternalNode e in references.Children)
            {
                if (e._refs.Count > 0)
                {
                    refTable.Add(e.Name);
                    size += 8;
                    refCount++;
                }
            }
        }

        public override int OnCalculateSize(bool force)
        {
            int size = 0x20;
            _postProcessNodes = new List<MoveDefEntryNode>();
            _lookupOffsets = new LookupManager();
            lookupCount = 0;
            lookupLen = 0;
            refCount = 0;
            refTable = new CompactStringTable();
            foreach (MoveDefEntryNode e in sections._sectionList)
            {
                e._lookupCount = 0;
                if (e is MoveDefReferenceNode) //TODO: Try to organize visually better somehow!
                {
                    MoveDefReferenceNode checkRef = e as MoveDefReferenceNode;
                    refCalculate(ref refTable, ref refCount, ref size, ref checkRef);
                }
                else
                {
                    if (e is MoveDefExternalNode)
                    {
                        MoveDefExternalNode ext = e as MoveDefExternalNode;
                        if (ext._refs.Count > 0)
                        {
                            MoveDefEntryNode entry = ext._refs[0];

                            if ((entry.Parent is MoveDefDataNode || entry.Parent is MoveDefMiscNode) && !entry.isExtra)
                            {
                                lookupCount++;
                            }

                            if (!(entry is MoveDefRawDataNode))
                            {
                                entry.CalculateSize(true);
                            }
                            else if (entry.Children.Count > 0)
                            {
                                int off = 0;
                                foreach (MoveDefEntryNode n in entry.Children)
                                {
                                    off += n.CalculateSize(true);
                                    entry._lookupCount += n._lookupCount;
                                }

                                entry._entryLength = entry._calcSize = off;
                            }
                            else
                            {
                                entry.CalculateSize(true);
                            }

                            e._lookupCount = entry._lookupCount;
                            e._childLength = entry._childLength;
                            e._entryLength = entry._entryLength;
                            e._calcSize = entry._calcSize;
                        }
                        else
                        {
                            e.CalculateSize(true);
                        }
                    }
                    else
                    {
                        e.CalculateSize(true);
                    }

                    size += (e._calcSize == 0 ? e._childLength + e._entryLength : e._calcSize) + 8;
                    lookupCount += e._lookupCount;
                    refTable.Add(e.Name); //Add name to ref list
                }
            }
            foreach (MoveDefEntryNode e in sections._namedFeatures)
            {
                refTable.Add(e.Name); //Add name to ref list
                size += 8;
            }

            if (references != null)
            {
                refCalculate(ref refTable, ref refCount, ref size, ref references);
            }
            size += 0x80; //TODO: Figure out why this is cropping out 0x1A off the reftable at the bottom???

            return size + (lookupLen = lookupCount * 4) + refTable.TotalSize;
        }

        private int refCount;

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            //Children are built in order but before their parent! 

            _rebuildBase = address + 0x20;

            FDefHeader* header = (FDefHeader*) address;
            header->_fileSize = length;
            header->_externalSubRoutineCount = refCount;
            header->_dataTableEntryCount = sections._sectionList.Count + sections._namedFeatures.Count;
            header->_lookupEntryCount = lookupCount;
            header->_pad1 = header->_pad2 = header->_pad3 = 0;

            VoidPtr dataAddress = _rebuildBase;

            int lookupOffset = 0, sectionsOffset = 0;

            bool debugReferences = false;

            if (debugReferences) //used to debug dataCommon and the usage of itemSwingData and patternPowerMul
                header->_dataTableEntryCount = 0;


            //TODO: itemSwingData and patternPowerMul ended up added here in dataCommon!
            if (!debugReferences)
            {
                foreach (MoveDefEntryNode e in sections._sectionList)
                {
                    lookupOffset += e._calcSize == 0 ? e._childLength + e._entryLength : e._calcSize;
                    sectionsOffset += e._childLength;
                }
            }

            VoidPtr lookupAddress = dataAddress + lookupOffset;
            VoidPtr sectionsAddr = dataAddress + sectionsOffset;
            VoidPtr dataHeaderAddr = dataAddress;

            //TODO: itemSwingData and patternPowerMul ended up added here in dataCommon!
            if (!debugReferences)
            {
                foreach (MoveDefEntryNode e in sections._sectionList)
                {
                    e._lookupOffsets.Clear();
                    if (e.Name == "data" || e.Name == "dataCommon")
                    {
                        dataHeaderAddr = sectionsAddr; //Don't rebuild yet
                        sectionsAddr += e._entryLength;
                    }
                    else //Rebuild other sections first
                    {
                        if (e is MoveDefExternalNode)
                        {
                            MoveDefExternalNode ext = e as MoveDefExternalNode;
                            if (ext._refs.Count > 0)
                            {
                                MoveDefEntryNode entry = ext._refs[0];

                                if (!(entry is MoveDefRawDataNode))
                                {
                                    entry.Rebuild(sectionsAddr, entry._calcSize, true);
                                }
                                else if (entry.Children.Count > 0)
                                {
                                    entry._entryOffset = sectionsAddr;
                                    int off = 0;
                                    foreach (MoveDefEntryNode n in entry.Children)
                                    {
                                        n.Rebuild(sectionsAddr + off, n._calcSize, true);
                                        off += n._calcSize;
                                        entry._lookupOffsets.AddRange(n._lookupOffsets);
                                    }
                                }
                                else
                                {
                                    entry.Rebuild(sectionsAddr, entry._calcSize, true);
                                }

                                e._entryOffset = entry._entryOffset;
                                e._lookupOffsets = entry._lookupOffsets;
                            }
                            else
                            {
                                e.Rebuild(sectionsAddr, e._calcSize, true);
                            }
                        }
                        else
                        {
                            e.Rebuild(sectionsAddr, e._calcSize, true);
                        }

                        //TODO: Make sure this writes properly for animCmd Action Nodes!
                        if (!(e is MoveDefActionNode) && (e._lookupCount != e._lookupOffsets.Count &&
                            !((e as MoveDefExternalNode)._refs[0] is MoveDefActionNode)))
                        {
                            Console.WriteLine();
                        }

                        _lookupOffsets.AddRange(e._lookupOffsets.ToArray());
                        sectionsAddr += e._calcSize;
                    }
                }
            }
            if (data != null)
            {
                data.dataHeaderAddr = dataHeaderAddr;
                data.Rebuild(address + 0x20, data._childLength, true);
            }
            else if (dataCommon != null)
            {
                dataCommon.dataHeaderAddr = dataHeaderAddr;
                dataCommon.Rebuild(address + 0x20, dataCommon._childLength, true);
            }

            if (references != null) // will be null for dataCommon
            {
                foreach (MoveDefExternalNode e in references.Children)
                {
                    for (int i = 0; i < e._refs.Count; i++)
                    {
                        bint* addr = (bint*)e._refs[i]._entryOffset;
                        if (i == e._refs.Count - 1)
                        {
                            *addr = -1;
                        }
                        else
                        {
                            *addr = (int)e._refs[i + 1]._entryOffset - (int)_rebuildBase;

                            //references don't use lookup table
                            //_lookupOffsets.Add((int)addr - (int)_rebuildBase);
                        }
                    }
                }
            }


            _lookupOffsets.values.Sort();

            if (lookupCount != _lookupOffsets.Count)
            {
                Console.WriteLine(lookupCount - _lookupOffsets.Count);
            }

            header->_lookupOffset = (int) lookupAddress - (int) _rebuildBase;
            header->_lookupEntryCount = _lookupOffsets.Count;

            if (data != null && data.warioSwing4StringOffset > 0 && data.warioParams6 != null)
            {
                ((WarioExtraParams6*) data.warioParams6._entryOffset)->_offset = data.warioSwing4StringOffset;
            }

            int val = -1;
            if (data != null && data.zssFirstOffset > 0)
            {
                val = data.zssFirstOffset;
            }

            bint* values = (bint*) lookupAddress;
            foreach (int i in _lookupOffsets.values)
            {
                if (val == i && data != null && data.zssParams8 != null)
                {
                    *(bint*) data.zssParams8._entryOffset = 29;
                    *((bint*) data.zssParams8._entryOffset + 1) = (int) values - (int) _rebuildBase;
                }

                *values++ = i;
            }

            dataAddress = (VoidPtr) values;
            VoidPtr refTableAddr = dataAddress + 
                sections._sectionList.Count * 8 +
                sections._namedFeatures.Count * 8 +
                refCount * 8;
            // where itemSwingData and patternPowerMul are written to!
            if (!debugReferences)
                refTable.WriteTable(refTableAddr);

            //TODO: itemSwingData and patternPowerMul ended up added here in dataCommon!
            if (!debugReferences)
            {
                foreach (MoveDefEntryNode e in sections._sectionList)
                {
                    *values++ = (int)e._entryOffset - (int)_rebuildBase;
                    *values++ = (int)refTable[e.Name] - (int)refTableAddr;
                }
                
                foreach (MoveDefEntryNode e in sections._namedFeatures)
                {
                    *values++ = (int)e._entryOffset - (int)_rebuildBase;
                    *values++ = (int)refTable[e.Name] - (int)refTableAddr;
                }
                
            }




            //will be null for dataCommon
            if (references != null) 
            {
                foreach (MoveDefExternalNode e in references.Children)
                {
                    if (e._refs.Count > 0)
                    {
                        *values++ = (int)e._refs[0]._entryOffset - (int)_rebuildBase;
                        *values++ = (int)refTable[e.Name] - (int)refTableAddr;
                    }
                }
            }


            //Some nodes handle rebuilding their own children, 
            //so if one of those children has changed, the node will stay dirty and may rebuild over itself.
            //Manually set IsDirty to false to avoid that.
            IsDirty = false;

            BaseAddress = _rebuildBase;
        }
    }

    public class LookupManager
    {
        public List<int> values = new List<int>();
        public int Count => values.Count;

        public void Add(int value)
        {
            if (value > 0 && !values.Contains(value))
            {
                if (value < 1480)
                {
                    Console.WriteLine(value);
                }
                else
                {
                    values.Add(value);
                }
            }
            else
            {
                Console.WriteLine(value);
            }
        }

        public void AddRange(int[] vals)
        {
            foreach (int value in vals)
            {
                if (value > 0 && !values.Contains(value))
                {
                    if (value < 1480)
                    {
                        Console.WriteLine(value);
                    }
                    else
                    {
                        values.Add(value);
                    }
                }
                else
                {
                    Console.WriteLine(value);
                }
            }
        }
    }

    public class NameSizeGroup
    {
        public string Name;
        public int Size;

        public NameSizeGroup(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }

    public unsafe class MoveDefCommonReferenceNode : MoveDefEntryNode
    {
        public override bool OnInitialize()
        {
            base.OnInitialize();

            _name = "Externals";
            //CalculateDataLen(); //TODO: Calculate Size!
            return true;
        }
    }
    public unsafe class MoveDefSectionNode : MoveDefEntryNode
    {
        internal FDefStringEntry* Header => (FDefStringEntry*) WorkingUncompressed.Address;
        public Dictionary<NameSizeGroup, FDefStringEntry> DataTable = new Dictionary<NameSizeGroup, FDefStringEntry>();
        private FDefStringTable* stringTable;
        public int DataSize, dataOffset;
        public MoveDefCommonReferenceNode actReferences,act2References;

        public MoveDefSectionNode(int dataSize, VoidPtr table)
        {
            DataSize = dataSize;
            stringTable = (FDefStringTable*) table;
        }

        public override bool OnInitialize()
        {
            base.OnInitialize();

            _name = "Sections";

            for (int i = 0; i < WorkingUncompressed.Length / 8; i++)
            {
                DataTable.Add(new NameSizeGroup(stringTable->GetString(Header[i]._stringOffset), 0), Header[i]);
            }

            CalculateDataLen();

            foreach (KeyValuePair<NameSizeGroup, FDefStringEntry> data in DataTable)
            {
                if (data.Key.Name == "data" || data.Key.Name == "dataCommon" || data.Key.Name == "animParam")
                {
                    dataOffset = data.Value._dataOffset;
                }
            }

            return true;
        }

        private void CalculateDataLen()
        {
            List<KeyValuePair<NameSizeGroup, FDefStringEntry>> sorted =
                DataTable.OrderBy(x => (int) x.Value._dataOffset).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                if (i < sorted.Count - 1)
                {
                    sorted[i].Key.Size = (int) (sorted[i + 1].Value._dataOffset - sorted[i].Value._dataOffset);
                }
                else
                {
                    sorted[i].Key.Size = (int) (((MoveDefNode) Parent).lookupOffset - sorted[i].Value._dataOffset);
                }

                //Console.WriteLine(sorted[i].ToString());
            }
        }

        public List<MoveDefEntryNode> _sectionList, _namedFeatures; //_namedFeatures are dependent sections of data or dataCommon
        public List<MoveDefEntryNode> _enmWeaponList;
        public override void OnPopulate()
        {
            _sectionList = new List<MoveDefEntryNode>();
            _namedFeatures = new List<MoveDefEntryNode>();
            _enmWeaponList = new List<MoveDefEntryNode>();

            actReferences = new MoveDefCommonReferenceNode();
            actReferences._parent = this;
            actReferences._name = "References";
            act2References = new MoveDefCommonReferenceNode();
            act2References._parent = this;
            act2References._name = "External References";
            int offsetID = 0;
            bool hasDataCommon = false; //TODO: Try to make this start as false. dataCommon isn't where anticipated.
            bool isEnemy = RootNode._name.StartsWith("Enm"); //check if an enemy node
            VoidPtr gotDataCommon = null;
            //Parse external offsets first
            foreach (KeyValuePair<NameSizeGroup, FDefStringEntry> data in DataTable)
            {
                if (data.Key.Name == "dataCommon")
                {
                    hasDataCommon = true;
                }
                else if (hasDataCommon || isEnemy)
                {
                    if ((isEnemy && data.Key.Name.StartsWith("statusAnimCmd")) ||
                      (data.Key.Name.StartsWith("effectAnimCmd_") ||
                        (data.Key.Name.StartsWith("gameAnimCmd_") && !data.Key.Name.Contains("DamageFly")))) //see MoveDefActionNode.cs for why
                        goto EstablishExtAction;
                    else if (data.Key.Name.StartsWith("statusAnimCmd"))
                    {
                        string subName = data.Key.Name.Substring(13); //I hope you like reading horrors below accounting for inconsistent labeling from someone else's spreadsheet!
                        if ((subName.StartsWith("Pre_") && !subName.EndsWith("_SpecialN")) || //only one exception
                            subName.StartsWith("Group_") || //these groups are entirely called externally
                            (subName[0] == '_' &&
                                (subName.EndsWith("_SpecialJumpAttack") ||
                                (subName == "_AIRChkCliffForce"))))//)))))))))):)
                            goto EstablishExtAction;
                        else
                            continue; //angry yet?
                    }
                    else
                        continue;

                    EstablishExtAction: //Why not a normal subroutine? Good question. The game didn't make these ones. Be mad at them.
                    int i = 0;
                    MoveDefActionNode r = new MoveDefActionNode(data.Key.Name, false, actReferences) { offsetID = offsetID };
                    r.Initialize(actReferences, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                    _sectionList.Add(r);
                }
                else if (!hasDataCommon && data.Key.Name.Contains("Disguise"))
                {
                    MoveDefRawDataNode r = new MoveDefRawDataNode(data.Key.Name) { _parent = this, offsetID = offsetID };
                    r.Initialize(this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                    Root._externalSections.Add(r);
                    //if (!data.Key.Name.Contains("Disguise")) //TODO: Try to hide for organization later?
                    //Right now, omitting this just makes it not compile correctly. Not the desirable result.
                    _sectionList.Add(r);
                }
            }

                /*
                else
                {
                    //TODO: Create type MoveDefFitKirbyCopyNode
                    if (hasDataCommon &&
                        (data.Key.Name.StartsWith("gameAnimCmd") ||
                         data.Key.Name.StartsWith("statusAnimCmd") ||
                         data.Key.Name.StartsWith("effectAnimCmd"))

                         && !data.Key.Name.Contains("Disguise"))
                    {
                        MoveDefActionNode r = new MoveDefActionNode(data.Key.Name, false, actReferences) { offsetID = offsetID };
                        r.Initialize(actReferences, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                        Root._externalSections.Add(r);
                        _sectionList.Add(r);
                    }
                     //TODO: If done, for some reason it gets ignored by dataCommon as if separate????
                    else
                    {
                        MoveDefRawDataNode r = new MoveDefRawDataNode(data.Key.Name) { _parent = this, offsetID = offsetID };
                        r.Initialize(this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                        Root._externalSections.Add(r);
                        //if (!data.Key.Name.Contains("Disguise")) //TODO: Try to hide for organization later?
                        //Right now, omitting this just makes it not compile correctly. Not the desirable result.
                            _sectionList.Add(r);
                    }

                }

                offsetID++;
                }

                */
                offsetID = 0;
            //Now add the data node
            foreach (KeyValuePair<NameSizeGroup, FDefStringEntry> data in DataTable)
            {
                if (data.Key.Name == "data" && !(RootNode.Name.StartsWith("FitKirby") && !RootNode.Name.Equals("FitKirby")))
                {
                    if (RootNode._name.StartsWith("Enm")) //check if an enemy node
                    {
                        (Root.dataEnm = new MoveDefEnmDataNode((uint)DataSize, data.Key.Name) { offsetID = offsetID }).Initialize(
                            this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                        _sectionList.Insert(0, Root.dataEnm);
                    }
                    else
                    {
                        //TODO: Make new format MoveDefKirbyCopyNode. Kirby's copy abilities aren't treated like normal fighters!
                        (Root.data = new MoveDefDataNode((uint)DataSize, data.Key.Name) { offsetID = offsetID }).Initialize(
                            this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                        _sectionList.Insert(0, Root.data);
                    }
                }
                else if (data.Key.Name == "dataCommon")
                {
                    (Root.dataCommon = new MoveDefDataCommonNode((uint) DataSize, data.Key.Name) {offsetID = offsetID})
                        .Initialize(this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                    _sectionList.Insert(0,Root.dataCommon);
                    break;
                }

                else if (data.Key.Name == "animParam")
                {
                    (Root.animParam = new MoveDefAnimParamNode((uint) DataSize, data.Key.Name) { offsetID = offsetID }).Initialize(this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                    _sectionList.Insert(0, Root.animParam);
                }
                else if (data.Key.Name == "subParam")
                {
                    (Root.subParam = new MoveDefSubParamNode((uint) DataSize, data.Key.Name) { offsetID = offsetID }).Initialize(this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                    _sectionList.Insert(1, Root.subParam);
                }
                else if (data.Key.Name == "itemParam")
                {
                    Root.itemParam = new ItmParamEntryNode();
                    Root.itemParam._name = data.Key.Name;
                    Root.itemParam.Initialize(this, new DataSource(BaseAddress + data.Value._dataOffset, ItmParamEntry.Size));
                }
                else if (data.Key.Name.StartsWith("s_wpnAnmCmdDataSet"))
                {
                    (Root.enmWeapon = new MoveDefEnmWpnNode((uint)DataSize, data.Key.Name) { offsetID = offsetID }).Initialize(this, new DataSource(BaseAddress + data.Value._dataOffset, data.Key.Size));
                    //_enmWeaponList.Add(Root.enmWeapon);
                    //_sectionList.Insert(1, Root.enmWeapon);
                }
                offsetID++;
            }
            if (hasDataCommon)
            {
                //Children.Add(actReferences);
            }

  //          SortChildren();
   //         _sectionList.Sort(Compare);
        }
    }

    public class SpecialOffset
    {
        public int Index;
        public int Offset;
        public int Size;

        public override string ToString()
        {
            return string.Format("[{2}] Offset={0} Size={1}", Offset, Size, Index);
        }
    }

    public unsafe class MoveDefActionsNode : MoveDefEntryNode
    {
        internal bint* Header => (bint*) WorkingUncompressed.Address;

        internal List<int> ActionOffsets = new List<int>();

        public MoveDefActionsNode(string name)
        {
            _name = name;
        }

        public override bool OnInitialize()
        {
            base.OnInitialize();
            for (int i = 0; i < WorkingUncompressed.Length / 4; i++)
            {
                ActionOffsets.Add(Header[i]);
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
                    new MoveDefActionNode("Action " + Convert.ToString(i, 16), false, null).Initialize(this,
                        new DataSource(BaseAddress + offset, 0));
                }
                else
                {
                    Children.Add(new MoveDefActionNode("Action " + Convert.ToString(i, 16), true, this));
                }

                i++;
            }
        }
    }
}