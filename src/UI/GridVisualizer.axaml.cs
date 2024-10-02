// src/UI/GridVisualizer.axaml.cs
using Avalonia.Controls;
using Avalonia.Media;
using GridMind.Environment;
using GridMind.Agents;

namespace GridMind.UI
{
    public partial class GridVisualizer : UserControl, IObserver<GridCell>
    {
        private readonly Environment.Grid grid;
        private readonly Agent agent;
        private const int CellSize = 30;  // Uniform cell size (adjust as needed)

        public GridVisualizer()
        {
            InitializeComponent();
        }

        public GridVisualizer(Environment.Grid grid, Agent agent) : this()
        {
            this.grid = grid;
            this.agent = agent;

            // Subscribe to the agent's FogOfWar
            agent.FogOfWar.Subscribe(this);

            RenderGrid();
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(GridCell value)
        {
            // When a new cell is explored, update the grid
            RenderGrid();
        }

        public void RenderGrid()
        {
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.Children.Clear();

            // Define rows and columns based on grid dimensions
            for (int row = 0; row < grid.Rows; row++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition(CellSize, GridUnitType.Pixel));
            }

            for (int col = 0; col < grid.Columns; col++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition(CellSize, GridUnitType.Pixel));
            }

            // Render each cell with uniform size
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Columns; col++)
                {
                    var cell = grid.GetCell(row, col);

                    var cellBlock = new TextBlock
                    {
                        Text = GetCellText(cell, agent.ExploredCells.Contains(cell)),
                        Background = GetCellBackground(cell, agent.ExploredCells.Contains(cell)),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        FontSize = 20,
                        MinWidth = CellSize,
                        MinHeight = CellSize,
                        MaxWidth = CellSize,
                        MaxHeight = CellSize,
                    };

                    // Set row and column for the cellBlock
                    Avalonia.Controls.Grid.SetRow(cellBlock, row);
                    Avalonia.Controls.Grid.SetColumn(cellBlock, col);
                    MainGrid.Children.Add(cellBlock);
                }
            }
        }

        // Update the visualization based on agent's new position
        public void UpdateAgentPosition()
        {
            RenderGrid();
        }

        private string GetCellText(GridCell cell, bool isExplored)
        {
            if (!isExplored) return "";  // Hide text for unexplored cells

            if (cell == agent.Position) return "A";  // Agent position
            return cell.Type switch
            {
                CellType.Start => "S",
                CellType.Goal => "G",
                CellType.Obstacle => "X",
                _ => "."  // Default for empty cells
            };
        }

        private IBrush GetCellBackground(GridCell cell, bool isExplored)
        {
            if (!isExplored) return Brushes.DarkGray;  // Color for unexplored cells (fog of war)

            if (cell == agent.Position) return Brushes.LightGreen;
            if (cell.Type == CellType.Start) return Brushes.LightBlue;
            if (cell.Type == CellType.Goal) return Brushes.Gold;
            if (cell.Type == CellType.Obstacle) return Brushes.Gray;
            return Brushes.White;
        }
    }
}
