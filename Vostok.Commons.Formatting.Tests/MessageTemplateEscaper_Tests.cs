using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Commons.Formatting.Tests
{
    [TestFixture]
    public class MessageTemplateEscaper_Tests
    {
        [Test]
        public void Should_EscapeBrackets_CasualUsage()
        {
            MessageTemplateEscaper.Escape
                    ("Session establishment complete on server {10.217.9.47:2181}, sessionid = 0x5047bed84ab9a42, negotiated timeout = 10000")
               .Should()
               .Be("Session establishment complete on server {{10.217.9.47:2181}}, sessionid = 0x5047bed84ab9a42, negotiated timeout = 10000");
        }

        [Test]
        public void Should_NotAllocateUselessMemory_WhenThereIsNothingToEscape()
        {
            // ReSharper disable once ConvertToConstant.Local
            var str = "casual string";
            MessageTemplateEscaper.Escape(str).Should().BeSameAs(str);
        }
    }
}