using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Should;
using Mimeo.Design;
using Mimeo.Templating.Formatting;
using System.Linq;

namespace Mimeo.Tests
{
    [TestFixture]
    public class StencilTests
    {
        private Mimeograph<BlogTemplate> _mimeo;

        [SetUp]
        public void SetUp()
        {
            _mimeo = new Mimeograph<BlogTemplate>(b => {
                b.Tokenize(p => p.BlogTitle, "{BlogTitle}");
                b.Tokenize(p => p.PageTitle, "{PageTitle}");
                b.Tokenize(p => p.JavaScriptIncludes, "{JavaScriptIncludes}");
                b.TokenizeIf(p => p.Post, "{Post}", p => p.Post != null, block => {
                    block.Tokenize(p => p.PostTitle, "{PostTitle}");
                    block.Tokenize(p => p.PostDescription, "{PostDescription}");
                    block.Tokenize(p => p.PostBody, "{PostBody}");
                }).EndsWith("{/Post}");
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

            var stream = template.ToStream();
            _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();

            _mimeo.Render("newtemplate", new BlogTemplate()).ShouldEqual("asdfasdf");
        }

        [Test]
        public void Stencil_GetContent_replaces_blogtitle()
        {
            const string template = "{BlogTitle}";

            var stream = template.ToStream();
            _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();

            _mimeo.Render("newtemplate", new BlogTemplate{BlogTitle = "asdf"}).ShouldEqual("asdf");
        }

        [Test]
        public void Stencil_GetContents_replaces_BlogTitle_and_preserves_plaintext()
        {
            const string template = "asdf{BlogTitle}asdf";

            var stream = template.ToStream();
            _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();

            _mimeo.Render("newtemplate", new BlogTemplate { BlogTitle = "ASDF" }).ShouldEqual("asdfASDFasdf");
        }

        [Test]
        public void Stencil_GetContents_handles_block_with_simple_replacement()
        {
            const string template = "{Post}{PostTitle}{/Post}";
            var stream = template.ToStream();
            var stencil = _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();
            var blog = new BlogTemplate { Post = new BlogPost { PostTitle = "asdf" } };
            var result = _mimeo.Render("newtemplate", blog);
            result.ShouldEqual("asdf");
        }

        [Test]
        public void Stencil_GetContents_handles_block()
        {
            const string template = "{Posts}.{/Posts}";
            var stream = template.ToStream();
            _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();
            var blog = new BlogTemplate { Posts = new List<BlogPost> { new BlogPost(), new BlogPost(), new BlogPost() } };
            var result = _mimeo.Render("newtemplate", blog);
            result.ShouldEqual("...");
        }

        [Test]
        public void Stencil_GetContents_handles_block_with_posttitles()
        {
            const string template = "{Posts}.{PostTitle}.{/Posts}";

            var stream = template.ToStream();
            var stencil = _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();

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

            var stream = template.ToStream();
            var stencil = _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();

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

            var stream = template.ToStream();
            var stencil = _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();

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

            var stream = template.ToStream();
            var stencil = _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();

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

        [Test]
        public void Interpolate_replaces_properly_with_different_parameters()
        {
            _mimeo.Builder.Interpolate("{ContentPage('", ".*", "')}", data => data.ToString());
            var stream = "{ContentPage('asdf')}{ContentPage('qwerty')}".ToStream();
            var stencil = _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();
            var sb = new StringBuilder();
            stencil.Render(new object(), sb);
            sb.ToString().ShouldEqual("asdfqwerty");
        }

        [Test]
        public void Interpolate_replaces_with_simple_negative()
        {
            _mimeo.Builder.Interpolate("{ContentPage('", ".*", "')}", data => data.ToString());
            var stream = "{BlogTitle} {ContentPage('asdf')} {ContentPage('qwerty')}".ToStream();
            _mimeo.CreateStencil("newtemplate", stream);
            stream.Dispose();
            var result = _mimeo.Render("newtemplate", new BlogTemplate { BlogTitle = "My Blog" });
            result.ShouldEqual("My Blog asdf qwerty");
        }

        [Test]
        public void TokenBuilder_with_value_formatters_uses_value_formatters()
        {
            var mimeograph = new Mimeographs();
            var builder = new BlogTemplateTokenBuilder();
            builder.Configure();
            mimeograph.Add(new Mimeograph<BlogTemplate>(builder));

            const string template = "{BlogTitle}";
            mimeograph.CreateStencil<BlogTemplate>("blah", template);
            var result = mimeograph.Render("blah", new BlogTemplate { BlogTitle = "Hi" });

            result.ShouldEqual("HI");
        }

        [Test]
        public void TokenBuilder_with_value_formatter_for_int_uses_value_formatter()
        {
            var mimeograph = new Mimeographs();
            var builder = new BlogTemplateTokenBuilder();
            builder.Configure();
            mimeograph.Add(new Mimeograph<BlogTemplate>(builder));
            mimeograph.CreateStencil<BlogTemplate>("blah", "{Id}");
            var result = mimeograph.Render("blah", new BlogTemplate { Id = 3 });
            result.ShouldEqual("3");
        }

        [Test]
        public void TokenBuilder_uses_value_formatters_properly()
        {
            var mimeograph = new Mimeographs();
            var builder = new BlogTemplateTokenBuilder();
            builder.Configure();
            mimeograph.Add(new Mimeograph<BlogTemplate>(builder));
            mimeograph.CreateStencil<BlogTemplate>("blah", "{Id}....{BlogTitle}");
            var result = mimeograph.Render("blah", new BlogTemplate { Id = 3, BlogTitle = "hi" });
            result.ShouldEqual("3....HI");
        }

        [Test]
        public void TokenBuilder_uses_value_formatters_properly_and_not_on_regular_text_in_template()
        {
            var mimeograph = new Mimeographs();
            var builder = new BlogTemplateTokenBuilder();
            builder.Configure();
            mimeograph.Add(new Mimeograph<BlogTemplate>(builder));
            mimeograph.CreateStencil<BlogTemplate>("blah", "{Id}....{BlogTitle}asdf");
            var result = mimeograph.Render("blah", new BlogTemplate { Id = 3, BlogTitle = "hi" });
            result.ShouldEqual("3....HIasdf");
        }

        [Test]
        public void TokenBuilder_does_not_use_skipped_formatters()
        {
            var mimeograph = new Mimeographs();
            var builder = new BlogTemplateTokenBuilder();
            builder.Configure();
            mimeograph.Add(new Mimeograph<BlogTemplate>(builder));
            mimeograph.CreateStencil<BlogTemplate>("blah", "{Id}....{SecondBlogTitle} asdf");
            var result = mimeograph.Render("blah", new BlogTemplate { Id = 3, BlogTitle = "hi" });
            result.ShouldEqual("3....hi asdf");
        }

        private class BlogTemplateTokenBuilder : TokenBuilder<BlogTemplate>
        {
            public void Configure()
            {
                UseFormatters(set =>
                {
                    set.Add<string>(s => s.ToUpper());
                    set.Add<int, ObjectFormatter>();
                    set.Add<object, ObjectFormatter>();
                });

                Tokenize(bp => bp.BlogTitle, "{SecondBlogTitle}", set =>
                {
                    set.SkipFormatter<string>();
                }); 
                Tokenize(p => p.BlogTitle, "{BlogTitle}");
                Tokenize(p => p.PageTitle, "{PageTitle}");
                Tokenize(p => p.JavaScriptIncludes, "{JavaScriptIncludes}");
                Tokenize(p => p.Id, "{Id}");
                TokenizeIf(p => p.Post, "{Post}", p => p.Post != null, block =>
                {
                    block.Tokenize(p => p.PostTitle, "{PostTitle}");
                    block.Tokenize(p => p.PostDescription, "{PostDescription}");
                    block.Tokenize(p => p.PostBody, "{PostBody}");
                }).EndsWith("{/Post}");
                Block(p => p.Posts, "{Posts}", block =>
                {
                    block.Tokenize(p => p.PostTitle, "{PostTitle}");
                    block.Tokenize(p => p.PostDescription, "{PostDescription}");
                    block.Tokenize(p => p.PostBody, "{PostBody}");
                    block.Block(p => p.Comments, "{Comments}", comments =>
                    {
                        comments.Tokenize(c => c.Author, "{Author}");
                        comments.Tokenize(c => c.Email, "{Email}");
                        comments.Tokenize(c => c.Text, "{CommentText}");
                    }).EndsWith("{/Comments}");
                }).EndsWith("{/Posts}");
            }

            public class ObjectFormatter : IValueFormatter
            {
                public string Format(object value)
                {
                    return value.ToString();
                }
            }
        }
    }
}
