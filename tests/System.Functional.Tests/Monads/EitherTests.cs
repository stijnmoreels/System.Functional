using System.Functional.Monads;
using FsCheck.Xunit;

namespace System.Functional.Tests.Monads
{
    public class EitherTests
    {
        [Property]
        public bool Left_Is_Left(int x)
        {
            // ReSharper disable once EqualExpressionComparison
            return Either.Left<int, string>(x) == Either.Left<int, string>(x);
        }

        [Property]
        public bool Right_Is_Right(string x)
        {
            // ReSharper disable once EqualExpressionComparison
            return Either.Right<int, string>(x) == Either.Right<int, string>(x);
        }

        [Property]
        public bool Functor_Law_Holds(int x, Func<int, string> f, Func<string, int> g)
        {
            return Either.Left<int, byte>(x)
                .SelectLeft(f)
                .SelectLeft(g)
                .GetLeftOrRight(_ => _, _ => _) == g(f(x));
        }

        [Property]
        public bool Applicative_Law_Holds(Func<byte, byte> f, byte x)
        {
            return Either.Right<byte, Func<byte, byte>>(f)
                .ApplyRight(Either<byte, byte>.Right(x))
                .GetLeftOrRight(_ => _, _ => _) == f(x);
        }

        [DomainProperty]
        public bool Monadic_Law_Holds(byte x, Func<byte, Either<int, string>> f)
        {
            return Either.Left<byte, string>(x).SelectManyLeft(f) == f(x);
        }
    }
}
