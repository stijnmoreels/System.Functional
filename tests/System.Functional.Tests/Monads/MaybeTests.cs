﻿using System.Functional.Monads;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace System.Functional.Tests.Monads
{
    public class MaybeTests
    {
        [Fact]
        public bool Nothing_Is_Nothing()
        {
            // ReSharper disable once EqualExpressionComparison
            return Maybe<int>.Nothing == Maybe<int>.Nothing;
        }

        [Property]
        public bool Just_Something_Is_Just_Something(int s)
        {
            // ReSharper disable once EqualExpressionComparison
            return Maybe.Just(s) == Maybe.Just(s);
        }

        [Property]
        public bool Functor_Law_Holds(int x, Func<int, int> f, Func<int, int> g)
        {
            return Maybe.Just(x)
                .Select(f)
                .Select(g) == Maybe.Just(g(f(x)));
        }

        [Property]
        public bool Applicative_Law_Holds(Func<float, float> f, float x)
        {
            return Maybe.Just(f)
                .Apply(Maybe.Just(x)) 
                == Maybe.Just(f(x));
        }

        [DomainProperty]
        public bool Monadic_Law_Holds(byte x, Func<byte, Maybe<int>> f)
        {
            return Maybe.Just(x).SelectMany(f) == f(x);
        }

        [DomainProperty]
        public Property Where_Holds(int x, bool predication)
        {
            bool isNothing = Maybe.Just(x).Where(_ => predication) == Maybe<int>.Nothing;
            return (predication != isNothing)
                .When(x != default(int));
        }

        [DomainProperty]
        public bool SelectMany_Is_Select_Just(Maybe<int> m, Func<int, int> f)
        {
            return m.Select(f) == m.SelectMany(i => Maybe.Just(f(i)));
        }

        [DomainProperty]
        public Property GetOrElse_Lazy_Loading(int x, int y)
        {
            var lazyY = new Lazy<int>(() => y);
            return Maybe.Just(x)
                .GetOrElse(lazyY)
                .Equals(x)
                .ToProperty()
                .And(!lazyY.IsValueCreated);
        }

        [DomainProperty]
        public Property OrElse_Holds(Maybe<int> m, Maybe<int> n)
        {
            var sut = m.OrElse(n);
            return (sut == m && m != Maybe<int>.Nothing)
                .ToProperty()
                .Or(sut == n && m == Maybe<int>.Nothing);
        }

        [DomainProperty]
        public Property Aggregate_Holds(Maybe<int> maybeX, int seed, Func<int, int, int> f)
        {
            int sut = maybeX.Aggregate(seed, f);
            return sut.Equals(seed)
                .ToProperty()
                .Or(sut == f(seed, maybeX.GetOrElse(0)));
        }

        [DomainProperty]
        public Property Join_Holds(
            Maybe<char> maybeX, 
            Maybe<char> maybeY,
            Func<char, int> f,
            Func<char, char, int> h)
        {
            Maybe<int> sut = maybeX.Join(maybeY, f, f, h);
            bool isNothing = sut == Maybe<int>.Nothing;
            bool isJoined = sut == maybeX.SelectMany(x => maybeY.Select(y => h(x, y)));

            return isNothing.ToProperty()
                .Or(isJoined)
                .Classify(isNothing, "Nothing")
                .Classify(isJoined, "Joined");
        }

        [DomainProperty]
        public Property Zip_Holds(Maybe<int> maybeX, Maybe<int> maybeY, Func<int, int, int> f)
        {
            Maybe<int> sut = maybeX.Zip(maybeY, f);
            return sut.Equals(maybeX.SelectMany(x => maybeY.Select(y => f(x, y))))
                .ToProperty()
                .Or(sut.Equals(Maybe<int>.Nothing));
        }

        [Property]
        public bool Choose_Holds(NonEmptyArray<bool> xs, int value)
        {
            return xs.Get
              .Select(x => x.ThenMaybe(value))
              .Choose(x => x)
              .SequenceEqual(Enumerable.Repeat(value, xs.Get.Count(x => x)));
        }
    }
}
