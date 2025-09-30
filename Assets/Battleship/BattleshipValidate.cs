using System.Collections.Generic;
using UnityEngine;

namespace Battleship
{

    public class BattleshipValidate : MonoBehaviour
    {
        IBattleship player;
        bool[,] playerGrid;
        Vector2Int gridSize;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (int i = 0; i < 100; i++) //Numer of tests to run
            {
                player = new AntLin();

                //set up grid size
                gridSize = new Vector2Int(Random.Range(10, 101), Random.Range(10, 101));

                //create a new game, 
                playerGrid = player.NewGame(gridSize, "opponentName");

                //Validate player
                bool playerValid = Validate(playerGrid);

                Debug.Log(player.GetName() + " valid: " + playerValid);
            }
        }

        private bool Validate(bool[,] playerGrid)
        {
            //check so grid size is correct
            if (playerGrid.GetLength(0) != gridSize.x || playerGrid.GetLength(1) != gridSize.y)
                return false;

            //check for number of ship squares, should be 13
            int sum = 0;
            foreach (bool gridSpace in playerGrid)
            {
                if (gridSpace)
                    sum++;
            }

            if (sum != 13)
                return false;

            //Validation for all ship types
            List<int> ships = new List<int>();
            int shipLength = 0;
            bool lastcheck = false;
            for (int x = 0; x < playerGrid.GetLength(0); x++)
            {
                for (int y = 0; y < playerGrid.GetLength(1); y++)
                {
                    if (playerGrid[x, y])
                    {
                        shipLength++;
                        lastcheck = true;
                    }

                    if (lastcheck && !playerGrid[x, y] || y == playerGrid.GetLength(1) - 1)
                    {
                        if (shipLength > 1)
                            ships.Add(shipLength);
                        //we have found the end of a ship.

                        shipLength = 0;
                        lastcheck = false;
                    }
                }
            }

            for (int y = 0; y < playerGrid.GetLength(1); y++)
            {
                for (int x = 0; x < playerGrid.GetLength(0); x++)
                {
                    if (playerGrid[x, y])
                    {
                        shipLength++;
                        lastcheck = true;
                    }

                    if (lastcheck && !playerGrid[x, y] || x == playerGrid.GetLength(0) - 1)
                    {
                        if (shipLength > 1)
                            ships.Add(shipLength);
                        //we have found the end of a ship.

                        shipLength = 0;
                        lastcheck = false;
                    }
                }
            }

            //We should find 4 ships, if we have found more or less, 
            //they are not positioned correctly.
            if (ships.Count != 4)
                return false;

            ships.Sort();
            int[] correctShips = { 2, 3, 3, 4 };

            for (int i = 0; i < ships.Count; i++)
            {
                if (correctShips[i] != ships[i])
                    return false;
            }

            //all checks passed, we are valid
            return true;
        }
    }
}