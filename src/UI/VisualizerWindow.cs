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
                await Task.Delay(250); // Adjust delay for slower/faster playback
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
            int viewRadius = 6; // Define the radius of the viewable grid around the agent
            var container = this.FindControl<Avalonia.Controls.Grid>("MainGridContainer");

            // Create a new grid dynamically for rendering
            var visualGrid = new Avalonia.Controls.Grid();

            // Define row and column structures for the visual grid
            for (int i = 0; i < viewRadius * 2 + 1; i++)
            {
                visualGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
                visualGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            }

            // Get the size of the MainGridContainer
            double containerWidth = container.Bounds.Width;
            double containerHeight = container.Bounds.Height;

            // Calculate the ideal cell size
            double cellWidth = containerWidth / (viewRadius * 2 + 1);
            double cellHeight = containerHeight / (viewRadius * 2 + 1);

            // Render each cell within the view radius around the agent
            for (int rowOffset = -viewRadius; rowOffset <= viewRadius; rowOffset++)
            {
                for (int colOffset = -viewRadius; colOffset <= viewRadius; colOffset++)
                {
                    int wrappedRow = grid.GetWrappedRow(agent.Position.Row + rowOffset);
                    int wrappedCol = grid.GetWrappedColumn(agent.Position.Column + colOffset);

                    var cell = grid.GetWrappedCell(wrappedRow, wrappedCol);

                    var cellBlock = new TextBlock
                    {
                        Background = GetCellBackground(cell),
                        Width = cellWidth,
                        Height = cellHeight,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
                    };

                    // Set the cellBlock's position in the visual grid
                    Avalonia.Controls.Grid.SetRow(cellBlock, rowOffset + viewRadius);
                    Avalonia.Controls.Grid.SetColumn(cellBlock, colOffset + viewRadius);
                    visualGrid.Children.Add(cellBlock);
                }
            }

            // Replace the previous content of MainGridContainer with the updated visual grid
            container.Children.Clear();
            container.Children.Add(visualGrid);
        }

        private IBrush GetCellBackground(GridCell cell)
        {
            if (!agent.ExploredCells.Contains(cell))
                return Brushes.Gray; // Unexplored
            if (cell == agent.Position)
                return Brushes.LightGreen;
            if (cell.Type == CellType.Start)
                return Brushes.LightBlue;
            if (cell.Type == CellType.Goal)
                return Brushes.Gold;
            if (cell.Type == CellType.Obstacle)
                return Brushes.Red;
            return Brushes.White;
        }
    }
}
