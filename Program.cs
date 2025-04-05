namespace bar;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}

interface IResult<out T>
{

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