namespace Http.Grammar
{
    using System;

    using TextFx;

    public class FieldContentLexer : Lexer<FieldContent>
    {
        public override bool TryRead(ITextScanner scanner, Element previousElementOrNull, out FieldContent element)
        {
            throw new NotImplementedException();
        }
    }
}