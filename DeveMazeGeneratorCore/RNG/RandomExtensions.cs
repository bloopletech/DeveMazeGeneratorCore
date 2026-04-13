using System.Reflection;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.RNG;

public static class RandomExtensions
{
    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_impl")]
    //[return: UnsafeAccessorType("System.Random+ImplBase")]
    //private static extern ref object GetImplField(Random @this);

    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s0")]
    //private static extern ref ulong GetS0Field([UnsafeAccessorType("System.Random+XoshiroImpl")] object @this);

    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s1")]
    //private static extern ref ulong GetS1Field([UnsafeAccessorType("System.Random+XoshiroImpl")] object @this);

    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s2")]
    //private static extern ref ulong GetS2Field([UnsafeAccessorType("System.Random+XoshiroImpl")] object @this);

    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s3")]
    //private static extern ref ulong GetS3Field([UnsafeAccessorType("System.Random+XoshiroImpl")] object @this);

    //public static ulong[] GetSeed(this Random random)
    //{
    //    var fields = random.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

    //    var impl = GetImplField(random);
    //    var s0 = GetS0Field(impl!);
    //    var s1 = GetS1Field(impl);
    //    var s2 = GetS2Field(impl);
    //    var s3 = GetS3Field(impl);
    //    return [s0, s1, s2, s3];
    //}

    public static ulong[] GetSeedReflection(this Random random)
    {
        var implField = random.GetType().GetField("_impl", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var impl = implField.GetValue(random);

        var implType = impl!.GetType();
        if(implType.FullName != "System.Random+XoshiroImpl") throw new InvalidOperationException("Random does not have a XoshiroImpl");

        var seedFields = implType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var s0Field = seedFields.First(f => f.Name == "_s0");
        var s1Field = seedFields.First(f => f.Name == "_s1");
        var s2Field = seedFields.First(f => f.Name == "_s2");
        var s3Field = seedFields.First(f => f.Name == "_s3");

        var s0 = (ulong)s0Field.GetValue(impl)!;
        var s1 = (ulong)s1Field.GetValue(impl)!;
        var s2 = (ulong)s2Field.GetValue(impl)!;
        var s3 = (ulong)s3Field.GetValue(impl)!;
        return [s0, s1, s2, s3];

    }
}
