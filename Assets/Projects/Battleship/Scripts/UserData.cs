using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public class UserData
    {
        private int _playerID = -1;
        private string userName = "";
        private bool[,] grid = null;
        private IBattleship _interfaceRef = null;
        private List<ShipData> _ships = null;

        public int PlayerID { get => _playerID; set => _playerID = value; }
        public string UserName { get => userName; set => userName = value; }
        public bool[,] Grid { get => grid; set => grid = value; }
        public IBattleship InterfaceRef { get => _interfaceRef; set => _interfaceRef = value; }
        public List<ShipData> ShipData { get => _ships; set => _ships = value; }
        public int RemainingCells => _ships.Sum(x => x.Length) - _ships.Sum(x => x.Damaged);
        public bool NoRemainingShips => _ships.Count(x => x.Sunk) == _ships.Count;
    }
}
