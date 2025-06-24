using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt.sln.context;

namespace WallyArt.sln.expression
{
    public interface IExpression          /* For funtions and numerical or logics operations   This dosent have direct impact o the canvas and have to parse for levels of priority */
    {
        int Evaluate(Context context);
    }

    /* Numbers */
    public class ConstantExpression : IExpression
    {
        public int Value;
        public ConstantExpression(int value) => Value = value;

        public int Evaluate(Context context) => Value; 
    }

    /* Variables alredy named for the user */
    public class VariableExpression : IExpression
    {
        public string Name;

        public VariableExpression(string name)
        {
            Name = name;
        }

        public int Evaluate(Context context)
        {
            if (!context.Variables.ContainsKey(Name))
            {
                throw new Exception($" The variable {Name} is not defined");
            }
            return context.Variables[Name];
        }
    }

    /* Arithmetic operatio */                                                   
    public class AddE : IExpression  /* Sum + */
    {
        public IExpression Left;
        public IExpression Rigth;

        public AddE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) + Rigth.Evaluate(context);
        }
    }

    public class SubE : IExpression  /* Subtraction - */
    {
        public IExpression Left;
        public IExpression Rigth;

        public SubE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) - Rigth.Evaluate(context);
        }
    }

    public class MulE : IExpression  /* Multiplication * */
    {
        public IExpression Left;
        public IExpression Rigth;

        public MulE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) * Rigth.Evaluate(context);
        }
    }

    public class DivE : IExpression  /* Division / */
    {
        public IExpression Left;
        public IExpression Rigth;

        public DivE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            int rigthVal = Rigth.Evaluate(context);

            if (rigthVal == 0)
            {
                throw new Exception($" You can't divide for 0");
            }

            return Left.Evaluate(context) / rigthVal;
        }
    }

    public class ModE : IExpression  /* Module % */
    {
        public IExpression Left;
        public IExpression Rigth;

        public ModE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) % Rigth.Evaluate(context);
        }
    }

    public class PowE : IExpression  /* Power ** */
    {
        public IExpression Left;
        public IExpression Rigth;

        public PowE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return (int)Math.Pow(Left.Evaluate(context), Rigth.Evaluate(context));
        }
    }

    /* Comparison operators  Give back 1 == true, 0 == false */
    public class EqualE : IExpression  /* Equal == */
    {
        public IExpression Left;
        public IExpression Rigth;

        public EqualE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) == Rigth.Evaluate(context) ? 1 : 0;
        }
    }

    public class GreaterE : IExpression  /* Greater than > */
    {
        public IExpression Left;
        public IExpression Rigth;

        public GreaterE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) > Rigth.Evaluate(context) ? 1 : 0;
        }
    }

    public class LessE : IExpression  /* Less than < */
    {
        public IExpression Left;
        public IExpression Rigth;

        public LessE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) < Rigth.Evaluate(context) ? 1 : 0;
        }
    }

    public class GreaterEqualE : IExpression  /* Greater Egual >= */
    {
        public IExpression Left;
        public IExpression Rigth;

        public GreaterEqualE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) >= Rigth.Evaluate(context) ? 1 : 0;
        }
    }

    public class LessEqualE : IExpression  /* Less Equal <= */
    {
        public IExpression Left;
        public IExpression Rigth;

        public LessEqualE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) <= Rigth.Evaluate(context) ? 1 : 0;
        }
    }

    public class NotEqualE : IExpression  /* Diferent != */
    {
        public IExpression Left;
        public IExpression Rigth;

        public NotEqualE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) != Rigth.Evaluate(context) ? 1 : 0;
        }
    }

    /* Logics operators    Give back 1 == true, 0 == false */
    public class AndE : IExpression  /* And && */
    {
        public IExpression Left;
        public IExpression Rigth;

        public AndE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) != 0 && Rigth.Evaluate(context) != 0 ? 1 : 0;
        }
    }

    public class OrE : IExpression  /* Or || */
    {
        public IExpression Left;
        public IExpression Rigth;

        public OrE(IExpression left, IExpression rigth)
        {
            Left = left;
            Rigth = rigth;
        }

        public int Evaluate(Context context)
        {
            return Left.Evaluate(context) != 0 || Rigth.Evaluate(context) != 0 ? 1 : 0;
        }
    }

    /* Funtions    There evaluate here and not in the Context.cs cause their don't have repercution on what happens in the visual context */  
    public class GetActualXE : IExpression
    {
        public int Evaluate(Context context) => context.X;
    }

    public class GetActualYE : IExpression
    {
        public int Evaluate(Context context) => context.Y;
    }

    public class GetCanvasSizeE : IExpression
    {
        public int Evaluate(Context context) => context.CanvasSize;  
    }

    public class GetColorCountE : IExpression
    {
        public string Color;
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;

        public GetColorCountE(string color, int x1, int y1, int x2, int y2)
        {
            Color = color;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public int Evaluate(Context context)
        {
            if (!context.ColorMap.ContainsKey(Color))
            {
                throw new Exception($" The color {Color} does not appear in the Canvas");
            }

            Color target = context.ColorMap[Color];
            int count = 0;

            int minX = Math.Min(X1, X2);
            int maxX = Math.Max(X1, X2);
            int minY = Math.Min(Y1, Y2);
            int maxY = Math.Max(Y1, Y2);

            for(int x = minX; x <= maxX; x++)
            {
                for(int y = minY; y <= maxY; y++)
                {
                    if (x >= 0 && y >= 0 && x < context.CanvasSize && y < context.CanvasSize)
                    {
                        Color pixelColor = context.InttoColor(context.Canvas[x, y]);

                        if(pixelColor.ToArgb() == target.ToArgb())
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }
    }

    public class IsBrushColorE : IExpression
    {
        public string Color;

        public IsBrushColorE(string color)
        {
            Color = color;
        }

        public int Evaluate(Context context)
        {
            if(!context.ColorMap.ContainsKey(Color))
            {
                throw new Exception($" The color {Color} is not valid");
            }

            if (Color == context.BrushColor)
            {
                return 1;
            }
            return 0;
        }
    }

    public class IsBrushSizeE : IExpression
    {
        public int Size;

        public IsBrushSizeE(int size)
        {
            Size = size;
        }

        public int Evaluate(Context context)
        {
            if (Size == context.brushSize)
            {
                return 1;
            }
            return 0;
        }
    }

    public class IsCanvasColorE : IExpression
    {
        public string Color;
        public int Vertical;
        public int Horizontal;

        public IsCanvasColorE(string color, int vertical, int horizontal)
        {
            Color = color;
            Vertical = vertical;
            Horizontal = horizontal;
        }

        public int Evaluate(Context context)
        {
            if (!context.ColorMap.ContainsKey(Color))
            {
                throw new Exception($" The color {Color} is not valid");
            }

            Color target = context.ColorMap[Color];
            Color actual = context.InttoColor(context.Canvas[context.X + Horizontal, context.Y + Horizontal]);

            if (actual.ToArgb() == target.ToArgb())
            {
                return 1;
            }
            return 0;
        }
    }
}
