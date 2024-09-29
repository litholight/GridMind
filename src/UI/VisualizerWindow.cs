// src/UI/VisualizerWindow.axaml.cs
using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using GridMind.Environment;
using GridMind.Agents;

namespace GridMind.UI
{
    public partial class VisualizerWindow : Window
    {
        private readonly Environment.Grid grid;
        private readonly Agent agent;
        private Action onNextMove;
        private bool isPlaying;
        private GridVisualizer? visualizerControl;

        public VisualizerWindow(Environment.Grid grid, Agent agent, Action onNextMove)
        {
            this.grid = grid;
            this.agent = agent;
            this.onNextMove = onNextMove;

            InitializeComponent();

            // Find controls defined in the XAML layout
            visualizerControl = this.FindControl<GridVisualizer>("GridVisualizerControl");
            StepButton = this.FindControl<Button>("StepButton");
            PlayButton = this.FindControl<Button>("PlayButton");
            StatusLabel = this.FindControl<TextBlock>("StatusLabel");

            // Attach event handlers to the buttons
            StepButton.Click += async (_, __) => await StepThroughAsync();
            PlayButton.Click += (_, __) => TogglePlay();

            // Initialize the GridVisualizer control with the grid and agent
            visualizerControl = new GridVisualizer(grid, agent);
            this.FindControl<Avalonia.Controls.Grid>("MainGridContainer").Children.Add(visualizerControl);
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
                await Task.Delay(500);  // Adjust delay for slower/faster playback
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
                isPlaying = false;  // Stop the playback
                PlayButton.Content = "Play";
            }
            else
            {
                PlayButton.Content = "Pause";
                _ = PlayFramesAsync();
            }
        }
    }
}
