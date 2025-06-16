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

            DrawGrid(25);

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

        private void DrawGrid(int gridSize)
        {
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;
            int cellSize = Math.Min(width / gridSize, height / gridSize);

            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                Pen pen = new Pen(Color.Black);

                // Dibujar líneas horizontales
                for (int i = 0; i <= gridSize; i++)
                {
                    int y = i * cellSize;
                    g.DrawLine(pen, 0, y, width, y);
                }

                // Dibujar líneas verticales
                for (int i = 0; i <= gridSize; i++)
                {
                    int x = i * cellSize;
                    g.DrawLine(pen, x, 0, x, height);
                }
            }

            pictureBox1.Image = bitmap;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }
    }

}