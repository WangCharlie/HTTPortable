﻿namespace Http.Grammar
{
    using System;

    using TextFx;

    public class TokenLexer : Lexer<Token>
    {
        public override bool TryRead(ITextScanner scanner, Element previousElementOrNull, out Token element)
        {
            throw new NotImplementedException();
        }
    }
}