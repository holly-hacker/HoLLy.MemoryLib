using System;
using System.Runtime.InteropServices;

namespace HoLLy.Memory
{
    internal static class Native
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryBasicInformation lpBuffer, uint dwLength);

        //TODO: assuming a 64 bit process!
        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryBasicInformation
        {
#if x86
            //for 32-bit osu!
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public AllocationProtect AllocationProtect;
            public IntPtr RegionSize;
            public MemoryState State;
            public AllocationProtect Protect;
            public MemoryType Type;
#else
            //for 64-bit osu!
            public ulong BaseAddress;
            public ulong AllocationBase;

            /// <summary>
            /// The memory protection option when the region was initially allocated. This member is 0 if the caller 
            /// does not have access.
            /// </summary>
            public AllocationProtect AllocationProtect;
            private int _alignment1;
            public ulong RegionSize;

            /// <summary> The state of the pages in the region. </summary>
            public MemoryState State;

            /// <summary> The access protection of the pages in the region. </summary>
            public AllocationProtect Protect;

            /// <summary> The type of pages in the region. </summary>
            public MemoryType Type;
            private int _alignment2;
#endif
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            Terminate = 0x0001,
            CreateThread = 0x0002,
            VirtualMemoryOperation = 0x0008,
            VirtualMemoryRead = 0x0010,
            VirtualMemoryWrite = 0x0020,
            DuplicateHandle = 0x0040,
            CreateProcess = 0x0080,
            SetQuota = 0x0100,
            SetInformation = 0x0200,
            QueryInformation = 0x0400,
            QueryLimitedInformation = 0x1000,

            Synchronize = 0x00100000,
            //All = 0x001F_0FFF,
            All = Terminate | CreateThread | 0x0004 | VirtualMemoryOperation
                | VirtualMemoryRead | VirtualMemoryWrite | DuplicateHandle | CreateProcess
                | SetQuota | SetInformation | QueryInformation | 0x0800
                | 0x001F_0000
        }

        [Flags]
        public enum AllocationProtect : uint
        {
            PageNoAccess = 0x00000001,
            PageReadonly = 0x00000002,
            PageReadWrite = 0x00000004,
            PageWriteCopy = 0x00000008,
            PageExecute = 0x00000010,
            PageExecuteRead = 0x00000020,
            PageExecuteReadWrite = 0x00000040,
            PageExecuteWriteCopy = 0x00000080,
            PageGuard = 0x00000100,
            PageNoCache = 0x00000200,
            PageWriteCombine = 0x00000400
        }

        [Flags]
        public enum MemoryState : uint
        {
            /// <summary>
            /// Indicates committed pages for which physical storage has been allocated, either in memory or in the 
            /// paging file on disk.
            /// </summary>
            MemCommit = 0x1000,

            /// <summary>
            /// Indicates reserved pages where a range of the process's virtual address space is reserved without any 
            /// physical storage being allocated. For reserved pages, the information in the 
            /// <see cref="MemoryBasicInformation.Protect"/> member is undefined.
            /// </summary>
            MemReserved = 0x2000,

            /// <summary>
            /// Indicates free pages not accessible to the calling process and available to be allocated. For free 
            /// pages, the information in the <see cref="MemoryBasicInformation.AllocationBase"/>, 
            /// <see cref="MemoryBasicInformation.AllocationProtect"/>, <see cref="MemoryBasicInformation.Protect"/>, 
            /// and <see cref="MemoryBasicInformation.Type"/> members is undefined.
            /// </summary>
            MemFree = 0x10000,
        }

        [Flags]
        public enum MemoryType : uint
        {
            /// <summary> Indicates that the memory pages within the region are private (that is, not shared by other processes). </summary>
            MemPrivate = 0x20000,

            /// <summary> Indicates that the memory pages within the region are mapped into the view of a section. </summary>
            MemMapped = 0x40000,

            /// <summary> Indicates that the memory pages within the region are mapped into the view of an image section. </summary>
            MemImage = 0x1000000,
        }
    }
}
