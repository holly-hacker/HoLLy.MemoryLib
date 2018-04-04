using System;

namespace HoLLy.Memory.OOP
{
    public class StructureBase
    {
        public MemoryAbstraction Memory => _mem;
        public IntPtr Addr => _addr;
        public bool IsNull => Addr == IntPtr.Zero;
        public bool Prefetched => _prefetchedBytes != null;
        protected virtual int PrefetchSize => -1;

        private readonly MemoryAbstraction _mem;

        private IntPtr _addr;
        private byte[] _prefetchedBytes;

        public StructureBase(MemoryAbstraction mem, IntPtr addr)
        {
            _mem = mem;
            _addr = addr;
        }

        public void Prefetch()
        {
#if DEBUG
            if (PrefetchSize < 0) throw new Exception("Bad prefetch size!");
#endif
            if (Prefetched || PrefetchSize < 0) return;

            _prefetchedBytes = Memory.ReadBytes(Addr, PrefetchSize);
        }

        private IntPtr Offset(int offset) => new IntPtr(Addr.ToInt64() + offset);

        //prefetched
        protected IntPtr Pointer(int offset) => Prefetched ? new IntPtr(BitConverter.ToInt32(_prefetchedBytes, offset)) : Memory.ReadIntPtr(Offset(offset));
        protected int Int(int offset) => Prefetched ? BitConverter.ToInt32(_prefetchedBytes, offset) : Memory.ReadInt(Offset(offset));
        protected float Float(int offset) => Prefetched ? BitConverter.ToSingle(_prefetchedBytes, offset) : Memory.ReadFloat(Offset(offset));
        protected double Double(int offset) => Prefetched ? BitConverter.ToDouble(_prefetchedBytes, offset) : Memory.ReadDouble(Offset(offset));

        //not prefetched
        protected string String(int offset) => Memory.ReadDotNetString(Offset(offset));
        //protected List<T> List<T>(int offset) where T : StructureBase => Memory.ReadDotNetList<T>(Offset(offset));

        //other stuff
        public T To<T>() where T : StructureBase => (T)Activator.CreateInstance(typeof(T), Memory, Addr);
    }
}
