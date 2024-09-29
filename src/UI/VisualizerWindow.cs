// src/UI/VisualizerWindow.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GridMind.Environment;
using System;
using System.Threading.Tasks;

namespace GridMind.UI
{
    public partial class VisualizerWindow : Window
    {
        private Environment.Grid? grid;
        private Agents.Agent? agent;
        private Action onNextMove;
        private Button stepButton;
        private Button playButton;
        private bool isPlaying;

        public VisualizerWindow(Environment.Grid grid, Agents.Agent agent, Action onNextMove)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.agent = agent ?? throw new ArgumentNullException(nameof(agent));
            this.onNextMove = onNextMove ?? throw new ArgumentNullException(nameof(onNextMove));

            InitializeComponent();

            // Safely find and assign controls
            stepButton = this.FindControl<Button>("StepButton") ?? throw new NullReferenceException("StepButton not found");
            playButton = this.FindControl<Button>("PlayButton") ?? throw new NullReferenceException("PlayButton not found");

            stepButton.Click += async (_, __) => StepThrough();
            playButton.Click += (_, __) => TogglePlay();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void StepThrough()
        {
            onNextMove.Invoke();  // Trigger the next move in the program
            RenderGrid();
        }

        private async Task PlayFramesAsync()
        {
            isPlaying = true;
            while (isPlaying)
            {
                StepThrough();
                await Task.Delay(500);  // Adjust delay for slower/faster playback
            }
        }

        private void TogglePlay()
        {
            if (isPlaying)
            {
                isPlaying = false;  // Stop the playback
                playButton.Content = "Play";
            }
            else
            {
                playButton.Content = "Pause";
                _ = PlayFramesAsync();
            }
        }

        // Render the current state of the grid
        private void RenderGrid()
        {
            Console.Clear();
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Columns; col++)
                {
                    if (agent.Position.Row == row && agent.Position.Column == col)
                    {
                        Console.Write("A ");
                    }
                    else
                    {
                        var cell = grid.GetCell(row, col);
                        switch (cell.Type)
                        {
                            case CellType.Start:
                                Console.Write("S ");
                                break;
                            case CellType.Goal:
                                Console.Write("G ");
                                break;
                            case CellType.Obstacle:
                                Console.Write("X ");
                                break;
                            default:
                                Console.Write(". ");
                                break;
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();  // Add some spacing
        }
    }
}
