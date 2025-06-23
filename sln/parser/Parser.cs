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

        private static readonly Dictionary<(int, int), string> DireccionesValidas = new()    /* Valid directions */
        {
            [(-1, -1)] = "left and up",
            [(-1, 0)] = "left",
            [(-1, 1)] = "left and down",
            [(0, 1)] = "down",
            [(1, 1)] = "rigth and down",
            [(1, 0)] = "rigth",
            [(1, -1)] = "rigth and up",
            [(0, -1)] = "up"
        };

        private static readonly HashSet<string> ReservNames = new()      /* Reserv words for instructions and funtions */
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

        /* Give back the token in the current position without move the cursor */
        public Token Peek(int offset = 1) => index + offset < tokens.Count ? tokens[index + offset] : tokens[^1];

        /* Move the cursor to the next token */
        private void Advance() => index++;

        /* Give back true if the current value is {value} */
        private bool Match(string value)
        {
            if (Current.Value == value)
            {
                Advance();
                return true;
            }
            return false;
        }

        /* Give back true if value is the expeted */
        private void Expect(string value)
        {
            if (!Match(value))
            {
                throw new Exception($"Erroe at line {Current.Line}: Expected a value");
            }
        }

        /* Variation of Expect for numbers */
        private int ExpectNumber()
        {
            if (Current.Type == TokenType.Number)
            {
                int val = int.Parse(Current.Value);
                Advance();
                return val;
            }
            throw new Exception($"Error at line {Current.Line}: Expected a number but found this {Current.Value}");
        }

        /* Variation of Expect for words */
        private string ExpectString()
        {
            if (Current.Type == TokenType.String)
            {
                string val = Current.Value;
                Advance();
                return val;
            }
            throw new Exception($"Error at line {Current.Line}: Expected a string but found this {Current.Value}");
        }

        /* Variation of Expect for identifiers */
        private string ExpectIdentifier()
        {
            if (Current.Type == TokenType.Identifier)
            {
                string val = Current.Value;
                Advance();
                return val;
            }
            throw new Exception($"Error at line {Current.Line}: Expected a identifier but found this {Current.Value}");
        }

        public List<Instruction> Parse()             /* Parse for things in Instruction.cs whitout preferense order (well only the variables but for resons of declaration mode) */
        {
            var instructions = new List<Instruction>();

            while (Current.Type != TokenType.EOF)
            {
                if (Current.Type == TokenType.Identifier && Peek().Value == "<-")
                {
                    string var = Current.Value;

                    /* If begin with - or a number its an invalid variable name */
                    if (char.IsDigit(var[0]) || !char.IsLetter(var[0]))
                    {
                        throw new Exception($"Error at line {Current.Line}: A variable name can't begin whit a number or the symbol -");
                    }

                    /* If it's a reserv word then it's an invalid variable name (this is the reason) */
                    if (ReservNames.Contains(var))
                    {
                        throw new Exception($"Error at line {Current.Line}: Can't use the reserved word {var} for name a variable");
                    }

                    Advance();
                    Expect("<-");
                    IExpression expr = ParseExpression();
                    instructions.Add((new VariableAssignI(var, expr, Current.Line)));
                }
                else if (Current.Value == "Spawn")      /* Here begin the instructions */
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
                    if (!DireccionesValidas.ContainsKey((dx, dy)))     /* If isn't a valid direction give error */
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
                    if (!DireccionesValidas.ContainsKey((dx, dy)))        /* If isn't a valid direction give error */
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
                    if (!DireccionesValidas.ContainsKey((dx, dy)))         /* If isn't a valid direction give error */
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
                else if (Current.Type == TokenType.Identifier )    /* labels  if after the name it's an EOF type then it's a label */
                {
                    string label = Current.Value;

                    if(!Regex.IsMatch(label, "^[a-zA-Z_][\\w\\-]*$"))       /* Only can match with letters, numbers and the symbol - */
                    {
                        throw new Exception($"Error at line {Current.Line}: {label} is an ivalid label name");
                    }
                    Advance();
                    instructions.Add(new LabelI(label, Current.Line));
                }
                else if (Current.Value == "GoTo")       /* Goto */
                {
                    Advance();
                    Expect("[");
                    string label = ExpectIdentifier();
                    Expect("]");
                    Expect("(");
                    IExpression condition = ParseExpression();       /* Have to parse first the condition */
                    Expect(")");
                    instructions.Add(new GoToI(label, condition, Current.Line));
                }
                else           /* If there isn't neither of this then I dont know what it's give error*/
                {
                    throw new Exception($"Error at line {Current.Line}: Unknow expression '{Current.Value}'");
                }
            }

            if (instructions.Count == 0 || instructions[0] is not SpawnI)      /* Verify that the first instruction always is a Spawn */
            {
                MessageBox.Show("All valid code must begin whit a Spawn(X, Y) instruction");
            }

            int spawns = instructions.Count(instr => instr is SpawnI);
            if (spawns != 1)                                                  /* Verify that there's only one Spawn instruction */
            {
                throw new Exception($"Error: You can only use one Spawn instruction, were found {spawns}");
            }
            return instructions;
        }

        /* Here begin the parser for the IExpression.cs it's made in order of presence */
        private IExpression ParseExpression()
        {
            return Parse0();
        }

        private IExpression Parse0()
        {
            IExpression left = ParseAnd();
            while (Match("||"))
            {
                Advance();
                IExpression rigth = ParseAnd();
                left = new OrE(left, rigth);
            }
            return left;
        }

        private IExpression ParseAnd()
        {
            IExpression left = ParseComparison();
            while (Match("&&"))
            {
                Advance();
                IExpression rigth = ParseComparison();
                left = new AndE(left, rigth);
            }
            return left;
        }

        private IExpression ParseComparison()
        {
            IExpression left = ParseAddSub();
            while (Match("==") || Match("!=") || Match(">") || Match("<") || Match(">=") || Match("<="))
            {
                string op = Current.Value;
                Advance();
                IExpression rigth = ParseAddSub();

                left = op switch
                {
                    "==" => new EqualE(left, rigth),
                    "!=" => new NotEqualE(left, rigth),
                    ">" => new GreaterE(left, rigth),
                    "<" => new LessE(left, rigth),
                    ">=" => new GreaterEqualE(left, rigth),
                    "<=" => new LessEqualE(left, rigth),
                    _ => throw new Exception ($"Error: Unrecognized operator {op}")
                };
            }
            return left;
        }

        private IExpression ParseAddSub()
        {
            IExpression left = ParseMulDivMod();
            while (Match("+") || Match("-"))
            {
                string op = Current.Value;
                Advance();
                IExpression rigth = ParseMulDivMod();

                left = op == "+" ? new AddE(left, rigth) 
                     : new SubE(left, rigth);
            }
            return left;
        }

        private IExpression ParseMulDivMod()
        {
            IExpression left = ParsePower();
            while (Match("*") || Match("/") || Match("%"))
            {
                string op = Current.Value;
                Advance();
                IExpression rigth = ParsePower();

                left = op == "*" ? new MulE(left, rigth)
                     : op == "/" ? new DivE(left, rigth)
                     : new ModE(left, rigth);
            }
            return left;
        }

        private IExpression ParsePower()
        {
            IExpression left = ParseAtom();
            while (Match("**"))
            {
                Advance();
                IExpression rigth = ParseAtom();
                left = new PowE(left, rigth);
            }
            return left;
        }

        private IExpression ParseAtom()     /* Atom it's for funtions */
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
