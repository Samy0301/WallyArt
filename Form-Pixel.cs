using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WallyArt
{
    public partial class Form_Pixel : Form
    {
        private int fila = 50;
        private int columna = 50;
        private int pixelzise = 20;
        private Pixel[,] matrizPixel;

        public Form_Pixel()
        {
            InitializeComponent();
            matrizPixel = new Pixel[fila, columna];
            InicializarPixel();
            
        }

        private void InicializarPixel()
        {
            for(int i = 0; i < fila; i++)
            {
                for(int j = 0; j < columna; j++)
                {
                    matrizPixel[i, j] = new Pixel { X = j, Y = i };
                }
            }
        }
    }

    public class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color color { get; set; } = Color.Black;
    }
}
