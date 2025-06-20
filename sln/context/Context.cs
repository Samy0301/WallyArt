using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt;
using WallyArt.sln.parser;

namespace WallyArt.sln.context
{
    /* Mantiene el estado del canvas y maneja la ejecucion visual */
    public class Context
    {
        public int[,] Canvas;                           /* Matriz de canvas*/
        public int CanvasSize;                          /* Dimencion del canvas (cuadrado) */
        public int X;                                   /* Posicion actual  de Wally */
        public int Y;                                   /* "  "  "  "  "  "  "  "  " */ 
        public string BrushColor = "Black";             /* Color actual del pincel */
        public int brushSize = 1;                       /* Tamaño actua del pincel */
        private PictureBox pictureBox1;                 /* Visual del canvas en From1 */

        private Dictionary<string, Color> ColorMap = new Dictionary<string, Color>()
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


        public Context(int size, PictureBox pb)
        {
            CanvasSize = size;
            Canvas = new int[size, size];
            pictureBox1 = pb;
            Redraw();
        }

        public void Spawn(int x, int y)
        {
            if (x < 0 || y < 0 || x >= CanvasSize || y >= CanvasSize)
            {
                throw new Exception($"Wally fuera del canvas en Spawn({x}, {y})");
            }

            X = x;
            Y = y;
        }

        public void SetBrushColor(string color)
        {
            if (!ColorMap.ContainsKey(color))
            {
                throw new Exception($"Color invalido ({color}) usa {string.Join(", ", ColorMap.Keys)}");
            }
            BrushColor = color;
        }

        public int BrushSize
        {
            get => brushSize;
            set => brushSize = Math.Max(1, value % 2 == 0 ? value - 1 : value);
        }

         public void DrawLine(int dx, int dy, int dist)
        {
            int half = brushSize / 2;

            for (int i = 0; i <= dist; i++)
            {
                int nx = X + i * dx;
                int ny = Y + i * dy;

                if (nx < 0 || ny < 0 || nx >= CanvasSize || ny >= CanvasSize)
                {
                    throw new Exception($"Wally se salio del canvas en DrawLine a ({nx}, {ny})");
                }

                for(int dx2 = -half; dx2 <= half; dx2++)
                {
                    for(int dy2 = -half; dy2 <= half; dy2++)
                    {
                        int fx = nx + dx2;
                        int fy = ny + dy2;

                        if (fx >= 0 && fy >= 0 && fx < CanvasSize && fy < CanvasSize)
                        {
                            Canvas[fx, fy] = ColortoInt(BrushColor);
                        }
                    }
                }
            }

            X += dx * dist;
            Y += dy * dist;

            Redraw();
         }

        public void DrawCircle(int dx, int dy, int radius)
        {
            /* Calcular la nueva posicion */
            int centerX = X + dx * radius;
            int centerY = Y + dy * radius;

            /* Validar que este dentro del camvas */
            if(centerX<0|| centerY < 0 || centerX >= CanvasSize || centerY >= CanvasSize)
            {
                throw new Exception($"DrawCircle fuera del canvas en ({centerX}, {centerY})");
            }

            int half = BrushSize / 2;

            /* Pintar el circulo con centro en la nueva posicin */
            for(int dx2 = -radius; dx2 <= radius; dx2++)
            { 
                for(int dy2 = -radius; dy2 <= radius; dy2++)
                {
                    int distance2 = dx2 * dx2 + dy2 * dy2;
                    int r2 = radius * radius;

                    if (distance2 <= r2 && distance2 >= r2 - radius)
                    {
                        int fx = centerX + dx2;
                        int fy = centerY + dy2;

                        for (int bx = -half; bx <= half; bx++) 
                        {
                            for (int by = -half; by <= half; by++)
                            {
                                int px = fx + bx;
                                int py = fy + by;

                                if (px >= 0 || py >= 0 || px < CanvasSize || py < CanvasSize)
                                {
                                    Canvas[px, py] = ColortoInt(BrushColor);
                                }
                            }
                        }   
                    }
                }
            }

            X = centerX;
            Y = centerY;

            Redraw();
        }

        public void DrawRectangle(int dx, int dy, int distance, int width, int height)
        {

        }

        public void Fill()
        {

        }

        public int ColortoInt(string color)
        {
            return ColorMap.ContainsKey(color) ? ColorMap[color].ToArgb() : Color.Transparent.ToArgb();
        }

        private Color InttoColor(int val)=>Color.FromArgb(val);

        public void Redraw()
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

        public void ClearCanvas()
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
