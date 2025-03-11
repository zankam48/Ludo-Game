namespace LudoGame.Classes;
public class Dice
{
    private Random rand;
    
    public Dice()
    {
        rand = new Random();
    }
    
    public int Roll()
    {
        return rand.Next(1, 7);
    }
}