namespace WallyArt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Multiline = true; // Asegura que el TextBox sea multilinea
            textBox1.ScrollBars = ScrollBars.Vertical; // Agrega barras de desplazamiento si es necesario
        }

        private void New_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos PW (*.pw)|*.pw";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = File.ReadAllText(openFileDialog.FileName);
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
                textBox1.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos de Texto (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, textBox1.Text);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Actualiza el Label con los números de las líneas mientras se escribe
            UpdateLineNumbers();
        }

        // Método para actualizar los números de las líneas en el Label
        private void UpdateLineNumbers()
        {
            string[] lines = textBox1.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string lineNumbers = string.Join(Environment.NewLine, Enumerable.Range(1, lines.Length));
            label1.Text = lineNumbers;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Detecta si se presiona la tecla Enter
            if (e.KeyCode == Keys.Enter)
            {
                // Agrega una nueva línea al TextBox
                textBox1.AppendText(Environment.NewLine);
                // Actualiza el Label con los números de las líneas
                UpdateLineNumbers();
            }
        }
    }

}