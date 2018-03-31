using System.Collections.Generic;
using System.Functional.Monads;

namespace System.Functional
{
    /// <summary>
    /// Functional Extensions for the overall development environment.
    /// </summary>
    public static class FunctionalEx
    {
        /// <summary>
        /// Identity function.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <returns></returns>
        public static Func<TA, TA> Id<TA>() => x => x;

        /// <summary>
        /// Constant function that returns always <paramref name="x"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Func<TA, TB> Const<TA, TB>(this TB x) => _ => x;

        /// <summary>
        /// Converts a C# method to a <see cref="Func{TA,TB}"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Func<TA, TB> ToFunc<TA, TB>(Func<TA, TB> f) => f;

        /// <summary>
        /// Converts a C# method to a <see cref="Func{TA, TB, TC}"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Func<TA, TB, TC> ToFunc<TA, TB, TC>(Func<TA, TB, TC> f) => f;

        /// <summary>
        /// Converts a void C# method to a <see cref="Func{TA, TA}"/>.
        /// Makes an Endomorphism of a dead-end function.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Func<TA, TA> ToFunc<TA>(this Action<TA> f) => x => { f(x); return x; };

        /// <summary>
        /// Combines two values into a <see cref="Tuple{TA, TB}"/>
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static Tuple<TA, TB> TupleWith<TA, TB>(this TA x, TB y) => Tuple.Create(x, y);

        /// <summary>
        /// Combines three values into a triple, <see cref="Tuple{TA, TB, TC}"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        public static Tuple<TA, TB, TC> TripleWith<TA, TB, TC>(this TA x, TB y, TC z) => Tuple.Create(x, y, z);

        /// <summary>
        /// Pipes a given <paramref name="x"/> to a given function: <paramref name="f"/>
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TB PipeTo<TA, TB>(this TA x, Func<TA, TB> f) => f(x);

        /// <summary>
        /// Pipes two values <paramref name="xy"/> to a given function: <paramref name="f"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="xy">The xy.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TC PipeTo<TA, TB, TC>(this Tuple<TA, TB> xy, Func<TA, TB, TC> f) => f(xy.Item1, xy.Item2);

        /// <summary>
        /// Pipes three values <paramref name="xyz"/> to a given function: <paramref name="f"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="xyz">The xyz.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TD PipeTo<TA, TB, TC, TD>(this Tuple<TA, TB, TC> xyz, Func<TA, TB, TC, TD> f) => f(xyz.Item1, xyz.Item2, xyz.Item3);

        /// <summary>
        /// Composes two functions together into a single function (f >> g).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public static Func<TA, TC> Compose<TA, TB, TC>(this Func<TA, TB> f, Func<TB, TC> g) => x => g(f(x));

        /// <summary>
        /// Makes a curried function out of a given function: <paramref name="f"/>.
        /// This way arguments can be passed in one at the time.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Func<TA, Func<TB, TC>> Curry<TA, TB, TC>(this Func<TA, TB, TC> f) => a => b => f(a, b);

        /// <summary>
        /// Makes a curried function out of a given function: <paramref name="f"/>.
        /// This way arguments can be passed in one at the time.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Func<TA, Func<TB, Func<TC, TD>>> Curry<TA, TB, TC, TD>(this Func<TA, TB, TC, TD> f) => a => b => c => f(a, b, c);

        /// <summary>
        /// Runs some 'dead-end' function on a given value: <paramref name="x"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TA Do<TA>(this TA x, Action<TA> f)
        {
            f(x);
            return x;
        }

        /// <summary>
        /// Applies the specified function <paramref name="f"/> to a value <paramref name="x"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static TB Apply<TA, TB>(this Func<TA, TB> f, TA x) => f(x);

        /// <summary>
        /// Applies the specified function <paramref name="f"/> to a value <paramref name="x"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Func<TB, TC> Apply<TA, TB, TC>(this Func<TA, TB, TC> f, TA x) => y => f(x, y);

        /// <summary>
        /// Applies the specified function <paramref name="f"/> to a value <paramref name="x"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Func<TB, TC, TD> Apply<TA, TB, TC, TD>(this Func<TA, TB, TC, TD> f, TA x) => (y, z) => f(x, y, z);

        /// <summary>
        /// Reverse the input types from a given function.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Func<TB, TA, TC> Flip<TA, TB, TC>(this Func<TA, TB, TC> f) => (b, a) => f(a, b);

        /// <summary>
        /// Reverse the values from a given <see cref="Tuple{TA, TB}"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="xy">The xy.</param>
        /// <returns></returns>
        public static Tuple<TB, TA> Flip<TA, TB>(this Tuple<TA, TB> xy) => Tuple.Create(xy.Item2, xy.Item1);

        /// <summary>
        /// Gets a default/other value if the given value <paramref name="x"/> is 'null'.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static TA IfNullThen<TA>(this TA x, TA y) => x == null ? y : x;

        /// <summary>
        /// Runs a 'dead-end' function on a value <paramref name="x"/> when it's not 'null'.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TA IfNotNullThen<TA>(this TA x, Action<TA> f) => x != null ? x.Do(f) : default(TA);

