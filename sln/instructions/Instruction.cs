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

    public class SwapI : Instruction
    {
        public int X;
        public int Y;
        
        public SwapI(int x, int y, int line) : base(line) 
        {
            X = x; Y = y; 
        }
        public override void Execute(Context context)
        {
           
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
            
        }
    }

    public class DrawLineI : Instruction
    {
        public DrawLineI( , int line) : base(line) 
        {

        }

        public override void Execute(Context context)
        {
            
        }
    }

    public class ZyseI : Instruction
    {
        public ZyseI( , int line) : base(line)
        {

        }

        public override void Execute(Context context)
        {
            
        }
    }
}
