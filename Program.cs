using System;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("----Ludo Game----");
            Console.WriteLine("Choose the number of players (2-4):");
            
            if (!int.TryParse(Console.ReadLine(), out int numberOfPlayers))
            {
                Console.WriteLine("Invalid input. Please enter a number between 2 and 4.");
                return;
            }

            if (numberOfPlayers < 2 || numberOfPlayers > 4)
            {
                Console.WriteLine("Invalid number of players. Please enter a number between 2 and 4.");
                return;
            }

            Game game = new Game(numberOfPlayers);
            game.StartGame();
        }
        catch (FormatException ex)
        {
            Console.WriteLine("Invalid input format. Please enter a valid number.");
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred.");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Thank you for playing Ludo!");
        }
    }
}
