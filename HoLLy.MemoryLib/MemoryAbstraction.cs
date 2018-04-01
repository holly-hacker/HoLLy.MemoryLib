using System;

namespace HoLLy.Memory
{
    internal class MemoryAbstraction
    {
        internal IntPtr Handle { get; }

        public MemoryAbstraction(IntPtr handle) => Handle = handle;

        public int ReadInt(IntPtr address) => BitConverter.ToInt32(ReadBytes(address, 4), 0);
        public float ReadFloat(IntPtr address) => BitConverter.ToSingle(ReadBytes(address, 4), 0);
        public double ReadDouble(IntPtr address) => BitConverter.ToDouble(ReadBytes(address, 8), 0);
        public IntPtr ReadIntPtr(IntPtr address) => new IntPtr(BitConverter.ToInt32(ReadBytes(address, 4), 0));

        /*
        public string ReadDotNetString(IntPtr address)
        {
            var vtable = ReadIntPtr(address);
            var len = ReadInt(vtable + 0x4);
            var buffer = ReadBytes(vtable + 0x8, len * 2);  //NOTE: not correct for codepoints larger than 2 bytes!
            return Encoding.Unicode.GetString(buffer);
        }

        public List<T> ReadDotNetList<T>(IntPtr address) where T : StructureBase
        {
            var vtable = ReadIntPtr(address);
            var arrBase = ReadIntPtr(vtable + 0x4) + 0x8;
            var size = ReadInt(vtable + 0xC);

            //for speed, we'll read the entire chunk at once and then read the pointers from it
            List<T> l = new List<T>(size);
            byte[] arrData = ReadBytes(arrBase, size * 4);
            for (int i = 0; i < size; i++)
            {
                var ptr = BitConverter.ToInt32(arrData, i * 4);
                l.Add(new StructureBase(this, new IntPtr(ptr)).To<T>());
            }

            return l;
        }
        */

        public byte[] ReadBytes(IntPtr address, int size)
        {
            var buffer = new byte[size];
            Native.ReadProcessMemory(Handle, address, buffer, size, out _);
            return buffer;
        }
    }
}
