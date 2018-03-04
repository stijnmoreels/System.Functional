using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class FunctionalExtensions
    {
        public static Func<TA, TA> Id<TA>() => x => x;

        public static Func<TA, TB> Const<TA, TB>(this TB x) => _ => x;

        public static Func<TA, TB> ToFunc<TA, TB>(Func<TA, TB> f) => f;

        public static Func<TA, TB, TC> ToFunc<TA, TB, TC>(Func<TA, TB, TC> f) => f;

        public static Func<TA, TA> ToFunc<TA>(this Action<TA> f) => x => { f(x); return x; };

        public static Tuple<TA, TB> TupleWith<TA, TB>(this TA x, TB y) => Tuple.Create(x, y);

        public static Tuple<TA, TB, TC> TripleWith<TA, TB, TC>(this TA x, TB y, TC z) => Tuple.Create(x, y, z);

        public static TB PipeTo<TA, TB>(this TA x, Func<TA, TB> f) => f(x);

        public static TC PipeTo<TA, TB, TC>(this Tuple<TA, TB> xy, Func<TA, TB, TC> f) => f(xy.Item1, xy.Item2);

        public static TD PipeTo<TA, TB, TC, TD>(this Tuple<TA, TB, TC> xyz, Func<TA, TB, TC, TD> f) => f(xyz.Item1, xyz.Item2, xyz.Item3);

        public static Func<TA, TC> Compose<TA, TB, TC>(this Func<TA, TB> f, Func<TB, TC> g) => x => g(f(x));

        public static Func<TA, Func<TB, TC>> Curry<TA, TB, TC>(this Func<TA, TB, TC> f) => a => b => f(a, b);

        public static Func<TA, Func<TB, Func<TC, TD>>> Curry<TA, TB, TC, TD>(this Func<TA, TB, TC, TD> f) => a => b => c => f(a, b, c);

        public static TA Do<TA>(this TA x, Action<TA> f)
        {
            f(x);
            return x;
        }

        public static TB Apply<TA, TB>(this Func<TA, TB> f, TA x) => f(x);

        public static Func<TB, TC> Apply<TA, TB, TC>(this Func<TA, TB, TC> f, TA x) => y => f(x, y);

        public static Func<TB, TA, TC> Flip<TA, TB, TC>(this Func<TA, TB, TC> f) => (b, a) => f(a, b);

        public static Tuple<TB, TA> Flip<TA, TB>(this Tuple<TA, TB> xy) => Tuple.Create(xy.Item2, xy.Item1);

        public static TA IfNullThen<TA>(this TA x, TA y) => x == null ? y : x;

        public static TB IfThenElse<TA, TB>(this TA x, Func<TA, bool> predicate, Func<TA, TB> f, Func<TA, TB> g) => predicate(x) ? f(x) : g(x);

        public static IEnumerable<TA> ForEach<TA>(this IEnumerable<TA> xs, Action<TA> f)
        {
            foreach (TA x in xs) { f(x); }
            return xs;
        }

        public static IEnumerable<TA> For<TA>(this IEnumerable<TA> xs, Action<int, TA> f)
        {
            var i = 0;
            return xs.ForEach(x => f(i++, x));
        }

        public static TB Use<TA, TB>(this TA x, Func<TA, TB> f) where TA : IDisposable { using (x) return f(x); }

        public static TB Try<TA, TB, TEx>(this TA x, Func<TA, TB> f, Func<TEx, TB> g) where TEx : Exception
        {
            try
            {
                return f(x);
            }
            catch (TEx ex)
            {
                return g(ex);
            }
        }

        public static TB TryFinally<TA, TB, TEx>(this TA x, Func<TA, TB> f, Func<TEx, TB> g, Action<TA> h) where TEx : Exception
        {
            try { return f(x); }
            catch (TEx ex) { return g(ex); }
            finally { h(x); }
        }

        public static TA WhileDo<TA>(this TA x, Func<TA, bool> predicate, Action<TA> f)
        {
            while (predicate(x)) { f(x); }
            return x;
        }

        public static IEnumerable<TB> WhilePipeTo<TA, TB>(this TA x, Func<TA, bool> predicate, Func<TA, TB> f)
        {
            while (predicate(x)) { yield return f(x); }
        }

        public static TA DoWhile<TA>(this TA x, Func<TA, bool> predicate, Action<TA> f)
        {
            do { f(x); } while (predicate(x));
            return x;
        }

        public static IEnumerable<TB> DoPipeTo<TA, TB>(this TA x, Func<TA, bool> predicate, Func<TA, TB> f)
        {
            do { yield return f(x); } while (predicate(x));
        }
    }
}
