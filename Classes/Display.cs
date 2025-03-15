namespace LudoGame.Classes
{
    using LudoGame.Interfaces;
    using System;

    public class Display : IDisplay
    {
        public void DisplayBoard(Board board)
        {
            board.PrintBoard();
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public int GetIntInput(string prompt)
        {
            int value;
            while (!int.TryParse(GetInput(prompt), out value))
            {
                Console.WriteLine("Invalid input, please try again.");
            }
            return value;
        }
    }
}
