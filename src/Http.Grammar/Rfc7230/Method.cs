﻿using System.Diagnostics.Contracts;


namespace Http.Grammar.Rfc7230
{
    using SLANG;

    public class Method : Element
    {
        public Method(Token token, ITextContext context)
            : base(token.Data, context)
        {
            Contract.Requires(token != null);
        }
    }
}
