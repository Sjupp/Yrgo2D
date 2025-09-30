using UnityEngine;

namespace Battleship
{
    public interface IBattleship
    {
        //returns the name of the player, this should be your name!
        //If you don't use your name, you cant win.
        string GetName();

        //This function gets called when we start a new game.
        //return your game field (where you have placed your ships)
        //True for ship and false for blank square.
        bool[,] NewGame(Vector2Int gridSize, string opponentName);

        //When it's your turn to fire, return where you want to fire
        Vector2Int Fire();

        //After a hit is resolved the result function will be called.
        //Here your class will get to know if you hit something.
        void Result(Vector2Int position, bool hit, bool sunk);
    }
}