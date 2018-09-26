using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Formatting.Tests
{
    [TestFixture]
    internal class StringBuilderCache_Tests
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(16)]
        [TestCase(100)]
        [TestCase(1000)]
        public void Acquire_should_always_provide_a_builder_of_at_least_requested_capacity(int capacity)
        {
            StringBuilderCache.Acquire(capacity).Capacity.Should().BeGreaterOrEqualTo(capacity);
        }

        [Test]
        public void Acquire_should_hand_out_different_instances_when_called_multiple_times_in_a_row_from_same_thread()
        {
            var builder1 = StringBuilderCache.Acquire(10);
            var builder2 = StringBuilderCache.Acquire(10);
            var builder3 = StringBuilderCache.Acquire(10);

            builder1.Should().NotBeSameAs(builder2);
            builder1.Should().NotBeSameAs(builder3);
        }

        [Test]
        public void Acquire_should_cache_and_reuse_returned_instances()
        {
            var builder = StringBuilderCache.Acquire(10);

            for (var i = 0; i < 10; i++)
            {
                StringBuilderCache.Release(builder);

                StringBuilderCache.Acquire(10).Should().BeSameAs(builder);
            }
        }

        [Test]
        public void Acquire_should_clean_up_builder_before_handing_it_out()
        {
            var builder = StringBuilderCache.Acquire(10);

            builder.Append("123");

            StringBuilderCache.Release(builder);

            StringBuilderCache.Acquire(10).Length.Should().Be(0);
        }
    }
}
