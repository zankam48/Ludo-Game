namespace LudoGame.Interfaces
{
    public interface IDisplay
    {
        void DisplayBoard(Classes.Board board);
        void DisplayMessage(string message);
        string GetInput(string prompt);
        int GetIntInput(string prompt);
    }
}
