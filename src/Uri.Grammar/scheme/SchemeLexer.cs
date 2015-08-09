﻿namespace Uri.Grammar
{
    using System;

    using TextFx;
    using TextFx.ABNF;

    public class SchemeLexer : Lexer<Scheme>
    {
        private readonly ILexer<Sequence> innerLexer;

        public SchemeLexer(ILexer<Sequence> innerLexer)
        {
            if (innerLexer == null)
            {
                throw new ArgumentNullException("innerLexer");
            }

            this.innerLexer = innerLexer;
        }

        public override bool TryRead(ITextScanner scanner, Element previousElementOrNull, out Scheme element)
        {
            Sequence result;
            if (this.innerLexer.TryRead(scanner, null, out result))
            {
                element = new Scheme(result);
                if (previousElementOrNull != null)
                {
                    previousElementOrNull.NextElement = element;
                    element.PreviousElement = previousElementOrNull;
                }

                return true;
            }

            element = default(Scheme);
            return false;
        }
    }
}