using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mimeo.Design;
using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using NUnit.Framework;
using Should;
using Mimeo.Templating;
using System;

namespace Mimeo.Tests
{
    [TestFixture]
    public class ManualInputParserTest
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
                    ctx.Tokenize(d => d.Comments, @"{Comments}")
                        .AsBlock(commentContext =>
                        {
                            commentContext.Tokenize(c => c.Email, @"{Comment.Email}");
                            commentContext.Tokenize(c => c.Author, @"{Comment.Author}");
                            commentContext.Tokenize(c => c.Text, @"{Comment.Text}");
                        }).EndsWith(@"{/Comments}");
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
        public void ManualInputParser_Parse_should_return_single_simple_negative_space()
        {
            var inputParser = new ManualInputParser("{PageTitle}");
            
            var stencil = inputParser.Parse(_builder.Token);
            
            stencil.Count().ShouldEqual(1);
        }

        [Test]
        public void ManualInputParser_Parse_should_return_single_positive_space()
        {
            var inputParser = new ManualInputParser("asdf");
            var stencil = inputParser.Parse(_builder.Token);
            stencil.Count().ShouldEqual(1);
        }

        [Test]
        public void ManualInputParser_should_return_several_tokenmatches()
        {
            var inputParser = new ManualInputParser(_template);

            var stencil = inputParser.Parse(_builder.Token);

            stencil.Any().ShouldBeTrue();
        }

        [Test]
        public void ManualInputParser_CurrentIsToken_should_be_true()
        {
            const string template = "...{PageTitle}...";

            var inputParser = new ManualInputParser(template);

            inputParser.CurrentIsToken(3, "{PageTitle}").ShouldBeTrue();
        }

        [Test]
        public void ManualInputParser_CurrentIsToken_should_be_false()
        {
            const string template = "...{PageTitle}...";

            var inputParser = new ManualInputParser(template);

            inputParser.CurrentIsToken(1, "{PageTitle}").ShouldBeFalse();
        }

        [Test]
        public void ManualInputParser_Parse_returns_3_results()
        {
            const string template = "...{PageTitle}...";

            var inputParser = new ManualInputParser(template);

            var stencil = inputParser.Parse(_builder.Token);

            stencil.Count().ShouldEqual(3);
            stencil.ElementAt(0).ShouldBeType<Positive>();
            stencil.ElementAt(1).ShouldBeType<SimpleNegative>();
            stencil.ElementAt(2).ShouldBeType<Positive>();
        }

        [Test]
        public void ManualInputParser_Parse_returns_3_results_for_token_literal_token()
        {
            const string template = "{PageTitle}...{PageTitle}";

            var inputParser = new ManualInputParser(template);

            var stencil = inputParser.Parse(_builder.Token);

            stencil.Count().ShouldEqual(3);
            stencil.ElementAt(0).ShouldBeType<SimpleNegative>();
            stencil.ElementAt(1).ShouldBeType<Positive>();
            stencil.ElementAt(2).ShouldBeType<SimpleNegative>();
        }

        //[Test]
        //public void ManualInputParser_Parse_block_creates_complex_negative()
        //{
        //    const string template = @"{Post}...{Comments}...{Comment.Email}...{/Comments}...{/Post}";
        //    var inputParser = new ManualInputParser(template);
        //    var stencil = inputParser.Parse(_builder.Token);

        //    stencil.Any().ShouldBeTrue();
        //    stencil.Count().ShouldEqual(5);
        //}
    }
}
