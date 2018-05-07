# Introduction
A simple .NET Core library to manage [Apache Shiro](http://shiro.apache.org/permissions.html)-styled permissions.

# Installation

```
$ dotnet add package ShiroTrie 
```

 # Example
 
 ```c#
var permissions = new[]
{
    "file:read",
    "file:write",
    "directory:delete,create",
    "image:png,jpeg:read,write",
    "audio:mp3,ogg:high,low:sample",
    "user:*",
    "mail",
    "tty"
};

// add permissions to trie
var trie = new Trie();
trie.Add(scopes);


// test permissions
var checks = new[]
{
    "file:read",
    "file:create",
    "file:update",
    "file:delete",
    "printer",
    "scanner:read",
    "image:png:read",
    "audio:mp3:high:sample",
    "audio:mp3:high:sample:once",
    "user:delete",
    "user:*",
    "user:create",
    "mail",
    "bash-tty",
};

foreach (var check in checks)
{
    Console.WriteLine("{0} = {1}", check, trie.Check(check));
}
```
