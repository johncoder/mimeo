using NUnit.Framework;
using Mimeo.Design;
using System.Collections.Generic;
using System;

namespace Mimeo.Tests
{
	[TestFixture]
	public class StencilDesignerTest
	{
        private IStencilDesigner<BlogTemplate> _designer;

	    [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Designer_creates_simple_token_for_property()
        {
            _designer.Tokenize(b => b.BlogTitle, @"{PageTitle}");
            _designer.Tokenize(b => b.Posts, @"{Posts}")
                .AsBlock(postContext => {
                    postContext.Tokenize(d => d.PostTitle, @"{Post.Title}");
                    postContext.Tokenize(d => d.PostDescription, @"{Post.Description}");
                    postContext.Tokenize(d => d.PostBody, @"{Post.Body}").Encode(false);
                    postContext.Tokenize(d => d.PostedOn.ToShortDateString(), @"{Post.Date}");
                    postContext.Tokenize(d => d.Comments, @"{Comments}")
                        .AsBlock(commentContext => {
                            commentContext.Tokenize(c => c.Email, @"{Comment.Email}");
                            commentContext.Tokenize(c => c.Author, @"{Comment.Author}");
                            commentContext.Tokenize(c => c.Text, @"{Comment.Text}");
                        }).EndsWith(@"{/Comments}");
                }).EndsWith(@"{/Posts}");
        }

        class BlogTemplate
        {
            public string PageTitle { get; set; }
            public string BlogTitle { get; set; }
            public BlogPost Post { get; set; }
            public IEnumerable<BlogPost> Posts { get; set; }
        }

        class BlogPost
        {
            public string PostTitle { get; set; }
            public string PostBody { get; set; }
            public string PostDescription { get; set; }
            public DateTime PostedOn { get; set; }
            public IEnumerable<Comment> Comments { get; set; }
        }

        class Comment
        {
            public string Email { get; set; }
            public string Author { get; set; }
            public string Text { get; set; }
        }
	}
}
