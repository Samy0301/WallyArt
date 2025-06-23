using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallyArt.sln.ast
{
    /* Represent a individual token with type, value and line */
    public class Token
    {
        public TokenType Type;
        public string Value;
        public int Line;
        
        public Token(TokenType type, string value, int line)
        {
            Type = type;
            Value = value;
            Line = line;
        }
    }

    /* Define the types of tokens that the lenguage can recognize */
    public enum TokenType
    {
        Keyword,       /* Reserv words like Swap, DrawLine, etc */
        Identifier,    /* Names of labels or variables */
        Number,        /* Numbers */   
        String,        /* Text between "" */
        Symbol,        /* Symbols like ,, (), [], etc  */
        Operator,      /* Operators like <, >, ==, etc */
        EOF            /* File end */
    }
}
