﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Text.Scanning;
using Text.Scanning.Core;

namespace Http.Grammar.Rfc7230
{
    public class OWSLexer : Lexer<OWSToken>
    {
        private readonly HorizontalTabLexer hTabLexer;
        private readonly SpaceLexer SpaceLexer;

        public OWSLexer()
            : this(new SpaceLexer(), new HorizontalTabLexer())
        {
        }

        public OWSLexer(SpaceLexer SpaceLexer, HorizontalTabLexer hTabLexer)
        {
            Contract.Requires(SpaceLexer != null);
            Contract.Requires(hTabLexer != null);
            this.SpaceLexer = SpaceLexer;
            this.hTabLexer = hTabLexer;
        }

        public override OWSToken Read(ITextScanner scanner)
        {
            var context = scanner.GetContext();
            OWSToken token;
            if (TryRead(scanner, out token))
            {
                return token;
            }

            throw new SyntaxErrorException(context, "Expected 'OWS'");
        }

        public override bool TryRead(ITextScanner scanner, out OWSToken token)
        {
            if (scanner.EndOfInput)
            {
                token = default(OWSToken);
                return false;
            }

            var context = scanner.GetContext();
            IList<WhiteSpace> elements = new List<WhiteSpace>();
            for (;;)
            {
                Space sp;
                if (SpaceLexer.TryRead(scanner, out sp))
                {
                    elements.Add(new WhiteSpace(sp, context));
                }
                else
                {
                    HorizontalTab hTab;
                    if (hTabLexer.TryRead(scanner, out hTab))
                    {
                        elements.Add(new WhiteSpace(hTab, context));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            token = new OWSToken(elements, context);
            return true;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.SpaceLexer != null);
            Contract.Invariant(this.hTabLexer != null);
        }
    }
}