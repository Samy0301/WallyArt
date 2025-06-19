using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallyArt.sln.context
{
    /* Mantiene el estado del canvas y maneja la ejecucion visual */
    public class Context
    {
        public int[,] Canvas;                           /* Matriz de canvas*/
        public int CanvasSize;                          /* Dimencion del canvas (cuadrado) */
        public int X;                                   /* Posicion actual  de Wally */
        public int Y;                                   /* "  "  "  "  "  "  "  "  " */ 
        public string BrushColor = "Transparent";       /* Color actual del pincel */
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

            pictureBox1.Image = new Bitmap(bmp, pictureBox1.Width, pictureBox1.Height);
        }
    }
}
