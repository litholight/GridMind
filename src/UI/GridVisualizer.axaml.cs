// src/UI/GridVisualizer.axaml.cs
using Avalonia.Controls;
using Avalonia.Media;
using GridMind.Environment;
using GridMind.Agents;

namespace GridMind.UI
{
    public partial class GridVisualizer : UserControl
    {
        private readonly Environment.Grid grid;
        private readonly Agent agent;

        public GridVisualizer()
        {
            InitializeComponent();
        }

        public GridVisualizer(Environment.Grid grid, Agent agent) : this()
        {
            this.grid = grid;
            this.agent = agent;
            RenderGrid();
        }

        // Render the grid using Avalonia controls
        public void RenderGrid()
        {
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.Children.Clear();

            // Define rows and columns based on grid dimensions
            for (int row = 0; row < grid.Rows; row++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            }

            for (int col = 0; col < grid.Columns; col++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            }

            // Render each cell
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Columns; col++)
                {
                    var cell = grid.GetCell(row, col);
                    var cellBlock = new TextBlock
                    {
                        Text = GetCellText(cell),
                        Background = GetCellBackground(cell),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        FontSize = 20
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
            // Re-render the grid with updated agent position
            RenderGrid();
        }

        private string GetCellText(GridCell cell)
        {
            if (cell == agent.Position) return "A";  // Agent position
            return cell.Type switch
            {
                CellType.Start => "S",
                CellType.Goal => "G",
                CellType.Obstacle => "X",
                _ => "."
            };
        }

        private IBrush GetCellBackground(GridCell cell)
        {
            if (cell == agent.Position) return Brushes.LightGreen;
            if (cell.Type == CellType.Start) return Brushes.LightBlue;
            if (cell.Type == CellType.Goal) return Brushes.Gold;
            if (cell.Type == CellType.Obstacle) return Brushes.Gray;
            return Brushes.White;
        }
    }
}
