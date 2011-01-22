using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimeo.Design;
using Mimeo.Design.Syntax;
using NUnit.Framework;

namespace Mimeo.Tests
{
    [TestFixture]
    public class StencilTests
    {
        private ITokenRoot<BlogTemplate> _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new TokenBuilder<BlogTemplate>();
            _builder.Tokenize(b => b.BlogTitle, @"{PageTitle}");
            _builder.Tokenize(b => b.JavaScriptIncludes, @"{JavaScriptIncludes}");
            _builder.Block(b => b.Post, @"{Post}", b => b.Post != null, ctx =>
            {
                ctx.Tokenize(b => b.PostTitle, @"{Title}");
                ctx.Tokenize(b => b.PostDescription, @"{Description}");
                ctx.Tokenize(b => b.PostBody, @"{PostBody}");
                ctx.Block(d => d.Comments, @"{Comments}", commentContext =>
                {
                    commentContext.Tokenize(c => c.Email, @"{Comment.Email}");
                    commentContext.Tokenize(c => c.Author, @"{Comment.Author}");
                    commentContext.Tokenize(c => c.Text, @"{Comment.Text}");
                }).EndsWith(@"{/Comments}");
            }).EndsWith(@"{/Post}");
            _builder.Block(b => b.Posts, @"{Posts}", postContext =>
            {
                postContext.Tokenize(d => d.PostTitle, @"{Post.Title}");
                postContext.Tokenize(d => d.PostDescription, @"{Post.Description}");
                postContext.Tokenize(d => d.PostBody, @"{Post.Body}").Encode(false);
                postContext.Tokenize(d => d.PostedOn.ToShortDateString(), @"{Post.Date}");
                postContext.Block(d => d.Comments, @"{Comments}", commentContext =>
                {
                    commentContext.Tokenize(c => c.Email, @"{Comment.Email}");
                    commentContext.Tokenize(c => c.Author, @"{Comment.Author}");
                    commentContext.Tokenize(c => c.Text, @"{Comment.Text}");
                }).EndsWith(@"{/Comments}");
            }).EndsWith(@"{/Posts}");
        }

        [Test]
        public void Stencil_GetContent_fills_single_positive()
        {
            const string template = "asdf";

            var mimeo = new Mimeo();
        }
    }
}
