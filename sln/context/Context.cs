using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallyArt;
using WallyArt.sln.parser;
using WallyArt.sln.expression;

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

        public Dictionary<string, Color> ColorMap = new Dictionary<string, Color>()
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

        public Dictionary<string, int> Variables = new Dictionary<string, int>();

        public Dictionary<string, int> Labels = new Dictionary<string, int>();

        public int NextLine;    /* Controla el cambio de linea */

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
            int CX = X + dx * radius;
            int CY = Y + dy * radius;

            /* Actualiza la posicion */
            X = CX;
            Y = CY;

            int color = ColorMap[BrushColor].ToArgb();

            Pintar(CX, CY - radius);                          /* Aribba */
            Pintar(CX, CY + radius);                          /* Abajo*/
            Pintar(CX - radius, CY);                          /* Izquierda */
            Pintar(CX + radius, CY);                          /* Derecha */
            Pintar(CX - (radius - 1), CY - (radius - 1));     /* Arriba izquierda */
            Pintar(CX + (radius - 1), CY - (radius - 1));     /* Arriba derecha */
            Pintar(CX - (radius - 1), CY + (radius - 1));     /* Abajo izquierda */  
            Pintar(CX + (radius - 1), CY + (radius - 1));     /* Abajo derecha */

            void Pintar(int x, int y)
            {
                if (x >= 0 && y >= 0 && x < CanvasSize && y < CanvasSize)
                {
                    Canvas[x, y] = color;
                }
            }
            
            Redraw();
        }

        public void DrawRectangle(int dx, int dy, int distance, int width, int height)
        {
            X = X + dx * distance;
            Y = Y + dy * distance;

            int x0 = X;
            int y0 = Y;

            int color = ColorMap[BrushColor].ToArgb();

            for(int i = 0; i < width; i++)
            {
                Pintar(x0 + i, y0);                         /* Borde superior */
                Pintar(x0 + i, y0 + (height - 1));          /* Borde inferior */
            }

            for(int j = 0; j < height; j++)
            {
                Pintar(x0, y0 + j);                         /* Borde izquierdo */
                Pintar(x0 + (width - 1), y0 + j);           /* Borde derecho */
            }

            void Pintar(int x, int y)
            {
                if (x >= 0 && y >= 0 && x < CanvasSize && y < CanvasSize)
                {
                    Canvas[x, y] = color;
                }
            }
        }

        public void Fill()
        {
            int startX = X;
            int startY = Y;
            int targetColor = Canvas[startX, startY];
            int newColor = ColorMap[BrushColor].ToArgb();

            if (targetColor == newColor) return;

            Queue<(int x, int y)> cola = new();
            cola.Enqueue((startX, startY));

            while (cola.Count > 0)
            {
                var (x, y) = cola.Dequeue();

                if (x >= 0 && Y >= 0 && x < CanvasSize && y < CanvasSize) continue;

                Canvas[x, y] = newColor;

                /* Agregarvecinos */
                cola.Enqueue((x + 1, y));
                cola.Enqueue((x - 1, y));
                cola.Enqueue((x, y + 1));
                cola.Enqueue((x, y - 1));
            }
        }

        public int ColortoInt(string color)
        {
            return ColorMap.ContainsKey(color) ? ColorMap[color].ToArgb() : Color.Transparent.ToArgb();
        }

        public Color InttoColor(int val) => Color.FromArgb(val);

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
