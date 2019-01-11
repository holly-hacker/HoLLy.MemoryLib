using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HoLLy.Memory.Patterns
{
    public struct PatternByte
    {
        public byte Val;
        public bool Skip;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(byte b) => Skip || b == Val;

        private static PatternByte Normal(byte b) => new PatternByte { Val = b };
        private static PatternByte Wildcard => new PatternByte { Skip = true };

        public static PatternByte[] Parse(string pattern)
        {
            //check pattern
            if (pattern.Split(' ').Any(a => a.Length % 2 != 0)) throw new Exception("Bad pattern");

            //TODO: first split by spaces, in case user passes "FF 3 23 0"
            string noSpaces = pattern.Replace(" ", string.Empty);
            if (noSpaces.Length % 2 != 0) throw new Exception("Bad pattern");

            int byteCount = noSpaces.Length / 2;
            var arr = new PatternByte[byteCount];
            for (int i = 0; i < byteCount; i++)
                arr[i] = byte.TryParse(noSpaces.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out byte b)
                    ? Normal(b)
                    : Wildcard;

            return arr;
        }

        public static uint FirstNonWildcardByte(PatternByte[] pattern)
        {
            uint firstNonWildcard = 0;
            for (uint i = 0; i < pattern.Length; ++i)
            {
                if (pattern[i].Skip) continue;
                firstNonWildcard = i;
                break;
            }

            return firstNonWildcard;
        }

        public override string ToString() => Skip ? "??" : Val.ToString("X2");
    }
}