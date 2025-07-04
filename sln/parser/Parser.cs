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
                throw new Exception($" Line {Current.Line}: Expected a value");
            }
        }

        /* Variation of Expect for numbers */
        private IExpression ExpectNumberOrVariable()
        {
            if (Current.Type == TokenType.Number)
            {
                var val = new ConstantExpression(int.Parse(Current.Value));
                Advance();
                return val;
            }
            else if(Current.Type == TokenType.Identifier)
            {
                return new VariableE(Current.Value);
            }
            throw new Exception($" Line {Current.Line}: Expected a number but found this {Current.Value}");
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
            throw new Exception($" Line {Current.Line}: Expected a string but found this {Current.Value}");
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
            throw new Exception($" Line {Current.Line}: Expected a identifier but found this {Current.Value}");
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
                        throw new Exception($" Line {Current.Line}: A variable name can't begin whit a number or the symbol -");
                    }
                    
                    /* If it's a reserv word then it's an invalid variable name (this is the reason) */
                    if (ReservNames.Contains(var))
                    {
                        throw new Exception($" Line {Current.Line}: Can't use the reserved word {var} for name a variable");
                    }

                    Advance();
                    Expect("<-");
                    IExpression? expr = ParseExpression();
                    instructions.Add((new VariableAssignI(var, expr, Current.Line)));
                }
                else if (Current.Value == "Spawn")      /* Here begin the instructions */
                {
                    Advance();
                    Expect("(");
                    IExpression x = ExpectNumberOrVariable();
                    Expect(",");
                    IExpression y = ExpectNumberOrVariable();
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
                    IExpression dx = ExpectNumberOrVariable();
                    Expect(",");
                    IExpression dy = ExpectNumberOrVariable();
                    if (!DireccionesValidas.ContainsKey((dx, dy)))     /* If isn't a valid direction give error */
                    {
                        throw new Exception($" Line {Current.Line}: ({dx}, {dy}) is an invalid direction");
                    }
                    Expect(",");
                    IExpression dist = ExpectNumberOrVariable();
                    Expect(")");
                    instructions.Add(new DrawLineI(dx, dy, dist, Current.Line));
                }
                else if (Current.Value == "Size")
                {
                    Advance();
                    Expect("(");
                    IExpression k = ExpectNumberOrVariable();
                    Expect(")");
                    instructions.Add(new SizeI(k, Current.Line));
                }
                else if (Current.Value == "DrawCircle")
                {
                    Advance();
                    Expect("(");
                    IExpression dx = ExpectNumberOrVariable();
                    Expect(",");
                    IExpression dy = ExpectNumberOrVariable();
                    if (!DireccionesValidas.ContainsKey((dx, dy)))      /* If isn't a valid direction give error */
                    {
                        throw new Exception($" Line {Current.Line}: ({dx}, {dy}) is an invalid direction");
                    }
                    Expect(",");
                    IExpression radius = ExpectNumberOrVariable();
                    Expect(")");
                    instructions.Add(new DrawCircleI(dx, dy, radius, Current.Line));
                }
                else if (Current.Value == "DrawRectangle")
                {
                    Advance();
                    Expect("(");
                    IExpression dx = ExpectNumberOrVariable();
                    Expect(",");
                    IExpression dy = ExpectNumberOrVariable();
                    if (!DireccionesValidas.ContainsKey((dx, dy)))         /* If isn't a valid direction give error */
                    {
                        throw new Exception($" Line {Current.Line}: ({dx}, {dy}) is an invalid direction");
                    }
                    Expect(",");
                    IExpression distance = ExpectNumberOrVariable();
                    Expect(",");
                    IExpression width = ExpectNumberOrVariable();
                    Expect(",");
                    IExpression height = ExpectNumberOrVariable();
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
                else if (Current.Type == TokenType.Identifier && Regex.IsMatch(Current.Value, @"^[a-zA-Z_][\w\-]*$") && !ReservNames.Contains(Current.Value))   /* labels  if the name is valid and not an reserv word then it's a label */
                {
                    string label = Current.Value;
                    Advance();
                    instructions.Add(new LabelI(label, Current.Line));
                }
                else           /* If there isn't neither of this then I dont know what it's give error*/
                {
                    throw new Exception($" Line {Current.Line}: Unknow instruction '{Current.Value}'");
                }
            }

            if (instructions.Count == 0 || instructions[0] is not SpawnI)      /* Verify that the first instruction always is a Spawn */
            {
                MessageBox.Show("All valid code must begin whit a Spawn(X, Y) instruction");
            }

            int spawns = instructions.Count(instr => instr is SpawnI);
            if (spawns != 1)                                                  /* Verify that there's only one Spawn instruction */
            {
                throw new Exception($" You can only use one Spawn instruction, were found {spawns}");
            }
            return instructions;
        }

        /* Here begin the parser for the IExpression.cs it's made in order of presence */
        public class OperatorInfo
        {
            public int Precedence;
            public bool RigthAssociative;
            public Func<IExpression, IExpression, IExpression> Create;

            public OperatorInfo(int precedence, bool rigthAssoc, Func<IExpression, IExpression, IExpression> create)
            {
                Precedence = precedence;
                RigthAssociative = rigthAssoc;
                Create = create;
            }
        }

        /* Diccionario de operadores validos */
        private static readonly Dictionary<string, OperatorInfo> Operators = new()
        {
            { "**", new OperatorInfo(4, true, (a,b) => new PowE(a,b)) },
            { "*", new OperatorInfo(3, false, (a,b) => new MulE(a,b)) },
            { "/", new OperatorInfo(3, false, (a,b) => new DivE(a,b)) },
            { "%", new OperatorInfo(3, false, (a,b) => new ModE(a,b)) },
            { "+", new OperatorInfo(2, false, (a,b) => new AddE(a,b)) },
            { "-", new OperatorInfo(2, false, (a,b) => new SubE(a,b)) },

            { "==", new OperatorInfo(1, false, (a,b) => new EqualE(a,b)) },
            { "!=", new OperatorInfo(1, false, (a,b) => new NotEqualE(a,b)) },
            { ">=", new OperatorInfo(1, false, (a,b) => new GreaterEqualE(a,b)) },
            { "<=", new OperatorInfo(1, false, (a,b) => new LessEqualE(a,b)) },
            { ">", new OperatorInfo(1, false, (a,b) => new GreaterE(a,b)) },
            { "<", new OperatorInfo(1, false, (a,b) => new LessE(a,b)) },

            { "&&", new OperatorInfo(0, false, (a,b) => new AndE(a,b)) },
            { "||", new OperatorInfo(-1, false, (a,b) => new OrE(a,b)) },
        };

        /* Meodo de Shuting Yard */
        public IExpression ParseExpression()
        {
            var output = new Stack<IExpression>();
            var operators = new Stack<string>();
            int expressionLine = Current.Line;

            while (Current.Type != TokenType.EOF && Current.Value != ")" && Current.Value != "]" && Current.Line == expressionLine)
            {
                if (Current.Value == ",")
                {
                    Advance();
                    continue;
                }
                if (Current.Type == TokenType.Number)
                {
                    output.Push(new ConstantExpression(int.Parse(Current.Value)));
                    Advance();
                }
                else if (Current.Type == TokenType.Identifier)
                {
                    var expr = TryParseFuntionOrVariable();
                    output.Push(expr);
                    Advance();
                }
                else if (Current.Type == TokenType.Operator && Operators.ContainsKey(Current.Value))
                {
                    var o1 = Operators[Current.Value];

                    while (operators.Count > 0 && Operators.ContainsKey(operators.Peek()))
                    {
                        var o2 = Operators[operators.Peek()];

                        if ((o1.RigthAssociative && o1.Precedence < o2.Precedence) || (!o1.RigthAssociative && o1.Precedence <= o2.Precedence))
                        {
                            ApplyOperator(operators.Pop(), output);
                        }
                        else break;
                    }
                    operators.Push(Current.Value);
                    Advance();
                }
                else if (Match("("))
                {
                    operators.Push("(");
                    Advance();
                }
                else if (Match(")"))
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        ApplyOperator(operators.Pop(), output);
                    }

                    if (operators.Count == 0 || operators.Peek() != "(")
                    {
                        throw new Exception($" Line {Current.Line}: Mismatched parentheses");
                    }
                    Advance();
                }
                else
                {
                    throw new Exception($" Line {Current.Line}: Unexpected token {Current.Value}");
                }
            }

            while (operators.Count > 0)
            {
                if (operators.Peek() == "(") throw new Exception($" Line {Current.Line}: Mismatched parentheses");
                ApplyOperator(operators.Pop(), output);
            }

            if (output.Count != 1) throw new Exception($" Line{Current.Line}: Invalid expression");
            return output.Pop();
        }

        /* Aplicar operador a la pila */
        private void ApplyOperator(string op, Stack<IExpression> output)
        {
            if (output.Count < 2) throw new Exception($" Line {Current.Line}: Not enough operands for operator {op}");

            var rigth = output.Pop();
            var left = output.Pop();
            output.Push(Operators[op].Create(left, rigth));
        }

        /* Detectar funciones o variables */
        private IExpression TryParseFuntionOrVariable()
        {
            string name = Current.Value;

            if (name == "GetActualX")
            {
                Advance();
                Expect("(");
                Expect(")");
                return new GetActualXE();
            }
            else if (name == "GetActualY")
            {
                Advance();
                Expect("(");
                Expect(")");
                return new GetActualYE();
            }
            else if (name == "GetCanvasSize")
            {
                Advance();
                Expect("(");
                Expect(")");
                return new GetCanvasSizeE();
            }
            else if (name == "GetColorCount")
            {
                Advance();
                Expect("(");
                string color = ExpectString();
                Expect(",");
                IExpression X1 = ExpectNumberOrVariable();
                Expect(",");
                IExpression Y1 = ExpectNumberOrVariable();
                Expect(",");
                IExpression X2 = ExpectNumberOrVariable();
                Expect(",");
                IExpression Y2 = ExpectNumberOrVariable();
                Expect(")");
                return new GetColorCountE(color, X1, Y1, X2, Y2);
            }
            else if (name == "IsBrushColor")
            {
                Advance();
                Expect("(");
                string color = ExpectString();
                Expect(")");
                return new IsBrushColorE(color);
            }
            else if (name == "IsBrushSize")
            {
                Advance();
                Expect("(");
                IExpression size = ExpectNumberOrVariable();
                Expect(")");
                return new IsBrushSizeE(size);
            }
            else if (name == "IsCanvasColor")
            {
                Advance();
                Expect("(");
                string color = ExpectString();
                Expect(",");
                IExpression x = ExpectNumberOrVariable();
                Expect(",");
                IExpression y = ExpectNumberOrVariable();
                Expect(")");
                return new IsCanvasColorE(color, x, y);
            }
            else
            {
                return new VariableE(name);
            }

        }
    }
}
