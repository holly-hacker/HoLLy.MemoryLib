using System;
using System.Text;

namespace HoLLy.Memory
{
    internal class MemoryAbstraction
    {
        internal IntPtr Handle { get; }

        public MemoryAbstraction(IntPtr handle) => Handle = handle;

        public sbyte ReadSByte(IntPtr address)  => (sbyte)ReadBytes(address, 1)[0];
        public short ReadShort(IntPtr address)  => BitConverter.ToInt16(ReadBytes(address, 2), 0);
        public int ReadInt(IntPtr address)      => BitConverter.ToInt32(ReadBytes(address, 4), 0);
        public long ReadLong(IntPtr address)    => BitConverter.ToInt64(ReadBytes(address, 8), 0);

        public byte ReadByte(IntPtr address)    => ReadBytes(address, 1)[0];
        public ushort ReadUShort(IntPtr address)=> BitConverter.ToUInt16(ReadBytes(address, 2), 0);
        public uint ReadUInt(IntPtr address)    => BitConverter.ToUInt32(ReadBytes(address, 4), 0);
        public ulong ReadULong(IntPtr address)  => BitConverter.ToUInt64(ReadBytes(address, 8), 0);

        public float ReadFloat(IntPtr address)  => BitConverter.ToSingle(ReadBytes(address, 4), 0);
        public double ReadDouble(IntPtr address)=> BitConverter.ToDouble(ReadBytes(address, 8), 0);
        public IntPtr ReadIntPtr(IntPtr address)=> new IntPtr(BitConverter.ToInt32(ReadBytes(address, 4), 0));

        /// <summary>
        /// Read a .NET string through the pointer to it
        /// </summary>
        /// <param name="address">The pointer to the .NET string</param>
        /// <returns></returns>
        public string ReadDotNetString(IntPtr address)
        {
            var vtable = ReadIntPtr(address);
            var len = ReadInt(new IntPtr(vtable.ToInt64() + 0x4));
            var buffer = ReadBytes(new IntPtr(vtable.ToInt64() + 0x8), len * 2);  //NOTE: not correct for codepoints larger than 2 bytes!
            return Encoding.Unicode.GetString(buffer);
        }

        /*
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
