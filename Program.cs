

namespace bar;

class Program
{
    static void Main(string[] args)
    {
        Test5();
    }

    private static void Test5()
    {
        var A = new Cond(char.IsAsciiLetterLower);
        var AA = new OneMore<char>(A);
        var B = AA.Parse("1djs5dsf");
    }

    private static void Test4()
    {
        var A = new Cond(char.IsAsciiLetterLower);
        var B = new XParser('B');
        var BB = new ZeroMore<char>(B);
        var AA = new ZeroMore<char>(A);
        var C = BB.Parse("BBBxh");
        var D = AA.Parse("djsfl4dsf");
    }

    private static void Test3()
    {
        var A = new Cond(char.IsAsciiLetterLower);
        var B = A.Parse("bAbc");
    }

    private static void Test2()
    {
        var A = new XParser('A');
        var B = new XParser('B');
        var C = new OrElse<char>(A, B);
        var D = C.Parse("A0Babc");
    }

    private static void Test()
    {
        var A = new XParser('A');
        var B = A.Parse("DAbc");
    }
}

// then Extensions

class OneMore<T> : IParser<T[]>
{
    public IParser<T> A { get; }

    public IResult<T[]> Parse(string input)
    {
        var B = new ZeroMore<T>(A); // or do in the constructor?
        var result = B.Parse(input);
        // Assert result.HasValue = true

        if (result.Value.Length > 0)
        {
            return result;
        }

        return Result<T[]>.Fail("OneMore failed");
    }

    public OneMore(IParser<T> a)
    {
        A = a;
    }
}

class ZeroMore<T> : IParser<T[]>
{
    public IParser<T> A { get; }

    public IResult<T[]> Parse(string input)
    {
        var list = new List<T>();
        var rest = input;

        while (true)
        {
            var B = A.Parse(rest);

            if (B.HasValue)
            {
                list.Add(B.Value);
                rest = B.Rest;
            }
            else
            {
                break;
            }
        }

        return Result<T[]>.Ok(list.ToArray(), rest);
    }

    public ZeroMore(IParser<T> a)
    {
        A = a;
    }
}

class Cond : IParser<char>
{
    public Func<char, bool> Condition { get; }

    public IResult<char> Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Result<char>.Fail("No more input");
        }

        var A = input.First();

        if (Condition(A))
        {
            return Result<char>.Ok(A, input.Substring(1));
        }

        return Result<char>.Fail("Condition failed");
    }

    public Cond(Func<char, bool> condition)
    {
        Condition = condition;
    }
}

class OrElse<T> : IParser<T>
{
    public IParser<T> A { get; }
    public IParser<T> B { get; }

    public IResult<T> Parse(string input)
    {
        var result = A.Parse(input);

        if (result.HasValue)
        {
            return result;
        }

        return B.Parse(input);
    }

    public OrElse(IParser<T> a, IParser<T> b)
    {
        A = a;
        B = b;
    }
}

class XParser : IParser<char>
{
    public char X { get; }

    public IResult<char> Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Result<char>.Fail("No more input");
        }
        var A = input.First();
        if (A == X)
        {
            return Result<char>.Ok(A, input.Substring(1));
        }

        return Result<char>.Fail($"Expected {X}. Got {A}.");
    }

    public XParser(char x)
    {
        X = x;
    }
}

interface IResult<out T>
{
    T Value { get; }
    bool HasValue { get; }
    string Rest { get; }
}

interface IParser<out T>
{
    IResult<T> Parse(string input);
}

class Result<T> : IResult<T>
{
    T _value = default(T);
    string _rest = string.Empty;
    string _error = string.Empty;

    public T Value => _value;
    public string Rest => _rest;
    public string Error => _error;

    public bool HasValue { get; }

    private Result(T value, string rest)
    {
        _value = value;
        _rest = rest;
        HasValue = true;
    }

    private Result(string error)
    {
        _error = error;
    }

    public static Result<T> Ok(T value, string rest)
    {
        return new Result<T>(value, rest);
    }

    public static Result<T> Fail(string error)
    {
        return new Result<T>(error);
    }
}