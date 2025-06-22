using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt.sln.ast;
using WallyArt.sln.instructions;
using WallyArt.sln.context;
using System.Text.RegularExpressions;
using WallyArt.sln.ast;
using WallyArt.sln.expression;

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

        private Token Current => index < tokens.Count ? tokens[index] : tokens[^1];

        private static readonly Dictionary<(int, int), string> DireccionesValidas = new()
        {
            [(-1, -1)] = "diagonal arriba izquierda",
            [(-1, 0)] = "izquierda",
            [(-1, 1)] = "diagonal abajo izquierda",
            [(0, 1)] = "abajo",
            [(1, 1)] = "diagonal abajo derecha",
            [(1, 0)] = "derecha",
            [(1, -1)] = "diagonal arriba derecha",
            [(0, -1)] = "arriba"
        };

        public Token Peek(int offset = 1) => index + offset < tokens.Count ? tokens[index + offset] : tokens[^1];

        private void Advance() => index++;

        private static readonly HashSet<string> ReservNames = new()
        {
            "Spawn",
            "Color",
            "Size",
            "DrawLine",
            "DrawCircle",
            "DrawRectangle",
            "Fill",
            "GetActualX",
            "GetActualY",
            "GetCanvasSize",
            "GetColorCount",
            "IsBrushColor",
            "IsBrushSize",
            "IsCanvasColor"
        };

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
                throw new Exception($"Erroe at line {Current.Line}: Expected a value");
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
            throw new Exception($"Error at line {Current.Line}: Expected number");
        }

        private string ExpectString()
        {
            if (Current.Type == TokenType.String)
            {
                string val = Current.Value;
                Advance();
                return val;
            }
            throw new Exception($"Error at line {Current.Line}: Expected string");
        }

        public List<Instruction> Parse()
        {
            var instructions = new List<Instruction>();

            while (Current.Type != TokenType.EOF)
            {
                if (Current.Type == TokenType.Identifier && Peek().Value == "<-")
                {
                    string var = Current.Value;

                    /* Nombre invalido si empieza con - o numero */
                    if (char.IsDigit(var[0]) || !char.IsLetter(var[0]))
                    {
                        throw new Exception($"Error at line {Current.Line}: A variable name can't begin whit a number or the symbol -");
                    }

                    /* Nombre invalido si es una palabra reservada */
                    if (ReservNames.Contains(var))
                    {
                        throw new Exception($"Error at line {Current.Line}: Can't use the reserved word {var} for name a variable");
                    }

                    Advance();
                    Expect("<-");
                    IExpression expr = ParseExpression();
                    instructions.Add((new VariableAssignI(var, expr, Current.Line)));
                }
                else if (Current.Value == "Spawn")
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
                    if (!DireccionesValidas.ContainsKey((dx, dy)))
                    {
                        throw new Exception($"Error at line {Current.Line}: ({dx}, {dy}) is an invalid direction");
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
                else if (Current.Value == "DrawCircle")
                {
                    Advance();
                    Expect("(");
                    int dx = ExpectNumber();
                    Expect(",");
                    int dy = ExpectNumber();
                    if (!DireccionesValidas.ContainsKey((dx, dy)))
                    {
                        throw new Exception($"Error at line {Current.Line}: ({dx}, {dy}) is an invalid direction");
                    }
                    Expect(",");
                    int radius = ExpectNumber();
                    Expect(")");
                    instructions.Add(new DrawCircleI(dx, dy, radius, Current.Line));
                }
                else if (Current.Value == "DrawRectangle")
                {
                    Advance();
                    Expect("(");
                    int dx = ExpectNumber();
                    Expect(",");
                    int dy = ExpectNumber();
                    if (!DireccionesValidas.ContainsKey((dx, dy)))
                    {
                        throw new Exception($"Error at line {Current.Line}: ({dx}, {dy}) is an invalid direction");
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
                    throw new Exception($"Error at line {Current.Line}: Unknow expression '{Current.Value}'");
                }
            }

            if (instructions.Count == 0 || instructions[0] is not SpawnI)
            {
                MessageBox.Show("All valid code must begin whit a Spawn(X, Y) instruction");
            }

            int spawns = instructions.Count(instr => instr is SpawnI);
            if (spawns != 1)
            {
                throw new Exception($"Error: You can only use one Spawn instruction, were found {spawns}");
            }
            return instructions;
        }

        private IExpression ParseExpression()
        {
            if (Current.Type == TokenType.Number)
            {
                int val = int.Parse(Current.Value);
                Advance();
                return new ConstantExpression(val);
            }
            else if (Current.Type == TokenType.Identifier)
            {
                string name = Current.Value;
                Advance();

                if (name == "GetActualX")
                {
                    Expect("(");
                    Expect(")");
                    return new GetActualXE();
                }
                else if (name == "GetActualY")
                {
                    Expect("(");
                    Expect(")");
                    return new GetActualYE();
                }
                else if (name == "GetCanvasSize")
                {
                    Expect("(");
                    Expect(")");
                    return new GetCanvasSizeE();
                }
                else if (name == "GetColorCount")
                {
                    Expect("(");
                    string color = ExpectString();
                    Expect(",");
                    int x1 = ExpectNumber();
                    Expect(",");
                    int y1 = ExpectNumber();
                    Expect(",");
                    int x2 = ExpectNumber();
                    Expect(",");
                    int y2 = ExpectNumber();
                    Expect(")");
                    return new GetColorCountE(color, x1, y1, x2, y2);
                }
                else if (name == "IsBrushColor")
                {
                    Expect("(");
                    string color = ExpectString();
                    Expect(")");
                    return new IsBrushColorE(color);
                }
                else if (name == "IsBrushSize")
                {
                    Expect("(");
                    int size = ExpectNumber();
                    Expect(")");
                    return new IsBrushSizeE(size);
                }
                else if (name == "IsCanvasColor")
                {
                    Expect("(");
                    string color = ExpectString();
                    Expect(",");
                    int vertical = ExpectNumber();
                    Expect(",");
                    int horizontal = ExpectNumber();
                    Expect(")");
                    return new IsCanvasColorE(color, vertical, horizontal);
                }
                else
                {
                    return new VariableExpression(name);
                }
            }
            throw new Exception($"Error at line {Current.Line}: Invalid expression"); 
        }
    }
}
