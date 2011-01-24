using System.Linq;
using System.Text;
using Mimeo.Design;
using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using NUnit.Framework;
using Should;
using Mimeo.Templating;

namespace Mimeo.Tests
{
    [TestFixture]
    public class ManualInputParserTest
    {
        private ITokenRoot<BlogTemplate> _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new TokenBuilder<BlogTemplate>();
            _builder.Tokenize(b => b.BlogTitle, @"{PageTitle}");
            _builder.Tokenize(b => b.JavaScriptIncludes, @"{JavaScriptIncludes}");
            _builder.TokenizeIf(b => b.Post, @"{Post}", b => b.Post != null, ctx => {
                    ctx.Tokenize(b => b.PostTitle, @"{PostTitle}");
                    ctx.Tokenize(b => b.PostDescription, @"{PostDescription}");
                    ctx.Tokenize(b => b.PostBody, @"{PostBody}");
                    ctx.Block(d => d.Comments, @"{Comments}", commentContext =>
                    {
                        commentContext.Tokenize(c => c.Email, @"{Comment.Email}");
                        commentContext.Tokenize(c => c.Author, @"{Comment.Author}");
                        commentContext.Tokenize(c => c.Text, @"{Comment.Text}");
                    }).EndsWith(@"{/Comments}");
                }).EndsWith(@"{/Post}");
            _builder.Block(b => b.Posts, @"{Posts}", postContext => {
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
        public void ManualInputParser_Parse_should_return_single_simple_negative_space()
        {
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, "{PageTitle}");
            
            stencil.Count().ShouldEqual(1);
        }

        [Test]
        public void ManualInputParser_Parse_should_return_single_positive_space()
        {
            IInputParser inputParser = new ManualInputParser();
            var stencil = inputParser.Parse(_builder.Token, "asdf");
            stencil.Count().ShouldEqual(1);
            stencil.Single().ShouldBeType<Positive>();
        }

        [Test]
        public void ManualInputParser_Parse_returns_3_results()
        {
            const string template = "...{PageTitle}...";

            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(3);
            stencil.ElementAt(0).ShouldBeType<Positive>();
            stencil.ElementAt(1).ShouldBeType<SimpleNegative>();
            stencil.ElementAt(2).ShouldBeType<Positive>();
        }

        [Test]
        public void ManualInputParser_Parse_returns_3_results_for_token_literal_token()
        {
            const string template = "{PageTitle}...{PageTitle}";

            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(3);
            stencil.ElementAt(0).ShouldBeType<SimpleNegative>();
            stencil.ElementAt(1).ShouldBeType<Positive>();
            stencil.ElementAt(2).ShouldBeType<SimpleNegative>();
        }

        [Test]
        public void ManualInputParser_Parse_block_creates_complex_negative()
        {
            const string template = @"{Post}......{/Post}";
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(1);
            stencil.Single().ShouldBeType<ComplexNegative<BlogTemplate, BlogPost>>();
            stencil.Single().CanHandle(new BlogTemplate()).ShouldBeTrue();
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().Single().Spaces.Count().ShouldEqual(1);
        }

        [Test]
        public void ManualInputParser_Parse_block_creates_two_complex_negative()
        {
            const string template = @"{Post}......{/Post}{Post}......{/Post}";
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(2);
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().Count().ShouldEqual(2);
            stencil.ElementAt(0).CanHandle(new BlogTemplate()).ShouldBeTrue();
            stencil.ElementAt(1).CanHandle(new BlogTemplate()).ShouldBeTrue();
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().ElementAt(0).Spaces.Count().ShouldEqual(1);
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().ElementAt(1).Spaces.Count().ShouldEqual(1);
        }

        [Test]
        public void ManualInputParser_Parse_block_creates_complex_positive_complex()
        {
            const string template = @"{Post}......{/Post}positive{Post}......{/Post}";
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(3);
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().Count().ShouldEqual(2);

            stencil.ElementAt(0).CanHandle(new BlogTemplate()).ShouldBeTrue();
            var element0 = stencil.ElementAt(0) as ComplexNegative<BlogTemplate, BlogPost>;
            element0.Spaces.Count().ShouldEqual(1);
            
            var element1 = stencil.ElementAt(1) as Positive;
            element1.ShouldNotBeNull();
            element1.Value.ShouldEqual("positive");

            stencil.ElementAt(2).CanHandle(new BlogTemplate()).ShouldBeTrue();
            var element2 = stencil.ElementAt(2) as ComplexNegative<BlogTemplate, BlogPost>;
            element2.Spaces.Count().ShouldEqual(1);
        }

        [Test]
        public void ManualInputParser_Parse_block_gets_nested_simple_negative()
        {
            const string template = @"{Post}{PostTitle}{/Post}";
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(1);
            var cn = stencil.ElementAt(0) as ComplexNegative<BlogTemplate, BlogPost>;
            cn.Spaces.Single().ShouldBeType<SimpleNegative>();
            var sb = new StringBuilder();
            cn.Spaces.First().GetContents(new BlogPost { PostTitle = "asdf" }, sb);
            sb.ToString().ShouldEqual("asdf");
        }

        [Test]
        public void ManualInputParser_Parse_block_creates_complex_nested_complex()
        {
            const string template = @"{Post}{Comments}...{/Comments}{/Post}";
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(1);
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().Single().Spaces.Count().ShouldEqual(1);
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().Single().Spaces.Single().ShouldBeType
                <ComplexNegative<BlogPost, Comment>>();
            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().Single().Spaces
                .OfType<ComplexNegative<BlogPost, Comment>>().Single().Spaces.Count().ShouldEqual(1);
        }

        [Test]
        public void ManualInputParser_Parse_block_creates_complex_nested_complex_surrounded_by_positives()
        {
            const string template = @"....{Post}{Comments}...{/Comments}{/Post}....";
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(3);
            var element0 = stencil.ElementAt(0) as Positive;
            element0.ShouldNotBeNull();
            element0.Value.ShouldEqual("....");

            var element1 = stencil.ElementAt(1) as ComplexNegative<BlogTemplate, BlogPost>;
            element1.ShouldNotBeNull();
            element1.Spaces.Count().ShouldEqual(1);
            var element1space = element1.Spaces.Single() as ComplexNegative<BlogPost, Comment>;
            element1space.ShouldNotBeNull();
            var element1spacespace = element1space.Spaces.Single() as Positive;
            element1spacespace.ShouldNotBeNull();
            element1spacespace.Value.ShouldEqual("...");

            stencil.OfType<ComplexNegative<BlogTemplate, BlogPost>>().Single().Spaces
                .OfType<ComplexNegative<BlogPost, Comment>>().Single().Spaces.Count().ShouldEqual(1);
            var element2 = stencil.ElementAt(2) as Positive;
            element2.ShouldNotBeNull();
            element2.Value.ShouldEqual("....");
        }

        [Test]
        public void ManualInputParser_Parse_creates_spaces_for_simple_inside_block_surrounded_by_1_char()
        {
            const string template = "{Posts}.{Post.Title}.{/Posts}";
            IInputParser inputParser = new ManualInputParser();
            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(1);
            var complexNeg = stencil.Single() as ComplexNegative<BlogTemplate, BlogPost>;
            complexNeg.Spaces.Count().ShouldEqual(3);
        }

        [Test]
        public void ManualInputParser_Parse_creates_spaces_for_simple_inside_block_surrounded_by_many_chars()
        {
            const string template = "{Posts}...{Post.Title}...{/Posts}";
            IInputParser inputParser = new ManualInputParser();
            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(1);
            var complexNeg = stencil.Single() as ComplexNegative<BlogTemplate, BlogPost>;
            complexNeg.Spaces.Count().ShouldEqual(3);
        }

        [Test]
        public void ManualInputParser_Parse_creates_spaces_for_simple_inside_adjacent_block_surrounded_by_many_chars()
        {
            const string template = "{Posts}...{Post.Title}...{/Posts}{Posts}...{Post.Title}...{/Posts}";
            IInputParser inputParser = new ManualInputParser();
            var stencil = inputParser.Parse(_builder.Token, template);

            stencil.Count().ShouldEqual(2);
            var complexNeg0 = stencil.ElementAt(0) as ComplexNegative<BlogTemplate, BlogPost>;
            complexNeg0.Spaces.Count().ShouldEqual(3);
            var complexNeg1 = stencil.ElementAt(1) as ComplexNegative<BlogTemplate, BlogPost>;
            complexNeg1.Spaces.Count().ShouldEqual(3);
        }
    }
}
