using LudoGame.Interfaces;

namespace LudoGame.Classes;
public class Dice : IDice
{
    private Random _rand;
    
    public Dice()
    {
        _rand = new Random();
    }
    
    public int Roll()
    {
        return _rand.Next(1, 7);
    }
}