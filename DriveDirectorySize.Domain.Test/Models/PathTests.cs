using DriveDirectorySize.Domain.Models;
using System;
using Xunit;

namespace DriveDirectorySize.Domain.Test.Models
{
    public class PathTests
    {
        [Theory]
        [InlineData(@"c:\one\two\three", "three")]
        [InlineData(@"c:\", "c:")]
        [InlineData(@"c:\one\two", "two")]
        public void IdentityWillBeTheLastDirectoryInThePath(string fullPath, string expected)
        {
            var path = new Path(fullPath);

            Assert.Equal(expected, path.Identity);
        }

        [Theory]
        [InlineData(@"c:\one\two\three", "two")]
        [InlineData(@"c:\one", "c:")]
        [InlineData(@"c:\one\two", "one")]
        public void ParentWillBeTheSecondToLastDirectoryInThePath(string fullPath, string expected)
        {
            var path = new Path(fullPath);

            Assert.Equal(expected, path.Parent);
        }

        [Theory]
        [InlineData(@"c:\", 1)]
        [InlineData(@"c:\test", 2)]
        [InlineData(@"c:\test\this\out", 4)]
        public void LengthWillBeLengthOfParts(string fullPath, int expected)
        {
            var path = new Path(fullPath);

            Assert.Equal(expected, path.Length);
        }

        [Theory]
        [InlineData(@"c:\", 0)]
        [InlineData(@"c:\test", 1)]
        [InlineData(@"c:\test\this\out", 3)]
        public void DepthWillBeLengthOfPartsMinusOne(string fullPath, int expected)
        {
            var path = new Path(fullPath);

            Assert.Equal(expected, path.Depth);
        }

        [Fact]
        public void ParentWillBeEmptyIfRootDirectory()
        {
            var path = new Path(@"c:\");

            Assert.Equal("", path.Parent);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FullPathCannotBeNull(string fullPath)
        {
            Assert.Throws<ArgumentNullException>(() => new Path(fullPath));
        }

        [Theory]
        [InlineData(@"c:\one\two\three", @"c:\one\two")]
        public void ParentPathWillBeEntirePathExceptIdentity(string fullPath, string expected)
        {
            var path = new Path(fullPath);

            Assert.Equal(expected, path.ParentPath);
        }

        [Fact]
        public void ParentPathWillBeEmptyIfRootDirectory()
        {
            var path = new Path(@"c:\");

            Assert.Equal("", path.ParentPath);
        }

        [Fact]
        public void FullPathWillHaveTrailingDividerRemoved()
        {
            var fullPath = @"c:\test\";
            var expected = @"c:\test";

            var path = new Path(fullPath);

            Assert.Equal(expected, path.FullPath);
        }

        [Fact]
        public void RootWillBeTheFirstDirectoryInFullPath()
        {
            var fullPath = @"myroot\notroot\anothernotroot";
            var expected = "myroot";

            var path = new Path(fullPath);

            Assert.Equal(expected, path.Root);
        }

        [Theory]
        [InlineData(@"first\second\third\fourth\", new[] { "first", "second", "third", "fourth" })]
        [InlineData(@"first\second", new[] { "first", "second" })]
        [InlineData(@"first", new[] { "first" })]
        public void PartsWillBeFullPathParsedIntoCorrectDirectoryPlaces(string fullPath, string[] expected)
        {
            var path = new Path(fullPath);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], path.Parts[i]);
            }
        }

        [Theory]
        [InlineData(@"first\second\third", 0, "first")]
        [InlineData(@"first\second\third", 1, "second")]
        [InlineData(@"first\second\third", 2, "third")]
        [InlineData(@"first\second\third", -1, "")]
        [InlineData(@"first\", 1, "")]
        public void IdentityAtDepthWillReturnIdentityAtGivenDepth(string fullPath, int depth, string expected)
        {
            var path = new Path(fullPath);

            var result = path.IdentityAtDepth(depth);

            Assert.Equal(expected, result);
        }

        public void IsDescendantOfRequiresAPath()
        {
            var path = new Path("test");
            Assert.Throws<ArgumentNullException>(() => path.IsDescendantOf(null));
        }

        [Theory]
        [InlineData(@"first\second", @"first\second\third")]
        [InlineData(@"first\second", @"first\second\third\fourth")]
        [InlineData(@"first", @"first\second\third\fourth")]
        public void IsDescendantOfWillReturnTrueWhenComparedPathIsAnAncestor(string compareFullPath, string currentFullPath)
        {
            var comparePath = new Path(compareFullPath);
            var currentPath = new Path(currentFullPath);

            Assert.True(currentPath.IsDescendantOf(comparePath));
        }

        [Theory]
        [InlineData(@"first\second", @"first\third")]
        [InlineData(@"first\second\third\fourth", @"first\second")]
        [InlineData(@"first", @"other\second\")]
        public void IsDescendantOfWillReturnFalseWhenComparedPathIsNotAnAncestor(string compareFullPath, string currentFullPath)
        {
            var comparePath = new Path(compareFullPath);
            var currentPath = new Path(currentFullPath);

            Assert.False(currentPath.IsDescendantOf(comparePath));
        }

        public void DefaultsToFullPath()
        {
            var fullPath = @"c:\test\this";
            var path = new Path(fullPath);

            Assert.Equal(fullPath, path.ToString());
        }

        public void ImplicitStringIsFullPath()
        {
            var fullPath = @"c:\test\this";
            var path = new Path(fullPath);

            Assert.Equal(fullPath, path);
        }
    }
}
