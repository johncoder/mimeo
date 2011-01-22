using System.Collections.Generic;
using NUnit.Framework;
using Should;

namespace Mimeo.Tests
{
    [TestFixture]
    public class StencilTests
    {
        private Mimeo<BlogTemplate> _mimeo;

        [SetUp]
        public void SetUp()
        {
            _mimeo = new Mimeo<BlogTemplate>(b => {
                b.Tokenize(p => p.BlogTitle, "{BlogTitle}");
                b.Tokenize(p => p.PageTitle, "{PageTitle}");
                b.Tokenize(p => p.JavaScriptIncludes, "{JavaScriptIncludes}");
                b.Block(p => p.Posts, "{Posts}", block => {
                    block.Tokenize(p => p.PostTitle, "{PostTitle}");
                    block.Tokenize(p => p.PostDescription, "{PostDescription}");
                    block.Tokenize(p => p.PostBody, "{PostBody}");
                    block.Block(p => p.Comments, "{Comments}", comments => {
                        comments.Tokenize(c => c.Author, "{Author}");
                        comments.Tokenize(c => c.Email, "{Email}");
                        comments.Tokenize(c => c.Text, "{CommentText}");
                    }).EndsWith("{/Comments}");
                }).EndsWith("{/Posts}");
            });
        }

        [Test]
        public void Stencil_GetContent_fills_single_positive()
        {
            const string template = "asdfasdf";

            _mimeo.CreateStencil("newtemplate", template);

            _mimeo.Render("newtemplate", new BlogTemplate()).ShouldEqual("asdfasdf");
        }

        [Test]
        public void Stencil_GetContent_replaces_blogtitle()
        {
            const string template = "{BlogTitle}";

            _mimeo.CreateStencil("newtemplate", template);

            _mimeo.Render("newtemplate", new BlogTemplate{BlogTitle = "asdf"}).ShouldEqual("asdf");
        }

        [Test]
        public void Stencil_GetContents_replaces_BlogTitle_and_preserves_plaintext()
        {
            const string template = "asdf{BlogTitle}asdf";

            _mimeo.CreateStencil("newtemplate", template);

            _mimeo.Render("newtemplate", new BlogTemplate { BlogTitle = "ASDF" }).ShouldEqual("asdfASDFasdf");
        }

        [Test]
        public void Stencil_GetContents_handles_block()
        {
            const string template = "{Posts}.{/Posts}";
            _mimeo.CreateStencil("newtemplate", template);
            var blog = new BlogTemplate { Posts = new List<BlogPost> { new BlogPost(), new BlogPost(), new BlogPost() } };
            var result = _mimeo.Render("newtemplate", blog);
            result.ShouldEqual("...");
        }

        [Test]
        public void Stencil_GetContents_handles_block_with_posttitles()
        {
            const string template = "{Posts}.{PostTitle}.{/Posts}";

            var stencil = _mimeo.CreateStencil("newtemplate", template);

            var blog = new BlogTemplate
            {
                Posts = new List<BlogPost>
                {
                    new BlogPost { PostTitle = "1" },
                    new BlogPost { PostTitle = "2" },
                    new BlogPost { PostTitle = "3" } 
                }
            };

            var result = _mimeo.Render("newtemplate", blog);
            result.ShouldEqual(".1..2..3.");
        }

        [Test]
        public void Stencil_GetContents_handles_block_with_nested_blocks()
        {
            const string template = "{Posts}{PostTitle} {Comments}...{/Comments}{/Posts}";

            var stencil = _mimeo.CreateStencil("newtemplate", template);

            var blog = new BlogTemplate
            {
                Posts = new List<BlogPost>
                {
                    new BlogPost {
                        PostTitle = "1",
                        Comments = new List<Comment>
                        {
                            new Comment()
                        }
                    },
                    new BlogPost { PostTitle = "2" },
                    new BlogPost { PostTitle = "3" }
                }
            };

            var result = _mimeo.Render("newtemplate", blog);
            result.ShouldEqual("1 ...2 3 ");
        }

        [Test]
        public void Stencil_GetContents_handles_block_with_nested_blocks_and_simple_replacements()
        {
            const string template = "{Posts}{PostTitle} {Comments}...{CommentText} {/Comments}{/Posts}";

            var stencil = _mimeo.CreateStencil("newtemplate", template);

            var blog = new BlogTemplate
            {
                Posts = new List<BlogPost>
                {
                    new BlogPost {
                        PostTitle = "1",
                        Comments = new List<Comment>
                        {
                            new Comment { Text = "hi" }
                        }
                    },
                    new BlogPost { PostTitle = "2" },
                    new BlogPost { PostTitle = "3" }
                }
            };

            var result = _mimeo.Render("newtemplate", blog);
            result.ShouldEqual("1 ...hi 2 3 ");
        }

        [Test]
        public void Stencil_GetContents_handles_block_with_many_nested_blocks_and_simple_replacements()
        {
            const string template = "{Posts}{PostTitle} {Comments}...{CommentText} {/Comments}{/Posts}";

            var stencil = _mimeo.CreateStencil("newtemplate", template);

            var blog = new BlogTemplate
            {
                Posts = new List<BlogPost>
                {
                    new BlogPost {
                        PostTitle = "1",
                        Comments = new List<Comment>
                        {
                            new Comment { Text = "hi1" },
                            new Comment { Text = "hi2" },
                            new Comment { Text = "hi3" }
                        }
                    },
                    new BlogPost { PostTitle = "2" },
                    new BlogPost { PostTitle = "3" }
                }
            };

            var result = _mimeo.Render("newtemplate", blog);
            result.ShouldEqual("1 ...hi1 ...hi2 ...hi3 2 3 ");
        }
    }
}
