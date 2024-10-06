// src/Utilities/PerformanceMetrics.cs
using System;

namespace GridMind.Utilities
{
    public class PerformanceMetrics
    {
        public string AgentName { get; set; }
        public int StepsTaken { get; set; }
        public int CellsExplored { get; set; }
        public int RepeatedVisits { get; set; }
        public TimeSpan TimeToGoal { get; set; }
        public bool GoalReached { get; set; }

        public PerformanceMetrics(string agentName)
        {
            AgentName = agentName;
            StepsTaken = 0;
            CellsExplored = 0;
            RepeatedVisits = 0;
            TimeToGoal = TimeSpan.Zero;
            GoalReached = false;
        }

        public void IncrementSteps()
        {
            StepsTaken++;
        }

        public void IncrementCellsExplored()
        {
            CellsExplored++;
        }

        public void IncrementRepeatedVisits()
        {
            RepeatedVisits++;
        }
    }
}
