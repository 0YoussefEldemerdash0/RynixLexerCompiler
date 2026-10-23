using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace RynixLexerCompiler
{
    public enum TokenClass
    {
        KW_VAR, KW_VAL, KW_FUN, KW_CLASS, KW_IF, KW_ELSE, KW_WHILE, KW_FOR, KW_IN,
        KW_RETURN, KW_TRUE, KW_FALSE, KW_NULL, KW_NEW, KW_PRINT, KW_INPUT,
        KW_BREAK, KW_CONTINUE, KW_INIT,
        KW_INT, KW_FLOAT, KW_STRING, KW_BOOL, KW_CHAR, KW_VOID, KW_ARRAY,
        Identifier,
        IntLiteral, FloatLiteral, StringLiteral, CharLiteral,
        Plus, Minus, Multiply, Divide, Modulus,
        Equal, NotEqual, LessThan, GreaterThan, LessEqual, GreaterEqual,
        Assign, Range,
        LParen, RParen, LBrace, RBrace, LBracket, RBracket, Colon, Comma, Dot
    }

    public class Token
    {
        public TokenClass Class { get; }
        public string Lexeme { get; }
        public int Line { get; }
        public int Column { get; }
        public int Index { get; }

        public Token(TokenClass tokenClass, string lexeme, int line, int column, int index)
        {
            Class = tokenClass;
            Lexeme = lexeme;
            Line = line;
            Column = column;
            Index = index;
        }

        public override string ToString() => $"{Class}  '{Lexeme}'  (line {Line}, col {Column})";
    }

    public class ScanError
    {
        public string Message { get; }
        public string Lexeme { get; }
        public int Line { get; }
        public int Column { get; }
        public int Index { get; }

        public ScanError(string message, string lexeme, int line, int column, int index)
        {
            Message = message;
            Lexeme = lexeme;
            Line = line;
            Column = column;
            Index = index;
        }

        public override string ToString() =>
            $"Line {Line}, Col {Column}: {Message}" + (string.IsNullOrEmpty(Lexeme) ? "" : $"  -> {Lexeme}");
    }
}