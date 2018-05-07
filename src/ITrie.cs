namespace ShiroTrie
{
    using System.Collections.Generic;


    internal interface ITrie
    {
        int Count { get; }
        void Add(IEnumerable<string> values);
        bool Check(string value);
        void Print();
    }
}
