﻿using System;
using System.Globalization;
using System.Linq;
using Http.Content_Length;
using Http.Headers;
using Http.Transfer_Encoding;
using Txt;

namespace Http
{
    /// <summary>Provides extension methods for the <see cref="T:Http.IHeaderCollection" /> interface.</summary>
    public static class ExtensionsForIHeaderCollection
    {
        /// <summary>
        /// Gets and converts the string representation of the 'Content-Length' header to its 64-bit signed integer
        /// equivalent. A return value indicates whether the header was present and that the conversion succeeded or failed.
        /// </summary>
        /// <param name="instance">The instance to extend.</param>
        /// <param name="result">
        /// When this method returns, contains the 64-bit signed integer value equivalent of the
        /// 'Content-Length' header, if the operation succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
        public static bool TryGetContentLength(this IHeaderCollection instance, out long result)
        {
            foreach (var header in instance)
            {
                if (!string.Equals(header.Name, ContentLengthHeader.FieldName, StringComparison.Ordinal))
                {
                    continue;
                }

                var value = header.SingleOrDefault();
                if (value == null)
                {
                    result = default(long);
                    return false;
                }

                var lexer = new ContentLengthLexer();
                using (ITextScanner scanner = new TextScanner(new StringTextSource(value)))
                {
                    var readResult = lexer.Read(scanner);
                    if (!readResult.Success)
                    {
                        continue;
                    }

                    if (long.TryParse(readResult.Element.Text, NumberStyles.None, NumberFormatInfo.InvariantInfo, out result))
                    {
                        return true;
                    }
                }
            }

            result = default(long);
            return false;
        }


        public static bool TryGetTransferEncoding(this IHeaderCollection instance, out TransferEncodingHeader result)
        {
            const string HeaderField = "Transfer-Encoding";
            foreach (var header in instance)
            {
                if (!string.Equals(header.Name, HeaderField, StringComparison.Ordinal))
                {
                    continue;
                }

                var value = string.Join(",", header);
                var lexer = new TransferEncodingLexer();
                using (ITextScanner scanner = new TextScanner(new StringTextSource(value)))
                {
                    var readResult = lexer.Read(scanner);
                    if (!readResult.Success)
                    {
                        break;
                    }

                    var elements = readResult.Element.Elements;
                    result = new TransferEncodingHeader(elements.Select(coding => coding.Text).ToList());
                    return true;
                }
            }

            result = default(TransferEncodingHeader);
            return false;
        }
    }
}