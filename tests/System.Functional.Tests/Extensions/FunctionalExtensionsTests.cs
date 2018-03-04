using FsCheck.Xunit;
using static System.FunctionalExtensions;

namespace System.Functional.Tests.Extensions
{
    public class FunctionalExtensionsTests
    {
        [Property]
        public bool Identity_Function_Holds(int x, Func<int, int> f) => Id<int>()(f(x)) == f(Id<int>()(x));

        [Property]
        public bool Const_Function_Holds(string x, int y) => x.Const<int, string>()(y) == Id<string>()(x);

        [Property]
        public bool Compose_Functions_Holds(int x, Func<int, int> f, Func<int, int> g, Func<int, int> h) => f.Compose(g).Compose(h)(x) == h(g(f(x)));
    }
}
