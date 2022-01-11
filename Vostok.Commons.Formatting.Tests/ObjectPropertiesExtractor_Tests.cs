using System;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local
#pragma warning disable 414

namespace Vostok.Commons.Formatting.Tests
{
    [TestFixture]
    internal class ObjectPropertiesExtractor_Tests
    {
        [Test]
        public void Should_extract_properties_from_an_anonymous_object()
        {
            var obj = new {A = 1, B = 2};

            ObjectPropertiesExtractor.ExtractProperties(obj)
                .Should()
                .BeEquivalentTo(("A", 1), ("B", 2));

            var (count, props) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(obj);
            count.Should().Be(2);
            props.Should()
                .BeEquivalentTo(ObjectPropertiesExtractor.ExtractProperties(obj));
        }

        [Test]
        public void Should_extract_properties_from_a_custom_object()
        {
            var obj = new Container();

            ObjectPropertiesExtractor.ExtractProperties(obj)
                .Should()
                .BeEquivalentTo(("A", 1), ("B", 2));

            var (count, props) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(obj);
            count.Should().Be(2);
            props.Should()
                .BeEquivalentTo(ObjectPropertiesExtractor.ExtractProperties(obj));
        }

        [Test]
        public void Should_not_extract_private_properties()
        {
            var obj = new PrivateProperty();

            ObjectPropertiesExtractor.ExtractProperties(obj)
                .Should()
                .BeEmpty();

            var (count, props) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(obj);
            count.Should().Be(0);
            props.Should().BeEmpty();
        }

        [Test]
        public void Should_not_extract_public_fields()
        {
            var obj = new PublicField();

            ObjectPropertiesExtractor.ExtractProperties(obj)
                .Should()
                .BeEmpty();

            var (count, props) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(obj);
            count.Should().Be(0);
            props.Should().BeEmpty();
        }

        [Test]
        public void Should_not_extract_private_fields()
        {
            var obj = new PrivateField();

            ObjectPropertiesExtractor.ExtractProperties(obj)
                .Should()
                .BeEmpty();

            var (count, props) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(obj);
            count.Should().Be(0);
            props.Should().BeEmpty();
        }

        [Test]
        public void Should_return_error_messages_as_values_for_failing_properties()
        {
            var obj = new ThrowingProperty();

            ObjectPropertiesExtractor.ExtractProperties(obj)
                .Should()
                .Equal(("A", "<error: 123>"));

            var (count, props) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(obj);
            count.Should().Be(1);
            props.Should()
                .BeEquivalentTo(ObjectPropertiesExtractor.ExtractProperties(obj));
        }

        [Test]
        public void Should_support_properties_that_differ_by_case_only()
        {
            var obj = new {A = 1, a = 2};

            ObjectPropertiesExtractor.ExtractProperties(obj)
                .Should()
                .BeEquivalentTo(("A", 1), ("a", 2));

            var (count, props) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(obj);
            count.Should().Be(2);
            props.Should()
                .BeEquivalentTo(ObjectPropertiesExtractor.ExtractProperties(obj));
        }

        private class Container
        {
            public int A => 1;

            public int B => 2;
        }

        private class PrivateProperty
        {
            private int A => 1;
        }

        private class PublicField
        {
            public int A = 1;
        }

        private class PrivateField
        {
            private int A = 1;
        }

        private class ThrowingProperty
        {
            public int A => throw new Exception("123");
        }
    }
}