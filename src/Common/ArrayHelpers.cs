using System.Collections.Generic;

namespace Common
{
    public static class ArrayUtils
    {
        public static uint ParseuInt(this IList<byte> data, int offest = 0)
        {
            return (uint)(data[offest++] + data[offest++] * 256 + data[offest++] * 65536 + data[offest] * 16777216);
        }
    }
}
