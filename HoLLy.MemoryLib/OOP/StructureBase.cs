using System;

namespace HoLLy.Memory.OOP
{
    public class StructureBase
    {
        public bool IsNull => _addr == IntPtr.Zero;
        public bool Prefetched => _prefetchedBytes != null;
        protected virtual int PrefetchSize => -1;

        protected readonly MemoryAbstraction Mem;

        private IntPtr _addr;
        private byte[] _prefetchedBytes;

        public StructureBase(MemoryAbstraction mem, IntPtr addr)
        {
            Mem = mem;
            _addr = addr;
        }

        public void Prefetch()
        {
#if DEBUG
            if (PrefetchSize < 0) throw new Exception("Bad prefetch size!");
#endif
            if (Prefetched || PrefetchSize < 0) return;

            _prefetchedBytes = Mem.ReadBytes(_addr, PrefetchSize);
        }

        private IntPtr Offset(int offset) => new IntPtr(_addr.ToInt64() + offset);

        //prefetched
        protected IntPtr Pointer(int offset) => Prefetched ? new IntPtr(BitConverter.ToInt32(_prefetchedBytes, offset)) : Mem.ReadIntPtr(Offset(offset));
        protected int Int(int offset) => Prefetched ? BitConverter.ToInt32(_prefetchedBytes, offset) : Mem.ReadInt(Offset(offset));
        protected float Float(int offset) => Prefetched ? BitConverter.ToSingle(_prefetchedBytes, offset) : Mem.ReadFloat(Offset(offset));
        protected double Double(int offset) => Prefetched ? BitConverter.ToDouble(_prefetchedBytes, offset) : Mem.ReadDouble(Offset(offset));

        //not prefetched
        protected string String(int offset) => Mem.ReadDotNetString(Offset(offset));
        //protected List<T> List<T>(int offset) where T : StructureBase => Mem.ReadDotNetList<T>(Offset(offset));

        //other stuff
        public T To<T>() where T : StructureBase => (T)Activator.CreateInstance(typeof(T), Mem, _addr);
    }
}
