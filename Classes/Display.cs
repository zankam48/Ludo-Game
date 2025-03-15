namespace LudoGame.Classes
{
    using LudoGame.Interfaces;
    using System;

    public class Display : IDisplay
    {
        // Displays the board by calling the board's PrintBoard method.
        public void DisplayBoard(Board board)
        {
            board.PrintBoard();
        }

        // Displays a message on the console.
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        // Prompts the user and returns their input as a string.
        public string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        // Prompts the user until a valid integer is entered.
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
