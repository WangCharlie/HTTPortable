﻿using System;
using System.Diagnostics.Contracts;
using Text.Scanning;

namespace Http.Grammar.Rfc7230
{
    public class FieldContentLexer : Lexer<FieldContent>
    {
        private readonly ILexer<FieldVisibleCharacter> fieldVisibleCharacterLexer;
        private readonly ILexer<RequiredWhiteSpace> requiredWhiteSpaceLexer;

        public FieldContentLexer()
            : this(new FieldVisibleCharacterLexer(), new RequiredWhiteSpaceLexer())
        {
        }

        public FieldContentLexer(ILexer<FieldVisibleCharacter> fieldVisibleCharacterLexer, ILexer<RequiredWhiteSpace> requiredWhiteSpaceLexer)
        {
            Contract.Requires(fieldVisibleCharacterLexer != null);
            Contract.Requires(requiredWhiteSpaceLexer != null);
            this.fieldVisibleCharacterLexer = fieldVisibleCharacterLexer;
            this.requiredWhiteSpaceLexer = requiredWhiteSpaceLexer;
        }

        public override FieldContent Read(ITextScanner scanner)
        {
            var context = scanner.GetContext();
            FieldContent element;
            if (this.TryRead(scanner, out element))
            {
                return element;
            }

            throw new SyntaxErrorException(context, "Expected 'field-content'");
        }

        public override bool TryRead(ITextScanner scanner, out FieldContent element)
        {
            if (scanner.EndOfInput)
            {
                element = default(FieldContent);
                return false;
            }

            var context = scanner.GetContext();
            FieldVisibleCharacter fieldVisibleCharacterLeft;
            if (!this.fieldVisibleCharacterLexer.TryRead(scanner, out fieldVisibleCharacterLeft))
            {
                element = default(FieldContent);
                return false;
            }

            RequiredWhiteSpace requiredWhiteSpace;
            if (this.requiredWhiteSpaceLexer.TryRead(scanner, out requiredWhiteSpace))
            {
                FieldVisibleCharacter fieldVisibleCharacterRight;
                if (this.fieldVisibleCharacterLexer.TryRead(scanner, out fieldVisibleCharacterRight))
                {
                    element = new FieldContent(fieldVisibleCharacterLeft, requiredWhiteSpace, fieldVisibleCharacterRight, context);
                    return true;
                }

                this.requiredWhiteSpaceLexer.PutBack(scanner, requiredWhiteSpace);
            }

            element = new FieldContent(fieldVisibleCharacterLeft, context);
            return true;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.fieldVisibleCharacterLexer != null);
            Contract.Invariant(this.requiredWhiteSpaceLexer != null);
        }

    }
}
