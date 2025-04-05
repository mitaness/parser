using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bar;

internal class Program2
{
    static void Main(string[] args)
    {
        Test();
    }

    private static void Test()
    {
        var A = new SpaceParser();
        var B = A.Parse("   aab");
    }
}

interface IElement
{

}

class WordElement : IElement
{
    public string Content { get; set; }
}

class SpaceElement : IElement
{
    public string Content { get; set; }
    public int Length => Content.Length;
}

class WordParser : IParser<IElement>
{
    public IResult<IElement> Parse(string input)
    {
        var C = new Cond(char.IsAsciiLetterLower);
        var word = new OneMore<char>(C);
        return word.Parse(input)
            .AndThenMap(xs => new string(xs))
            .AndThenMap(x => new WordElement { Content = x });
    }
}

class SpaceParser : IParser<IElement>
{
    public IResult<IElement> Parse(string input)
    {
        var C = new Cond(char.IsWhiteSpace);
        var space = new OneMore<char>(C);
        return space.Parse(input)
            .AndThenMap(xs => new string(xs))
            .AndThenMap(x => new SpaceElement { Content = x });
    }
}
