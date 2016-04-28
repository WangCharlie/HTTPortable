﻿using System;
using Txt;
using Txt.ABNF;

namespace Http.comment
{
    public sealed class CommentLexer : Lexer<Comment>
    {
        private readonly ILexer<Concatenation> innerLexer;

        public CommentLexer(ILexer<Concatenation> innerLexer)
        {
            if (innerLexer == null)
            {
                throw new ArgumentNullException(nameof(innerLexer));
            }

            this.innerLexer = innerLexer;
        }

        public override ReadResult<Comment> Read(ITextScanner scanner)
        {
            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }
            var result = innerLexer.Read(scanner);
            if (result.Success)
            {
                return ReadResult<Comment>.FromResult(new Comment(result.Element));
            }
            return ReadResult<Comment>.FromSyntaxError(SyntaxError.FromReadResult(result, scanner.GetContext()));
        }
    }}