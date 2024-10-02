// src/Commands/ICommand.cs
namespace GridMind.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
