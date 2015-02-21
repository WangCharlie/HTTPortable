﻿using System.Diagnostics.Contracts;
using Text.Scanning;

namespace Http.Grammar.Rfc7230
{
    public class ObsText : Element
    {
        public ObsText(char data, ITextContext context)
            : base(data, context)
        {
            Contract.Requires(data >= '\u0080' && data <= '\u00FF');
        }
    }
}