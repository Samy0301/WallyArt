using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt.sln.ast;
using WallyArt.sln.instructions;

namespace WallyArt.sln.parser
{
    public class Parser
    {
        private List<Token> tokens;
        private int index;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.index = 0;
        }

        private Token Current => tokens[index];
        private void Advance() => index++;

        private bool Match(string value)
        {
            if (Current.Value == value)
            {
                Advance();
                return true;
            }
            return false;
        }

        private void Expect(string value)
        {
            if (!Match(value))
            {
                throw new Exception($"Excpected value at line {Current.Line}");
            }
        }

        private int ExpectNumber()
        {
            if (Current.Type == TokenType.Number)
            {
                int val = int.Parse(Current.Value);
                Advance();
                return val;
            }
            throw new Exception($"Expected number at line {Current.Line}");
        }

        private string ExpectString()
        {
            if (Current.Type == TokenType.String)
            {
                string val = Current.Value;
                Advance();
                return val;
            }
            throw new Exception($"Expected sting at line {Current.Line}");
        }

        public List<Instruction> Parse()
        {
            var instructions = new List<Instruction>();

            while (Current.Type != TokenType.EOF)
            {
                if (Current.Value == "Spawn")
                {
                    Advance();
                    Expect("(");
                    int x = ExpectNumber();
                    Expect(",");
                    int y = ExpectNumber();
                    Expect(")");
                    instructions.Add(new SpawnI(x, y, Current.Line));
                }
                else if (Current.Value == "Color")
                {
                    Advance();
                    Expect("(");
                    string color = ExpectString();
                    Expect(")");
                    instructions.Add(new ColorI(color, Current.Line));
                }
                else if (Current.Value == "DrawLine")
                {
                    Advance();
                    Expect("(");
                    int dx = ExpectNumber();
                    Expect(",");
                    int dy = ExpectNumber();
                    Expect(",");
                    int dist = ExpectNumber();
                    Expect(")");
                    instructions.Add(new DrawLineI(dx, dy, dist, Current.Line));
                }
                else if (Current.Value == "Size")
                {
                    Advance();
                    Expect("(");
                    int k = ExpectNumber();
                    Expect(")");
                    instructions.Add(new SizeI(k, Current.Line));
                }
                else
                {
                    throw new Exception($"Unknow expresion '{Current.Value}' al line {Current.Line}");
                }
            }
            return instructions;
        }
    }
}
