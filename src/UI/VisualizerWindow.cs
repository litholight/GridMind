// src/UI/VisualizerWindow.axaml.cs
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using GridMind.Agents;
using GridMind.Environment;
using GridMind.Navigation;
using GridMind.Utilities;
using ReactiveUI;

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
            visualizerControl = this.FindControl<GridVisualizer>("GridVisualizerControl");
            StepButton = this.FindControl<Button>("StepButton");
            PlayButton = this.FindControl<Button>("PlayButton");
            StatusLabel = this.FindControl<TextBlock>("StatusLabel");
            ControlToggleButton = this.FindControl<Button>("ControlToggleButton");

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

            performanceLabel = this.FindControl<TextBlock>("PerformanceLabel");

            // Subscribe to performance changes
            agent.PerformanceTracker.Subscribe(new PerformanceObserver(PerformanceLabel));
        }

        private async Task StepThroughAsync()
        {
            onNextMove.Invoke();
            visualizerControl?.UpdateAgentPosition();

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
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (isHumanControl)
            {
                if (e.Key == Key.Back)
                {
                    humanController.UndoLastMove();
                    visualizerControl?.UpdateAgentPosition();
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
                visualizerControl?.UpdateAgentPosition();
            }
        }
    }
}
