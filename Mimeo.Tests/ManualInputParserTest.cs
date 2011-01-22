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
        public void ManualInputParser_should_return_several_tokenmatches()
        {
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, _template);

            stencil.Any().ShouldBeTrue();
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
        public void Try_ManualInputParser_on_template_read_from_file()
        {
            IInputParser inputParser = new ManualInputParser();

            var stencil = inputParser.Parse(_builder.Token, _template);

            stencil.Count().ShouldEqual(9);
            var last = stencil.Last() as Positive;
            last.Value.ToCharArray().ShouldEqual(_template.Reverse().Take(last.Value.Length).Reverse().ToArray());
        }
    }
}
