using System.Collections.Generic;
using UnityEngine;

namespace Battleship
{
    public class ShipData
    {
        private List<Vector2Int> _shipCoordinates = new();
        private List<Vector2Int> _damagedCoordinates = new();

        public ShipData(List<Vector2Int> shipCoordinates)
        {
            ShipCoordinates = shipCoordinates;
        }

        public List<Vector2Int> ShipCoordinates { get => _shipCoordinates; set => _shipCoordinates = value; }
        public List<Vector2Int> DamagedCoordinates { get => _damagedCoordinates; set => _damagedCoordinates = value; }

        public int Length => _shipCoordinates.Count;
        public int Damaged => _damagedCoordinates.Count;
        public bool Sunk => _shipCoordinates.Count == _damagedCoordinates.Count;

    }
}
