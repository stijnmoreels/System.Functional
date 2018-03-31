using System.Collections.Generic;

namespace System.Functional.Monads
{
    /// <summary>
    /// Static class to reduce the need for generic type annotations when using the Factory Methods <see cref="Left{TLeft, TRight}"/> and <see cref="Right{TLeft, TRight}"/>.
    /// </summary>
    public static class Either
    {
        /// <summary>
        /// Creates a new <see cref="Either{TLeft,TRight}" /> on the left side.
        /// </summary>
        /// <typeparam name="TLeft">The type of the left.</typeparam>
        /// <typeparam name="TRight">The type of the right.</typeparam>
        /// <param name="l">The l.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft l)
        {
            return Either<TLeft, TRight>.Left(l);
        }

        /// <summary>
        /// Creates a new <see cref="Either{TLeft,TRight}" /> on the right side.
        /// </summary>
        /// <typeparam name="TLeft">The type of the left.</typeparam>
        /// <typeparam name="TRight">The type of the right.</typeparam>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight r)
        {
            return Either<TLeft, TRight>.Right(r);
        }
    }

    /// <summary>
    /// Representation of either the left or right side value.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public class Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
    {
        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly bool _isLeft, _isRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="Either{TLeft, TRight}"/> class.
        /// </summary>
        /// <param name="l">The l.</param>
        private Either(TLeft l)
        {
            _isLeft = true;
            _isRight = false;

            _left = l;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Either{TLeft, TRight}"/> class.
        /// </summary>
        /// <param name="r">The r.</param>
        private Either(TRight r)
        {
            _isLeft = false;
            _isRight = true;

            _right = r;
        }

        /// <summary>
        /// Creates a new <see cref="Either{TLeft,TRight}" /> on the left side.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Left(TLeft l) => new Either<TLeft, TRight>(l);

        /// <summary>
        /// Creates a new <see cref="Either{TLeft,TRight}" /> on the right side.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Right(TRight r) => new Either<TLeft, TRight>(r);

        /// <summary>
        /// Projects the left side to another value.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TOther, TRight> SelectLeft<TOther>(Func<TLeft, TOther> f)
        {
            return _isLeft 
                ? new Either<TOther, TRight>(f(_left)) 
                : new Either<TOther, TRight>(_right);
        }

        /// <summary>
        /// Projects the right side to another value.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TLeft, TOther> SelectRight<TOther>(Func<TRight, TOther> f)
        {
            return _isRight
                ? new Either<TLeft, TOther>(f(_right)) 
                : new Either<TLeft, TOther>(_left);
        }

        /// <summary>
        /// Projects the left side using a given selector.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TOther, TRight> SelectManyLeft<TOther>(Func<TLeft, Either<TOther, TRight>> f)
        {
            return _isLeft
                ? f(_left)
                : new Either<TOther, TRight>(_right);
        }

        /// <summary>
        /// Projects the right side using a given selector.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TLeft, TOther> SelectManyRight<TOther>(Func<TRight, Either<TLeft, TOther>> f)
        {
            return _isRight
                ? f(_right)
                : new Either<TLeft, TOther>(_left);
        }

        /// <summary>
        /// Runs a 'dead-end' function on the left side value.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TLeft, TRight> DoLeft(Action<TLeft> f)
        {
            if (_isLeft) { f(_left); }
            return this;
        }

        /// <summary>
        /// Runs a 'dead-end' function on the right side value.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TLeft, TRight> DoRight(Action<TRight> f)
        {
            if (_isRight) { f(_right); }
            return this;
        }

        /// <summary>
        /// Aggregates over the left side value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="seed">The seed.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public TResult AggregateLeft<TResult>(TResult seed, Func<TResult, TLeft, TResult> f)
        {
            return _isLeft ? f(seed, _left) : seed;
        }

        /// <summary>
        /// Aggregates over the right side value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="seed">The seed.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public TResult AggregateRight<TResult>(TResult seed, Func<TResult, TRight, TResult> f)
        {
            return _isRight ? f(seed, _right) : seed;
        }

        /// <summary>
        /// Zips the left sides of both instances while discarding the right side of the other instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherLeft">The type of the other left.</typeparam>
        /// <typeparam name="TOtherRight">The type of the other right.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TResult, TRight> ZipLeft<TResult, TOtherLeft, TOtherRight>(
            Either<TOtherLeft, TOtherRight> y,
            Func<TLeft, TOtherLeft, TResult> f)
        {
            return _isLeft && y._isLeft
                ? new Either<TResult, TRight>(f(_left, y._left)) 
                : new Either<TResult, TRight>(_right);
        }

        /// <summary>
        /// Zips the right sides of both instances while discarding the left side of the other instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherLeft">The type of the other left.</typeparam>
        /// <typeparam name="TOtherRight">The type of the other right.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Either<TLeft, TResult> ZipRight<TResult, TOtherLeft, TOtherRight>(
            Either<TOtherLeft, TOtherRight> y,
            Func<TRight, TOtherRight, TResult> f)
        {
            return _isRight && y._isRight
                ? new Either<TLeft, TResult>(f(_right, y._right)) 
                : new Either<TLeft, TResult>(_left);
        }

