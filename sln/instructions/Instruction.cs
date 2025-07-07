using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt.sln.context;
using WallyArt.sln.expression;

namespace WallyArt.sln.instructions
{
    public abstract class Instruction             /* For instructions who have impact on the canvas       Variables, labels and the goto are here cause dont have orden of presence in the parser (soso) */
    {
        public int Line;
        public abstract void Execute(Context context);

        public Instruction(int line)
        {
            Line = line;
        }
    }

    /* Instructions      This are the only comands that do paint in the canvas */
    public class SpawnI : Instruction
    {
        public IExpression X;
        public IExpression Y;
        
        public SpawnI(IExpression x, IExpression y, int line) : base(line) 
        {
            X = x; Y = y; 
        }
        public override void Execute(Context context)
        {
            context.Spawn(X.Evaluate(context), Y.Evaluate(context));
        }
    }

    public class ColorI : Instruction
    {
        public string Color;
        public ColorI(string color, int line) : base(line)
        {
            Color = color;
        }
        public override void Execute(Context context)
        {
            context.SetBrushColor(Color);
        }
    }

    public class SizeI : Instruction
    {
        public IExpression Size;
        public SizeI(IExpression size, int line) : base(line) 
        {
            Size = size;
        }

        public override void Execute(Context context)
        {
            int s = Size.Evaluate(context);

            if (s % 2 == 0) s -= 1;
            context.BrushSize = s;
        }
    }

    public class DrawLineI : Instruction
    {
        public IExpression dirX;
        public IExpression dirY;
        public IExpression Distance;
        public DrawLineI(IExpression dx, IExpression dy, IExpression dist, int line) : base(line)
        {
            dirX = dx;
            dirY = dy;
            Distance = dist;
        }

        public override void Execute(Context context)
        {
            context.DrawLine(dirX.Evaluate(context), dirY.Evaluate(context), Distance.Evaluate(context));
        }
    }

    public class DrawCircleI : Instruction
    {
        public IExpression dirX;
        public IExpression dirY;
        public IExpression Radius;

        public DrawCircleI(IExpression dx, IExpression dy, IExpression radius, int line) : base(line)
        {
            dirX = dx;
            dirY = dy;
            Radius = radius;
        }

        public override void Execute(Context context)
        {
            context.DrawCircle(dirX.Evaluate(context), dirY.Evaluate(context), Radius.Evaluate(context));
        }

    }

    public class DrawRectangleI : Instruction
    {
        IExpression dirX;
        IExpression dirY;
        IExpression Distance;
        IExpression Width;
        IExpression Height;

        public DrawRectangleI(IExpression dx, IExpression dy, IExpression distance, IExpression width, IExpression height, int line) : base(line)
        {
            dirX = dx;
            dirY = dy;
            Distance = distance;
            Width = width;
            Height = height;
        }

        public override void Execute(Context context)
        {
            context.DrawRectangle(dirX.Evaluate(context), dirY.Evaluate(context), Distance.Evaluate(context), Width.Evaluate(context), Height.Evaluate(context));
        }
    }

    public class FillI : Instruction
    {
        public FillI(int line) : base(line)
        {

        }

        public override void Execute(Context context)
        {
            context.Fill();
        }
    }

    /* Variables     Can be declare in every line but begin and used in the condition of goto or like argument of a funtion or instruction */

    public class VariableAssignI : Instruction
    {
        public string VarName;
        public IExpression Expression;

        public VariableAssignI(string varName, IExpression expression, int line) : base(line)
        {
            VarName = varName;
            Expression = expression;
        }

        public override void Execute(Context context)
        {
            int value = Expression.Evaluate(context);
            context.Variables[VarName] = value;
        }
    }

    /* Label     There only funtion is declare a place on the code and can be used in the label part of goto, can be declare in every line but begin */
    public class LabelI : Instruction
    {
        public string Name;

        public LabelI(string name, int line) : base(line)
        {
            Name = name;
        }

        public override void Execute(Context context)
        {
            if (!context.Labels.ContainsKey(Name))
            {
                context.Labels[Name] = Line;
            }
        }
    }

    /* Goto    Can be declare in every line but begin only execute when the condition is true otherwise will be treated like a label */
    public class GoToI : Instruction
    {
        public string Label;
        public IExpression Condition;

        public GoToI(string label, IExpression condition, int line) : base(line)
        {
            Label = label;
            Condition = condition;
        }

        public override void Execute(Context context)
        {
            if (Condition.Evaluate(context) != 0)
            {
                if (!context.Labels.ContainsKey(Label))
                {
                    throw new Exception($" Line {Line}: The label {Label} wasn't found");
                }
                context.NextLine = context.Labels[Label];
            }
        }
    }
}
