using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Functional
{
    /// <summary>
    /// Functional Extensions on the <see cref="Task"/> class.
    /// </summary>
    public static class TaskEx
    {
        /// <summary>
        /// Awaits the result of the given <see cref="Task{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static T AwaitResult<T>(this Task<T> t) => t.GetAwaiter().GetResult();

        /// <summary>
        /// Continue with the specified constant <paramref name="value"/> when the given <paramref name="task"/> is completed.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="value">The constant value.</param>
        /// <returns></returns>
        public static Task<TA> Const<TA>(this Task task, TA value)
        {
            return task.ContinueWith(t => value);
        }

        /// <summary>
        /// Run some 'dead-end' function on the <see cref="Task"/> wraped value.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static Task<TA> Do<TA>(this Task<TA> source, Action<TA> action)
        {
            return source.SelectMany(
                a =>
                {
                    action(a);
                    return source;
                });
        }

        /// <summary>
        /// Applies the specified <paramref name="source"/> <see cref="Task"/> to a function.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="taskFunc">The task function.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Task<TB> Apply<TA, TB>(this Task<Func<TA, TB>> taskFunc, Task<TA> source)
        {
            return source.SelectMany(a => taskFunc.Select(func => func(a)));
        }

        /// <summary>
        /// Applies the specified <paramref name="source"/> <see cref="Task"/> to a function.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="taskFunc">The task function.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Task<Func<TB, TC>> Apply<TA, TB, TC>(this Task<Func<TA, TB, TC>> taskFunc, Task<TA> source)
        {
            return source.SelectMany(a => taskFunc.Select<Func<TA, TB, TC>, Func<TB, TC>>(func => b => func(a, b)));
        }

        /// <summary>
        /// Applies the specified source.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="taskFunc">The task function.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Task<Func<TB, TC, TD>> Apply<TA, TB, TC, TD>(
            this Task<Func<TA, TB, TC, TD>> taskFunc,
            Task<TA> source)
        {
            return source.SelectMany(
                a => taskFunc.Select<Func<TA, TB, TC, TD>, Func<TB, TC, TD>>(func => (b, c) => func(a, b, c)));
        }

        /// <summary>
        /// Lifts the specified f.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR">The type of the r.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Task<TR> Lift<T, TR>(Func<T, TR> f, Task<T> t)
        {
            return t.Select(f);
        }

        /// <summary>
        /// Lift2s the specified selector.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="TR">The type of the r.</typeparam>
        /// <param name="f">The selector.</param>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        public static Task<TR> Lift<T1, T2, TR>(Func<T1, T2, TR> f, Task<T1> t1, Task<T2> t2)
        {
            return f.Curry().FromResult().Apply(t1).Apply(t2);
        }

        /// <summary>
        /// Lift3s the specified selector.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="TR">The type of the r.</typeparam>
        /// <param name="f">The selector.</param>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <param name="t3">The t3.</param>
        /// <returns></returns>
        public static Task<TR> Lift<T1, T2, T3, TR>(
            Func<T1, T2, T3, TR> f,
            Task<T1> t1,
            Task<T2> t2,
            Task<T3> t3)
        {
            return f.Curry().FromResult().Apply(t1).Apply(t2).Apply(t3);
        }

        /// <summary>
        /// Binds the result of the completed <paramref name="source"/> <see cref="Task"/> to given <paramref name="binder"/> function.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="binder">The binder.</param>
        /// <returns></returns>
        public static Task<TB> SelectMany<TA, TB>(this Task<TA> source, Func<TA, Task<TB>> binder)
        {
            return source.ContinueWith(xTask => binder(xTask.Result)).Unwrap();
        }

        /// <summary>
        /// Maps the wrapped value inside a <see cref="Task"/> to another value using the given <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static Task<TB> Select<TA, TB>(this Task<TA> source, Func<TA, TB> selector)
        {
            return source.SelectMany(x => selector(x).FromResult());
        }

        /// <summary>
        /// Filters the given <paramref name="source"/> <see cref="Task"/> with a <paramref name="predicate"/> on the wrapped value.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <remarks>The given source will be cancelled when the predicate doesn't hold.</remarks>
        /// <returns></returns>
        public static Task<TA> Where<TA>(this Task<TA> source, Func<TA, bool> predicate)
        {
            return source.SelectMany(
                x => predicate(x)
                         ? Task.FromResult(x)
                         : throw new OperationCanceledException("Value doesn't pass predicate: " + x));
        }

        /// <summary>
        /// Zips the two given <see cref="Task"/>'s when both completes using a <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static Task<TC> Zip<TA, TB, TC>(this Task<TA> source, Task<TB> other, Func<TA, TB, TC> selector)
        {
            return source.SelectMany(a => other.Select(b => selector(a, b)));
        }

        /// <summary>
        /// Joins the given <see cref="Task"/>'s when both result in the same value using the selectors: <paramref name="outerKeySelector"/> and <paramref name="innerKeySelector"/>; when both <see cref="Task"/>'s completes.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="outerKeySelector">The outer key selector.</param>
        /// <param name="innerKeySelector">The inner key selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <returns></returns>
        public static Task<TD> Join<TA, TB, TC, TD>(
            this Task<TA> source,
            Task<TB> inner,
            Func<TA, TC> outerKeySelector,
            Func<TB, TC> innerKeySelector,
            Func<TA, TB, TD> resultSelector)
        {
            Task.WaitAll(source, inner);
            return source.SelectMany(
                ta => inner.SelectMany(
                    tb =>
                    {
                        TC outerC = outerKeySelector(ta);
                        TC innerC = innerKeySelector(tb);
                        return EqualityComparer<TC>.Default.Equals(outerC, innerC)
                                   ? Task.FromResult(resultSelector(ta, tb))
                                   : throw new OperationCanceledException("Not Equal: " + outerC + " != " + innerC);
                    }));
        }

        /// <summary>
        /// Groups the two given <see cref="Task"/>'s when both result in the same value using the selectors: <paramref name="outerKeySelector"/> and <paramref name="innerKeySelector"/>.
        /// </summary>
        /// <typeparam name="TA">The type of a.</typeparam>
        /// <typeparam name="TB">The type of the b.</typeparam>
        /// <typeparam name="TC">The type of the c.</typeparam>
        /// <typeparam name="TD">The type of the d.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="outerKeySelector">The outer key selector.</param>
        /// <param name="innerKeySelector">The inner key selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <returns></returns>
        public static Task<TD> GroupJoin<TA, TB, TC, TD>(
            this Task<TA> source,
            Task<TB> inner,
            Func<TA, TC> outerKeySelector,
            Func<TB, TC> innerKeySelector,
            Func<TA, Task<TB>, TD> resultSelector)
        {
            return source.SelectMany(
                a => resultSelector(
                    a,
                    inner.Where(
                        b =>
                        {
                            TC outerC = outerKeySelector(a);
                            TC innerC = innerKeySelector(b);
                            return EqualityComparer<TC>.Default.Equals(outerC, innerC);
                        })).FromResult());
        }

        private static Task<TA> FromResult<TA>(this TA value)
        {
            return Task.FromResult(value);
        }

        /// <summary>
        /// Rescues the specified <see cref="Task{T}"/> if it fails by continuing with another <see cref="Task{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="otherTask">The other task.</param>
        /// <returns></returns>
        public static Task<T> Rescue<T>(this Task<T> source, Func<Task<T>> otherTask)
        {
            return source.ContinueWith(async t => t.Status == TaskStatus.Faulted ? await otherTask() : t.Result).Unwrap();
        }
    }

    /// <summary>
    /// Boolean combinations in the context of <see cref="Task{T}"/>'s.
    /// </summary>
    public static class TaskComb
    {
        /// <summary>
        /// Ifs the specified predicate holds, return either the left or the right <see cref="Task{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static Func<Task<T>, Task<T>, Task<T>> If<T>(Task<bool> predicate)
        {
            return (a, b) => predicate.SelectMany(p => p ? a : b);
        }

        /// <summary>
        /// AND expression for wrapped booleans.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static Task<bool> And(this Task<bool> a, Task<bool> b)
        {
            return If<bool>(a)(b, Task.FromResult(false));
        }

        /// <summary>
        /// OR expression for wrapped booleans.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static Task<bool> Or(this Task<bool> a, Task<bool> b)
        {
            return If<bool>(a)(Task.FromResult(true), b);
        }
    }
}
