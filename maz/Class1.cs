namespace maz;

class ZeroMore<T> : IParser<T[]>
{
    public IParser<T> A { get; }

    public IResult<T[]> Parse(string input)
    {
        List<T> list = new List<T>();
        var rest = input;

        while (true)
        {
            var B = A.Parse(rest);
            if (!B.HasValue)
            {
                break;
            }

            list.Add(B.Value);
            rest = B.Rest;
        }

        return new Success<T[]>(list.ToArray(), rest);
    }

    public ZeroMore(IParser<T> a)
    {
        A = a;
    }
}

class OneMore<T> : IParser<T[]>
{
    public IParser<T> A { get; }

    public IResult<T[]> Parse(string input)
    {
        var B = new ZeroMore<T>(A);
        var C = B.Parse(input);

        if (C.Value.Length != 0)
        {
            return C;
        }
        return new Fail<T[]>("OneMore: fail");
    }

    public OneMore(IParser<T> a)
    {
        A = a;
    }
}

class Times<T> : IParser<T[]>
{
    public IParser<T> A { get; }
    public int N { get; }

    public IResult<T[]> Parse(string input)
    {
        List<T> list = new List<T>();
        var rest = input;

        for (int i = 0; i < N; i++)
        {
            var B = A.Parse(rest);

            if (!B.HasValue)
            {
                break;
            }

            list.Add(B.Value);
            rest = B.Rest;
        }

        if (list.Count != N)
        {
            return new Fail<T[]>("Times: fail");
        }

        return new Success<T[]>(list.ToArray(), rest);
    }

    public Times(IParser<T> a, int n)
    {
        A = a;
        N = n;
    }
}
