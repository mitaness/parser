using System.Text;

namespace maz;

class Program
{
    static void Main(string[] args)
    {
        Test7();
        Console.ReadKey();
    }

    private static void Test8()
    {
        var input = "/c";
        var N = input.Length - 1;
        var test = input.PadRight(5, 'u');
        var value = Convert85(test);
        var data = GetString(value, N);
    }

    private static void Test7()
    {
        var A = new ZParser();
        var B = new Times<char>(A, 5);
        var C = new ThenParser<char[], string>(B, xs => new string(xs));
        var D = new ZeroMore<string>(C);

        var input = "9jqo^BlbD-BleB1DJ+*+F(f,q/0JhKF<GL>Cj@.4Gp$d7F!,L7@<6@)/0JDEF<G%<+EV:2F!,O<DJ+*.@<*K0@<6L(Df-\\0Ec5e;DffZ(EZee.Bl.9pF\"AGXBPCsi+DGm>@3BB/F*&OCAfu2/AKYi(DIb:@FD,*)+C]U=@3BN#EcYf8ATD3s@q?d$AftVqCh[NqF<G:8+EV:.+Cf>-FD5W8ARlolDIal(DId<j@<?3r@:F%a+D58'ATD4$Bl@l3De:,-DJs`8ARoFb/0JMK@qB4^F!,R<AKZ&-DfTqBG%G>uD.RTpAKYo'+CT/5+Cei#DII?(E,9)oF*2M7/c";
        var L = input.Length;
        var E = D.Parse(input);
        var sb = new StringBuilder();
        foreach (var item in E.Value)
        {
            var value = Convert85(item);
            var data = GetString(value);
            sb.Append(data);
        }
        if (E.Rest != "")
        {
            var data = ConvertPartial(E.Rest); 
            sb.Append(data);
        }
    }

    private static string ConvertPartial(string input)
    {
        var N = input.Length - 1;
        var test = input.PadRight(5, 'u');
        var value = Convert85(test);
        var data = GetString(value, N);
        return data;
    }

    private static void Test6()
    {
        var value = Convert85("9jqo^");
        var data = GetString(value);
    }

    private static string GetString(uint X, int take = 4)
    {
        var bytes = ConvertBinaryInternal(X)
            .Reverse()
            .Take(take)
            .Select(x => (byte)x)
            .ToArray();
        return Encoding.ASCII.GetString(bytes);
    }

    private static IEnumerable<uint> ConvertBinaryInternal(uint X)
    {
        //var X = value >> 1; // divide by 2
        while (X != 0)
        {
            yield return X & 255;
            X = X >> 8; // divide by 256
        }
    }

    //static IEnumerable<int> ToBinary(int X)
    //{
    //    while (X != 0)
    //    {
    //        yield return X & 1;
    //        X = X >> 1;
    //    }
    //}

    private static uint Convert85(string input)
    {
        var chars = input.ToCharArray();
        var bytes = Encoding.ASCII.GetBytes(input);
        var A = bytes.Select(x => (byte)(x - 33)).ToArray();
        //var B = uint.MaxValue;
        //var C = sizeof(uint);
        var D = A.Aggregate(0U, (acc, value) => acc * 85U + value);
        return D;
        // D	1298230816	uint

    }

    private static void Test5()
    {
        var A = new ZParser();
        var B = new Times<char>(A, 5);
        var C = new ThenParser<char[], string>(B, xs => new string(xs));
        var D = new ZeroMore<string>(C);
        var E = D.Parse("9jqo^BlbD-BleB1DJ"); // https://cryptii.com/pipes/ascii85-encoding
    }

    private static void Test4()
    {
        // buffer(5)
        var input = "";
        var start = '!'; // 33
        var end = 'u'; // 117
        var A = 'v';
        var B = A > start;
        var C = A <= end;
        var value = Char.GetNumericValue(A);
        throw new NotImplementedException();
    }

    private static void Test3()
    {
        var A = new XParser('A');
        var AA = new Times<char>(A, 2);
        var B = AA.Parse("AAAAx3");
    }

    private static void Test2()
    {
        var A = new XParser('A');
        var B = new XParser('B');
        var C = new XParser('C');
        var D = A
            .Parse("ABCd1")
            .AndThen(x => B)
            .AndThen(x => C)
            ;
    }

    private static void Test()
    {
        var A = new XParser('A');
        var B = new XParser('B');
        var C = new OrElse<char>(A, B);

        var D = C.Parse("BXA");
    }
}

interface IResult<out T>
{
    T Value { get; }
    bool HasValue { get; }
    string Rest { get; }
    string Error { get; }
}

interface IParser<T>
{
    IResult<T> Parse(string input);
}

class Success<T> : IResult<T>
{
    T _value;

    public T Value => _value;

    public bool HasValue => true;

    public string Rest { get; }

    public string Error => throw null;

    public Success(T value, string rest)
    {
        _value = value;
        Rest = rest;
    }
}

class Fail<T> : IResult<T>
{
    string _error;

    public T Value => throw null;

    public bool HasValue => false;

    public string Rest => throw null;

    public string Error => _error;

    public Fail(string error)
    {
        _error = error;
    }
}

class XParser : IParser<char>
{
    public char X { get; }

    public IResult<char> Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new Fail<char>("No more input");
        }

        if (input.StartsWith(X))
        {
            return new Success<char>(X, input.Substring(1));
        }

        return new Fail<char>($"Expected {X}. Got {input.First()}");
    }

    public XParser(char x)
    {
        X = x;
    }
}

class OrElse<T> : IParser<T>
{
    public IParser<T> A { get; }
    public IParser<T> B { get; }

    public IResult<T> Parse(string input)
    {
        var C = A.Parse(input);

        if (C.HasValue)
        {
            return C;
        }

        return B.Parse(input);
    }

    public OrElse(IParser<T> a, IParser<T> b)
    {
        A = a;
        B = b;
    }
}

static class Extensions
{
    public static IResult<T> AndThen<T>(this IResult<T> result, Func<T, IParser<T>> fn)
    {
        if (result.HasValue)
        {
            return fn(result.Value).Parse(result.Rest);
        }

        return result;
    }

    public static IResult<U> AndThenMap<T, U>(this IResult<T> result, Func<T, IParser<U>> fn)
    {
        if (result.HasValue)
        {
            return fn(result.Value).Parse(result.Rest);
        }

        return new Fail<U>(result.Error);
    }

    //public static IParser<T> Select<T>(this IParser<T> parser, Func<T, > fn)
    //{
    //    parser.Parse()
    //    return parser;
    //}
}

class ThenParser<T, U> : IParser<U>
{
    public IParser<T> A { get; }

    public Func<T, U> Fn { get; }

    public IResult<U> Parse(string input)
    {
        var B = A.Parse(input);

        if (B.HasValue)
        {
            return new Success<U>(Fn(B.Value), B.Rest);
        }

        return new Fail<U>(B.Error);
    }

    public ThenParser(IParser<T> a, Func<T, U> fn)
    {
        A = a;
        Fn = fn;
    }
}
