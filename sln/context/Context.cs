using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt;
using WallyArt.sln.parser;
using WallyArt.sln.expression;
using System.Drawing;

namespace WallyArt.sln.context
{
    /* Mantain the state of canvas and manages the visual context */
    public class Context
    {
        public int[,] Canvas;                           /* Canvas matrix */
        public int CanvasSize;                          /* Canvas dimension (square) */
        public int X;                                   /* Wally's current position */
        public int Y;                                   /* "  "  "  "  "  "  "  "  " */ 
        public string BrushColor = "Black";             /* Brush current color */
        public int brushSize = 1;                       /* Brush current size */
        private PictureBox pictureBox1;                 /* Canvas visual on the Form1 */

        public Dictionary<string, Color> ColorMap = new Dictionary<string, Color>()     /* All valid colors */
        {
            ["Red"] = Color.Red,
            ["Blue"] = Color.Blue,
            ["Green"] = Color.Green,
            ["Yellow"] = Color.Yellow,
            ["Oranje"] = Color.Orange,
            ["Purple"] = Color.Purple,
            ["Black"] = Color.Black,
            ["White"] = Color.White,
            ["Transparent"] = Color.Transparent,
        };

        public Dictionary<string, int> Variables = new Dictionary<string, int>();    /* Here variables are stored for later use */

        public Dictionary<string, int> Labels = new Dictionary<string, int>();       /* Here labels stored their lines for later access */

        public int NextLine;    /* Control the line jump */

        public Context(int size, PictureBox pb)
        {
            CanvasSize = size;
            Canvas = new int[size, size];
            pictureBox1 = pb;
            Redraw();
        }

        public void Pintar0(int cx, int cy)                 /* Method use for the instructions to paint on the canvas */
        {
            Pintar( cx,  cy, BrushSize, ColorMap[BrushColor].ToArgb());
        }

        public void Pintar(int cx, int cy, int size, int color)    /* Auxiliary method for painting */
        {
            int half = size / 2;

            for(int dx = -half; dx <= half; dx++)
            {
                for(int dy = -half; dy <= half; dy++)
                {
                    int x = cx + dx;
                    int y = cy + dy;

                    if (x >= 0 && x < CanvasSize && y >= 0 && y < CanvasSize)
                    {
                        Canvas[x, y] = color;
                    }
                }
            }
        }

        /* Here begin the Execute method of instructions */
        public void Spawn(int x, int y)
        {
            if (x < 0 || y < 0 || x >= CanvasSize || y >= CanvasSize)
            {
                throw new Exception($" Wally get's out of Canvas in ({x}, {y})");
            }

            X = x;
            Y = y;
        }

        public void SetBrushColor(string color)
        {
            if (!ColorMap.ContainsKey(color))
            {
                throw new Exception($" Invalid color {color} better use {string.Join(", ", ColorMap.Keys)}");
            }
            BrushColor = color;
        }

        public int BrushSize
        {
            get => brushSize;
            set => brushSize = Math.Max(1, value % 2 == 0 ? value - 1 : value);    /* Look the size not to be a par number */
        }

         public void DrawLine(int dx, int dy, int dist)
         { 
            for(int i = 1; i < dist; i++)
            {
                Pintar0(X, Y);

                X += dx;
                Y += dy;

                Pintar0(X, Y);
            }
            X += dx;
            Y += dy;
            Redraw();
         }

        public void DrawCircle(int dx, int dy, int radius)
        {
            X += dx * radius;
            Y += dy * radius;

            int centerX = X;
            int centerY = Y;

            /* Draw the circular edge with degree pitch (angle) */
            for(int angle=0; angle < 360; angle++)
            {
                double rad = Math.PI * angle / 180.0;

                int px = centerX + (int)Math.Round(radius * Math.Cos(rad));
                int py = centerY + (int)Math.Round(radius * Math.Sin(rad));

                Pintar0(px, py);
            }
            Redraw();
        }

        public void DrawRectangle(int dx, int dy, int distance, int width, int height)
        {
            X =+ dx * distance;
            Y =+ dy * distance;

            int startx = X;
            int starty = Y;
            int endx = startx + (width - 1);
            int endy = starty + (height - 1);

            for(int x = startx; x < endx; x++) Pintar0(x, starty);
            for (int x = startx; x < endx; x++) Pintar0(x, endy);
            for (int y = starty; y < endy; y++) Pintar0(startx, y);
            for(int y = starty; y < endy; y++) Pintar0(endx, y);

            Redraw();
        }

        public void Fill()
        {
            int targetColor = Canvas[X, Y];

            if (targetColor == ColorMap[BrushColor].ToArgb()) return;

            Queue<(int , int)> cola = new Queue<(int, int)>();          /* Use a tail to draw all the neighbors */
            bool[,] visited = new bool[CanvasSize, CanvasSize];
            cola.Enqueue((X, Y));
            visited[X, Y] = true;

            while (cola.Count > 0)
            {
                var (x, y) = cola.Dequeue();

                Pintar0(x, y);

                foreach(var(nx, ny) in Vecinos(x, y))
                {
                    if (nx >= 0 && nx < CanvasSize && ny >= 0 && ny < CanvasSize && !visited[nx, ny] && Canvas[nx, ny] == targetColor)
                    {
                        visited[nx, ny] = true;
                        cola.Enqueue((nx, ny));
                    }
                }
            }
        }

        private List<(int,int)> Vecinos(int x, int y)            /* Method use by Fill to look the neighbor box */
        {
            return new List<(int, int)>
            {
                (x + 1, y),
                (x - 1, y),
                (x, y + 1),
                (x, y - 1)
            };
        }

        public int ColortoInt(string color)             /* Method for convert words to numbers this way the program understand what a color is */
        {
            return ColorMap.ContainsKey(color) ? ColorMap[color].ToArgb() : Color.Transparent.ToArgb();
        }

        public Color InttoColor(int val) => Color.FromArgb(val);      /* Opposite of ColortoInt */

        public void Redraw()         /* Method for update the canva on the picturebox */
        {
            Bitmap bmp = new Bitmap(CanvasSize, CanvasSize);

            for (int x = 0; x < CanvasSize; x++)
            {
                for(int y = 0; y < CanvasSize; y++)
                {
                    bmp.SetPixel(x, y, InttoColor(Canvas[x, y]));
                }
            }

            Bitmap scaled = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            using (Graphics g = Graphics.FromImage(scaled))           
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(bmp, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
                Pen gridPen = new Pen(Color.Black, 1);
                float cellW = (float)pictureBox1.Width / CanvasSize;
                float cellH = (float)pictureBox1.Height / CanvasSize;

                for(int i = 0; i <= CanvasSize; i++)
                {
                    int x = (int)Math.Round(i * cellW);
                    g.DrawLine(gridPen, x, 0, x, pictureBox1.Height);
                }

                for (int i = 0; i <= CanvasSize; i++)
                {
                    int y = (int)Math.Round(i * cellH);
                    g.DrawLine(gridPen, 0, y, pictureBox1.Width, y);
                }
            }
            pictureBox1.Image = scaled;
        }

        public void ClearCanvas()        /* Mehod for the button (Clean) */
        {
            for(int x = 0; x < CanvasSize; x++)
            {
                for(int y = 0; y < CanvasSize; y++)
                {
                    Canvas[x, y] = ColortoInt("Transparent");
                }
            }
            Redraw();
        }
    }
}
