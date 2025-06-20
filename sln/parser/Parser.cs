using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt.sln.ast;
using WallyArt.sln.instructions;
using WallyArt.sln.context;

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

        private static readonly Dictionary<(int, int), string> DireccionesValidas = new()
        {
            [(-1,-1)] = "diagonal arriba izquierda",
            [(-1,0)] = "izquierda",
            [(-1,1)] = "diagonal abajo izquierda",
            [(0,1)] = "abajo",
            [(1,1)] = "diagonal abajo derecha",
            [(1,0)] = "derecha",
            [(1,-1)] = "diagonal arriba derecha",
            [(0,-1)] = "arriba"
        };

        
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
            throw new Exception($"Expected string at line {Current.Line}");
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
                    if(!DireccionesValidas.ContainsKey((dx, dy)))
                    {
                        throw new Exception($"Direccion invalida ({dx}, {dy}) en la linea {Current.Line}");
                    }
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
                else if(Current.Value == "DrawCircle")
                {
                    Advance();
                    Expect("(");
                    int dx = ExpectNumber();
                    Expect(",");
                    int dy = ExpectNumber();
                    if(!DireccionesValidas.ContainsKey((dx, dy)))
                    {
                        throw new Exception($"Direccion invalida ({dx}, {dy}) en la linea {Current.Line}");
                    }
                    Expect(",");
                    int radius = ExpectNumber();
                    Expect(")");
                    instructions.Add(new DrawCircleI(dx, dy, radius, Current.Line));
                }
                else if(Current.Value == "DrawRectangle")
                {
                    Advance();
                    Expect("(");
                    int dx = ExpectNumber();
                    Expect(",");
                    int dy = ExpectNumber();
                    if(!DireccionesValidas.ContainsKey((dx, dy)))
                    {
                        throw new Exception($"Direccion invalida({dx}, {dy}) en la linea {Current.Line}");
                    }
                    Expect(",");
                    int distance = ExpectNumber();
                    Expect(",");
                    int width = ExpectNumber();
                    Expect(",");
                    int height = ExpectNumber();
                    Expect(")");
                    instructions.Add(new DrawRectangleI(dx, dy, distance, width, height, Current.Line));
                }
                else if (Current.Value == "Fill")
                {
                    Advance();
                    Expect("(");
                    Expect(")");
                    instructions.Add(new FillI(Current.Line));
                }
                else
                {
                    throw new Exception($"Unknow expresion '{Current.Value}' al line {Current.Line}");
                }
            }

            if (instructions.Count == 0 || instructions[0] is not SpawnI)
            {
                MessageBox.Show("Todo codigo valido debe empezar con la instruccion Spawn(X, Y)");
            }

            int spawns = instructions.Count(instr => instr is SpawnI);
            if (spawns != 1)
            {
                throw new Exception($"Solo se permite una instruccion Spawn fueron encontradas {spawns}");
            }
            return instructions;
        }
    }
}
