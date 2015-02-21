﻿using Text.Scanning;
using Text.Scanning.Core;

namespace Http.Grammar.Rfc7230
{
    public class HttpVersionLexer : Lexer<HttpVersionToken>
    {
        private readonly ILexer<HttpNameToken> httpNameLexer;
        private readonly ILexer<Digit> digitLexer;

        public HttpVersionLexer()
            : this(new HttpNameLexer(), new DigitLexer())
        {
        }

        public HttpVersionLexer(ILexer<HttpNameToken> httpNameLexer, ILexer<Digit> digitLexer)
        {
            this.httpNameLexer = httpNameLexer;
            this.digitLexer = digitLexer;
        }

        public override HttpVersionToken Read(ITextScanner scanner)
        {
            HttpNameToken httpName;
            Digit digit1;
            Digit digit2;
            var context = scanner.GetContext();
            try
            {
                httpName = this.httpNameLexer.Read(scanner);
                var slashContext = scanner.GetContext();
                if (!scanner.TryMatch('/'))
                {
                    throw new SyntaxErrorException(slashContext, "Expected '/'");
                }

                digit1 = this.digitLexer.Read(scanner);
                var dotContext = scanner.GetContext();
                if (!scanner.TryMatch('.'))
                {
                    throw new SyntaxErrorException(dotContext, "Expected '.'");
                }

                digit2 = this.digitLexer.Read(scanner);
            }
            catch (SyntaxErrorException syntaxErrorException)
            {
                throw new SyntaxErrorException(context, "Expected 'HTTP-version'", syntaxErrorException);
            }

            return new HttpVersionToken(httpName, digit1, digit2, context);
        }

        public override bool TryRead(ITextScanner scanner, out HttpVersionToken element)
        {
            HttpNameToken httpName;
            Digit digit1;
            Digit digit2;
            var context = scanner.GetContext();
            if (!this.httpNameLexer.TryRead(scanner, out httpName))
            {
                element = default(HttpVersionToken);
                return false;
            }

            if (!scanner.TryMatch('/'))
            {
                this.httpNameLexer.PutBack(scanner, httpName);
                element = default(HttpVersionToken);
                return false;
            }

            if (!this.digitLexer.TryRead(scanner, out digit1))
            {
                scanner.PutBack('/');
                this.httpNameLexer.PutBack(scanner, httpName);
                element = default(HttpVersionToken);
                return false;
            }

            if (!scanner.TryMatch('.'))
            {
                this.digitLexer.PutBack(scanner, digit1);
                scanner.PutBack('/');
                this.httpNameLexer.PutBack(scanner, httpName);
                element = default(HttpVersionToken);
                return false;
            }

            if (!this.digitLexer.TryRead(scanner, out digit2))
            {
                scanner.PutBack('.');
                this.digitLexer.PutBack(scanner, digit1);
                scanner.PutBack('/');
                this.httpNameLexer.PutBack(scanner, httpName);
                element = default(HttpVersionToken);
                return false;
            }

            element = new HttpVersionToken(httpName, digit1, digit2, context);
            return true;
        }
    }
}
