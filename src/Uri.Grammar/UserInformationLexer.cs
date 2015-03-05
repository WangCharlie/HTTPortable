﻿namespace Uri.Grammar
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Text.Scanning;
    using UserInfoCharacter = Text.Scanning.Alternative<Unreserved, PercentEncoding, SubcomponentsDelimiter, Text.Scanning.Element>;

    public class UserInformationLexer : Lexer<UserInformation>
    {
        private readonly ILexer<Unreserved> unreservedLexer;

        private readonly ILexer<PercentEncoding> percentEncodingLexer;

        private readonly ILexer<SubcomponentsDelimiter> subcomponentsDelimiterLexer;

        public UserInformationLexer()
            : this(new UnreservedLexer(), new PercentEncodingLexer(), new SubcomponentsDelimiterLexer())
        {
        }

        public UserInformationLexer(ILexer<Unreserved> unreservedLexer, ILexer<PercentEncoding> percentEncodingLexer, ILexer<SubcomponentsDelimiter> subcomponentsDelimiterLexer)
            : base("userinfo")
        {
            Contract.Requires(unreservedLexer != null);
            Contract.Requires(percentEncodingLexer != null);
            Contract.Requires(subcomponentsDelimiterLexer != null);
            this.unreservedLexer = unreservedLexer;
            this.percentEncodingLexer = percentEncodingLexer;
            this.subcomponentsDelimiterLexer = subcomponentsDelimiterLexer;
        }

        public override bool TryRead(ITextScanner scanner, out UserInformation element)
        {
            var elements = new List<UserInfoCharacter>();
            var context = scanner.GetContext();
            while (!scanner.EndOfInput)
            {
                var innerContext = scanner.GetContext();
                Unreserved unreserved;
                if (this.unreservedLexer.TryRead(scanner, out unreserved))
                {
                    elements.Add(new UserInfoCharacter(unreserved, innerContext));
                }
                else
                {
                    PercentEncoding percentEncoding;
                    if (this.percentEncodingLexer.TryRead(scanner, out percentEncoding))
                    {
                        elements.Add(new UserInfoCharacter(percentEncoding, innerContext));
                    }
                    else
                    {
                        SubcomponentsDelimiter subcomponentsDelimiter;
                        if (this.subcomponentsDelimiterLexer.TryRead(scanner, out subcomponentsDelimiter))
                        {
                            elements.Add(new UserInfoCharacter(subcomponentsDelimiter, innerContext));
                        }
                        else
                        {
                            Element colon;
                            if (this.TryReadColon(scanner, out colon))
                            {
                                elements.Add(new UserInfoCharacter(colon, innerContext));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            element = new UserInformation(elements, context);
            return true;
        }

        private bool TryReadColon(ITextScanner scanner, out Element element)
        {
            if (scanner.EndOfInput)
            {
                element = default(Element);
                return false;
            }

            var context = scanner.GetContext();
            if (scanner.TryMatch(':'))
            {
                element = new Element(":", context);
                return true;
            }

            element = default(Element);
            return false;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.unreservedLexer != null);
            Contract.Invariant(this.percentEncodingLexer != null);
            Contract.Invariant(this.subcomponentsDelimiterLexer != null);
        }
    }
}
