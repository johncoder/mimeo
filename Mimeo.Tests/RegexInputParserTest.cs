using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mimeo.Design;
using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using NUnit.Framework;
using Should;

namespace Mimeo.Tests
{
    [TestFixture]
    public class RegexInputParserTest
    {
        private ITokenRoot<BlogTemplate> _builder;
        private string _template;

        [SetUp]
        public void SetUp()
        {
            _template = File.ReadAllText(Assembly.GetExecutingAssembly().Location.Replace("Mimeo.Tests.dll", "TestData\\Sample1.txt"));
            _builder = new TokenBuilder<BlogTemplate>();
            _builder.Tokenize(b => b.BlogTitle, @"{PageTitle}");
            _builder.Tokenize(b => b.Post, @"{Post}", b => b.Post != null)
                .AsBlock(ctx =>
                {
                    ctx.Tokenize(b => b.PostTitle, @"{Title}");
                    ctx.Tokenize(b => b.PostDescription, @"{Description}");
                    ctx.Tokenize(b => b.PostBody, @"{PostBody}");
                }).EndsWith(@"{/Post}");
            _builder.Tokenize(b => b.Posts, @"{Posts}")
                .AsBlock(postContext =>
                {
                    postContext.Tokenize(d => d.PostTitle, @"{Post.Title}");
                    postContext.Tokenize(d => d.PostDescription, @"{Post.Description}");
                    postContext.Tokenize(d => d.PostBody, @"{Post.Body}").Encode(false);
                    postContext.Tokenize(d => d.PostedOn.ToShortDateString(), @"{Post.Date}");
                    postContext.Tokenize(d => d.Comments, @"{Comments}")
                        .AsBlock(commentContext =>
                        {
                            commentContext.Tokenize(c => c.Email, @"{Comment.Email}");
                            commentContext.Tokenize(c => c.Author, @"{Comment.Author}");
                            commentContext.Tokenize(c => c.Text, @"{Comment.Text}");
                        }).EndsWith(@"{/Comments}");
                }).EndsWith(@"{/Posts}");
        }

        [Test]
        public void FileReadingTest()
        {
            _template.ShouldNotBeNull();
            _template.ShouldNotBeEmpty();
            (_template.Length > 10).ShouldBeTrue();
            Debug.WriteLine(_template);
        }

        [Test]
        public void RegexInputParser_should_return_several_tokenmatches()
        {
            var inputParser = new RegexInputParser(_template);

            inputParser.Parse(_builder.Token);

            inputParser.Matches.Any().ShouldBeTrue();
        }
    }
}
