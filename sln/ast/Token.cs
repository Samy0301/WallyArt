using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallyArt.sln.ast
{
    /* Representa un token individual con su tipo, valor y linea */
    internal class Token
    {
        public TokenTipe Type;
        public string Value;
        public int Line;
        
        public Token(TokenTipe type, string value, int line)
        {
            Type = type;
            Value = value;
            Line = line;
        }
    }

    /* Define los tipos de tokens que se pueden recoocer en el lenguaje*/
    public enum TokenTipe
    {
        Keyword,       /* Palabras clave como Swap, DrawLine, etc */
        Identifier,    /* Nombres de variables o etiquetas */
        Number,        /* Numeros enteros */   
        String,        /* Cadena de texto entre comillas */
        Symbol,        /* Caracteres como comas, parentesis, corchetes, etc */
        Operator,      /* Operadores <, >, ==, etc */
        EOF            /* Fin del archivo */
    }
}
