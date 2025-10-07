using UnityEngine;

namespace Battleship
{
    public struct Turn
    {
        private int _turnOwnerID;
        private Vector2Int _fireTarget;
        private bool _hit;
        private int _shipCellsRemaining;
        private bool _gameOver;

        public Turn(int turnOwnerID, Vector2Int fireTarget, bool hit, int shipCellsRemaining, bool gameOver) : this()
        {
            TurnOwnerID = turnOwnerID;
            FireTarget = fireTarget;
            Hit = hit;
            ShipCellsRemaining = shipCellsRemaining;
            GameOver = gameOver;
        }

        public int TurnOwnerID { readonly get => _turnOwnerID; set => _turnOwnerID = value; }
        public Vector2Int FireTarget { readonly get => _fireTarget; set => _fireTarget = value; }
        public bool Hit { readonly get => _hit; set => _hit = value; }
        public int ShipCellsRemaining { readonly get => _shipCellsRemaining; set => _shipCellsRemaining = value; }
        public bool GameOver { readonly get => _gameOver; set => _gameOver = value; }
    }
}
