using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using System.Collections.Generic;
using Should;

namespace Mimeo.Sample.Tests
{
    [TestFixture]
    public class MainTests
    {
        protected DirectoryInfo InputDirectory { get; set; }
        protected DirectoryInfo OutputDirectory { get; set; }
        protected List<TestSet> TestSets { get; set; }

        private Mimeographs _mimeographs;

        /*
         File based testing:
         * 
         * Files---------------------------
         *      1) *.data.xml       - Xml Serialized Data for Fake Domain Object.
         *      2) *.template.html  - Input template containing tokens for replacement
         *      3) *.results.html   - Result statistics (i.e. Duration, length, etc.)
         *      4) *.output.html    - Raw output after calling .Render()
         
         */
        protected struct FileExtensions
        {
            public const string Data = ".data.xml";
            public const string Template = ".template.html";
            public const string Results = ".results.html";
            public const string ActualOutput = ".output.actual.html";
            public const string ExpectedOutput = ".output.expected.html";
        }

        protected class TestSet
        {
            public string Name { get; set; }
            public FileInfo Data { get; set; }
            public FileInfo Template { get; set; }
            public FileInfo ExpectedOutput { get; set; }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            InputDirectory  = new DirectoryInfo(Assembly.GetExecutingAssembly().Location.Replace("Mimeo.Sample.Tests.dll", "TestInput"));
            OutputDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location.Replace("Mimeo.Sample.Tests.dll", "TestOutput"));

            InputDirectory.Exists.ShouldBeTrue();
            if (!OutputDirectory.Exists) OutputDirectory.Create();
            else
            {
                var files = OutputDirectory.GetFiles();
                foreach(var file in files)
                    file.Delete();
            }

            var tests = from f in InputDirectory.GetFiles()
                        let testName = f.Name.Replace(FileExtensions.Data, "").Replace(FileExtensions.Template, "").Replace(FileExtensions.ExpectedOutput, "")
                        group f by testName into groups
                        select new TestSet
                        {
                            Name = groups.Key,
                            Data = groups.Single(x => x.Name.EndsWith(FileExtensions.Data)),
                            Template = groups.Single(x => x.Name.EndsWith(FileExtensions.Template)),
                            ExpectedOutput = groups.Single(x => x.Name.EndsWith(FileExtensions.ExpectedOutput))
                        };

            TestSets = tests.ToList();
        }

        [SetUp]
        public void SetUp()
        {
            ConfigureMimeographs();
        }

        private void ConfigureMimeographs()
        {
            _mimeographs = new Mimeographs();
            var blogTemplateMimeo = new Mimeograph<BlogTemplate>(b =>
            {
                b.Tokenize(p => p.BlogTitle, "{BlogTitle}");
                b.Tokenize(p => p.PageTitle, "{PageTitle}");
                b.Tokenize(p => p.JavaScriptIncludes, "{JavaScriptIncludes}");
                b.TokenizeIf(p => p.Post, "{Post}", p => p.Post != null, block => {
                    block.Tokenize(p => p.PostTitle, "{PostTitle}");
                    block.Tokenize(p => p.PostDescription, "{PostDescription}");
                    block.Tokenize(p => p.PostBody, "{PostBody}");
                    block.Block(p => p.Comments, "{Comments}", ctx => {
                        ctx.Tokenize(p => p.Text, "{CommentText}");
                    }).EndsWith("{/Comments}");
                }).EndsWith("{/Post}");
                b.Block(p => p.Posts, "{Posts}", block =>
                {
                    block.Tokenize(p => p.PostTitle, "{PostTitle}");
                    block.Tokenize(p => p.PostDescription, "{PostDescription}");
                    block.Tokenize(p => p.PostBody, "{PostBody}");
                    block.Block(p => p.Comments, "{Comments}", comments =>
                    {
                        comments.Tokenize(c => c.Author, "{Author}");
                        comments.Tokenize(c => c.Email, "{Email}");
                        comments.Tokenize(c => c.Text, "{CommentText}");
                    }).EndsWith("{/Comments}");
                }).EndsWith("{/Posts}");
            });
            _mimeographs.Add(blogTemplateMimeo);
        }

        [Test]
        public void ExecuteTestSets()
        {
            // add testing log stuff.

            var success = true;

            foreach(var test in TestSets)
            {
                var blogTemplate = test.Data.Deserialize<BlogTemplate>();
                var template = (new StreamReader(test.Template.OpenRead())).ReadToEnd();

                _mimeographs.CreateStencil<BlogTemplate>(test.Name, template);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var actualOutput = _mimeographs.Render(test.Name, blogTemplate);
                stopwatch.Stop();

                var resultsFile = new FileInfo("TestOutput\\" + test.Name + FileExtensions.Results);

                using (var resultsWriter = new StreamWriter(resultsFile.Open(FileMode.Append, FileAccess.Write, FileShare.Write)))
                {
                    resultsWriter.WriteLine("Completed in {0} milliseconds", stopwatch.ElapsedMilliseconds);
                }

                var actualOutputFile = new FileInfo("TestOutput\\" + test.Name + FileExtensions.ActualOutput);

                using (var actualOutputWriter = new StreamWriter(actualOutputFile.Open(FileMode.Append, FileAccess.Write, FileShare.Write)))
                {
                    actualOutputWriter.WriteLine(actualOutput);
                }

                var expectedOutput = File.ReadAllText(test.ExpectedOutput.FullName);

                try
                {
                    actualOutput.ShouldEqual(expectedOutput);
                }
                catch (Exception exception)
                {
                    // log exception
                    success = false;
                }
            }

            success.ShouldBeTrue();
        }
    }
}
