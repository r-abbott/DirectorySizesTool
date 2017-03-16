using DriveDirectorySize.Domain.Models;
using Xunit;

namespace DriveDirectorySize.Domain.Test.Models
{
    public class PathTests
    {
        [Theory]
        [InlineData(@"c:\one\two\three","three")]
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

        // TODO
        // ParentPath
        // FullPath
        // Root
        // PathToDepth
        // IdentityAtDepth
        // IsDescendentOf
        // IsChildOf
        // GetAncestorPath
        // Implicit Operator
    }
}
