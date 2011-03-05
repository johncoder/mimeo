using System;
using System.Collections.Generic;

namespace Mimeo.Tests
{
    class BlogTemplate
    {
        public string PageTitle { get; set; }
        public string BlogTitle { get; set; }
        public BlogPost Post { get; set; }
        public IEnumerable<BlogPost> Posts { get; set; }
        public string JavaScriptIncludes { get; set; }
        public string ContentPageUrl { get; set; }
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
