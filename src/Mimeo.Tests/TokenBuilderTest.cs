using System.Linq;
using Mimeo.Design.Syntax;
using NUnit.Framework;
using Mimeo.Design;
using Should;

namespace Mimeo.Tests
{
	[TestFixture]
	public class TokenBuilderTest
	{
        private ITokenRoot<BlogTemplate> _builder;

	    [SetUp]
        public void SetUp()
        {
            _builder = new TokenBuilder<BlogTemplate>();
        }

        [Test]
        public void Builder_can_create_a_simple_token_and_two_blocks()
        {
            _builder.Tokenize(b => b.BlogTitle, @"{PageTitle}");
            _builder.TokenizeIf(b => b.Post, @"{Post}", b => b.Post != null, ctx => {
                    ctx.Tokenize(b => b.PostTitle, @"{Title}");
                    ctx.Tokenize(b => b.PostDescription, @"{Description}");
                    ctx.Tokenize(b => b.PostBody, @"{PostBody}");
                }).EndsWith(@"{/Post}");
            _builder.Block(b => b.Posts, @"{Posts}", postContext => {
                    postContext.Tokenize(d => d.PostTitle, @"{Post.Title}");
                    postContext.Tokenize(d => d.PostDescription, @"{Post.Description}");
                    postContext.Tokenize(d => d.PostBody, @"{Post.Body}").Encode(false);
                    postContext.Tokenize(d => d.PostedOn.ToShortDateString(), @"{Post.Date}");
                    postContext.Block(d => d.Comments, @"{Comments}", commentContext => {
                            commentContext.Tokenize(c => c.Email, @"{Comment.Email}");
                            commentContext.Tokenize(c => c.Author, @"{Comment.Author}");
                            commentContext.Tokenize(c => c.Text, @"{Comment.Text}");
                        }).EndsWith(@"{/Comments}");
                }).EndsWith(@"{/Posts}");

            _builder.Token.Children.Count().ShouldEqual(3);
        }

        //[Test]
        //public void Builder_can_be_created_with_little_extra_markup()
        //{
        //    /*
             
        //     var blah = Token.Create<BlogPost>(new [] {
        //        { Replacement = b => b.BlogTitle, Identifier = @"" },
        //        { Children = b => b.Posts, { { Replacement
        //     });
             
        //    */
        //}

        [Test]
        public void Calling_tokenize_should_add_one_child_token()
        {
            _builder.Token.Children.Clear();

            _builder.Tokenize(b => b.PageTitle, @"{PageTitle}");

            _builder.Token.Children.Count().ShouldEqual(1);
        }

        [Test]
        public void Calling_tokenize_asblock_should_add_one_child_token()
        {
            _builder.Token.Children.Clear();

            _builder.Block(b => b.Posts, @"{Posts}", ctx => { }).EndsWith(@"{/Posts}");

            _builder.Token.Children.Count().ShouldEqual(1);
        }

        [Test]
        public void Calling_tokenize_asblock_should_add_one_child_token_and_terminates()
        {
            _builder.Token.Children.Clear();

            _builder.Block(b => b.Posts, @"{Posts}", ctx => { }).EndsWith(@"{/Posts}");

            _builder.Token.Children.Count().ShouldEqual(1);
            _builder.Token.Children.First().Terminator.ShouldNotBeNull();
            _builder.Token.Children.First().Terminator.ShouldEqual(@"{/Posts}");
        }

        [Test]
        public void Calling_Interpolate_parses_token()
        {
            _builder.Interpolate("{ContentPage('", ".*", "')}", data => "success");
            var token = _builder.Token.Children.First();
            token.ShouldBeType<InterpolationToken<BlogTemplate>>();
            token.Interpolation.ShouldNotBeNull();
        }
    }
}
