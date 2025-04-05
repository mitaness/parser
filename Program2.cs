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
        Test3();
    }

    private static void Test3()
    {
        var content = new ContentParser();
        var A = content.Parse("fjl jdsljf kdfjls<b></b>fkdsjl 5");
    }

    private static void Test2()
    {
        var A = new SpaceParser();
        var B = new WordParser();
        var C = new TagParser();
        var D = new Either<IElement>(A, B, C);
        var E = D.Parse("jkl   <ab></ab>");
    }

    private static void Test()
    {
        var A = new SpaceParser();
        var B = A.Parse("   aab");
        var C = new TagParser();
        var D = C.Parse("<div></div>4");
    }
}

class ContentParser : IParser<IElement[]>
{
    public IResult<IElement[]> Parse(string input)
    {
        var A = new SpaceParser();
        var B = new WordParser();
        var C = new TagParser();
        var D = new Either<IElement>(A, B, C);
        var E = new ZeroMore<IElement>(D);
        return E.Parse(input);
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

class TagElement : IElement
{
    public string TagName { get; set; }
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

class TagParser : IParser<IElement>
{
    public IResult<IElement> Parse(string input)
    {
        // new task: parse <p></p>
        var startTag = string.Empty;

        var C = new Cond(char.IsAsciiLetterLower);
        var tag = new OneMore<char>(C);
        var A = new XParser('<');
        return A.Parse(input)
            .AndThen(_ => tag)
            .AndThenMap(xs => new string(xs))
            .AndThen(x =>
            {
                startTag = x; // save the tag
                return new XParser('>');
            })
            .AndThen(_ => new XParser('<'))
            .AndThen(_ => new XParser('/'))
            .AndThen(_ => tag)
            .AndThenMap(xs => new string(xs))
            .Where(x => x == startTag)
            .AndThen(x => new XParser('>'))
            .AndThenMap(_ => new TagElement { TagName = startTag });
    }
}

class Either<T> : IParser<T>
{
    public IParser<T>[] Parsers { get; }

    public IResult<T> Parse(string input)
    {
        foreach (var item in Parsers)
        {
            var result = item.Parse(input);

            if (result.HasValue)
            {
                return result;
            }
        }

        return Result<T>.Fail("Either failed");
    }

    public Either(params IParser<T>[] parsers)
    {
        Parsers = parsers;
    }
}
