using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maz;

// to be done
// ASCII 85 decode last partial
// resolve < 256, less than start code is incorrect
// will try to resolve 256 - clear code
// reading 9bits, 10 bits ... buffering

internal class Class3
{
    //static void Main(string[] args)
    //{
    //    Test2();
    //}

    private static void Test3()
    {
        var range = Enumerable.Range(0, 10);
        var test = Encoding.ASCII.GetString(new byte[] { 245, 246 });
        var aa = "ABC" + 'C';
    }

    private static void Test2()
    {
        int code = 2;
        var decoder = new Decoder(4);
        decoder.AddFirstEntry(2);
        decoder.AddNextEntry(1);
        decoder.AddNextEntry(4);
        decoder.AddNextEntry(5);
        decoder.AddNextEntry(3);
        decoder.AddNextEntry(8);
        decoder.AddNextEntry(1);
    }

    private static void Test()
    {
        int code = 4;
        byte[] input = [2 + '@'];
        var value = Encoding.ASCII.GetString(input);
        var len = value.Length;
        Dictionary<int, Entry> dic = new();
        dic.Add(code, new Unresolved("B"));
    }
}

class Decoder
{
    public Dictionary<int, Entry> Dictio { get; }

    public int Next { get; private set; }
    public int Start { get; }

    public Decoder(int code)
    {
        Dictio = new();
        Next = code;
        Start = code;
    }

    public void AddFirstEntry(int code)
    {
        string value = ResolveCode(code);
        Dictio.Add(Next, new Unresolved(value));
    }

    public void AddNextEntry(int code)
    {
        //var entry = Resolve(code);
        var prev = Next++;
        var entry = Dictio[prev];

        var value = Resolve(code);

        Dictio[prev] = entry.Resolve(value.First().ToString());
        Dictio.Add(Next, new Unresolved(value));
    }

    private string Resolve(int code)
    {
        if (code < Start) return ResolveCode(code);
        var entry = Dictio[code];
        if (entry is Resolved x)
        {
            return x.Value;
        }

        return entry.Prefix + entry.Prefix.First();
    }

    private string ResolveCode(int code)
    {
        char A = '@';
        var B = A + code;
        byte[] input = [(byte)B];
        var value = Encoding.ASCII.GetString(input);
        return value;
    }
}

abstract class Entry
{
    public string Prefix { get; }

    public Entry(string prefix)
    {
        Prefix = prefix;
    }

    public abstract Entry Resolve(string x);
}

class Unresolved : Entry
{
    public Unresolved(string prefix) : base(prefix) { }

    public override Entry Resolve(string x)
    {
        return new Resolved(Prefix, x);
    }
}

[DebuggerDisplay("{Value}")]
class Resolved : Entry
{
    public string Value { get; }
    public string X { get; }

    public Resolved(string prefix, string x) : base(prefix)
    {
        Value = Prefix + x;
        X = x;
    }

    public override Entry Resolve(string x) => throw null;
}
