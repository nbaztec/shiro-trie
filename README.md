# Introduction
A simple .NET Core library to manage [Apache Shiro](http://shiro.apache.org/permissions.html)-styled permissions.


Inspired by the [shiro-trie](https://www.npmjs.com/package/shiro-trie) npm package 
and [Apache Shiro](https://shiro.apache.org/permissions.html) styled permissions.

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
var trie = new PermissionTrie();
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
    "mail:delete",
    "mail:delete:once",
    "bash-tty",
};

foreach (var check in checks)
{
    Console.WriteLine("{0} = {1}", check, trie.Check(check));
}


/*
> Output:
file:read = True
file:create = False
file:update = False
file:delete = False
printer = False
scanner:read = False
image:png:read = True
audio:mp3:high:sample = True
audio:mp3:high:sample:once = True
user:delete = True
user:* = True
user:create = True
mail = True
mail:delete = True
mail:delete:once = True
bash-tty = False
*/
```
