using System;
using Xunit;

namespace tests
{
    using System.Collections.Generic;
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
                Assert.True(trie.Check(scope), $"{scope} returned false");
            }

            foreach (var scope in expectedNegativeCases)
            {
                Assert.False(trie.Check(scope), $"{scope} returned true");
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
                "n2:s1:x1",
                "n2:s2:x2",
                "n3:x1:s1",
                "n3:x2:s1",
                "n3:x2:s1:x4",
                "n4:x1:s1:x2",
                "n4:x3:s1:x4",
                "n4:x3:s1:x4:s4",
            };

            var expectedNegativeCases = new[]
            {
                "n1:s2:x1",
                "n3:x1:s2",
                "n4:x1:s3:x2",
            };

            var trie = new PermissionTrie();
            trie.Add(scopes);
            foreach (var scope in expectedPositiveCases)
            {
                Assert.True(trie.Check(scope), $"{scope} returned false");
            }

            foreach (var scope in expectedNegativeCases)
            {
                Assert.False(trie.Check(scope), $"{scope} returned true");
            }
        }

        [Fact]
        public void ShouldRespectPureWildcardTokens()
        {
            var testScopes = new Dictionary<string, Tuple<string[], string[]>>
            {
                {
                    "*",
                    new Tuple<string[], string[]>(
                        new[]
                        {
                            "n1",
                            "n1:s1:x1",
                            "n1:s1",
                        },
                        new string[]{}
                    )
                },
                {
                    "*:*",
                    new Tuple<string[], string[]>(
                        new[]
                        {
                            "n1",
                            "n1:s1:x1",
                            "n1:s1",
                            "n2:x1:s1:s3",
                        },
                        new string[] {}
                    )
                },
                {
                    "*:*:*",
                    new Tuple<string[], string[]>(
                        new[]
                        {
                            "n1:s1:x1",
                            "n1:s1:x1:y1",
                            "n2:x1",
                        },
                        new[]
                        {
                            "n1",
                        }
                    )
                },
            };

            foreach (var testScope in testScopes)
            {
                var trie = new PermissionTrie();
                trie.Add(new[] { testScope.Key });

                var expectedPositiveCases = testScope.Value.Item1;
                var expectedNegativeCases = testScope.Value.Item2;

                foreach (var scope in expectedPositiveCases)
                {
                    Assert.True(trie.Check(scope), $"{scope} returned false for {testScope.Key}");
                }

                foreach (var scope in expectedNegativeCases)
                {
                    Assert.False(trie.Check(scope), $"{scope} returned true for {testScope.Key}");
                }
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
                "n1:s1:s2",
                "n1:s1:s2:x4",
                "n2:s1:s4",
                "n2:s2:s3",
                "n2:s2:s4",
                "n3",
                "n3:s1",
                "n4",
                "n4:s1:s2",
            };

            var expectedNegativeCases = new[]
            {
                "n1:s3",
                "n2:s3",
                "n2:s4",
                "n2:s1:s5",
                "n2:s5:s4",
                "n5",
            };

            var trie = new PermissionTrie();
            trie.Add(scopes);
            foreach (var scope in expectedPositiveCases)
            {
                Assert.True(trie.Check(scope), $"{scope} returned false");
            }

            foreach (var scope in expectedNegativeCases)
            {
                Assert.False(trie.Check(scope), $"{scope} returned true");
            }
        }
    }
}
