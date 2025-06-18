using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WallyArt.sln.ast
{
    /* Convierte el teto del editor en una lista de tokens */
    internal class Lexer
    {
        public string[] lines;
        public int lineNumber;

        public Lexer(string code)
        {
            lines = code.Split('\n');
            lineNumber = 0;
        }

        /* Extrae todos los tokens de todas las lineas */
        public  List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            foreach (var line in lines)
            {
                lineNumber++;
                string trimeed = line.Trim();

                if (trimeed == "")
                {
                    continue;
                }

                /* Separa en partes usando un exprecion regular */
                var parts = Regex.Matches(trimeed, "\".*?\"|[a-zA-Z_][\\w\\-]*|\\d+|<-|==|<=|>=|!=|[+\\-*/%()\\[\\],]");

                foreach (Match part in parts)
                {
                    string val = part.Value;

                    if (int.TryParse(val, out _))
                    {
                        tokens.Add(new Token(TokenTipe.Number, val, lineNumber));
                    }
                    else if (val.StartsWith("\""))
                    {
                        tokens.Add(new Token(TokenTipe.String, val.Trim('"'), lineNumber));
                    }
                    else if (val == "<-" || val == "==" || val == "<=" || val == ">=" || val == "<" || val == ">")
                    {
                        tokens.Add(new Token(TokenTipe.Operator, val, lineNumber));
                    }
                    else if ("(),[]".Contains(val))
                    {
                        tokens.Add(new Token(TokenTipe.Symbol, val, lineNumber));
                    }
                    else if (Regex.IsMatch(val, "^[a-zA-Z_]\\w*$"))
                    {
                        tokens.Add(new Token(TokenTipe.Identifier, val, lineNumber));
                    }
                    else
                    {
                        throw new Exception($"Unknwn token '{val}' at line {lineNumber}");
                    }
                }
            }

            tokens.Add(new Token(TokenTipe.EOF, "EOF", lineNumber));  /* Token especial de fin */
            return tokens;
        }
        
    }
}
