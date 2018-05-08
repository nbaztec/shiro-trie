namespace ShiroTrie
{
    public class PermissionTrieOptions
    {
        public string NamespaceSeparator { get; set; } = ":";
        public string ScopeSeparator { get; set; } = ",";
        public string WildcardString { get; set; } = "*";
        public char LeafCharacter { get; set; } = '\0';
    }
}
