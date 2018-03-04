using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace System.Functional.Monads
{
    /// <summary>
    /// Static class to reduce the need for generic type annotations when using the Factory Methods <see cref="Just{T}"/> and <see cref="Nothing{T}"/>.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Creates a missing value representation for the given type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Maybe<T> Nothing<T>() => Maybe<T>.Nothing;

        /// <summary>
        /// Wraps a given value of type <see cref="T"/> into a <see cref="Maybe{TA}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Maybe<T> Just<T>(T x) => Maybe<T>.Just(x);
    }

    /// <inheritdoc />
    /// <summary>
    /// Representation for a indication of a possible missing value.
    /// Use static method <see cref="M:System.Functional.Monads.Maybe`1.Just``1(``0)" /> when there's a value present, and <see cref="P:System.Functional.Monads.Maybe`1.Nothing" /> when there isn't.
    /// </summary>
    /// <typeparam name="TA">The type of a.</typeparam>
    public class Maybe<TA> : IEquatable<Maybe<TA>>
    {
        private readonly TA _value;
        private readonly bool _isPresent;

        /// <summary>
        /// Prevents a default instance of the <see cref="Maybe{TA}"/> class from being created.
        /// </summary>
        private Maybe()
        {
            _isPresent = false;
            _value = default(TA);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Maybe{TA}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private Maybe(TA value)
        {
            _isPresent = true;
            _value = value;
        }

        /// <summary>
        /// Gets a missing value representation for the given type <see cref="TA"/>.
        /// </summary>
        /// <value>The nothing branch.</value>
        public static Maybe<TA> Nothing => new Maybe<TA>();

        /// <summary>
        /// Wraps a given value of type <see cref="T"/> into a <see cref="Maybe{TA}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Maybe<T> Just<T>(T x) => new Maybe<T>(x);

        /// <summary>
        /// Projects the wrapped <see cref="TA"/> value to a given function <paramref name="f"/> when there's a value present for this instance.
        /// </summary>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Maybe<TB> SelectMany<TB>(Func<TA, Maybe<TB>> f) => _isPresent ? f(_value) : Maybe<TB>.Nothing;

        /// <summary>
        /// Filters a <see cref="Maybe{TA}"/> based on a given predicate <paramref name="f"/>.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Maybe<TA> Where(Func<TA, bool> f) => _isPresent && f(_value) ? this : Nothing;

        /// <summary>
        /// Projects the wrapped <see cref="TA"/> value to another value <see cref="TB"/> if there's a value present by using the given function <paramref name="f"/>.
        /// </summary>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Maybe<TB> Select<TB>(Func<TA, TB> f) => _isPresent ? Maybe<TB>.Just(f(_value)) : Maybe<TB>.Nothing;

        /// <summary>
        /// Applies an aggregator function <paramref name="f"/> over this <see cref="Maybe{TA}"/> instance when there's a value present, using a initial <paramref name="seed"/> value.
        /// </summary>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="seed">The seed.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public TB Aggregate<TB>(TB seed, Func<TB, TA, TB> f) => _isPresent ? f(seed, _value) : seed;

        /// <summary>
        /// Applies a given function <paramref name="f"/> over two instances of <see cref="Maybe"/> when there's a value present for both.
        /// </summary>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Maybe<TC> Zip<TB, TC>(Maybe<TB> y, Func<TA, TB, TC> f) => 
            _isPresent && y._isPresent 
                ? Maybe<TC>.Just(f(_value, y._value)) 
                : Maybe<TC>.Nothing;

        /// <summary>
        /// Correlates the wrapped values <see cref="TA"/> and <see cref="TB"/> based on matching key selector functions <paramref name="f"/> and <paramref name="g"/>, 
        /// using a result selector <paramref name="h"/> to combine both.
        /// </summary>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public Maybe<TD> Join<TB, TC, TD>(Maybe<TB> y, Func<TA, TC> f, Func<TB, TC> g, Func<TA, TB, TD> h) =>
            _isPresent && y._isPresent && EqualityComparer<TC>.Default.Equals(f(_value), g(y._value))
                ? Maybe<TD>.Just(h(_value, y._value))
                : Maybe<TD>.Nothing;

        /// <summary>
        /// Gets the wrapped <see cref="TA"/> value when there's exists one, otherwise use the given <paramref name="otherwise"/> value.
        /// </summary>
        /// <param name="otherwise">The otherwise in case there isn't a value present.</param>
        /// <returns></returns>
        public TA GetOrElse(TA otherwise) => _isPresent ? _value : otherwise;

        /// <summary>
        /// Gets the wrapped <see cref="TA"/> value when there's exists one, otherwise use the given <paramref name="otherwise"/> value.
        /// </summary>
        /// <param name="otherwise">The otherwise in case there isn't a value present.</param>
        /// <returns></returns>
        public TA GetOrElse(Lazy<TA> otherwise) => _isPresent ? _value : otherwise.Value;

        /// <summary>
        /// Gets the wrapped <see cref="TA"/> value when there's exists one, otherwise use the given <paramref name="otherwise"/> value.
        /// </summary>
        /// <param name="otherwise">The otherwise in case there isn't a value present.</param>
        /// <returns></returns>
        public TA GetOrElse(Func<TA> otherwise) => _isPresent ? _value : otherwise();

        /// <summary>
        /// Switch to another <see cref="Maybe{TA}"/> instance when there's no value present for this instance.
        /// </summary>
        /// <param name="other">The other <see cref="Maybe{TA}"/> instance.</param>
        /// <returns></returns>
        public Maybe<TA> OrElse(Maybe<TA> other) => _isPresent ? this : other;

        /// <summary>
        /// Runs a "dead-end" function on the wrapped <see cref="TA"/> value.
        /// This method can be used to execute "side-effect" functions on the wrapped <see cref="TA"/> value.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Maybe<TA> Do(Action<TA> f)
        {
            if (_isPresent) { f(_value); }
            return this;
        }

        

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Maybe<TA> x, Maybe<TA> y)
        {
            return !(x is null) && x.Equals(y);
        }

        /// <summary>
        /// Indicates whether the current object is not equal to another object of the same type.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Maybe<TA> x, Maybe<TA> y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string typeName = typeof(TA).Name;
            return _isPresent
                ? $"Just<{typeName}>: " + _value
                : $"Nothing<{typeName}>";
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Maybe<TA> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TA>.Default.Equals(_value, other._value) && _isPresent == other._isPresent;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Maybe<TA>) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TA>.Default.GetHashCode(_value) * 397) ^ _isPresent.GetHashCode();
            }
        }
    }

    /// <summary>
    /// Extension on the <see cref="Maybe{TA}"/> type for more easier functional compisition.
    /// </summary>
    public static class MaybeExtensions
    {
        /// <summary>
        /// Wraps a given value into a <see cref="Maybe{TA}"/> instance.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Maybe<TA> AsMaybe<TA>(this TA x) => Maybe<TA>.Just(x);

        /// <summary>
        /// Only wraps a given value of type <see cref="TA"/> into a <see cref="Maybe{TA}"/> when the given <paramref name="predicate"/> holds.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Maybe<TA> ThenMaybe<TA>(this bool predicate, TA x) => predicate ? Maybe.Just(x) : Maybe<TA>.Nothing;

        /// <summary>
        /// Applies the <see cref="Maybe{TA}"/> function to a given <see cref="Maybe{TA}"/> instance.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="maybeF">The maybe f.</param>
        /// <param name="maybeX">The maybe x.</param>
        /// <returns></returns>
        public static Maybe<TB> Apply<TA, TB>(this Maybe<Func<TA, TB>> maybeF, Maybe<TA> maybeX) => maybeF.SelectMany(maybeX.Select);

        /// <summary>
        /// Lifts the given function to the world of <see cref="Maybe{TA}"/> instances, (a -> b -> c) to (M a -> M b -> M c).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="xMaybe">The x maybe.</param>
        /// <param name="yMaybe">The y maybe.</param>
        /// <returns></returns>
        public static Maybe<TC> Lift<TA, TB, TC>(this Func<TA, TB, TC> f, Maybe<TA> xMaybe, Maybe<TB> yMaybe) => 
            xMaybe.SelectMany(x => yMaybe.Select(y => f(x, y)));

        /// <summary>
        /// Lifts the given function to the world of <see cref="Maybe{TA}"/> instances, (a -> b -> c -> d) to (M a -> M b -> M c -> M d).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="xMaybe">The x maybe.</param>
        /// <param name="yMaybe">The y maybe.</param>
        /// <param name="zMaybe">The z maybe.</param>
        /// <returns></returns>
        public static Maybe<TD> Lift<TA, TB, TC, TD>(this Func<TA, TB, TC, TD> f, Maybe<TA> xMaybe, Maybe<TB> yMaybe, Maybe<TC> zMaybe) => 
            xMaybe.SelectMany(x => yMaybe.SelectMany(y => zMaybe.Select(z => f(x, y, z))));

        /// <summary>
        /// Unwraps a double wrapped <see cref="Maybe{TA}"/> to a single <see cref="Maybe{TA}"/> instance.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Maybe<TA> Flatten<TA>(this Maybe<Maybe<TA>> x) => x.GetOrElse(Maybe<TA>.Nothing);

        /// <summary>
        /// Bind composition (cross-world) functions from (a -> M b, b -> M c) to (a -> M c).
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public static Func<TA, Maybe<TC>> Bind<TA, TB, TC>(this Func<TA, Maybe<TB>> f, Func<TB, Maybe<TC>> g) => a => f(a).SelectMany(g);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MaybePreludeExtensions
    {
        public static Maybe<short> TryParseShort(this string str) => short.TryParse(str, out short sho).ThenMaybe(sho);

        public static Maybe<byte> TryParseByte(this string s) => byte.TryParse(s, out byte b).ThenMaybe(b);

        public static Maybe<int> TryParseInt(this string s) => int.TryParse(s, out int i).ThenMaybe(i);

        public static Maybe<long> TryParseLong(this string s) => long.TryParse(s, out long l).ThenMaybe(l);

        public static Maybe<decimal> TryParseDecimal(this string s) => decimal.TryParse(s, out decimal d).ThenMaybe(d);

        public static Maybe<float> TryParseFloat(this string s) => float.TryParse(s, out float f).ThenMaybe(f);

        public static Maybe<char> TryParseChar(this string s) => char.TryParse(s, out char c).ThenMaybe(c);

        public static Maybe<double> TryParseDouble(this string s) => double.TryParse(s, out double d).ThenMaybe(d);

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> xs) => xs.FirstOrDefault().AsMaybe();

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> xs, Func<T, bool> f) => xs.FirstOrDefault(f).AsMaybe();

        public static Maybe<T> LastOrNothing<T>(this IEnumerable<T> xs) => xs.LastOrDefault().AsMaybe();

        public static Maybe<T> LastOrNothing<T>(this IEnumerable<T> xs, Func<T, bool> f) => xs.LastOrDefault(f).AsMaybe(); 

        public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> xs) => xs.SingleOrDefault().AsMaybe();

        public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> xs, Func<T, bool> f) => xs.SingleOrDefault(f).AsMaybe();

        public static Maybe<T> ElementAtOrNothing<T>(this IEnumerable<T> xs, int i) => xs.ElementAtOrDefault(i).AsMaybe();

        public static IEnumerable<T> Choose<T>(this IEnumerable<T> xs, Func<T, Maybe<T>> f)
        {
            return xs.Select(f).Aggregate(
                new Collection<T>(), 
                (acc, x) => { x.Do(acc.Add); return acc; });
        }
    }
}
