using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Vostok.Commons.Formatting.Tests
{
    [TestFixture]
    internal class ObjectValueFormatter_Tests
    {
        [Test]
        public void Should_format_null_value_into_empty_string()
        {
            Format(null).Should().BeEmpty();
        }

        [Test]
        public void Should_write_string_value_as_is()
        {
            var value = Guid.NewGuid().ToString();

            Format(value).Should().Be(value);
        }

        [Test]
        public void Should_use_provided_format_for_formattable_values()
        {
            Format(123, "D5").Should().Be("00123");
        }

        [TestCase(true, "True")]
        [TestCase(false, "False")]
        [TestCase((byte)123, "123")]
        [TestCase((sbyte)123, "123")]
        [TestCase((short)123, "123")]
        [TestCase((ushort)123, "123")]
        [TestCase(123, "123")]
        [TestCase(123L, "123")]
        [TestCase((ushort)123, "123")]
        [TestCase((uint)123L, "123")]
        [TestCase((ulong)123L, "123")]
        [TestCase('a', "a")]
        [TestCase((float)3.14, "3.14")]
        [TestCase(3.14d, "3.14")]
        public void Should_format_primitive_types_with_tostring_and_invariant_culture(object value, string expected)
        {
            Format(value).Should().Be(expected);
        }

        [Test]
        public void Should_format_guids_with_tostring()
        {
            var value = Guid.NewGuid();

            Format(value).Should().Be(value.ToString());
        }

        [Test]
        public void Should_format_timespans_with_tostring_and_invariant_culture()
        {
            var value = 5.Hours();

            Format(value).Should().Be(value.ToString(null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void Should_format_datetimes_with_tostring_and_invariant_culture()
        {
            var value = DateTime.Now;

            Format(value).Should().Be(value.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void Should_format_datetimeoffsets_with_tostring_and_invariant_culture()
        {
            var value = DateTimeOffset.Now;

            Format(value).Should().Be(value.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void Should_format_uris_with_tostring()
        {
            var value = new Uri("http://kontur.ru");

            Format(value).Should().Be(value.ToString());
        }

        [Test]
        public void Should_format_ip_addresses_with_tostring()
        {
            var value = IPAddress.Loopback;

            Format(value).Should().Be(value.ToString());
        }

        [Test]
        public void Should_format_value_tuples_with_tostring()
        {
            var value = (1, 2, 3);

            Format(value).Should().Be("(1, 2, 3)");
        }

        [Test]
        public void Should_format_ordinary_tuples_with_tostring()
        {
            var value = Tuple.Create(1, 2, 3);

            Format(value).Should().Be("(1, 2, 3)");
        }

        [Test]
        public void Should_format_reference_types_defining_custom_tostring_with_it()
        {
            var value = new ClassWithToString();

            Format(value).Should().Be("123");
        }

        [Test]
        public void Should_format_value_types_defining_custom_tostring_with_it()
        {
            var value = new StructWithToString();

            Format(value).Should().Be("123");
        }

        [Test]
        public void Should_format_flat_dictionaries_with_primitive_keys_as_json()
        {
            var value = new Dictionary<int, string>
            {
                [1] = "value1",
                [2] = "value2"
            };

            Format(value).Should().Be("{\"1\": \"value1\", \"2\": \"value2\"}");
        }

        [Test]
        public void Should_format_flat_dictionaries_with_string_keys_as_json()
        {
            var value = new Dictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            };

            Format(value).Should().Be("{\"key1\": \"value1\", \"key2\": \"value2\"}");
        }

        [Test]
        public void Should_format_flat_dictionaries_with_enum_keys_as_json()
        {
            var value = new Dictionary<DayOfWeek, string>
            {
                [DayOfWeek.Friday] = "value1",
                [DayOfWeek.Saturday] = "value2"
            };

            Format(value).Should().Be("{\"Friday\": \"value1\", \"Saturday\": \"value2\"}");
        }

        [Test]
        public void Should_format_readonly_dictionaries_as_ordinary_ones()
        {
            var value = new Dictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            };

            var readonlyValue = new ReadOnlyDictionary<string, string>(value);

            Format(readonlyValue).Should().Be("{\"key1\": \"value1\", \"key2\": \"value2\"}");
        }

        [Test]
        public void Should_format_flat_arrays_as_json_arrays()
        {
            var value = new[] {1, 2, 3};

            Format(value).Should().Be("[\"1\", \"2\", \"3\"]");
        }

        [Test]
        public void Should_format_flat_lists_as_json_arrays()
        {
            var value = new List<int> {1, 2, 3};

            Format(value).Should().Be("[\"1\", \"2\", \"3\"]");
        }

        [Test]
        public void Should_format_lazy_enumerables_as_json_arrays()
        {
            var value = Enumerable.Range(1, 3);

            Format(value).Should().Be("[\"1\", \"2\", \"3\"]");
        }

        [Test]
        public void Should_format_objects_with_public_properties_as_json_objects()
        {
            var value = new {key1 = "value1", key2 = "value2"};

            Format(value).Should().Be("{\"key1\": \"value1\", \"key2\": \"value2\"}");
        }

        [Test]
        public void Should_format_objects_without_any_recognizable_features_with_their_type_name()
        {
            Format(new object()).Should().Be(typeof(object).FullName);
        }

        [Test]
        public void Should_format_nested_objects_with_public_properties_as_json_objects()
        {
            var value = new {A = 1, B = new {C = 2, D = 3}};

            Format(value).Should().Be("{\"A\": \"1\", \"B\": {\"C\": \"2\", \"D\": \"3\"}}");
        }

        [Test]
        public void Should_format_nested_null_properties_explicitly_when_using_json_formatting()
        {
            var value = new {A = 1, B = null as object};

            Format(value).Should().Be("{\"A\": \"1\", \"B\": null}");
        }

        [Test]
        public void Should_format_nested_properties_with_custom_tostring_when_using_json_formatting()
        {
            var value = new {A = 1, B = new ClassWithToString()};

            Format(value).Should().Be("{\"A\": \"1\", \"B\": \"123\"}");
        }

        [Test]
        public void Should_format_nested_objects_with_depth_limit()
        {
            object value = 15;
            for (int i = 0; i < 10; i++)
                value = new {X = i, Y = value};

            Format(value).Should().Be(@"{""X"": ""9"", ""Y"": {""X"": ""8"", ""Y"": {""X"": ""7"", ""Y"": {""X"": ""6"", ""Y"": {""X"": ""5"", ""Y"": {""X"": ""4"", ""Y"": {""X"": ""3"", ""Y"": {""X"": ""2"", ""Y"": {""X"": ""1"", ""Y"": {""X"": ""<too deep>"", ""Y"": ""<too deep>""}}}}}}}}}}");
        }

        [Test]
        public void Should_not_fail_on_objects_with_circular_dependencies()
        {
            var value1 = new ClassWithDependency();
            var value2 = new ClassWithDependency();

            value1.Prop = value2;
            value2.Prop = value1;

            Format(value1).Should().Be(@"{""Prop"": {""Prop"": {""Prop"": {""Prop"": {""Prop"": {""Prop"": {""Prop"": {""Prop"": {""Prop"": {""Prop"": ""<too deep>""}}}}}}}}}}");
        }

        [Test]
        public void Should_format_objects_with_nested_enumerables_as_json()
        {
            var value = new {A = new[] {1, 2}, B = new[] {3, 4}};

            Format(value).Should().Be("{\"A\": [\"1\", \"2\"], \"B\": [\"3\", \"4\"]}");
        }

        [Test]
        public void Should_format_objects_with_nested_dictionaries_as_json()
        {
            var value = new
            {
                A = new Dictionary<DayOfWeek, int> {[DayOfWeek.Friday] = 1},
                B = new Dictionary<DayOfWeek, int> {[DayOfWeek.Saturday] = 2}
            };

            Format(value).Should().Be("{\"A\": {\"Friday\": \"1\"}, \"B\": {\"Saturday\": \"2\"}}");
        }

        [Test]
        public void Should_format_enumerables_with_nested_objects_as_json()
        {
            var value = new[]
            {
                new {A = 1},
                new {A = 2}
            };

            Format(value).Should().Be("[{\"A\": \"1\"}, {\"A\": \"2\"}]");
        }

        [Test]
        public void Should_format_enumerables_with_nested_enumerables_as_json()
        {
            var value = new[]
            {
                new List<int> {1, 2},
                new List<int> {3, 4}
            };

            Format(value).Should().Be("[[\"1\", \"2\"], [\"3\", \"4\"]]");
        }

        [Test]
        public void Should_format_enumerables_with_nested_dictionaries_as_json()
        {
            var value = new[]
            {
                new Dictionary<string, int> {["A"] = 1},
                new Dictionary<string, int> {["A"] = 2}
            };

            Format(value).Should().Be("[{\"A\": \"1\"}, {\"A\": \"2\"}]");
        }

        [Test]
        public void Should_format_dictionaries_with_nested_enumerables_as_json()
        {
            var value = new Dictionary<int, int[]>
            {
                [1] = new[] {2, 2},
                [2] = new[] {4, 4}
            };

            Format(value).Should().Be("{\"1\": [\"2\", \"2\"], \"2\": [\"4\", \"4\"]}");
        }

        [Test]
        public void Should_format_dictionaries_with_nested_objects_as_json()
        {
            var value = new Dictionary<int, object>
            {
                [1] = new {A = null as string},
                [2] = new {A = "value"}
            };

            Format(value).Should().Be("{\"1\": {\"A\": null}, \"2\": {\"A\": \"value\"}}");
        }

        [Test]
        public void Should_format_dictionaries_with_nested_dictionaries_as_json()
        {
            var value = new Dictionary<int, object>
            {
                [1] = new Dictionary<string, object> {["A"] = null},
                [2] = new Dictionary<string, object> {["A"] = "value"}
            };

            Format(value).Should().Be("{\"1\": {\"A\": null}, \"2\": {\"A\": \"value\"}}");
        }

        private static string Format(object value, string format = null, IFormatProvider formatProvider = null)
        {
            return ObjectValueFormatter.Format(value, format, formatProvider);
        }

        private class ClassWithDependency
        {
            public object Prop { get; set; }
        }

        private class ClassWithToString
        {
            public override string ToString() => "123";
        }

        private struct StructWithToString
        {
            public override string ToString() => "123";
        }
    }
}