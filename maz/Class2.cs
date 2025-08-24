namespace maz;

class ZParser : IParser<char>
{
    public IResult<char> Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new Fail<char>("No more input");
        }

        const char start = '!'; // 33
        const char end = 'u'; // 117
        var A = input.First();
        var B = A >= start;
        var C = A <= end;
        var InRange = B && C;

        if (InRange)
        {
            return new Success<char>(A, input.Substring(1));
        }

        return new Fail<char>($"ZParser: fail. Got {input.First()}");
    }
}