using System;
using System.Collections.Generic;

namespace HoLLy.Memory.OOP
{
    public class StructureBase
    {
        /// <summary>
        /// The underlying <see cref="MemoryAbstraction"/> object that handles this object's memory access.
        /// </summary>
        public MemoryAbstraction Memory { get; }

        private IntPtr Addr { get; }

        /// <summary> Is the pointer pointing to this structure <code>null</code>? </summary>
        public bool IsNull => Addr == IntPtr.Zero;
        public bool Prefetched => _prefetchedBytes != null;
        protected virtual int PrefetchSize => -1;

        private byte[] _prefetchedBytes;

        public StructureBase(MemoryAbstraction mem, IntPtr addr)
        {
            Memory = mem;
            Addr = addr;
        }

        /// <summary>
        /// Will read the entire object and cache it, preventing future on-demand memory reads
        /// </summary>
        public void Prefetch()
        {
#if DEBUG
            if (PrefetchSize < 0) throw new Exception("Bad prefetch size!");
#endif
            if (Prefetched || PrefetchSize < 0) return;

            _prefetchedBytes = Memory.ReadBytes(Addr, PrefetchSize);
        }

        private IntPtr Offset(int offset) => Addr + offset;

        //prefetched
        protected IntPtr Pointer(int offset) => Prefetched ? new IntPtr(BitConverter.ToInt32(_prefetchedBytes, offset)) : Memory.ReadIntPtr(Offset(offset));
        protected int Int(int offset) => Prefetched ? BitConverter.ToInt32(_prefetchedBytes, offset) : Memory.ReadInt(Offset(offset));
        protected float Float(int offset) => Prefetched ? BitConverter.ToSingle(_prefetchedBytes, offset) : Memory.ReadFloat(Offset(offset));
        protected double Double(int offset) => Prefetched ? BitConverter.ToDouble(_prefetchedBytes, offset) : Memory.ReadDouble(Offset(offset));

        /// <summary> Reads a <see cref="System.String"/> object from memory </summary>
        /// <remarks>This cannot be cached/prefetched.</remarks>
        protected string String(int offset) => Memory.ReadDotNetString(Offset(offset));

        /// <summary> Reads a <see cref="System.Collections.Generic.List&lt;T&gt;"/> object from memory. </summary>
        /// <remarks>This cannot be cached/prefetched.</remarks>
        protected List<T> List<T>(int offset) where T : StructureBase => Memory.ReadDotNetList<T>(Offset(offset));

        //other stuff
        public T To<T>() where T : StructureBase => (T)Activator.CreateInstance(typeof(T), Memory, Addr);
    }
}
