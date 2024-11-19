using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Trivial.Maths;
using Trivial.Reflection;

namespace Trivial.Text;

/// <summary>
/// The context of JSON switch case router.
/// </summary>
/// <typeparam name="TNode">The type of JSON node.</typeparam>
/// <typeparam name="TArgs">The type of args.</typeparam>
public class JsonSwitchContext<TNode, TArgs> : IJsonSwitchContextInfo<TArgs>, ICloneable where TNode : IJsonValueNode
{
    /// <summary>
    /// Initializes a new instance of the JsonSwitchContext class.
    /// </summary>
    /// <param name="source">The JSON node source.</param>
    /// <param name="args">The argument object.</param>
    public JsonSwitchContext(TNode source, TArgs args = default)
        : this(null, source, args)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonSwitchContext class.
    /// </summary>
    /// <param name="id">The identifier of the context.</param>
    /// <param name="source">The JSON node source.</param>
    /// <param name="args">The argument object.</param>
    public JsonSwitchContext(string id, TNode source, TArgs args = default)
    {
        Id = id ?? Guid.NewGuid().ToString("N");
        Source = source;
        Args = args;
        CreationTime = DateTime.Now;
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the creation date time.
    /// </summary>
    public DateTime CreationTime { get; }

    /// <summary>
    /// Gets the latest test time.
    /// </summary>
    public DateTime LatestTestTime { get; private set; }

    /// <summary>
    /// Gets the count of cases run.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets or sets the args.
    /// </summary>
    public TArgs Args { get; set; }

    /// <summary>
    /// Gets the args.
    /// </summary>
    object IJsonSwitchContextInfo.Args => Args;

    /// <summary>
    /// Gets the JSON node source.
    /// </summary>
    public TNode Source { get; }

    /// <summary>
    /// Gets the JSON node source.
    /// </summary>
    IJsonValueNode IJsonSwitchContextInfo.Source => Source;

    /// <summary>
    /// Gets the JSON value kind of the source.
    /// </summary>
    public JsonValueKind ValueKind => Source?.ValueKind ?? JsonValueKind.Null;

    /// <summary>
    /// Gets or sets the additional tag.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets a value indicating whether the switch is passed all cases.
    /// </summary>
    public bool IsPassed { get; private set; }

    /// <summary>
    /// Keeps available.
    /// </summary>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> KeepAvailable()
    {
        IsPassed = false;
        return this;
    }

    /// <summary>
    /// Keeps available.
    /// </summary>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> KeepAvailable(bool resetCount)
    {
        if (resetCount) Count = 0;
        return KeepAvailable();
    }

    /// <summary>
    /// Executes if all cases fail.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Default(Action block = null)
        => Default(false, block);

    /// <summary>
    /// Executes if all cases fail.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Default(Action<TNode> block)
        => Default(false, block);

    /// <summary>
    /// Executes if all cases fail.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Default(Action<TNode, JsonSwitchContext<TNode, TArgs>> block)
        => Default(false, block);

    /// <summary>
    /// Executes if all cases fail.
    /// </summary>
    /// <param name="doNotMarkPassed">true if does NOT mark current state as passed; otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Default(bool doNotMarkPassed, Action block)
    {
        if (IsPassed) return this;
        if (!doNotMarkPassed) IsPassed = true;
        IncreaseCount();
        block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes if all cases fail.
    /// </summary>
    /// <param name="doNotMarkPassed">true if does NOT mark current state as passed; otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Default(bool doNotMarkPassed, Action<TNode> block)
    {
        if (IsPassed) return this;
        if (!doNotMarkPassed) IsPassed = true;
        IncreaseCount();
        block?.Invoke(Source);
        return this;
    }

    /// <summary>
    /// Executes if all cases fail.
    /// </summary>
    /// <param name="doNotMarkPassed">true if does NOT mark current state as passed; otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Default(bool doNotMarkPassed, Action<TNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsPassed) return this;
        if (!doNotMarkPassed) IsPassed = true;
        IncreaseCount();
        block?.Invoke(Source, this);
        return this;
    }

    /// <summary>
    /// Executes a handler of code block.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Finally(Action block)
    {
        block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes a handler of code block.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Finally(Action<TNode> block)
    {
        block?.Invoke(Source);
        return this;
    }

    /// <summary>
    /// Executes a handler of code block.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Finally(Action<TNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        block?.Invoke(Source, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase router, Action block = null)
    {
        if (router is null) return this;
        router.Process(new(this), AfterTest, block);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase router, Action<TNode> block)
        => Case(router, block is null ? null : () => block(Source));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase router, Action<TNode, JsonSwitchContext<TNode, TArgs>> block)
        => Case(router, block is null ? null : () => block(Source, this));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="otherRouters">The other router instances.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase router, params BaseJsonSwitchCase[] otherRouters)
    {
        router?.Process(new(this), AfterTest, null);
        if (otherRouters is null) return this;
        foreach (var item in otherRouters)
        {
            item?.Process(new(this), AfterTest, null);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="otherRouters">The other router instances.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase router, params ReadOnlySpan<BaseJsonSwitchCase> otherRouters)
    {
        router?.Process(new(this), AfterTest, null);
        foreach (var item in otherRouters)
        {
            item?.Process(new(this), AfterTest, null);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="routers">The instance collection of the JSON switch-case router.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<BaseJsonSwitchCase> routers)
    {
        if (routers is null) return this;
        foreach (var item in routers)
        {
            item?.Process(new(this), AfterTest, null);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase<TArgs> router, Action block = null)
    {
        if (router is null) return this;
        router.Process(new(this), AfterTest, block);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase<TArgs> router, Action<TNode> block)
        => Case(router, block is null ? null : () => block(Source));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase<TArgs> router, Action<TNode, JsonSwitchContext<TNode, TArgs>> block)
        => Case(router, block is null ? null : () => block(Source, this));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="otherRouters">The other router instances.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase<TArgs> router, params BaseJsonSwitchCase<TArgs>[] otherRouters)
    {
        router?.Process(new(this), AfterTest, null);
        if (otherRouters is null) return this;
        foreach (var item in otherRouters)
        {
            item?.Process(new(this), AfterTest, null);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="router">The instance of the JSON switch-case router.</param>
    /// <param name="otherRouters">The other router instances.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BaseJsonSwitchCase<TArgs> router, params ReadOnlySpan<BaseJsonSwitchCase<TArgs>> otherRouters)
    {
        router?.Process(new(this), AfterTest, null);
        foreach (var item in otherRouters)
        {
            item?.Process(new(this), AfterTest, null);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="routers">The instance collection of the JSON switch-case router.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<BaseJsonSwitchCase<TArgs>> routers)
    {
        if (routers is null) return this;
        foreach (var item in routers)
        {
            item?.Process(new(this), AfterTest, null);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="T">The type of the JSON switch-case router.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<T>(JsonSwitchPredicate<T> predicate, Action<T> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, out var result))
        {
            AfterTest(true);
            block?.Invoke(result);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="T">The type of the JSON switch-case router.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<T>(JsonSwitchPredicate<T> predicate, Action<T, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, out var result))
        {
            AfterTest(true);
            block?.Invoke(result, this);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <typeparam name="TInfo">The type of additional info to return.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<TResult, TInfo>(JsonSwitchPredicate<TResult, TInfo> predicate, Action<TResult, TInfo> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, out var result, out var info))
        {
            AfterTest(true);
            block?.Invoke(result, info);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <typeparam name="TInfo">The type of additional info to return.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<TResult, TInfo>(JsonSwitchPredicate<TResult, TInfo> predicate, Action<TResult, TInfo, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, out var result, out var info))
        {
            AfterTest(true);
            block?.Invoke(result, info, this);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="TPredicateArgs">The type of args of the predicate handler.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="args">The args of the predicate handler.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<TPredicateArgs, TResult>(JsonSwitchArgsPredicate<TPredicateArgs, TResult> predicate, TPredicateArgs args, Action<TResult> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, args, out var result))
        {
            AfterTest(true);
            block?.Invoke(result);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="TPredicateArgs">The type of args of the predicate handler.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="args">The args of the predicate handler.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<TPredicateArgs, TResult>(JsonSwitchArgsPredicate<TPredicateArgs, TResult> predicate, TPredicateArgs args, Action<TResult, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, args, out var result))
        {
            AfterTest(true);
            block?.Invoke(result, this);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="TPredicateArgs">The type of args of the predicate handler.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <typeparam name="TInfo">The type of additional info to return.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="args">The args of the predicate handler.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<TPredicateArgs, TResult, TInfo>(JsonSwitchArgsPredicate<TPredicateArgs, TResult, TInfo> predicate, TPredicateArgs args, Action<TResult, TInfo> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, args, out var result, out var info))
        {
            AfterTest(true);
            block?.Invoke(result, info);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <typeparam name="TPredicateArgs">The type of args of the predicate handler.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <typeparam name="TInfo">The type of additional info to return.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="args">The args of the predicate handler.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<TPredicateArgs, TResult, TInfo>(JsonSwitchArgsPredicate<TPredicateArgs, TResult, TInfo> predicate, TPredicateArgs args, Action<TResult, TInfo, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (predicate == null || IsPassed) return this;
        if (predicate(Source, args, out var result, out var info))
        {
            AfterTest(true);
            block?.Invoke(result, info, this);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<TNode, bool> predicate, Action block)
    {
        if (IsPassed) return this;
        if (Test(predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<TNode, bool> predicate, Action<TNode> block)
    {
        if (IsPassed) return this;
        if (Test(predicate)) block?.Invoke(Source);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<TNode, bool> predicate, Action<TNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsPassed) return this;
        if (Test(predicate)) block?.Invoke(Source, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<string> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(true, out string s);
        AfterTest(b);
        if (b) block?.Invoke(s);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<string, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(true, out string s);
        AfterTest(b);
        if (b) block?.Invoke(s, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(string test, Action block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out string s), s, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(string test, Action<string> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out string s), s, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(string test, Action<string, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out string s), s, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(string test, StringComparison comparison, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Source.TryConvert(false, out string s) && string.Equals(s, test, comparison))
        {
            AfterTest(true);
            block?.Invoke();
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(string test, StringComparison comparison, Action<string> block)
    {
        if (IsNullOrPassed()) return this;
        if (Source.TryConvert(false, out string s) && string.Equals(s, test, comparison))
        {
            AfterTest(true);
            block?.Invoke(s);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(string test, StringComparison comparison, Action<string, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Source.TryConvert(false, out string s) && string.Equals(s, test, comparison))
        {
            AfterTest(true);
            block?.Invoke(s, this);
        }
        else
        {
            AfterTest(false);
        }

        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<string> test, Action block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out string s), s, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<string> test, Action<string> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out string s), s, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<string> test, Action<string, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out string s), s, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<string, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out string s), s, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<string, bool> predicate, Action<string> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out string s), s, predicate)) block?.Invoke(s);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<string, bool> predicate, Action<string, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out string s), s, predicate)) block?.Invoke(s, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<int> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out int i);
        AfterTest(b);
        if (b) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<int, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out int i);
        AfterTest(b);
        if (b) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(int test, Action block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out int i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(int test, Action<int> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out int i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(int test, Action<int, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out int i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, int test, Action block)
        => Case((int i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, int test, Action<int> block)
        => Case((int i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, int test, Action<int, JsonSwitchContext<TNode, TArgs>> block)
        => Case((int i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<int> test, Action block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<int> test, Action<int> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<int> test, Action<int, JsonSwitchContext<TNode, TArgs>> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(int? min, int? max, Action block)
        => Case(min, max, (i, c) => block?.Invoke());

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(int? min, int? max, Action<int> block)
        => Case(min, max, (i, c) => block?.Invoke(i));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(int? min, int? max, Action<int, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsPassed) return this;
        if (Source is null || !Source.TryConvert(false, out int i) || (min.HasValue && i < min.Value) || (max.HasValue && i > max.Value))
        {
            AfterTest(false);
            return this;
        }

        AfterTest(true);
        block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<int> test, Action block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out int i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<int> test, Action<int> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out int i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<int> test, Action<int, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out int i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<int, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out int i), i, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<int, bool> predicate, Action<int> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out int i), i, predicate)) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<int, bool> predicate, Action<int, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out int i), i, predicate)) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<long> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out long i);
        AfterTest(b);
        if (b) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<long, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out long i);
        AfterTest(b);
        if (b) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(long test, Action block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out long i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(long test, Action<long> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out long i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(long test, Action<long, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out long i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, long test, Action block)
        => Case((long i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, long test, Action<long> block)
        => Case((long i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, long test, Action<long, JsonSwitchContext<TNode, TArgs>> block)
        => Case((long i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<long> test, Action block)
        => test is null ? this : Case((long i) => test.Contains(i), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<long> test, Action<long> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<long> test, Action<long, JsonSwitchContext<TNode, TArgs>> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(long? min, long? max, Action block)
        => Case(min, max, (i, c) => block?.Invoke());

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(long? min, long? max, Action<long> block)
        => Case(min, max, (i, c) => block?.Invoke(i));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(long? min, long? max, Action<long, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsPassed) return this;
        if (Source is null || !Source.TryConvert(false, out long i) || (min.HasValue && i < min.Value) || (max.HasValue && i > max.Value))
        {
            AfterTest(false);
            return this;
        }

        AfterTest(true);
        block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<long> test, Action block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out long i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<long> test, Action<long> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out long i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(IEnumerable<long> test, Action<long, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out long i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<long, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out long i), i, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<long, bool> predicate, Action<long> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out long i), i, predicate)) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<long, bool> predicate, Action<long, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out long i), i, predicate)) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<float> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out float i) && !float.IsNaN(i);
        AfterTest(b);
        if (b) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<float, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out float i) && !float.IsNaN(i);
        AfterTest(b);
        if (b) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, float test, Action block)
        => Case((float i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, float test, Action<float> block)
        => Case((float i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, float test, Action<float, JsonSwitchContext<TNode, TArgs>> block)
        => Case((float i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<float> test, Action block)
        => test is null ? this : Case((float i) => test.Contains(i), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<float> test, Action<float> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<float> test, Action<float, JsonSwitchContext<TNode, TArgs>> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(float? min, float? max, Action block)
        => Case(min, max, (i, c) => block?.Invoke());

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(float? min, float? max, Action<float> block)
        => Case(min, max, (i, c) => block?.Invoke(i));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(float? min, float? max, Action<float, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsPassed) return this;
        if (Source is null || !Source.TryConvert(false, out float i) || (min.HasValue && !float.IsNaN(min.Value) && i < min.Value) || (max.HasValue && !double.IsNaN(max.Value) && i > max.Value))
        {
            AfterTest(false);
            return this;
        }

        AfterTest(true);
        block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<float, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out float i) && !float.IsNaN(i), i, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<float, bool> predicate, Action<float> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out float i) && !float.IsNaN(i), i, predicate)) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<float, bool> predicate, Action<float, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out float i) && !float.IsNaN(i), i, predicate)) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<double> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out double i) && !double.IsNaN(i);
        AfterTest(b);
        if (b) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<double, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out double i) && !double.IsNaN(i);
        AfterTest(b);
        if (b) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, double test, Action block)
        => Case((double i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, double test, Action<double> block)
        => Case((double i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, double test, Action<double, JsonSwitchContext<TNode, TArgs>> block)
        => Case((double i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<double> test, Action block)
        => test is null ? this : Case((double i) => test.Contains(i), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<double> test, Action<double> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<double> test, Action<double, JsonSwitchContext<TNode, TArgs>> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(double? min, double? max, Action block)
        => Case(min, max, (i, c) => block?.Invoke());

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(double? min, double? max, Action<double> block)
        => Case(min, max, (i, c) => block?.Invoke(i));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(double? min, double? max, Action<double, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsPassed) return this;
        if (Source is null || !Source.TryConvert(false, out double i) || (min.HasValue && !double.IsNaN(min.Value) && i < min.Value) || (max.HasValue && !double.IsNaN(max.Value) && i > max.Value))
        {
            AfterTest(false);
            return this;
        }

        AfterTest(true);
        block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<double, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out double i) && !double.IsNaN(i), i, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<double, bool> predicate, Action<double> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out double i) && !double.IsNaN(i), i, predicate)) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<double, bool> predicate, Action<double, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out double i) && !double.IsNaN(i), i, predicate)) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<decimal> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out decimal i);
        AfterTest(b);
        if (b) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<decimal, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(false, out decimal i);
        AfterTest(b);
        if (b) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, decimal test, Action block)
        => Case((decimal i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, decimal test, Action<decimal> block)
        => Case((decimal i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, decimal test, Action<decimal, JsonSwitchContext<TNode, TArgs>> block)
        => Case((decimal i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<decimal> test, Action block)
        => test is null ? this : Case((decimal i) => test.Contains(i), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<decimal> test, Action<decimal> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<decimal> test, Action<decimal, JsonSwitchContext<TNode, TArgs>> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(decimal? min, decimal? max, Action block)
        => Case(min, max, (i, c) => block?.Invoke());

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(decimal? min, decimal? max, Action<decimal> block)
        => Case(min, max, (i, c) => block?.Invoke(i));

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="min">The minimum value to compare; or null, if no limit for minimum.</param>
    /// <param name="max">The maximum value to compare; or null, if no limit for maximum.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(decimal? min, decimal? max, Action<decimal, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsPassed) return this;
        if (Source is null || !Source.TryConvert(false, out decimal i) || (min.HasValue && i < min.Value) || (max.HasValue && i > max.Value))
        {
            AfterTest(false);
            return this;
        }

        AfterTest(true);
        block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<decimal, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out decimal i), i, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<decimal, bool> predicate, Action<decimal> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out decimal i), i, predicate)) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<decimal, bool> predicate, Action<decimal, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out decimal i), i, predicate)) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<bool> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(true, out bool i);
        AfterTest(b);
        if (b) block?.Invoke(i);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<bool, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(true, out bool i);
        AfterTest(b);
        if (b) block?.Invoke(i, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(bool test, Action block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out bool i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(bool test, Action<bool> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out bool i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(bool test, Action<bool, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        return Case(Source.TryConvert(false, out bool i), i, test, block);
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BinaryBooleanOperator op, bool test, Action block)
        => Case((bool i) => BooleanOperations.Calculate(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BinaryBooleanOperator op, bool test, Action<bool> block)
        => Case((bool i) => BooleanOperations.Calculate(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(BinaryBooleanOperator op, bool test, Action<bool, JsonSwitchContext<TNode, TArgs>> block)
        => Case((bool i) => BooleanOperations.Calculate(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<bool, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out bool b), b, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<bool, bool> predicate, Action<bool> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out bool b), b, predicate)) block?.Invoke(b);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<bool, bool> predicate, Action<bool, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(false, out bool b), b, predicate)) block?.Invoke(b, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<DateTime> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(out DateTime dt);
        AfterTest(b);
        if (b) block?.Invoke(dt);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Action<DateTime, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        var b = Source.TryConvert(out DateTime dt);
        AfterTest(b);
        if (b) block?.Invoke(dt, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, DateTime test, Action block)
        => Case((DateTime i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, DateTime test, Action<DateTime> block)
        => Case((DateTime i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public JsonSwitchContext<TNode, TArgs> Case(BasicCompareOperator op, DateTime test, Action<DateTime, JsonSwitchContext<TNode, TArgs>> block)
        => Case((DateTime i) => Arithmetic.Compare(i, op, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<DateTime> test, Action block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<DateTime> test, Action<DateTime> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(StructValueSimpleInterval<DateTime> test, Action<DateTime, JsonSwitchContext<TNode, TArgs>> block)
        => test is null ? this : Case(test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<DateTime, bool> predicate, Action block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(out DateTime dt), dt, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<DateTime, bool> predicate, Action<DateTime> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(out DateTime dt), dt, predicate)) block?.Invoke(dt);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(Func<DateTime, bool> predicate, Action<DateTime, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (IsNullOrPassed()) return this;
        if (Test(Source.TryConvert(out DateTime dt), dt, predicate)) block?.Invoke(dt, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if it is contained by the value.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(JsonObjectNode test, Action<JsonObjectNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        var json = As<JsonObjectNode>();
        if (json is null) return this;
        var b = json.Contains(test);
        AfterTest(b);
        if (b) block?.Invoke(json, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if it is contained by the value.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(JsonObjectNode test, Action block)
    {
        var json = As<JsonObjectNode>();
        if (json is null) return this;
        var b = json.Contains(test);
        AfterTest(b);
        if (b) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="test">The value to compare if it is contained by the value.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case(JsonObjectNode test, Action<JsonObjectNode> block)
    {
        var json = As<JsonObjectNode>();
        if (json is null) return this;
        var b = json.Contains(test);
        AfterTest(b);
        if (b) block?.Invoke(json);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<T>(Action<T> block) where T : class, TNode
    {
        var obj = As<T>();
        if (obj is null) return this;
        AfterTest(true);
        block?.Invoke(obj);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<T>(Action<T, JsonSwitchContext<TNode, TArgs>> block) where T : class, TNode
    {
        var obj = As<T>();
        if (obj is null) return this;
        AfterTest(true);
        block?.Invoke(obj, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<T>(Func<T, bool> predicate, Action block) where T : class, TNode
    {
        var obj = As<T>();
        if (obj is null) return this;
        if (Test(true, obj, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<T>(Func<T, bool> predicate, Action<T> block) where T : class, TNode
    {
        var obj = As<T>();
        if (obj is null) return this;
        if (Test(true, obj, predicate)) block?.Invoke(obj);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Case<T>(Func<T, bool> predicate, Action<T, JsonSwitchContext<TNode, TArgs>> block) where T : class, TNode
    {
        var obj = As<T>();
        if (obj is null) return this;
        if (Test(true, obj, predicate)) block?.Invoke(obj, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, BaseJsonValueNode, bool> predicate, Action block)
    {
        var v = TryGetValue(key);
        if (v is null) return this;
        if (Test(key, v, predicate)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, BaseJsonValueNode, bool> predicate, Action<string, BaseJsonValueNode> block)
    {
        var v = TryGetValue(key);
        if (v is null) return this;
        if (Test(key, v, predicate)) block?.Invoke(key, v);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, BaseJsonValueNode, bool> predicate, Action<string, BaseJsonValueNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        var v = TryGetValue(key);
        if (v is null) return this;
        if (Test(key, v, predicate)) block?.Invoke(key, v, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Action<string, JsonArrayNode> block)
    {
        var v = TryGetValue<JsonArrayNode>(key, null);
        if (v is not null) block?.Invoke(key, v);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Action<string, JsonArrayNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        var v = TryGetValue<JsonArrayNode>(key, null);
        if (v is not null) block?.Invoke(key, v, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, JsonArrayNode, bool> predicate, Action block)
    {
        var v = TryGetValue(key, predicate);
        if (v is not null) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, JsonArrayNode, bool> predicate, Action<string, JsonArrayNode> block)
    {
        var v = TryGetValue(key, predicate);
        if (v is not null) block?.Invoke(key, v);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, JsonArrayNode, bool> predicate, Action<string, JsonArrayNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        var v = TryGetValue(key, predicate);
        if (v is not null) block?.Invoke(key, v, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="test">The value to compare if it is contained by the value.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, JsonObjectNode test, Action block)
        => PropertyCase(key, (key, v) => v.Contains(test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="test">The value to compare if it is contained by the value.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, JsonObjectNode test, Action<string, JsonObjectNode> block)
        => PropertyCase(key, (key, v) => v.Contains(test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="test">The value to compare if it is contained by the value.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, JsonObjectNode test, Action<string, JsonObjectNode, JsonSwitchContext<TNode, TArgs>> block)
        => PropertyCase(key, (key, v) => v.Contains(test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Action<string, JsonObjectNode> block)
    {
        var v = TryGetValue<JsonObjectNode>(key, null);
        if (v is not null) block?.Invoke(key, v);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Action<string, JsonObjectNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        var v = TryGetValue<JsonObjectNode>(key, null);
        if (v is not null) block?.Invoke(key, v, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, JsonObjectNode, bool> predicate, Action block)
    {
        var v = TryGetValue(key, predicate);
        if (v is not null) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, JsonObjectNode, bool> predicate, Action<string, JsonObjectNode> block)
    {
        var v = TryGetValue(key, predicate);
        if (v is not null) block?.Invoke(key, v);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase(string key, Func<string, JsonObjectNode, bool> predicate, Action<string, JsonObjectNode, JsonSwitchContext<TNode, TArgs>> block)
    {
        var v = TryGetValue(key, predicate);
        if (v is not null) block?.Invoke(key, v, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Action block)
        => PropertyCase<T>(key, strict, null as Func<string, T, bool>, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Action<string, T> block)
        => PropertyCase(key, strict, null as Func<string, T, bool>, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Action<string, T, JsonSwitchContext<TNode, TArgs>> block)
        => PropertyCase(key, strict, null as Func<string, T, bool>, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, T test, Action block) where T : IEquatable<T>
        => PropertyCase<T>(key, strict, (key, v) => ObjectConvert.Equals(v, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, T test, Action<string, T> block) where T : IEquatable<T>
        => PropertyCase(key, strict, (key, v) => ObjectConvert.Equals(v, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if they are same.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, T test, Action<string, T, JsonSwitchContext<TNode, TArgs>> block) where T : IEquatable<T>
        => PropertyCase(key, strict, (key, v) => ObjectConvert.Equals(v, test), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, IEnumerable<T> test, Action block) where T : IEquatable<T>
        => PropertyCase<T>(key, strict, (key, v) => test is not null && test.Contains(v), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, IEnumerable<T> test, Action<string, T> block) where T : IEquatable<T>
        => PropertyCase(key, strict, (key, v) => test is not null && test.Contains(v), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, IEnumerable<T> test, Action<string, T, JsonSwitchContext<TNode, TArgs>> block) where T : IEquatable<T>
        => PropertyCase(key, strict, (key, v) => test is not null && test.Contains(v), block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, StructValueSimpleInterval<T> test, Action block) where T : struct, IEquatable<T>, IComparable<T>
        => test is null ? this : PropertyCase<T>(key, strict, test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, StructValueSimpleInterval<T> test, Action<string, T> block) where T : struct, IEquatable<T>, IComparable<T>
        => test is null ? this : PropertyCase(key, strict, test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="test">The value to compare if contains.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, StructValueSimpleInterval<T> test, Action<string, T, JsonSwitchContext<TNode, TArgs>> block) where T : struct, IEquatable<T>, IComparable<T>
        => test is null ? this : PropertyCase(key, strict, test.Contains, block);

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Func<T, bool> predicate, Action block)
    {
        if (TryGetValue(key, strict, predicate, out _)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Func<T, bool> predicate, Action<string, T> block)
    {
        if (TryGetValue(key, strict, predicate, out var result)) block?.Invoke(key, result);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Func<T, bool> predicate, Action<string, T, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (TryGetValue(key, strict, predicate, out var result)) block?.Invoke(key, result, this);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Func<string, T, bool> predicate, Action block)
    {
        if (TryGetValue(key, strict, predicate, out _)) block?.Invoke();
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Func<string, T, bool> predicate, Action<string, T> block)
    {
        if (TryGetValue(key, strict, predicate, out var result)) block?.Invoke(key, result);
        return this;
    }

    /// <summary>
    /// Executes the handler of code block if matches the testing.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strict">true if enable strict mode (check value kind); otherwise, false.</param>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> PropertyCase<T>(string key, bool strict, Func<string, T, bool> predicate, Action<string, T, JsonSwitchContext<TNode, TArgs>> block)
    {
        if (TryGetValue(key, strict, predicate, out var result)) block?.Invoke(key, result, this);
        return this;
    }

    /// <summary>
    /// Executes the specific handler.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <param name="fallback">The fallback handler.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Continue(Action<TNode> callback, Action<TNode> fallback = null)
    {
        if (IsPassed) callback?.Invoke(Source);
        else fallback?.Invoke(Source);
        return this;
    }

    /// <summary>
    /// Executes the specific handler.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <param name="fallback">The fallback handler.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Continue(Action<TNode, JsonSwitchContext<TNode, TArgs>> callback, Action<TNode, JsonSwitchContext<TNode, TArgs>> fallback = null)
    {
        if (IsPassed) callback?.Invoke(Source, this);
        else fallback?.Invoke(Source, this);
        return this;
    }

    /// <summary>
    /// Executes the specific handler.
    /// </summary>
    /// <param name="hasPassed">true if execute the handler if it has already passed; otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Continue(bool hasPassed, Action<TNode> block = null)
    {
        if (IsPassed == hasPassed) block?.Invoke(Source);
        return this;
    }

    /// <summary>
    /// Executes the specific handler.
    /// </summary>
    /// <param name="hasPassed">true if execute the handler if it has already passed; otherwise, false.</param>
    /// <param name="block">The handler of code block.</param>
    /// <returns>The JSON switch context instance itself.</returns>
    public JsonSwitchContext<TNode, TArgs> Continue(bool hasPassed, Action<TNode, JsonSwitchContext<TNode, TArgs>> block = null)
    {
        if (IsPassed == hasPassed) block?.Invoke(Source, this);
        return this;
    }

    /// <summary>
    /// Configurates the result of happy path route.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <returns>The happy path route.</returns>
    public JsonSwitchThen<TNode, TArgs, TResult> Config<TResult>(JsonSwitchPredicate<TResult> predicate)
    {
        if (predicate is null) return new(this);
        if (predicate(Source, out var result))
        {
            AfterTest(true);
            return new(result, this);
        }
        else
        {
            AfterTest(false);
            return new(this);
        }
    }

    /// <summary>
    /// Configurates the result of happy path route.
    /// </summary>
    /// <typeparam name="TPredicateArgs">The type of predicate args.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="args">The predicate args.</param>
    /// <returns>The happy path route.</returns>
    public JsonSwitchThen<TNode, TArgs, TResult> Config<TPredicateArgs, TResult>(JsonSwitchArgsPredicate<TPredicateArgs, TResult> predicate, TPredicateArgs args)
    {
        if (predicate is null) return new(this);
        if (predicate(Source, args, out var result))
        {
            AfterTest(true);
            return new JsonSwitchThen<TNode, TArgs, TResult>(result, this);
        }
        else
        {
            AfterTest(false);
            return new(this);
        }
    }

    /// <summary>
    /// Configurates the result of happy path route.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <typeparam name="TInfo">The type of additional info to return.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <returns>The happy path route.</returns>
    public JsonSwitchThen<TNode, TArgs, TResult, TInfo> Config<TResult, TInfo>(JsonSwitchPredicate<TResult, TInfo> predicate)
    {
        if (predicate is null) return new(this);
        if (predicate(Source, out var result, out var info))
        {
            AfterTest(true);
            return new(result, info, this);
        }
        else
        {
            AfterTest(false);
            return new(this);
        }
    }

    /// <summary>
    /// Configurates the result of happy path route.
    /// </summary>
    /// <typeparam name="TPredicateArgs">The type of predicate args.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <typeparam name="TInfo">The type of additional info to return.</typeparam>
    /// <param name="predicate">A function to test the JSON node for a condition.</param>
    /// <param name="args">The predicate args.</param>
    /// <returns>The happy path route.</returns>
    public JsonSwitchThen<TNode, TArgs, TResult, TInfo> Config<TPredicateArgs, TResult, TInfo>(JsonSwitchArgsPredicate<TPredicateArgs, TResult, TInfo> predicate, TPredicateArgs args)
    {
        if (predicate is null) return new(this);
        if (predicate(Source, args, out var result, out var info))
        {
            AfterTest(true);
            return new(result, info, this);
        }
        else
        {
            AfterTest(false);
            return new(this);
        }
    }

    /// <summary>
    /// Clones.
    /// </summary>
    /// <param name="id">The identifier of the new instance.</param>
    /// <returns>A new instance.</returns>
    public JsonSwitchContext<TNode, TArgs> Clone(string id)
        => new(id, Source, Args)
        {
            LatestTestTime = LatestTestTime,
            Count = Count,
            IsPassed = IsPassed,
        };

    /// <summary>
    /// Clones.
    /// </summary>
    /// <returns>A new instance.</returns>
    public JsonSwitchContext<TNode, TArgs> Clone()
        => Clone(null);

    object ICloneable.Clone()
        => Clone();

    private JsonSwitchContext<TNode, TArgs> Case<TValue>(bool isPassed, TValue value, TValue test, Action block) where TValue : IEquatable<TValue>
    {
        if (AfterTest(isPassed && value.Equals(test))) block?.Invoke();
        return this;
    }

    private JsonSwitchContext<TNode, TArgs> Case<TValue>(bool isPassed, TValue value, TValue test, Action<TValue> block) where TValue : IEquatable<TValue>
    {
        if (AfterTest(isPassed && value.Equals(test))) block?.Invoke(value);
        return this;
    }

    private JsonSwitchContext<TNode, TArgs> Case<TValue>(bool isPassed, TValue value, TValue test, Action<TValue, JsonSwitchContext<TNode, TArgs>> block) where TValue : IEquatable<TValue>
    {
        if (AfterTest(isPassed && value.Equals(test))) block?.Invoke(value, this);
        return this;
    }

    private JsonSwitchContext<TNode, TArgs> Case<TValue>(bool isPassed, TValue value, IEnumerable<TValue> test, Action block) where TValue : IEquatable<TValue>
    {
        if (AfterTest(isPassed && test is not null && test.Contains(value))) block?.Invoke();
        return this;
    }

    private JsonSwitchContext<TNode, TArgs> Case<TValue>(bool isPassed, TValue value, IEnumerable<TValue> test, Action<TValue> block) where TValue : IEquatable<TValue>
    {
        if (AfterTest(isPassed && test is not null && test.Contains(value))) block?.Invoke(value);
        return this;
    }

    private JsonSwitchContext<TNode, TArgs> Case<TValue>(bool isPassed, TValue value, IEnumerable<TValue> test, Action<TValue, JsonSwitchContext<TNode, TArgs>> block) where TValue : IEquatable<TValue>
    {
        if (AfterTest(isPassed && test is not null && test.Contains(value))) block?.Invoke(value, this);
        return this;
    }

    private bool Test(Func<TNode, bool> predicate)
    {
        var pass = predicate is null || predicate(Source);
        return AfterTest(pass);
    }

    private bool Test<TValue>(bool isPassed, TValue value, Func<TValue, bool> predicate)
    {
        var pass = isPassed && (predicate is null || predicate(value));
        return AfterTest(pass);
    }

    private bool Test<TValue>(string key, TValue value, Func<string, TValue, bool> predicate)
    {
        var pass = predicate is null || predicate(key, value);
        return AfterTest(pass);
    }

    private void IncreaseCount()
    {
        if (Count < int.MaxValue) Count++;
    }

    private bool AfterTest(bool pass)
    {
        if (Count < int.MaxValue) Count++;
        LatestTestTime = DateTime.Now;
        if (pass) IsPassed = true;
        return pass;
    }

    private bool IsNullOrPassed()
    {
        if (IsPassed) return true;
        if (Source is not null) return false;
        AfterTest(false);
        return true;
    }

    private T As<T>() where T : class
    {
        if (IsPassed) return null;
        if (Source is T obj) return obj;
        AfterTest(false);
        return null;
    }

    private BaseJsonValueNode TryGetValue(string key)
    {
        var json = As<JsonObjectNode>();
        if (json is null) return null;
        var v = json.TryGetValue(key);
        if (v is not null && v.ValueKind != JsonValueKind.Undefined) return v;
        AfterTest(false);
        return null;
    }

    private bool TryGetValue<T>(string key, bool strict, Func<string, T, bool> predicate, out T result)
    {
        var v = TryGetValue(key);
        if (v is null)
        {
            result = default;
            return false;
        }

        var b = v.TryConvert(strict, out result, out _) && (predicate is null || predicate(key, result));
        AfterTest(b);
        if (!b) result = default;
        return b;
    }

    private bool TryGetValue<T>(string key, bool strict, Func<T, bool> predicate, out T result)
    {
        var v = TryGetValue(key);
        if (v is null)
        {
            result = default;
            return false;
        }

        var b = v.TryConvert(strict, out result, out _) && (predicate is null || predicate(result));
        AfterTest(b);
        if (!b) result = default;
        return b;
    }

    private T TryGetValue<T>(string key, Func<string, T, bool> predicate) where T : class
    {
        var v = TryGetValue(key);
        if (v is null) return null;
        var obj = v as T;
        var b = obj is not null && (predicate is null || predicate(key, obj));
        AfterTest(b);
        return b ? obj : null;
    }
}
