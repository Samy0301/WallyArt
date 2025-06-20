using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt.sln.context;

namespace WallyArt.sln.instructions
{
    public abstract class Instruction
    {
        public int Line;
        public abstract void Execute(Context context);

        public Instruction(int line)
        {
            Line = line;
        }
    }

    public class SpawnI : Instruction
    {
        public int X;
        public int Y;
        
        public SpawnI(int x, int y, int line) : base(line) 
        {
            X = x; Y = y; 
        }
        public override void Execute(Context context)
        {
            context.Spawn(X, Y);
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
        public int Size;
        public SizeI( int size, int line) : base(line) 
        {
            Size = size % 2 == 0 ? size - 1 : size;
        }

        public override void Execute(Context context)
        {
            context.BrushSize = Size;
        }
    }

    public class DrawLineI : Instruction
    {
        public int dirX;
        public int dirY;
        public int Distance;
        public DrawLineI( int dx, int dy, int dist, int line) : base(line)
        {
            dirX = dx;
            dirY = dy;
            Distance = dist;
        }

        public override void Execute(Context context)
        {
            context.DrawLine(dirX, dirY, Distance);
        }
    }

    public class DrawCircleI : Instruction
    {
        public int dirX;
        public int dirY;
        public int Radius;

        public DrawCircleI(int dx, int dy, int radius, int line) : base(line)
        {
            dirX = dx;
            dirY = dy;
            Radius = radius;
        }

        public override void Execute(Context context)
        {
            context.DrawCircle(dirX, dirY, Radius);
        }

    }

    public class DrawRectangleI : Instruction
    {
        int dirX;
        int dirY;
        int Distance;
        int Width;
        int Height;

        public DrawRectangleI(int dx, int dy, int distance, int width, int height, int line) : base(line)
        {
            dirX = dx;
            dirY = dy;
            Distance = distance;
            Width = width;
            Height = height;
        }

        public override void Execute(Context context)
        {
            context.DrawRectangle(dirX, dirY, Distance, Width, Height);
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
}
