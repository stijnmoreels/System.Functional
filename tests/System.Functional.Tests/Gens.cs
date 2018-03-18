using System.Functional.Monads;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.FSharp.Core;

namespace System.Functional.Tests
{
    public class CustomProperty : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomProperty"/> class.
        /// </summary>
        public CustomProperty()
        {
            Arbitrary = new[] {typeof(Gens)};
        }
    }

    public static class Gens
    {
        public static Arbitrary<Exception> ExceptionGen() => Gen.Constant(new Exception()).ToArbitrary();

        public static Arbitrary<Maybe<T>> MaybeGen<T>()
        {
            Gen<Maybe<T>> just = Arb.From<T>().Generator.Select(Maybe.Just);
            Gen<Maybe<T>> nothing = Gen.Constant(Maybe<T>.Nothing);

            return Gen.Frequency(Tuple.Create(1, nothing), Tuple.Create(2, just)).ToArbitrary();
        }
    }
}
