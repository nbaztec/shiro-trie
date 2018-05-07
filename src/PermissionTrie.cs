namespace ShiroTrie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class PermissionTrie : ITrie
    {
        private readonly TrieNode root;
        private readonly PermissionTrieOptions options;
        public int Count { get; private set; }

        public PermissionTrie()
        {
            this.root = new TrieNode();
            this.options = new PermissionTrieOptions
            {
                NamespaceSeparator = ":",
                ScopeSeparator = ",",
                WildcardString = "*",
            };
        }

        public PermissionTrie(PermissionTrieOptions options)
            : this()
        {
            this.options = options;
        }

        public void Add(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                this.addRecursive(value, this.root);
            }

            this.Count = this.countRecursive(this.root);
        }

        public bool Check(string value)
        {
            return this.checkRecursive(value, this.root);
        }

        public void Print()
        {
            this.printRecursive(string.Empty, this.root);
        }

        private void addRecursive(string value, TrieNode node)
        {
            var curNode = node;
            var parts = value.Split(this.options.NamespaceSeparator, StringSplitOptions.RemoveEmptyEntries);


            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                var components = part.Split(this.options.ScopeSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (components.Length == 1)
                {
                    curNode = curNode.Add(components[0]);
                    continue;
                }

                foreach (var component in components)
                {
                    var c = component.Trim();
                    var remainingString = string.Join(this.options.NamespaceSeparator, parts.Skip(i + 1));
                    var newComponent = c;

                    // append rest of permission string if present
                    if (!string.IsNullOrEmpty(remainingString))
                    {
                        newComponent += $"{this.options.NamespaceSeparator}{remainingString}";
                    }

                    this.addRecursive(newComponent, curNode);
                }

                break;
            }
        }

        private bool checkRecursive(string value, TrieNode node)
        {
            var curNode = node;
            var parts = value.Split(this.options.NamespaceSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var p = part.Trim();

                // exit early if trie ended prematurely
                if (curNode.IsLeaf)
                {
                    return false;
                }

                // handle wildcards
                if (curNode.Exists(this.options.WildcardString))
                {
                    curNode = curNode[this.options.WildcardString];
                    continue;
                }

                if (!curNode.Exists(p))
                {
                    return false;
                }

                curNode = curNode[p];
            }

            return curNode.IsLeaf;
        }

        private void printRecursive(string prefix, TrieNode node)
        {
            if (node.IsLeaf)
            {
                Console.WriteLine("{0}", prefix.Substring(0, prefix.Length - this.options.NamespaceSeparator.Length));
                return;
            }

            foreach (var key in node.Values())
            {
                this.printRecursive($"{prefix}{key}{this.options.NamespaceSeparator}", node[key]);
            }
        }

        private int countRecursive(TrieNode node)
        {
            if (node.IsLeaf)
            {
                return 1;
            }

            var count = 0;
            foreach (var key in node.Values())
            {
                count += this.countRecursive(node[key]);
            }

            return count;
        }
    }
}
