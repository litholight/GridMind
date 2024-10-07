// src/UI/VisualizerWindow.axaml.cs
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using GridMind.Agents;
using GridMind.Environment;
using GridMind.Navigation;
using GridMind.Utilities;

namespace GridMind.UI
{
    public partial class VisualizerWindow : Window, INotifyPropertyChanged
    {
        private readonly Environment.Grid grid;
        private readonly Agent agent;
        private Action onNextMove;
        private bool isPlaying;
        private GridVisualizer? visualizerControl;
        private IMovementStrategy aiStrategy;
        private HumanController humanController;
        private bool isHumanControl = false;
        private TextBlock performanceLabel;

        public VisualizerWindow(Environment.Grid grid, Agent agent, Action onNextMove)
        {
            this.grid = grid;
            this.agent = agent;
            this.onNextMove = onNextMove;

            InitializeComponent();

            agent.PropertyChanged += Agent_PropertyChanged;

            // Initialize strategies
            aiStrategy = new BreadthFirstSearchStrategy();
            humanController = new HumanController(agent, grid);

            // Find controls defined in the XAML layout
            StepButton = this.FindControl<Button>("StepButton");
            PlayButton = this.FindControl<Button>("PlayButton");
            StatusLabel = this.FindControl<TextBlock>("StatusLabel");
            ControlToggleButton = this.FindControl<Button>("ControlToggleButton");
            performanceLabel = this.FindControl<TextBlock>("PerformanceLabel");

            // Attach event handlers to the buttons
            StepButton.Click += async (_, __) => await StepThroughAsync();
            PlayButton.Click += (_, __) => TogglePlay();
            ControlToggleButton.Click += (_, __) => ToggleControlMode();

            // Initialize the GridVisualizer control with the grid and agent
            visualizerControl = new GridVisualizer(grid, agent);
            this.FindControl<Avalonia.Controls.Grid>("MainGridContainer")
                .Children.Add(visualizerControl);

            // Set focus to the window to receive key events
            this.Focus();

            // Subscribe to key down events
            this.KeyDown += OnKeyDownHandler;

            // Subscribe to performance changes
            agent.PerformanceTracker.Subscribe(new PerformanceObserver(performanceLabel));

            // Initial rendering of the grid
            RenderGrid();
        }

        private async Task StepThroughAsync()
        {
            onNextMove.Invoke();
            RenderGrid();

            if (agent.Position == agent.Goal)
            {
                StatusLabel.Text = "Agent has reached the goal!";
            }
        }

        private async Task PlayFramesAsync()
        {
            isPlaying = true;
            while (isPlaying && agent.Position != agent.Goal)
            {
                await StepThroughAsync();
                await Task.Delay(75); // Adjust delay for slower/faster playback
            }
            if (agent.Position == agent.Goal)
            {
                StatusLabel.Text = "Agent has reached the goal!";
            }
        }

        private void TogglePlay()
        {
            if (isPlaying)
            {
                isPlaying = false; // Stop the playback
                PlayButton.Content = "Play";
            }
            else
            {
                PlayButton.Content = "Pause";
                _ = PlayFramesAsync();
            }
        }

        private void ToggleControlMode()
        {
            isHumanControl = !isHumanControl;

            if (isHumanControl)
            {
                ControlToggleButton.Content = "Switch to AI Control";
                agent.SetMovementStrategy(humanController);
                StatusLabel.Text = "Control Mode: Human";
            }
            else
            {
                ControlToggleButton.Content = "Switch to Human Control";
                agent.SetMovementStrategy(aiStrategy);
                StatusLabel.Text = "Control Mode: AI";
            }

            // Refresh the grid to reflect fog of war change
            RenderGrid();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (isHumanControl)
            {
                if (e.Key == Key.Back)
                {
                    humanController.UndoLastMove();
                    RenderGrid();
                    return;
                }

                MovementDirection direction = MovementDirection.None;
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.W:
                        direction = MovementDirection.Up;
                        break;
                    case Key.Down:
                    case Key.S:
                        direction = MovementDirection.Down;
                        break;
                    case Key.Left:
                    case Key.A:
                        direction = MovementDirection.Left;
                        break;
                    case Key.Right:
                    case Key.D:
                        direction = MovementDirection.Right;
                        break;
                }

                if (direction != MovementDirection.None)
                {
                    humanController.SetDirection(direction);
                    _ = StepThroughAsync();
                }
            }
        }

        private void Agent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(agent.Position))
            {
                RenderGrid();
            }
        }

        private void RenderGrid()
        {
            int viewRadius = 3; // Define the radius of the viewable grid around the agent

            // Create a new grid dynamically for rendering
            var visualGrid = new Avalonia.Controls.Grid();

            for (int i = 0; i < viewRadius * 2 + 1; i++)
            {
                visualGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
                visualGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            }

            // Add cells to the visual grid
            for (int row = -viewRadius; row <= viewRadius; row++)
            {
                for (int col = -viewRadius; col <= viewRadius; col++)
                {
                    int wrappedRow = (agent.Position.Row + row + grid.Rows) % grid.Rows;
                    int wrappedCol = (agent.Position.Column + col + grid.Columns) % grid.Columns;

                    var cell = grid.GetWrappedCell(wrappedRow, wrappedCol);
                    var cellBlock = new TextBlock
                    {
                        Text = GetCellText(cell),
                        Background = GetCellBackground(cell),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        FontSize = 20
                    };

                    // Set the position in the visual grid
                    Avalonia.Controls.Grid.SetRow(cellBlock, row + viewRadius);
                    Avalonia.Controls.Grid.SetColumn(cellBlock, col + viewRadius);
                    visualGrid.Children.Add(cellBlock);
                }
            }

            // Replace the previous content of MainGridContainer with the updated grid
            var container = this.FindControl<Avalonia.Controls.Grid>("MainGridContainer");
            container.Children.Clear();
            container.Children.Add(visualGrid);
        }

        private string GetCellText(GridCell cell)
        {
            if (!agent.ExploredCells.Contains(cell))
                return "?"; // Unexplored cells show as "?"
            if (cell == agent.Position)
                return "A"; // Agent position
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
            if (!agent.ExploredCells.Contains(cell))
                return Brushes.DarkSlateGray; // Unexplored
            if (cell == agent.Position)
                return Brushes.LightGreen;
            if (cell.Type == CellType.Start)
                return Brushes.LightBlue;
            if (cell.Type == CellType.Goal)
                return Brushes.Gold;
            if (cell.Type == CellType.Obstacle)
                return Brushes.Gray;
            return Brushes.White;
        }
    }
}
