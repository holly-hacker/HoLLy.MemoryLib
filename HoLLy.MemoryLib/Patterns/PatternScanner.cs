using System;
using System.Diagnostics;
using System.Linq;

namespace HoLLy.Memory.Patterns
{
    public static class PatternScanner
    {
        private const int ScanStep = 0x1000;
        private const long MaxAddress = 0x7fffffff;

        /// <summary>
        /// Scans a memory section for a pattern, only looking in valid memory regions.
        /// </summary>
        /// <param name="abs">A <see cref="MemoryAbstraction"/> instance of the process to scan</param>
        /// <param name="pattern">The pattern array to scan for</param>
        /// <param name="result">The address, or <see cref="IntPtr.Zero"/> on failure</param>
        /// <returns>Whether the scan was successful or not</returns>
        public static bool Scan(MemoryAbstraction abs, PatternByte[] pattern, out IntPtr result)
        {
            result = IntPtr.Zero;
            var handle = abs.Handle;
            foreach (var mbi in NativeHelper.EnumerateMemoryRegions(handle).Where(a => a.State == Native.MemoryState.MemCommit))
                if (Scan(handle, pattern, new IntPtr((long)mbi.BaseAddress), (int)mbi.RegionSize, out result))
                    return true;

            return false;
        }

        /// <summary>
        /// Scans a memory section for a pattern, only looking in memory regions matching the given
        /// <paramref name="memType"/> and <paramref name="memProtect"/>.
        /// </summary>
        /// <param name="abs">A <see cref="MemoryAbstraction"/> instance of the process to scan</param>
        /// <param name="pattern">The pattern array to scan for</param>
        /// <param name="memType">The memory region type</param>
        /// <param name="memProtect">The memory region protection</param>
        /// <param name="result">The address, or <see cref="IntPtr.Zero"/> on failure</param>
        /// <returns>Whether the scan was successful or not</returns>
        public static bool Scan(MemoryAbstraction abs, PatternByte[] pattern, Native.MemoryType memType, Native.AllocationProtect memProtect, out IntPtr result)
        {
            result = IntPtr.Zero;
            var handle = abs.Handle;
            foreach (var mbi in NativeHelper.EnumerateMemoryRegions(handle, MaxAddress).Where(a =>
                a.Type == memType
                && a.Protect == memProtect
                && a.State == Native.MemoryState.MemCommit))
                if (Scan(handle, pattern, new IntPtr((long)mbi.BaseAddress), (int)mbi.RegionSize, out result))
                    return true;

            return false;
        }

        /// <summary>
        /// Scans a memory section for a pattern.
        /// </summary>
        /// <param name="handle">A handle to the process</param>
        /// <param name="pattern">The pattern array to scan for</param>
        /// <param name="baseAddress">The address to start at</param>
        /// <param name="size">The size of the region to scan</param>
        /// <param name="result">The address, or <see cref="IntPtr.Zero"/> on failure</param>
        /// <returns>Whether the scan was successful or not</returns>
        public static bool Scan(IntPtr handle, PatternByte[] pattern, IntPtr baseAddress, int size, out IntPtr result)
        {
            //TODO: ready for 64-bit
            uint step = (uint)Math.Min(size, ScanStep);
            uint min = (uint)baseAddress.ToInt32();
            uint max = (uint)(min + size);
            byte[] buffer = new byte[step + pattern.Length - 1];

            //skip wildcards, since they would always match
            uint firstNonWildcard = PatternByte.FirstNonWildcardByte(pattern);

            var sw = new Stopwatch();
            sw.Start();
            for (uint i = min; i < max; i += step)
            {
                //read buffer
                //TODO: limit to not go outside region?
                Native.ReadProcessMemory(handle, (IntPtr)i, buffer, buffer.Length, out _);

                //loop through buffer
                for (uint j = 0; j < step; ++j)
                {
                    bool match = true;

                    //loop through pattern
                    for (uint k = firstNonWildcard; k < pattern.Length; ++k)
                    {
                        if (pattern[k].Match(buffer[j + k])) continue;
                        match = false;
                        break;
                    }

                    if (match)
                    {
                        result = (IntPtr)(i + j);
                        sw.Stop();
                        Debug.WriteLine("Stopwatch sigscan: " + sw.Elapsed);
                        return true;
                    }
                }
            }

            result = IntPtr.Zero;
            return false;
        }

        [Conditional("DEBUG")]
        public static void PrintMemoryRegions(IntPtr handle)
        {
            foreach (var region in NativeHelper.EnumerateMemoryRegions(handle, MaxAddress))
                Console.WriteLine($"{region.BaseAddress:X8} - {region.BaseAddress + region.RegionSize:X8}: {region.State} / {region.Protect}");
        }
    }
}
