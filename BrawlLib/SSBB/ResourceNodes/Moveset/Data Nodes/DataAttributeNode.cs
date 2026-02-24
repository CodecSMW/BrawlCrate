using BrawlLib.Internal;
using BrawlLib.SSBB.Types;
using System.ComponentModel;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class MoveDefAttributeNode : MoveDefEntryNode
    {
        internal FDefAttributes* Header => (FDefAttributes*) WorkingUncompressed.Address;
        public override ResourceType ResourceFileType => ResourceType.Unknown;

        private UnsafeBuffer attributeBuffer;

        [Browsable(false)]
        public UnsafeBuffer AttributeBuffer
        {
            get
            {
                if (attributeBuffer != null)
                {
                    return attributeBuffer;
                }

                return attributeBuffer = new UnsafeBuffer(0x2E4);
            }
        }

        public MoveDefAttributeNode(string name)
        {
            _name = name;
        }

        #region OnInitRebuildCalc

        public override bool OnInitialize()
        {
            base.OnInitialize();
            attributeBuffer = new UnsafeBuffer(0x2E4);
            byte* pOut = (byte*) attributeBuffer.Address;
            byte* pIn = (byte*) Header;
            for (int i = 0; i < 0x2E4; i++)
            {
                *pOut++ = *pIn++;
            }

            return false;
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            _entryOffset = address;
            byte* pIn = (byte*) attributeBuffer.Address;
            byte* pOut = (byte*) address;
            for (int i = 0; i < 0x2E4; i++)
            {
                *pOut++ = *pIn++;
            }
        }

        public override int OnCalculateSize(bool force)
        {
            _lookupCount = 0;
            return 0x2E4;
        }

        #endregion
    }
}