using System.Collections.Generic;
using UnityEngine;

namespace Battleship
{
    public enum Direction
    {
        Right,
        Up,
        Left,
        Down
    }

    public static class Helper
    {
        public static Vector2Int VectorDirection(Direction direction)
        {
            Vector2Int vector = new Vector2Int();
            switch (direction)
            {
                case Direction.Right:
                    vector = Vector2Int.right;
                    break;
                case Direction.Up:
                    vector = Vector2Int.up;
                    break;
                case Direction.Left:
                    vector = Vector2Int.left;
                    break;
                case Direction.Down:
                    vector = Vector2Int.down;
                    break;
                default:
                    Debug.LogError("No valid direction chosen");
                    break;
            }
            return vector;
        }

        public static Direction RandomDirection()
        {
            return (Direction)Random.Range(0, 4);
        }
    }

    public class Hej
    {
        public void Foo()
        {
            var gurkÄtarKlass = new AntLin();
            var asdf = gurkÄtarKlass.Gurkor;
        }
    }

    public class AntLin : IBattleship
    {
        bool[,] myPlayField;
        Vector2Int gridSize;

        private float _gurkor = 0;

        public float Gurkor { get => _gurkor; }


        public string GetName()
        {
            return "Anton Lindkvist";
        }

        public bool[,] NewGame(Vector2Int gridSize, string opponentName)
        {
            //Create our field
            myPlayField = new bool[gridSize.x, gridSize.y];

            this.gridSize = gridSize;

            //You don't need to do anything with opponent Name
            //this is more if you want to keep track of your 
            //opponents names and tactics.

            //we now need to place our ships, lets just do one for the demo.

            var selectedEnum = Direction.Right;

            //Since we haven't placed all our ships, this would not validate.

            List<int> placedShips = new List<int> { 4, 3, 3, 2, 1 };
            int placedShipNumber = 0;
            bool placedAllShips = false;
            while (!placedAllShips)
            {
                if (TryPlaceShip(
                    new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y)),
                    placedShips[placedShipNumber],
                    Helper.RandomDirection()
                    ))
                {
                    placedShipNumber++;
                }

                if (placedShipNumber == placedShips.Count - 1)
                    placedAllShips = true;
            }

            return myPlayField;
        }

        public Vector2Int Fire()
        {
            //This function just fire randomly blindly and doesn't keep track of anything
            //This is probably not a good strategy.

            return new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        }

        public void Result(Vector2Int position, bool hit, bool sunk)
        {
            //Here we don't need to do anything, but it might be a good idea to fire 
            //next to previous hits?
        }

        private bool TryPlaceShip(Vector2Int startPosition, int shipSize, Direction direction)
        {
            var endPos = startPosition + Helper.VectorDirection(direction) * shipSize;
            if (IsCoordinateInsideGrid(endPos))
            {
                for (int i = 0; i < shipSize; i++)
                {
                    Vector2Int searchPos = startPosition + Helper.VectorDirection(direction) * i;
                    if (myPlayField[searchPos.x, searchPos.y] == true)
                    {
                        return false;
                    }
                }

                for (int i = 0; i < shipSize; i++)
                {
                    Vector2Int placedPos = startPosition + Helper.VectorDirection(direction) * i;
                    myPlayField[placedPos.x, placedPos.y] = true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsCoordinateInsideGrid(Vector2Int coordinate)
        {
            return !(coordinate.x < 0 || coordinate.x >= gridSize.x || coordinate.y < 0 || coordinate.y >= gridSize.y);
        }
    }
}