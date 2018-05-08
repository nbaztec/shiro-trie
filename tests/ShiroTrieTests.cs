using System;
using Xunit;

namespace tests
{
    using ShiroTrie;

    public class ShiroTrieTests
    {
        [Fact]
        public void ShouldBuildTrieWithExactInputs()
        {
            var scopes = new[]
            {
                "n1:s1",              // 1
                "n1:s2",              // 1
                "n1:s3,s4,s5",        // 3
                "n2:s1,s2:s3,s4",     // 4
                "n3:s1,s2:s3,s4:s5",  // 4
                "n4:*",               // 1
                "n5",                 // 1
                " ",                  // empty strings should be ignored
                "",
            };

            const int expectedFinalScopeCount = 15;

            var trie = new PermissionTrie();
            trie.Add(scopes);

            Assert.Equal(trie.Count, expectedFinalScopeCount);
        }

        [Fact]
        public void ShouldIgnoreTrailingWhitespace()
        {
            var scopes = new[]
            {
                "n1:s1 ",
                "n1:s2, s3",
                "n1  :  s4",
                " n1  :  s5:  s6",
            };

            var expectedPositiveCases = new[]
            {
                "n1:s1",
                "n1:s2",
                "n1:s3",
                "n1:s4",
                "n1:s5 :s6",
                "n1:s1 ",
                "n1: s2 ",
                " n1 : s3 ",
            };

            var expectedNegativeCases = new[]
            {
                "n1:s5",
                "n1:s6",
                "n1:s 1",
                "n 1:s1",
                "n1",
            };

            var trie = new PermissionTrie();
            trie.Add(scopes);
            foreach (var scope in expectedPositiveCases)
            {
                Assert.Equal(trie.Check(scope), true);
            }

            foreach (var scope in expectedNegativeCases)
            {
                Assert.Equal(trie.Check(scope), false);
            }
        }

        [Fact]
        public void ShouldRespectWildcardTokens()
        {
            var scopes = new[]
            {
                "n1:s1:*",
                "n2:*",
                "n3:*:s1",
                "n4:*:s1:*",
            };

            var expectedPositiveCases = new[]
            {
                "n1:s1:x1",
                "n1:s1:x3",
                "n2:x1",
                "n2:x2",
                "n3:x1:s1",
                "n3:x2:s1",
                "n4:x1:s1:x2",
                "n4:x3:s1:x4",
            };

            var expectedNegativeCases = new[]
            {
                "n1:s2:x1",
                "n2:x1:s1",
                "n3:x1:s2",
                "n4:x1:s3:x2",
                "n4:x3:s1:x4:s4",
            };

            var trie = new PermissionTrie();
            trie.Add(scopes);
            foreach (var scope in expectedPositiveCases)
            {
                Assert.Equal(trie.Check(scope), true);
            }

            foreach (var scope in expectedNegativeCases)
            {
                Assert.Equal(trie.Check(scope), false);
            }
        }

        [Fact]
        public void ShouldRespectWildcardTokens2()
        {
            var scopes = new[]
            {
                "*:*",
                "*:*:*",
            };

            var expectedPositiveCases = new[]
            {
                "n1:s1:x1",
                "n1:s1",
            };

            var expectedNegativeCases = new[]
            {
                "n1",
                "n2:x1:s1:s3",
            };

            var trie = new PermissionTrie();
            trie.Add(scopes);
            foreach (var scope in expectedPositiveCases)
            {
                var x = trie.Check(scope);
                Assert.Equal(x, true);
            }

            foreach (var scope in expectedNegativeCases)
            {
                var x = trie.Check(scope);
                Assert.Equal(x, false);
            }
        }

        [Fact]
        public void ShouldRespectScopeSeparatorToken()
        {
            var scopes = new[]
            {
                "n1:s1,s2",
                "n2:s1,s2:s3,s4",
                "n3,n4",
            };

            var expectedPositiveCases = new[]
            {
                "n1:s1",
                "n1:s2",
                "n2:s1:s3",
                "n2:s1:s4",
                "n2:s2:s3",
                "n2:s2:s4",
                "n3",
                "n4",
            };

            var expectedNegativeCases = new[]
            {
                "n1:s1:s2",
                "n1:s3",
                "n2:s3",
                "n2:s4",
                "n2:s1:s5",
                "n2:s5:s4",
                "n5",
                "n3:s1",
                "n4:s1:s2",
            };

            var trie = new PermissionTrie();
            trie.Add(scopes);
            foreach (var scope in expectedPositiveCases)
            {
                Assert.Equal(trie.Check(scope), true);
            }

            foreach (var scope in expectedNegativeCases)
            {
                Assert.Equal(trie.Check(scope), false);
            }
        }
    }
}
