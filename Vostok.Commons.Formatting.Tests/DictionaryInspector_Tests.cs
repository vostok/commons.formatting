using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Formatting.Tests
{
    [TestFixture]
    internal class DictionaryInspector_Tests
    {
        [Test]
        public void IsSimpleDictionary_should_return_true_for_dictionaries_with_primitive_keys()
        {
            DictionaryInspector.IsSimpleDictionary(typeof(Dictionary<int, string>)).Should().BeTrue();
            DictionaryInspector.IsSimpleDictionary(typeof(Dictionary<char, string>)).Should().BeTrue();
            DictionaryInspector.IsSimpleDictionary(typeof(Dictionary<double, string>)).Should().BeTrue();
        }

        [Test]
        public void IsSimpleDictionary_should_return_true_for_dictionaries_with_string_keys()
        {
            DictionaryInspector.IsSimpleDictionary(typeof(Dictionary<string, string>)).Should().BeTrue();
        }

        [Test]
        public void IsSimpleDictionary_should_return_true_for_dictionaries_with_enum_keys()
        {
            DictionaryInspector.IsSimpleDictionary(typeof(Dictionary<DayOfWeek, string>)).Should().BeTrue();
        }

        [Test]
        public void IsSimpleDictionary_should_return_true_for_any_dictionaries_implementing_the_interface()
        {
            DictionaryInspector.IsSimpleDictionary(typeof(SortedList<int, string>)).Should().BeTrue();
            DictionaryInspector.IsSimpleDictionary(typeof(ReadOnlyDictionary<int, string>)).Should().BeTrue();
            DictionaryInspector.IsSimpleDictionary(typeof(ConcurrentDictionary<int, string>)).Should().BeTrue();
        }

        [Test]
        public void IsSimpleDictionary_should_return_false_for_dictionaries_with_complex_keys()
        {
            DictionaryInspector.IsSimpleDictionary(typeof(Dictionary<string[], string>)).Should().BeFalse();
        }

        [Test]
        public void IsSimpleDictionary_should_return_false_for_non_dictionary_types()
        {
            DictionaryInspector.IsSimpleDictionary(typeof(DictionaryInspector)).Should().BeFalse();
            DictionaryInspector.IsSimpleDictionary(typeof(List<string>)).Should().BeFalse();
            DictionaryInspector.IsSimpleDictionary(typeof(HashSet<int>)).Should().BeFalse();
        }

        [Test]
        public void EnumerateSimpleDictionary_should_correctly_enumerate_an_ordinary_dictionary()
        {
            var dictionary = new Dictionary<int, int> { [1] = 2, [3] = 4 };

            DictionaryInspector.EnumerateSimpleDictionary(dictionary)
                .Should().BeEquivalentTo(("1", 2), ("3", 4));
        }

        [Test]
        public void EnumerateSimpleDictionary_should_correctly_enumerate_a_readonly_dictionary()
        {
            var dictionary = new Dictionary<int, int> { [1] = 2, [3] = 4 };
            var roDictionary = new ReadOnlyDictionary<int, int>(dictionary);

            DictionaryInspector.EnumerateSimpleDictionary(roDictionary)
                .Should().BeEquivalentTo(("1", 2), ("3", 4));
        }

        [Test]
        public void EnumerateSimpleDictionary_should_correctly_enumerate_a_concurrent_dictionary()
        {
            var dictionary = new ConcurrentDictionary<int, int> { [1] = 2, [3] = 4 };

            DictionaryInspector.EnumerateSimpleDictionary(dictionary)
                .Should().BeEquivalentTo(("1", 2), ("3", 4));
        }
    }
}