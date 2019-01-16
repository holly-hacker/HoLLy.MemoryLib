using System;
using System.Runtime.InteropServices;
#pragma warning disable 649

namespace HoLLy.Memory.Input
{
    internal struct InputStruct
    {
        public uint Type;
        public MixedInput Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct MixedInput
    {
        [FieldOffset(0)]
        public MouseInput Mouse;

        [FieldOffset(0)]
        public KeyboardInput Keyboard;
    }

    internal struct MouseInput
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    internal struct KeyboardInput
    {
        public ushort KeyCode;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
}
