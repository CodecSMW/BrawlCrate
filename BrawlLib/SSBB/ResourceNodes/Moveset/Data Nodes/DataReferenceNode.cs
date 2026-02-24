using BrawlLib.Internal;
using BrawlLib.SSBB.Types;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class MoveDefReferenceNode : MoveDefEntryNode
    {
        internal FDefStringEntry* Header => (FDefStringEntry*) WorkingUncompressed.Address;
        public override ResourceType ResourceFileType => ResourceType.MDefRefList;

        private FDefStringTable* stringTable;

        private Dictionary<string, FDefStringEntry> exSubRoutineTable = new Dictionary<string, FDefStringEntry>();

        public MoveDefReferenceNode(VoidPtr table)
        {
            stringTable = (FDefStringTable*) table;
        }

        private bool populated;

        public override bool OnInitialize()
        {
            _name = "References";
            for (int i = 0; i < WorkingUncompressed.Length / 8; i++)
            {
                if (!exSubRoutineTable.ContainsKey(stringTable->GetString(Header[i]._stringOffset)))
                {
                    exSubRoutineTable.Add(stringTable->GetString(Header[i]._stringOffset), Header[i]);
                }
            }

            OnPopulate();
            return true;
        }

        public override void OnPopulate()
        {
            if (!populated)
            {
                populated = true;
                foreach (KeyValuePair<string, FDefStringEntry> ex in exSubRoutineTable)
                {
                    new MoveDefReferenceEntryNode {_name = ex.Key}.Initialize(this,
                        new DataSource(BaseAddress + ex.Value._dataOffset, 4));
                }
            }
        }
    }

    public unsafe class MoveDefReferenceEntryNode : MoveDefExternalNode
    {
        internal bint* Header => (bint*) WorkingUncompressed.Address;
        public override ResourceType ResourceFileType => ResourceType.Unknown;

        public int[] Offsets => _offsets.ToArray();

        [HandleProcessCorruptedStateExceptions]
        public override bool OnInitialize()
        {
            _offsets = new List<int>();
            _offsets.Add(_offset);
            int offset = *Header;
            if (_offset >= 0) //Sometimes "offset" is garbage when _offset is -1. -1 indicates no data.
            {
                while (offset > 0)
                {
                    _offsets.Add(offset);
                    offset = *(bint*)(BaseAddress + offset);
                    if (_offsets.Contains(offset))
                    {
                        break;
                    }
                }
            }
            else //TODO: breakpoint for checking disabled action commands. Clean up later.
            {   //Example to test for: PM Lucas statusAnimCmd_AirLasso
                return false;
            } //The cause of this error is from removing the action command without
              //removing its node name, leaving it unreferenced. Confusing BrawlCrate but
              //not particularly affecting Brawl itself.

            //_offsets.Add(offset);
            //Root._externalRefs.Add(this);
            return false;
        }
    }
}