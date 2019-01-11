using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HoLLy.Memory
{
    public static class NativeHelper
    {
        public static bool Is64BitProcess() => IntPtr.Size == 8;
        public static bool Is64BitProcess(IntPtr handle) => Is64BitMachine() && !IsWow64Process(handle);
        public static bool Is64BitMachine() => Is64BitProcess() || IsWow64Process(Process.GetCurrentProcess().Handle);
        public static bool IsWow64Process(IntPtr handle) => Native.IsWow64Process(handle, out bool wow64) && wow64;

        public static IEnumerable<MemoryInfo> EnumerateMemoryRegions(IntPtr handle, long maxSize = int.MaxValue)
        {
            long address = 0;
            do
            {
                int result;
                MemoryInfo m;
                if (Is64BitProcess(handle)) {    // TODO: can cache this
                    result = Native.VirtualQueryEx64(handle, (IntPtr)address, out Native.MemoryBasicInformation64 m64, (uint)Marshal.SizeOf(typeof(Native.MemoryBasicInformation64)));
                    m = new MemoryInfo(m64);
                } else {
                    result = Native.VirtualQueryEx32(handle, (IntPtr)address, out Native.MemoryBasicInformation32 m32, (uint)Marshal.SizeOf(typeof(Native.MemoryBasicInformation32)));
                    m = new MemoryInfo(m32);
                }
                yield return m;
                if (address == (long)m.BaseAddress + (long)m.RegionSize)
                    break;
                address = (long)m.BaseAddress + (long)m.RegionSize;
            } while (address <= maxSize);
        }

        public class MemoryInfo
        {
            public ulong BaseAddress;
            public ulong RegionSize;
            public Native.MemoryState State;
            public Native.AllocationProtect Protect;
            public Native.MemoryType Type;

            public MemoryInfo(Native.MemoryBasicInformation32 m)
            {
                BaseAddress = (ulong)m.BaseAddress.ToInt64();
                RegionSize = (ulong)m.RegionSize.ToInt64();
                State = m.State;
                Protect = m.Protect;
                Type = m.Type;
            }

            public MemoryInfo(Native.MemoryBasicInformation64 m)
            {
                BaseAddress = m.BaseAddress;
                RegionSize = m.RegionSize;
                State = m.State;
                Protect = m.Protect;
                Type = m.Type;
            }
        }
    }
}
