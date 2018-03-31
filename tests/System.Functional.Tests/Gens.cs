using System.Functional.Monads;
using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;

namespace System.Functional.Tests
{
    public class DomainProperty : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainProperty"/> class.
        /// </summary>
        public DomainProperty()
        {
            Arbitrary = new[] {typeof(Gens)};
        }
    }

    public static class Gens
    {
        public static Arbitrary<Exception> ExceptionGen()
        {
            return Gen.Constant(new Exception()).ToArbitrary();
        }

        public static Arbitrary<Maybe<T>> MaybeGen<T>()
        {
            Gen<Maybe<T>> just = Arb.From<T>().Generator.Select(Maybe.Just);
            Gen<Maybe<T>> nothing = Gen.Constant(Maybe<T>.Nothing);

            return Gen.Frequency(
                    Tuple.Create(1, nothing), 
                    Tuple.Create(2, just))
                .ToArbitrary();
        }

        public static Arbitrary<Either<TLeft, TRight>> EitherGen<TLeft, TRight>()
        {
            Gen<Either<TLeft, TRight>> left = Arb.From<TLeft>().Generator
                .Select(Either.Left<TLeft, TRight>);

            Gen<Either<TLeft, TRight>> right = Arb.From<TRight>().Generator
                .Select(Either.Right<TLeft, TRight>);

            return Gen.OneOf(left, right).ToArbitrary();
        }

        public static Arbitrary<Task<T>> TaskGen<T>()
        {
            return Arb.From<T>()
                .Generator
                .Select(Task.FromResult)
                .ToArbitrary();
        }
    }
}
