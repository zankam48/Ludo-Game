namespace LudoGame.Classes
{
    using LudoGame.Interfaces;
    using System;

    public class Display : IDisplay
    {
        public void DisplayBoard(Board board)
        {
            for (int r = 0; r < Board.BoardSize; r++)
            {
                for (int c = 0; c < Board.BoardSize; c++)
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

        static public void InputKey(bool input)
        {
            Console.ReadKey(input);
        }

        public string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
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
