namespace LudoGame.Classes
{
    using LudoGame.Interfaces;
    using System;

    public class Display : IDisplay
    {
        public void DisplayBoard(Board board)
        {
            // board.PrintBoard();
            const int BOARD_SIZE = 15;
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    Console.Write(board.grid[r, c].Occupant + " ");
                }
                Console.WriteLine();
    
            }
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
