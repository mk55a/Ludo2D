
using System;
public class Helper 
{
    public static TileColor ConvertColorToTileColor(Color color)
    {
        switch (color)
        {
            case Color.RED:
                return TileColor.RED;
            case Color.BLUE:
                return TileColor.BLUE;
            case Color.YELLOW:
                return TileColor.YELLOW;
            case Color.GREEN:
                return TileColor.GREEN;
            default:
                throw new ArgumentException("Unknown color");
        }
    }
}
