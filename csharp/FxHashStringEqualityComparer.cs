using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

internal class FxHashStringEqualityComparer : IEqualityComparer<string>
{
    private readonly static nuint s_seed = IntPtr.Size == 4 ? 0x9e3779b9 : unchecked((nuint)0x517cc1b727220a95);

    public bool Equals(string? x, string? y) => EqualityComparer<string>.Default.Equals(x, y);

    public int GetHashCode([DisallowNull] string obj)
    {
        ReadOnlySpan<nuint> bytesAsNuint = MemoryMarshal.Cast<char, nuint>(obj);

        nuint hash = 0;
        for (int i = 0; i < bytesAsNuint.Length; i++)
        {
            hash = BitOperations.RotateLeft(hash, 5) ^ bytesAsNuint[i] * s_seed;
        }

        if (obj.Length % (IntPtr.Size / sizeof(char)) >= 2)
        {
            ReadOnlySpan<int> bytesAsInt = MemoryMarshal.Cast<char, int>(obj);
            hash = BitOperations.RotateLeft(hash, 5) ^ (nuint)bytesAsInt[bytesAsInt.Length - 1] * s_seed;
        }

        if (obj.Length % (sizeof(int) / sizeof(char)) >= 1)
        {
            ReadOnlySpan<short> bytesAsShort = MemoryMarshal.Cast<char, short>(obj);
            hash = BitOperations.RotateLeft(hash, 5) ^ (nuint)bytesAsShort[bytesAsShort.Length - 1] * s_seed;
        }

        return (int)hash;
    }
}
