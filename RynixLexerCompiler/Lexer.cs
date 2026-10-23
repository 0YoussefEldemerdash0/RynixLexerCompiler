using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RynixLexerCompiler;



namespace RynixLexerCompiler
{
    public class ScanResult
    {
        public List<Token> Tokens { get; } = new List<Token>();
        public List<ScanError> Errors { get; } = new List<ScanError>();
    }

    public class Scanner
    {
        private static readonly Dictionary<string, TokenClass> Reserved = new Dictionary<string, TokenClass>
        {
            {"var", TokenClass.KW_VAR},       {"val", TokenClass.KW_VAL},
            {"fun", TokenClass.KW_FUN},       {"class", TokenClass.KW_CLASS},
            {"if", TokenClass.KW_IF},         {"else", TokenClass.KW_ELSE},
            {"while", TokenClass.KW_WHILE},   {"for", TokenClass.KW_FOR},
            {"in", TokenClass.KW_IN},         {"return", TokenClass.KW_RETURN},
            {"true", TokenClass.KW_TRUE},     {"false", TokenClass.KW_FALSE},
            {"null", TokenClass.KW_NULL},     {"new", TokenClass.KW_NEW},
            {"print", TokenClass.KW_PRINT},   {"input", TokenClass.KW_INPUT},
            {"break", TokenClass.KW_BREAK},   {"continue", TokenClass.KW_CONTINUE},
            {"init", TokenClass.KW_INIT},
            {"Int", TokenClass.KW_INT},       {"Float", TokenClass.KW_FLOAT},
            {"String", TokenClass.KW_STRING}, {"Bool", TokenClass.KW_BOOL},
            {"Char", TokenClass.KW_CHAR},     {"Void", TokenClass.KW_VOID},
            {"Array", TokenClass.KW_ARRAY}
        };

        private string _src = "";
        private int _pos, _line, _col;
        private ScanResult _result = new ScanResult();

        // -- ---- helpers ----------------
        private bool AtEnd => _pos >= _src.Length;
        private char Cur => _pos < _src.Length ? _src[_pos] : '\0';
        private char Peek(int n = 1) => _pos + n < _src.Length ? _src[_pos + n] : '\0';

