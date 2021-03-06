﻿using System;
using System.Collections.Generic;

namespace Mimeo.Sample.Tests
{
    [Serializable]
    public class BlogTemplate
    {
        public string PageTitle { get; set; }
        public string BlogTitle { get; set; }
        public BlogPost Post { get; set; }
        public List<BlogPost> Posts { get; set; }
        public string JavaScriptIncludes { get; set; }
    }

    [Serializable]
    public class BlogPost
    {
        public string PostTitle { get; set; }
        public string PostBody { get; set; }
        public string PostDescription { get; set; }
        public DateTime PostedOn { get; set; }
        public List<Comment> Comments { get; set; }
    }

    [Serializable]
    public class Comment
    {
        public string Email { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
    }
}
