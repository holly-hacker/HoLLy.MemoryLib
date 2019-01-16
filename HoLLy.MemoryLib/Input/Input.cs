using System.Runtime.InteropServices;

namespace HoLLy.Memory.Input
{
    public static class Input
    {
        // TODO: look into timed events

        private static readonly int StructSize = Marshal.SizeOf(typeof(InputStruct));   // should be 40, probably

        public static void KeyDown(VirtualKeyCode keyCode)
        {
            var input = new InputStruct {
                Type = 1,
                Data = new MixedInput {
                    Keyboard = new KeyboardInput {
                        KeyCode = (ushort) keyCode
                    }
                }
            };

            Native.SendInput(1, new[] {input}, StructSize);
        }

        public static void KeyUp(VirtualKeyCode keyCode)
        {
            var input = new InputStruct {
                Type = 1,
                Data = new MixedInput {
                    Keyboard = new KeyboardInput {
                        KeyCode = (ushort)keyCode,
                        Flags = 0x2
                    }
                }
            };

            Native.SendInput(1, new[] {input}, StructSize);
        }
    }
}
