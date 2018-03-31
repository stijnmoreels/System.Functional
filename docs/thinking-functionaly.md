# Thinking Functionaly

The fastest way to understand the `FunctionalExtensions` included in this package; is to think that every line of code you write is an Expression.

If it's a variable assignment, and `if`/`else` statement, a `while` or `foreach` loop, ... every line is an Expression. Every line has a input and a output which means that every line can be "chained" together.

That's the main idea of these extensions, once you understand this; the sky is the limit!

## Piping

Extension Methods and Utility classes are some way to add the readability to your code; but naming is often an issue. `ParserUtility`? `StringUtilities`? ... What we actually want is a piping system that reads like a sentence.

That's the reason the package includes the `.PipeTo()`, `.Do()`, ... extensions.

### `.PipeTo()` and `.Do()`

Especially in temporary variable assignment, the `.PipeTo()` really shows how powerfull this extension is.
Why would you create a temporary variable to only pass it in to the next function? Why not chain the both functions together?

```csharp
var document = new XmlDocument();
doc.Load("file.xml");
var navivator = doc.CreateNavigator();
var expression = navigator.Compile("//xpath");
var iterator = navigator.Select(expression);
while (iterator.MoveNext()) Console.WriteLine(iterator.Current);
```

While, with the extensions we could remove all the intermediary variables and just pipe it all together (even in a single line).

```csharp
new XmlDocument()
    .Do(doc => doc.Load("file.xml"))
    .CreateNavigator()
    .PipeTo(nav => nav
        .Compile("//xpath")
        .PipeTo(nav.Select)
    .WhileDo(i => i.MoveNext(), i => Console.WriteLine(i.Current));
```

## Loops

This section will go through some extensions to make your loops in C# more fluently.

### `foreach { }` > `.ForEach()`

When running through a collection of values and run a 'dead-end' function (a function that doesn't return anything useful, `void` for example). We end up with nothing at the end of the loop.

What if we want to do something else with the list of values?

This is how you write it before:

```csharp
var xs = Enumerable.Range(1, 10);
foreach (var x in xs) Console.WriteLine(x);
xs = xs.Select(x => x + 1);
foreach (var x in xs) Console.WriteLine(x);
```

And this is how could write it with the extensions:

```csharp
Enumerable
    .Range(1, 10)
    .ForEach(Console.WriteLine)
    .Select(x => x + 1)
    .ForEach(Console.WriteLine);
```

The `.ForEach()` extension will return the original list for us so we can do something else with it instead of leaving the pipeline.

### `while { }` > `.WhilePipeTo`

When we need a function to run until some predicate holds, we use a `while` statement. With these extensions; we can write it more fluently.

This is how you would write it without the extensions:

```csharp
var xs = Enumerable.Range(1, 10);
var e = xs.GetEnumerator();
var ys = new List<int>();
while (e.MoveNext()) ys.Add(e.Current + 1);
```

But with the extensions; we could write something like this:

```csharp
Enumerable
    .Range(1, 10)
    .GetEnumerator()
    .WhilePipeTo(e => e.MoveNext(), e => e.Current + 1);
```

## Exception Handling

When catching an `Exception`, we directly think of a `try`/`catch` block. Not always, but sometimes this is quite a nest of duplication and headaches. Why? Maybe because of all the lines we have to write, maybe because we can't write it fluently, maybe we must add 'noise' to your clean code.

Anyhow, without the extensions you end up with something like this for example:

```csharp
try
{
    int x = 2;
    int y = x / 0;
}
catch (DivideByZeroException ex)
{
    Console.WriteLine(ex);
}
```

With the extensions, we could write it like this:

```csharp
2.Try(
    x => x / 0, 
    (DivideByZeroException ex) => 0);
```

## Disposable Resources

When working with `IDisposable` types, we use the `using` statment to make sure we actually call `.Dispose()` when the resource isn't needed anymore.

For example:

```csharp
string txt = String.Empty;
using (var sr = new StreamReader(fileName))
{
    txt = sr.ReadToEnd();
}
```

When we actually could write it like this:

```csharp
string txt = new StreamReader(fileName).Use(sr => sr.ReadToEnd());
```

The `.Use()` call will make sure that the resource is disposed after we called the function we pass in.