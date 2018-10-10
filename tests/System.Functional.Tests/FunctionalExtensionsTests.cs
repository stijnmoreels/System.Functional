using System.Functional.Monads;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using static System.Functional.FunctionalEx;

namespace System.Functional.Tests
{
    public class FunctionalExtensionsTests
    {
        [Property]
        public bool Identity_Function_Holds(int x, Func<int, int> f) => Id<int>()(f(x)) == f(Id<int>()(x));

        [Property]
        public bool Const_Function_Holds(NonNull<string> x, int y) => x.Get.Const<int, string>()(y) == Id<string>()(x.Get);

        [Property]
        public bool Compose_Functions_Holds(int x, Func<int, int> f, Func<int, int> g, Func<int, int> h) =>
            f.Compose(g).Compose(h)(x) == x.PipeTo(f).PipeTo(g).PipeTo(h);

        [Property]
        public bool Double_Flip_Tuple_Result_In_Original(Tuple<int, string> t) => t.Flip().Flip().Equals(t);

        [Property]
        public bool Apply_Same_As_PipeTo(int x, Func<int, string> f) => f.Apply(x) == x.PipeTo(f);

        [Property]
        public bool Curry_Same_As_Apply(byte x, int y, NonNull<string> z, Func<byte, int, string, int> f) =>
            f.Curry()(x)(y)(z.Get) == f.Apply(x).Apply(y).Apply(z.Get);

        [Property]
        public Property Null_Check_If_Null_Then(string x, NonNull<string> y)
        {
            string sut = x.IfNullThen(y.Get);
            return (x == null && sut == y.Get).ToProperty().Or(x != null && sut == x);
        }

        public Property Try_Returns_Expected_When_Throws(
            int x, 
            Func<int, int> f, 
            Maybe<Exception> maybeEx, 
            Func<Exception, int> g)
        {
            int result = x.Try(y => { maybeEx.Do(ex => throw ex); return f(y); }, g);

            bool noThrowal = result == f(x) && maybeEx == Maybe<Exception>.Nothing;
            bool doThrowel = result == maybeEx.GetOrElse((Exception) null).PipeTo(g);
            return noThrowal.ToProperty().Or(doThrowel);
        }

        [Property]
        public bool While_PipeTo_Works_With_Enumerator(int[] xs)
        {
            return xs.GetEnumerator()
                .WhilePipeTo(e => e.MoveNext(), e => e.Current)
                .Cast<int>()
                .SequenceEqual(xs);
        }
    }
}
