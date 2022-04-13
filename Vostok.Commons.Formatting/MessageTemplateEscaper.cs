using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Formatting
{
    /// <summary>
    /// Escapes given template according to https://vostok.gitbook.io/logging/concepts/syntax/message-templates
    /// </summary>
    [PublicAPI]
    internal class MessageTemplateEscaper
    {
        public static string Escape(string template)
        {
            var stringBuilder = new StringBuilder();

            foreach (var chr in template)
            {
                if (chr == '{' || chr == '}')
                    stringBuilder.Append(chr);
                stringBuilder.Append(chr);
            }

            return stringBuilder.ToString();
        }
    }
}