        /// <summary>
        /// Short handed if/else statement as a function.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public static TB IfThenElse<TA, TB>(this TA x, Func<TA, bool> predicate, Func<TA, TB> f, Func<TA, TB> g) => predicate(x) ? f(x) : g(x);

        /// <summary>
        /// Runs a 'dead-end' function <paramref name="f"/> on a list of elements <paramref name="xs"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="xs">The xs.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static IEnumerable<TA> ForEach<TA>(this IEnumerable<TA> xs, Action<TA> f)
        {
            foreach (TA x in xs) { f(x); }
            return xs;
        }

        /// <summary>
        /// Runs a 'dead-end' function <paramref name="f"/> with the index on a list of elements <paramref name="xs"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="xs">The xs.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static IEnumerable<TA> For<TA>(this IEnumerable<TA> xs, Action<int, TA> f)
        {
            var i = 0;
            return xs.ForEach(x => f(i++, x));
        }

        /// <summary>
        /// Runs a function: <paramref name="f"/> before disposing the given value: <paramref name="x"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TB Use<TA, TB>(this TA x, Func<TA, TB> f) where TA : IDisposable { using (x) return f(x); }

        /// <summary>
        /// Try to run a function: <paramref name="f" /> on a given value: <paramref name="x" />.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Maybe<TB> Try<TA, TB>(this TA x, Func<TA, TB> f)
        {
            try
            {
                return Maybe.Just(f(x));
            }
            catch
            {
                return Maybe<TB>.Nothing;
            }
        }

        /// <summary>
        /// Try to run a function: <paramref name="f" /> on a given value: <paramref name="x" />.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TEx">The type of the ex.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Either<TB, TEx> Try<TA, TB, TEx>(this TA x, Func<TA, TB> f) where TEx : Exception
        {
            try
            {
                return Either.Left<TB, TEx>(f(x));
            }
            catch (TEx ex)
            {
                return Either.Right<TB, TEx>(ex);
            }
        }

        /// <summary>
        /// Try to run a function: <paramref name="f"/> on a given value: <paramref name="x"/>,
        /// but catch the exception with the given function: <paramref name="g"/> if something goes wrong.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TEx">The type of the ex.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public static TB Try<TA, TB, TEx>(this TA x, Func<TA, TB> f, Func<TEx, TB> g) where TEx : Exception
        {
            try
            { return f(x); }
            catch (TEx ex)
            { return g(ex); }
        }

        /// <summary>
        /// Try to run a function: <paramref name="f"/> on a given value: <paramref name="x"/>,
        /// but catch the exception with the given function: <paramref name="g"/> if something goes wrong.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TEx">The type of the ex.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public static TB Try<TA, TB, TEx>(this TA x, Func<TA, TB> f, Func<TEx, TA, TB> g) where TEx : Exception
        {
            try
            { return f(x); }
            catch (TEx ex)
            { return g(ex, x); }
        }

        /// <summary>
        /// Try to run a function: <paramref name="f"/> on a given value: <paramref name="x"/>,
        /// but catch the exception with the given function: <paramref name="g"/> if something goes wrong.
        /// Afterwards, always run the function: <paramref name="h"/> in the 'finally' block.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TEx">The type of the ex.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public static TB TryFinally<TA, TB, TEx>(this TA x, Func<TA, TB> f, Func<TEx, TB> g, Action<TA> h) where TEx : Exception
        {
            try { return f(x); }
            catch (TEx ex) { return g(ex); }
            finally { h(x); }
        }

        /// <summary>
        /// Runs a given 'dead-end' function: <paramref name="f"/> while the given <paramref name="predicate"/> is correct (while { ... }).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TA WhileDo<TA>(this TA x, Func<TA, bool> predicate, Action<TA> f)
        {
            while (predicate(x)) { f(x); }
            return x;
        }

        /// <summary>
        /// Runs a given function: <paramref name="f"/> while the given <paramref name="predicate"/> is correct (while { ... }).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static IEnumerable<TB> WhilePipeTo<TA, TB>(this TA x, Func<TA, bool> predicate, Func<TA, TB> f)
        {
            while (predicate(x)) { yield return f(x); }
        }

        /// <summary>
        /// Runs a given 'dead-end' function: <paramref name="f"/> while the given <paramref name="predicate"/> is correct.
        /// But runs the function: <paramref name="f"/> once (do { ... } while).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static TA DoWhile<TA>(this TA x, Func<TA, bool> predicate, Action<TA> f)
        {
            do { f(x); } while (predicate(x));
            return x;
        }

        /// <summary>
        /// Runs a given function: <paramref name="f"/> while the given <paramref name="predicate"/> is correct.
        /// But runs the function: <paramref name="f"/> once (do { ... } while).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static IEnumerable<TB> DoPipeTo<TA, TB>(this TA x, Func<TA, bool> predicate, Func<TA, TB> f)
        {
            do { yield return f(x); } while (predicate(x));
        }
    }
}
