﻿using System.Diagnostics.Contracts;
using Text.Scanning;

namespace Http.Grammar.Rfc7230
{
    public class MethodToken : Element
    {
        private readonly Token token;

        public MethodToken(Token token, ITextContext context)
            : base(token.Data, context)
        {
            Contract.Requires(token != null);
            this.token = token;
        }

        public Token Token
        {
            get { return this.token; }
        }
    }
}