        /// <summary>
        /// Joins the left sides of both instances if both left values are equal.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherLeft">The type of the other left.</typeparam>
        /// <typeparam name="TOtherRight">The type of the other right.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public Either<TResult, TRight> JoinLeft<TResult, TOtherLeft, TOtherRight, TProp>(
            Either<TOtherLeft, TOtherRight> y,
            Func<TLeft, TProp> f,
            Func<TOtherLeft, TProp> g,
            Func<TLeft, TOtherLeft, TResult> h)
        {
            return _isLeft && y._isLeft && EqualityComparer<TProp>.Default.Equals(f(_left), g(y._left))
                ? new Either<TResult, TRight>(h(_left, y._left))
                : new Either<TResult, TRight>(_right);
        }

        /// <summary>
        /// Joins the right sides of both instances if both right values are equal.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherLeft">The type of the other left.</typeparam>
        /// <typeparam name="TOtherRight">The type of the other right.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public Either<TLeft, TResult> JoinRight<TResult, TOtherLeft, TOtherRight, TProp>(
            Either<TOtherLeft, TOtherRight> y,
            Func<TRight, TProp> f,
            Func<TOtherRight, TProp> g,
            Func<TRight, TOtherRight, TResult> h)
        {
            return _isRight && y._isRight && EqualityComparer<TProp>.Default.Equals(f(_right), g(y._right))
                ? new Either<TLeft, TResult>(h(_right, y._right))
                : new Either<TLeft, TResult>(_left);
        }

        /// <summary>
        /// Gets the left or right side.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public TResult GetLeftOrRight<TResult>(Func<TLeft, TResult> f, Func<TRight, TResult> g)
        {
            return _isLeft ? f(_left) : g(_right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Either<TLeft, TRight> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (_isLeft && other._isLeft) return EqualityComparer<TLeft>.Default.Equals(_left, other._left);
            if (_isRight && other._isRight) return EqualityComparer<TRight>.Default.Equals(_right, other._right);

            return false;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Either<TLeft, TRight>) obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return _isLeft ? _left.GetHashCode() : _right.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Either<TLeft, TRight> x, Either<TLeft, TRight> y)
        {
            return !(x is null) && x.Equals(y);
        }

        /// <summary>
        /// Indicates whether the current object is not equal to another object of the same type.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Either<TLeft, TRight> x, Either<TLeft, TRight> y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return _isLeft
                ? $"Left<{typeof(TLeft).Name}>: " + _left
                : $"Right<{typeof(TRight).Name}>: " + _right;
        }
    }

    /// <summary>
    /// Extensions on the <see cref="Either{TLeft, TRight}"/> type for more easier functional compisition.
    /// </summary>
    public static class EitherEx
    {
        /// <summary>
        /// Applies a given left side value to the given wrapped function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TLeft">The type of the left.</typeparam>
        /// <typeparam name="TRight">The type of the right.</typeparam>
        /// <param name="eitherF">The m.</param>
        /// <param name="eitherX">The x.</param>
        /// <returns></returns>
        public static Either<TResult, TRight> ApplyLeft<TResult, TLeft, TRight>(
            this Either<Func<TLeft, TResult>, TRight> eitherF, 
            Either<TLeft, TRight> eitherX)
        {
            return eitherF.SelectManyLeft(eitherX.SelectLeft);
        }

        /// <summary>
        /// Applies a given right side value to the given wrapped function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TLeft">The type of the left.</typeparam>
        /// <typeparam name="TRight">The type of the right.</typeparam>
        /// <param name="eitherF">The either f.</param>
        /// <param name="eitherX">The either x.</param>
        /// <returns></returns>
        public static Either<TLeft, TResult> ApplyRight<TResult, TLeft, TRight>(
            this Either<TLeft, Func<TRight, TResult>> eitherF,
            Either<TLeft, TRight> eitherX)
        {
            return eitherF.SelectManyRight(eitherX.SelectRight);
        }
    }
}
