using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace DeveMazeGeneratorCore.RNG;

// Based on https://raw.githubusercontent.com/dotnet/runtime/refs/tags/v10.0.105/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro256StarStarImpl.cs
// and https://raw.githubusercontent.com/dotnet/runtime/refs/tags/v10.0.105/src/libraries/System.Private.CoreLib/src/System/Random.ImplBase.cs
// Provides an implementation of the xoshiro256** algorithm.
public class Xoshiro256Random : ISeededRandom<Xoshiro256RandomSeed>
{
    // NextUInt64 is based on the algorithm from http://prng.di.unimi.it/xoshiro256starstar.c:
    //
    //     Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)
    //
    //     To the extent possible under law, the author has dedicated all copyright
    //     and related and neighboring rights to this software to the public domain
    //     worldwide. This software is distributed without any warranty.
    //
    //     See <http://creativecommons.org/publicdomain/zero/1.0/>.

    public Xoshiro256RandomSeed Seed { get; }

    private ulong _s0, _s1, _s2, _s3;

    public Xoshiro256Random()
    {
        var bytes = new byte[4 * sizeof(ulong)];
        do
        {

            RandomNumberGenerator.Fill(bytes);
            _s0 = BitConverter.ToUInt64(bytes, 0 * sizeof(ulong));
            _s1 = BitConverter.ToUInt64(bytes, 1 * sizeof(ulong));
            _s2 = BitConverter.ToUInt64(bytes, 2 * sizeof(ulong));
            _s3 = BitConverter.ToUInt64(bytes, 3 * sizeof(ulong));
        }
        while((_s0 | _s1 | _s2 | _s3) == 0); // at least one value must be non-zero
        Seed = new(_s0, _s1, _s2, _s3);
    }

    public Xoshiro256Random(Xoshiro256RandomSeed seed)
    {
        (_s0, _s1, _s2, _s3) = seed;
        Seed = seed;
    }

    public int Next()
    {
        while(true)
        {
            // Get top 31 bits to get a value in the range [0, int.MaxValue], but try again
            // if the value is actually int.MaxValue, as the method is defined to return a value
            // in the range [0, int.MaxValue).
            ulong result = NextUInt64() >> 33;
            if(result != int.MaxValue)
            {
                return (int)result;
            }
        }
    }

    public int Next(int maxValue)
    {
        Debug.Assert(maxValue >= 0);

        return (int)NextUInt32((uint)maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        Debug.Assert(minValue <= maxValue);

        return (int)NextUInt32((uint)(maxValue - minValue)) + minValue;
    }

    //public long NextInt64()
    //{
    //    while(true)
    //    {
    //        // Get top 63 bits to get a value in the range [0, long.MaxValue], but try again
    //        // if the value is actually long.MaxValue, as the method is defined to return a value
    //        // in the range [0, long.MaxValue).
    //        ulong result = NextUInt64() >> 1;
    //        if(result != long.MaxValue)
    //        {
    //            return (long)result;
    //        }
    //    }
    //}

    //public long NextInt64(long maxValue)
    //{
    //    Debug.Assert(maxValue >= 0);

    //    return (long)NextUInt64((ulong)maxValue);
    //}

    //public long NextInt64(long minValue, long maxValue)
    //{
    //    Debug.Assert(minValue <= maxValue);

    //    return (long)NextUInt64((ulong)(maxValue - minValue)) + minValue;
    //}

    public double NextDouble() =>
        // As described in http://prng.di.unimi.it/:
        // "A standard double (64-bit) floating-point number in IEEE floating point format has 52 bits of significand,
        //  plus an implicit bit at the left of the significand. Thus, the representation can actually store numbers with
        //  53 significant binary digits. Because of this fact, in C99 a 64-bit unsigned integer x should be converted to
        //  a 64-bit double using the expression
        //  (x >> 11) * 0x1.0p-53"
        (NextUInt64() >> 11) * (1.0 / (1ul << 53));

    /// <summary>Produces a value in the range [0, uint.MaxValue].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // small-ish hot path used by very few call sites
    private uint NextUInt32() => (uint)(NextUInt64() >> 32);

    /// <summary>Produces a value in the range [0, ulong.MaxValue].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // small-ish hot path used by a handful of "next" methods
    private ulong NextUInt64()
    {
        ulong s0 = _s0, s1 = _s1, s2 = _s2, s3 = _s3;

        ulong result = BitOperations.RotateLeft(s1 * 5, 7) * 9;
        ulong t = s1 << 17;

        s2 ^= s0;
        s3 ^= s1;
        s1 ^= s2;
        s0 ^= s3;

        s2 ^= t;
        s3 = BitOperations.RotateLeft(s3, 45);

        _s0 = s0;
        _s1 = s1;
        _s2 = s2;
        _s3 = s3;

        return result;
    }

    // NextUInt32 algorithm based on https://arxiv.org/pdf/1805.10941.pdf and https://github.com/lemire/fastrange.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint NextUInt32(uint maxValue)
    {
        ulong randomProduct = (ulong)maxValue * NextUInt32();
        uint lowPart = (uint)randomProduct;

        if(lowPart < maxValue)
        {
            uint remainder = (0u - maxValue) % maxValue;

            while(lowPart < remainder)
            {
                randomProduct = (ulong)maxValue * NextUInt32();
                lowPart = (uint)randomProduct;
            }
        }

        return (uint)(randomProduct >> 32);
    }

    //// NextUInt64 algorithm based on https://arxiv.org/pdf/1805.10941.pdf and https://github.com/lemire/fastrange.
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private ulong NextUInt64(ulong maxValue)
    //{
    //    ulong randomProduct = Math.BigMul(maxValue, NextUInt64(), out ulong lowPart);

    //    if(lowPart < maxValue)
    //    {
    //        ulong remainder = (0ul - maxValue) % maxValue;

    //        while(lowPart < remainder)
    //        {
    //            randomProduct = Math.BigMul(maxValue, NextUInt64(), out lowPart);
    //        }
    //    }

    //    return randomProduct;
    //}
}