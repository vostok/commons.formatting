using System;
using System.Net;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Formatting.Tests
{
    [TestFixture]
    internal class ToStringDetector_Tests
    {
        [TestCase(typeof(int))]
        [TestCase(typeof(double))]
        [TestCase(typeof(Guid))]
        [TestCase(typeof(string))]
        public void HasCustomToString_should_return_true_on_primitive_types(Type type)
        {
            ToStringDetector.HasCustomToString(type).Should().BeTrue();
        }

        [TestCase(typeof(DateTime))]
        [TestCase(typeof(DateTimeOffset))]
        [TestCase(typeof(Uri))]
        [TestCase(typeof(IPAddress))]
        public void HasCustomToString_should_return_true_on_well_known_formattable_types_types(Type type)
        {
            ToStringDetector.HasCustomToString(type).Should().BeTrue();
        }

        [Test]
        public void HasCustomToString_should_return_true_on_custom_reference_types_with_overriden_tostring()
        {
            ToStringDetector.HasCustomToString(typeof(ReferenceTypeWithToString)).Should().BeTrue();
        }

        [Test]
        public void HasCustomToString_should_return_true_on_custom_value_types_with_overriden_tostring()
        {
            ToStringDetector.HasCustomToString(typeof(ValueTypeWithToString)).Should().BeTrue();
        }

        [Test]
        public void HasCustomToString_should_return_true_on_enums()
        {
            ToStringDetector.HasCustomToString(typeof(EnumExample)).Should().BeTrue();
        }

        [Test]
        public void HasCustomToString_should_return_false_on_anonymous_types()
        {
            var value = new {A = 1, B = 2};

            ToStringDetector.HasCustomToString(value.GetType()).Should().BeFalse();
        }

        [Test]
        public void HasCustomToString_should_return_false_on_custom_reference_types_with_inherited_tostring()
        {
            ToStringDetector.HasCustomToString(typeof(ReferenceTypeWithoutToString)).Should().BeFalse();
        }

        [Test]
        public void HasCustomToString_should_return_false_on_custom_value_types_with_inherited_tostring()
        {
            ToStringDetector.HasCustomToString(typeof(ValueTypeWithoutToString)).Should().BeFalse();
        }

        [Test]
        public void HasCustomToString_should_return_false_on_interface_types()
        {
            ToStringDetector.HasCustomToString(typeof(IFormattable)).Should().BeFalse();
        }

        private enum EnumExample
        {
        }

        private class ReferenceTypeWithToString
        {
            public override string ToString() => string.Empty;
        }

        private class ReferenceTypeWithoutToString
        {
        }

        private struct ValueTypeWithToString
        {
            public override string ToString() => string.Empty;
        }

        private struct ValueTypeWithoutToString
        {
        }
    }
}