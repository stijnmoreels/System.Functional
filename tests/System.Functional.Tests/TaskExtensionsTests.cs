using System.Functional.Monads;
using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;
using static System.Functional.FunctionalEx;

namespace System.Functional.Tests
{
    public class TaskExtensionsTests
    {
        [Property]
        public bool Functor_Law_Holds(int x, Func<int, int> f, Func<int, int> g)
        {
            return Task.FromResult(x)
                .Select(f)
                .Select(g)
                .AwaitResult() == g(f(x));
        }

        [DomainProperty]
        public bool Applicative_Law_Holds(Task<string> x, Task<float> y, Func<string, float, int> f)
        {
            return Task.FromResult(f)
                .Apply(x)
                .Apply(y)
                .Result == f(x.AwaitResult(), y.AwaitResult());
        }

        [DomainProperty]
        public bool Monadic_Law_Holds(Task<byte> x, Func<byte, int> f, Func<int, int> g)
        {
            Func<byte, Task<int>> ft = f.Compose(Task.FromResult);
            Func<int, Task<int>> gt = g.Compose(Task.FromResult);

            return x.SelectMany(ft)
                .SelectMany(gt)
                .AwaitResult() == g(f(x.AwaitResult()));
        }

        [Property]
        public bool Where_Holds(int x, Func<int, bool> predicate)
        {
            Task<int> t = Task.FromResult(x).Where(predicate);
            t.Try(TaskEx.AwaitResult);

            return t.IsFaulted == !predicate(x);
        }

        [Property]
        public bool Zip_Holds(string x, int y, Func<string, int, byte> f)
        {
            return Task.FromResult(x)
                .Zip(Task.FromResult(y), f)
                .Result == f(x, y);
        }

        [Property]
        public Property Join_Holds(int x, int y, Func<int, int, int> f)
        {
            var id = Id<int>();
            Maybe<int> result = Task.FromResult(x)
                .Try(_ => _
                .Join(Task.FromResult(y), id, id, f)
                .Result);

            bool isJoined = result != Maybe<int>.Nothing;
            return (isJoined && x == y)
                .ToProperty()
                .Or(!isJoined && x != y);
        }

        [Property]
        public Property GroupJoin_Holds(int x, int y)
        {
            var id = Id<int>();
            Maybe<int> result = Task.FromResult(x)
                .Try(_ => _
                .GroupJoin(Task.FromResult(y), id, id, (a, bTask) => bTask.Result)
                .Result);

            bool isGroupJoined = result == Maybe.Just(y);
            return (isGroupJoined && x == y)
                .ToProperty()
                .Or(!isGroupJoined && x != y);
        }

        [DomainProperty]
        public Property Rescue_Only_Rescues_Faulted_Tasks(Maybe<Exception> ex, int x, int y)
        {
            int result = Task.FromResult(x)
                .Do<int>(_ => ex.Do(__ => throw __))
                .Rescue(() => Task.FromResult(y))
                .Result;

            bool notFaulted = ex == Maybe<Exception>.Nothing;
            return (notFaulted && result == x)
                .ToProperty()
                .Or(!notFaulted && result == y);
        }

        [DomainProperty]
        public bool Task_Combination_Holds(Task<bool> a, Task<bool> b, Task<bool> c)
        {
            return a.Or(b).And(c).AwaitResult() == ((a.AwaitResult() || b.AwaitResult()) && c.AwaitResult());
        }

        [DomainProperty]
        public Property Predicate_Combination_Holds(bool predicate, int x, int y)
        {
            int result = TaskComb.If<int>(
                Task.FromResult(predicate))(
                    Task.FromResult(x), 
                    Task.FromResult(y)).Result;

            return (predicate && result == x)
                .ToProperty()
                .Or(!predicate && result == y);
        }
    }
}
