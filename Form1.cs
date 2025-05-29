namespace WallyArt
{
    public partial class Form1 : Form
    {
        private int gridSize = 16;
        private int cellSize = 20;
        private Panel[,] cells;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cells = new Panel[gridSize, gridSize];

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Panel cell = new Panel();
                    cell.BackColor = Color.White;
                    cell.BorderStyle = BorderStyle.FixedSingle;
                    cell.Size = new Size(cellSize, cellSize);
                    cell.Location = new Point(col * cellSize, row * cellSize);
                    cell.Click += Cell_Click;

                    this.Controls.Add(cell);
                    cells[row, col] = cell;
                }
            }

            this.ClientSize = new Size(gridSize * cellSize + 1, gridSize * cellSize + 1);
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            Panel clickedCell = sender as Panel;
            if (clickedCell.BackColor == Color.White)
                clickedCell.BackColor = Color.Black;
            else
                clickedCell.BackColor = Color.White;
        }



    }  
    

    
}