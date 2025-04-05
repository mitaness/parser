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
        throw new NotImplementedException();
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
