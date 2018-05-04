namespace ShiroTrie
{
    using System.Collections.Generic;

    internal class TrieNode
    {
        private Dictionary<string, TrieNode> children { get; }
        public bool IsLeaf => this.children.Count == 0;

        public TrieNode()
        {
            this.children = new Dictionary<string, TrieNode>();
        }

        public TrieNode Add(string value)
        {
            value = value.Trim();
            var node = this.children.ContainsKey(value) ? this.children[value] : new TrieNode();
            this.children[value] = node;
            return node;
        }

        public TrieNode this[string key] => this.children[key];

        public bool Exists(string value)
        {
            return this.children.ContainsKey(value);
        }

        public Dictionary<string, TrieNode>.KeyCollection Values()
        {
            return this.children.Keys;
        }
    }
}
