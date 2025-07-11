﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WallyArt.sln.ast
{
    /* Change the text from the editor to a token list */
    public class Lexer
    {
        public string[] lines;
        public int lineNumber;

        public Lexer(string code)
        {
            lines = code.Split('\n');
            lineNumber = 0;
        }

        /* Take all tokens from all lines  */
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

                /* Curt in parts using a regular expresion */
                var parts = Regex.Matches(trimeed, "\".*?\"|[a-zA-Z_][\\w\\-]*|-?\\d+|<-|==|<=|>=|!=|\\|\\||&&|\\*\\*|[+\\-*/%()\\[\\],<>]");

                foreach (Match part in parts)
                {
                    string val = part.Value;

                    if (int.TryParse(val, out _))
                    {
                        tokens.Add(new Token(TokenType.Number, val, lineNumber));
                    }
                    else if (val.StartsWith("\""))
                    {
                        tokens.Add(new Token(TokenType.String, val.Trim('"'), lineNumber));
                    }
                    else if (val == "<-" || val == "==" || val == "<=" || val == ">=" || val == "<" || val == ">" || val == "!=" || val == "*" || val == "**" || val == "&&" || val == "||" || val == "+" || val == "-" || val == "/" || val == "%")
                    {
                        tokens.Add(new Token(TokenType.Operator, val, lineNumber));
                    }
                    else if ("+-*/%".Contains(val))
                    {
                        tokens.Add(new Token(TokenType.Operator, val, lineNumber));
                    }
                    else if ("(),[]".Contains(val))
                    {
                        tokens.Add(new Token(TokenType.Symbol, val, lineNumber));
                    }
                    else if (Regex.IsMatch(val, @"^[a-zA-Z_][\w\-]*$"))
                    {
                        tokens.Add(new Token(TokenType.Identifier, val, lineNumber));
                    }
                    else
                    {
                        throw new Exception($" Line {lineNumber}: Unknown token '{val}'");
                    }
                }
            }

            tokens.Add(new Token(TokenType.EOF, "EOF", lineNumber));  /* Especial token of end */
            return tokens;
        }
        
    }
}
