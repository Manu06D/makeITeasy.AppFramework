using FluentAssertions;

using System;
using System.Linq;

using Xunit;

namespace makeITeasy.CarCatalog.dotnet8.Tests
{
    public class Misc_Tests
    {

        [Fact]
        public void ArraySlicing_Tests()
        {
            string[] source = new[] { "one", "two", "three", "four", "five" };
            string[] result = new string[2];

            source.Skip(1).Take(2).Should().HaveCount(2).And.ContainInOrder("two", "three");

            Array.Copy(source, 1, result, 0, 2);
            result.Should().HaveCount(2).And.ContainInOrder("two", "three");

            ArraySegment<string> segment = new ArraySegment<string>(source);
            segment.Slice(1, 2).Should().HaveCount(2).And.ContainInOrder("two", "three");

            ReadOnlySpan<string> span = new ReadOnlySpan<string>(source);
            span.Slice(1, 2).ToArray().Should().HaveCount(2).And.ContainInOrder("two", "three");

            result = source[1..3];
            result.Should().HaveCount(2).And.ContainInOrder("two", "three");
        }
    }
}
