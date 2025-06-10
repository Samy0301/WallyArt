using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WallyArt
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);
        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        const int WM_VSCROLL = 0x0115;
        const int SB_VERT = 1;

        private int rows = 18;
        private int cols = 18;
        private int cellSize = 20;

        public Form1()
        {
            InitializeComponent();

            richTextBox1.TextChanged += RichTextBox1_TextChanged;
            richTextBox1.VScroll += RichTextBox1_VScroll;

            DrawGrid();

        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            int lineCount = richTextBox1.GetLineFromCharIndex(richTextBox1.TextLength) + 1;
            textBox2.Text = "";
            for(int i = 1; i<=lineCount; i++)
            {
                textBox2.AppendText(i + Environment.NewLine);
            }
        }

        private void RichTextBox1_VScroll(object sender, EventArgs e)
        {
            int nPos = GetScrollPos(richTextBox1.Handle, SB_VERT);
            SetScrollPos(textBox2.Handle, SB_VERT, nPos, true);
            SendMessage(textBox2.Handle, WM_VSCROLL, (int)(0x0004 + 0x10000 * nPos), 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void New_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos PW (*.pw)|*.pw";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void Paint_Click(object sender, EventArgs e)
        {
            
        }

        private void Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos Salvados (*.saved)|*.saved";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos de Texto (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DrawGrid()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                Pen gridPen = new Pen(Color.Gray);

                // Líneas horizontales
                for (int y = 0; y <= rows; y++)
                {
                    g.DrawLine(gridPen, 0, y * cellSize, cols * cellSize, y * cellSize);
                }

                // Líneas verticales
                for (int x = 0; x <= cols; x++)
                {
                    g.DrawLine(gridPen, x * cellSize, 0, x * cellSize, rows * cellSize);
                }
            }

            pictureBox1.Image = bmp;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }
    }

}