        private static bool IsDigit(char c) => c >= '0' && c <= '9';
        private static bool IsLetter(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        private static bool IsIdStart(char c) => IsLetter(c) || c == '_';
        private static bool IsIdPart(char c) => IsLetter(c) || IsDigit(c) || c == '_';

        private void Advance()
        {
            if (AtEnd) return;
            if (_src[_pos] == '\n') { _line++; _col = 1; }
            else _col++;
            _pos++;
        }

        private void AddToken(TokenClass cls, int start, int line, int col) =>
            _result.Tokens.Add(new Token(cls, _src.Substring(start, _pos - start), line, col, start));

        private void AddError(string msg, int start, int line, int col) =>
            _result.Errors.Add(new ScanError(msg, _src.Substring(start, _pos - start), line, col, start));

        private bool PrevIsOperand()
        {
            if (_result.Tokens.Count == 0) return false;
            switch (_result.Tokens[_result.Tokens.Count - 1].Class)
            {
                case TokenClass.Identifier:
                case TokenClass.IntLiteral:
                case TokenClass.FloatLiteral:
                case TokenClass.StringLiteral:
                case TokenClass.CharLiteral:
                case TokenClass.RParen:
                case TokenClass.RBracket:
                case TokenClass.KW_TRUE:
                case TokenClass.KW_FALSE:
                case TokenClass.KW_NULL:
                    return true;
                default:
                    return false;
            }
        }

        // ---------------- scan --------- 
        public ScanResult Scan(string source)
        {
            _src = source ?? "";
            _pos = 0; _line = 1; _col = 1;
            _result = new ScanResult();

            while (!AtEnd)
            {
                char c = Cur;

              // whitespace
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n') { Advance(); continue; }

                // comments
                if (c == '/' && Peek() == '/') { while (!AtEnd && Cur != '\n') Advance(); continue; }
                if (c == '/' && Peek() == '*') { ScanBlockComment(); continue; }

                if (IsIdStart(c)) ScanWord();
                else if (IsDigit(c)) ScanNumber(false);
                else if (c == '-' && IsDigit(Peek()) && !PrevIsOperand()) ScanNumber(true);
                else if (c == '"') ScanString();
                else if (c == '\'') ScanChar();
                else ScanSymbol();
            }
            return _result;
        }

        // ---------------- comments---------
        private void ScanBlockComment()
        {
            int start = _pos, line = _line, col = _col;
            Advance(); Advance(); 
            while (!AtEnd && !(Cur == '*' && Peek() == '/')) Advance();
            if (AtEnd)
            {
                _result.Errors.Add(new ScanError("Unterminated multi-line comment", "/*", line, col, start));
                return;
            }
            Advance(); Advance(); 
        }

        // ---------------- identifiers / keywords ----------------
        private void ScanWord()
        {
            int start = _pos, line = _line, col = _col;
            while (IsIdPart(Cur)) Advance();
            string word = _src.Substring(start, _pos - start);
            AddToken(Reserved.TryGetValue(word, out TokenClass kw) ? kw : TokenClass.Identifier, start, line, col);
        }

        // --  numbers ----------------
        private void ScanNumber(bool negative)
        {
            int start = _pos, line = _line, col = _col;
            if (negative) Advance();

            int digitsStart = _pos;
            while (IsDigit(Cur)) Advance();
            string intPart = _src.Substring(digitsStart, _pos - digitsStart);

            bool isFloat = false;
            if (Cur == '.' && IsDigit(Peek()))
            {
                isFloat = true;
                Advance();
                while (IsDigit(Cur)) Advance();

                if (Cur == 'e' || Cur == 'E')
                {
                    int k = 1;
                    if (Peek(k) == '+' || Peek(k) == '-') k++;
                    if (IsDigit(Peek(k)))
                    {
                        for (int i = 0; i < k; i++) Advance();
                        while (IsDigit(Cur)) Advance();
                    }
                }
            }

            if (IsIdStart(Cur))
            {
                while (IsIdPart(Cur)) Advance();
                AddError("Invalid token: identifiers cannot start with a digit / malformed number", start, line, col);
                return;
            }

            if (!isFloat)
            {
                bool leadingZero = intPart.Length > 1 && intPart[0] == '0';
                bool negZero = negative && intPart == "0";
                if (leadingZero || negZero)
                {
                    AddError("Invalid integer literal (leading zeros are not allowed)", start, line, col);
                    return;
                }
            }

            AddToken(isFloat ? TokenClass.FloatLiteral : TokenClass.IntLiteral, start, line, col);
        }

        // ---------------- string / char ----------------
        private void ScanString()
        {
            int start = _pos, line = _line, col = _col;
            Advance(); 

            while (true)
            {
                if (AtEnd || Cur == '\n')
                {
                    AddError("Unterminated string literal", start, line, col);
                    return;
                }
                if (Cur == '\\')
                {
                    if (Peek() == '\0' || Peek() == '\n')
                    {
                        Advance();
                        AddError("Unterminated string literal", start, line, col);
                        return;
                    }
                    Advance(); Advance(); 
                    continue;
                }
                if (Cur == '"') { Advance(); break; }
                Advance();
            }
            AddToken(TokenClass.StringLiteral, start, line, col);
        }

        private void ScanChar()
        {
            int start = _pos, line = _line, col = _col;
            Advance(); 

            if (AtEnd || Cur == '\n')
            {
                AddError("Unterminated character literal", start, line, col);
                return;
            }

            if (Cur == '\'')
            {
                Advance();
                AddError("Empty character literal", start, line, col);
                return;
            }

            if (Cur == '\\')
            {
                if (Peek() == '\0' || Peek() == '\n')
                {
                    Advance();
                    AddError("Unterminated character literal", start, line, col);
                    return;
                }
                Advance(); Advance();
            }
            else Advance();

            if (Cur == '\'')
            {
                Advance();
                AddToken(TokenClass.CharLiteral, start, line, col);
                return;
            }

            while (!AtEnd && Cur != '\n' && Cur != '\'') Advance();
            if (Cur == '\'')
            {
                Advance();
                AddError("Character literal must contain exactly one character", start, line, col);
            }
            else AddError("Unterminated character literal", start, line, col);
        }

        // ---------------- operators & delimiters ----------------
        private void ScanSymbol()
        {
            int start = _pos, line = _line, col = _col;
            char c = Cur;
            char n = Peek();

            if (c == '=' && n == '=') { Advance(); Advance(); AddToken(TokenClass.Equal, start, line, col); return; }
            if (c == '!' && n == '=') { Advance(); Advance(); AddToken(TokenClass.NotEqual, start, line, col); return; }
            if (c == '<' && n == '=') { Advance(); Advance(); AddToken(TokenClass.LessEqual, start, line, col); return; }
            if (c == '>' && n == '=') { Advance(); Advance(); AddToken(TokenClass.GreaterEqual, start, line, col); return; }
            if (c == '.' && n == '.') { Advance(); Advance(); AddToken(TokenClass.Range, start, line, col); return; }

            TokenClass cls;
            switch (c)
            {
                case '+': cls = TokenClass.Plus; break;
                case '-': cls = TokenClass.Minus; break;
                case '*': cls = TokenClass.Multiply; break;
                case '/': cls = TokenClass.Divide; break;
                case '%': cls = TokenClass.Modulus; break;
                case '<': cls = TokenClass.LessThan; break;
                case '>': cls = TokenClass.GreaterThan; break;
                case '=': cls = TokenClass.Assign; break;
                case '(': cls = TokenClass.LParen; break;
                case ')': cls = TokenClass.RParen; break;
                case '{': cls = TokenClass.LBrace; break;
                case '}': cls = TokenClass.RBrace; break;
                case '[': cls = TokenClass.LBracket; break;
                case ']': cls = TokenClass.RBracket; break;
                case ':': cls = TokenClass.Colon; break;
                case ',': cls = TokenClass.Comma; break;
                case '.': cls = TokenClass.Dot; break;
                default:
                    Advance();
                    AddError(c == '!' ? "Unexpected '!' (did you mean '!=' ?)" : "Unknown character", start, line, col);
                    return;
            }
            Advance();
            AddToken(cls, start, line, col);
        }
    }
}