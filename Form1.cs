using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using WallyArt.sln.context;
using WallyArt.sln.ast;
using WallyArt.sln.parser;
using WallyArt.sln.instructions;


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

        private int currentCanvasSize = 25;
        private Context context;

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

        private void Clean_Click(object sender, EventArgs e)
        {
            if (context != null)
            {
                context.ClearCanvas();
            }
        }

        private void Size_Click(object sender, EventArgs e)
        {
            try
            {
                int newSize = int.Parse(textBox1.Text);

                if(newSize < 1 || newSize > 100)
                {
                    MessageBox.Show("El tamaño debe estar entre 1 y 100");
                    return;
                }

                currentCanvasSize = newSize;
                context = null;
                DrawGrid(currentCanvasSize);    /* Actualiza el tamaño con el pasado por el usuario */
            }
            catch
            {
                MessageBox.Show("Introduzca un numero valido");
            }
        }

        private void Paint_Click(object sender, EventArgs e)
        {
            try
            {
                /*Analizar codigo */
                string code = richTextBox1.Text;
                Lexer lexer = new Lexer(code);
                List<Token> tokens = lexer.Tokenize();
                Parser parser = new Parser(tokens);
                List<Instruction> instructions = parser.Parse();

                if(instructions.Count ==0 || instructions[0] is not SpawnI)
                {
                    MessageBox.Show("Todo codigo valido debe empezar con la instruccion Spawn(X, Y)");
                }


                if (context == null)
                {
                     context = new Context(currentCanvasSize, pictureBox1);
                }


                /* Ejecutar instrucciones */
                foreach (var instr in instructions)
                {
                    instr.Execute(context);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
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

            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                Pen pen = new Pen(Color.Black, 1);

                float cellW = (float)width / gridSize;
                float cellH = (float)height / gridSize;

                for (int i = 0; i <= gridSize; i++)
                {
                    int x = (int)Math.Round(i * cellW);
                    g.DrawLine(pen, x, 0, x, height);
                }

                for (int i = 0; i <= gridSize; i++)
                {
                    int y = (int)Math.Round(i * cellH);
                    g.DrawLine(pen, 0, y, width, y);
                }
            }

            pictureBox1.Image = bitmap;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

       
    }

}