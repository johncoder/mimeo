# mimeo
A library for .NET that lets you build your own templating syntax.

Example:

```c#
Mimeograph<BlogTemplate> mimeo = new Mimeograph<BlogTemplate>(b => {
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

string template = "{BlogTitle}";
using (var stream = template.ToStream())
{
    mimeo.CreateStencil("newtemplate", stream);
}

mimeo.Render("newtemplate", new BlogTemplate{BlogTitle = "asdf"}).ShouldEqual("asdf");
```
