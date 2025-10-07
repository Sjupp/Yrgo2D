using System.Collections.Generic;

namespace Battleship
{
    public class GameData
    {
        private UserData _user0;
        private UserData _user1;
        private string _winner = "";
        private string _loser = "";
        private List<Turn> _turns = new List<Turn>();
        private int _gameResultDiscrepancy = 0;

        public UserData User0 { get => _user0; }
        public UserData User1 { get => _user1; }
        public string Winner { get => _winner; set => _winner = value; }
        public string Loser { get => _loser; set => _loser = value; }
        public List<Turn> TurnHistory { get => _turns; set => _turns = value; }
        public int GameResultDiscrepancy { get => _gameResultDiscrepancy; set => _gameResultDiscrepancy = value; }

        public void SetUsers(UserData user0, UserData user1)
        {
            _user0 = user0;
            _user1 = user1;
        }
    }
}
