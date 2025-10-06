using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battleship
{
    public struct Turn
    {
        //private string attackerName;
        private Vector2Int _fireTarget;
        private bool _hit;

        public Turn(Vector2Int fireTarget, bool hit) : this()
        {
            FireTarget = fireTarget;
            Hit = hit;
        }

        //public string AttackerName { get => attackerName; set => attackerName = value; }
        public Vector2Int FireTarget { readonly get => _fireTarget; set => _fireTarget = value; }
        public bool Hit { readonly get => _hit; set => _hit = value; }
    }

    public class GameData
    {
        private UserData _user0;
        private UserData _user1;

        private string _winner = "";
        private string _loser = "";
        private List<Turn> _turns = new List<Turn>();
        private int _gameResultDiscrepancy = 0;

        public string Winner { get => _winner; set => _winner = value; }
        public string Loser { get => _loser; set => _loser = value; }
        public List<Turn> TurnHistory { get => _turns; set => _turns = value; }
        public int GameResultDiscrepancy { get => _gameResultDiscrepancy; set => _gameResultDiscrepancy = value; }
        public UserData User0 { get => _user0; }
        public UserData User1 { get => _user1; }

        public void SetUsers(UserData user0, UserData user1)
        {
            _user0 = user0;
            _user1 = user1;
        }
    }

    public class SimulationData
    {
        private List<GameData> _gameHistory = new();

        public List<GameData> GameHistory { get => _gameHistory; set => _gameHistory = value; }

        // select three closest games
    }

    public class ShipData
    {
        private List<Vector2Int> _shipCoordinates = new();

        public ShipData(List<Vector2Int> shipCoordinates)
        {
            ShipCoordinates = shipCoordinates;
        }

        public List<Vector2Int> ShipCoordinates { get => _shipCoordinates; set => _shipCoordinates = value; }

        public int Length => _shipCoordinates.Count;
        public bool Sunk => _shipCoordinates.Count == 0;
    }

    public class UserData
    {
        private string userName = "";
        private bool[,] grid = null;
        private IBattleship _interfaceRef = null;
        private List<ShipData> _ships = null;

        public string UserName { get => userName; set => userName = value; }
        public bool[,] Grid { get => grid; set => grid = value; }
        public IBattleship InterfaceRef { get => _interfaceRef; set => _interfaceRef = value; }
        public List<ShipData> ShipData { get => _ships; set => _ships = value; }
        public int RemainingCells => _ships.Sum(x => x.Length);
        public bool NoRemainingShips { get => _ships.Count == 0; }
    }

    public class BattleshipSimulator : MonoBehaviour
    {
        [SerializeField]
        private List<IBattleship> _players = new List<IBattleship>();
        [SerializeField]
        private string _overrideNameOfPlayerA = null;
        [SerializeField]
        private string _overrideNameOfPlayerB = null;
        [SerializeField]
        private int _numberOfTestsToRun = 100;
        [SerializeField]
        private Vector2Int _gridSize = new Vector2Int(10, 10);

        private List<int> _presetShipSizes = new List<int>() { 4, 3, 3, 2, 1 };

        private void Start()
        {
            Dictionary<string, int> scoreTally = new Dictionary<string, int>();
            for (int i = 0; i < _numberOfTestsToRun; i++)
            {
                if (SetupGame(new OttWen(), new AntLin(), out GameData gameData))
                {
                    var gameResult = RunGame(gameData);
                    
                    Debug.Log("Game " + i + " result, winner: " + gameResult.Winner);
                    Debug.Log("Discrepancy " + gameResult.GameResultDiscrepancy);
                    
                    if (scoreTally.ContainsKey(gameResult.Winner))
                    {
                        scoreTally[gameResult.Winner] += 1;
                    }
                    else
                    {
                        scoreTally.Add(gameResult.Winner, 1);
                    }
                }
            }

            var maxKey = scoreTally.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            Debug.Log("Total winner " + maxKey + " with " + scoreTally[maxKey] + " wins!");
        }

        private bool SetupGame(IBattleship user0, IBattleship user1, out GameData gameData)
        {
            gameData = null;

            string username0 = user0.GetName();
            string username1 = user1.GetName();
            if (ValidateName(user0))
            {
                username0 = user0.GetName();
            }
            else
            {
                return false;
            }

            if (ValidateName(user1))
            {
                username1 = user1.GetName();
            }
            else
            {
                return false;
            }

            var userData0 = BuildUserData(user0, username1, 0);
            if (!ValidateGameBoard(userData0, _presetShipSizes))
            {
                return false;
            }

            var userData1 = BuildUserData(user1, username0, 1);
            if (!ValidateGameBoard(userData1, _presetShipSizes))
            {
                return false;
            }

            gameData = new GameData();
            gameData.SetUsers(userData0, userData1);

            return true;
        }

        private UserData BuildUserData(IBattleship user, string oppName, int userIndex)
        {
            var userData = new UserData();
            if (userIndex == 0)
            {
                if (_overrideNameOfPlayerA != null)
                {
                    userData.UserName = _overrideNameOfPlayerA;
                }
                else
                {
                    userData.UserName = user.GetName();
                }
            }
            else if (userIndex == 1)
            {
                if (_overrideNameOfPlayerB != null)
                {
                    userData.UserName = _overrideNameOfPlayerB;
                }
                else
                {
                    userData.UserName = user.GetName();
                }
            }
            userData.Grid = user.NewGame(_gridSize, oppName);
            userData.InterfaceRef = user;
            userData.ShipData = FindShips(userData.Grid);

            return userData;
        }

        private GameData RunGame(GameData gameData)
        {
            int maxAttempts = 10000;

            int turn = 0;
            bool gameOver = false;

            while (!gameOver)
            {
                var attackingPlayer = turn % 2 == 0 ? gameData.User0 : gameData.User1;
                var defendingPlayer = turn % 2 == 1 ? gameData.User0 : gameData.User1;
                var attackResult = Attack(attackingPlayer.InterfaceRef.Fire(), defendingPlayer);
                attackingPlayer.InterfaceRef.Result(attackResult.Item1, attackResult.Item2, attackResult.Item3);

                gameData.TurnHistory.Add(new Turn(attackResult.Item1, attackResult.Item2));

                if (defendingPlayer.NoRemainingShips)
                {
                    gameData.Winner = attackingPlayer.UserName;
                    gameData.Loser = defendingPlayer.UserName;
                    gameData.GameResultDiscrepancy = attackingPlayer.ShipData.Count - defendingPlayer.ShipData.Count;
                    gameOver = true;
                }
                else
                {
                    turn++;
                }

                if (turn >= maxAttempts)
                {
                    Debug.LogError("reached maxAttempts, aborting");
                    gameOver = true;
                }
            }

            return gameData;
        }

        private bool TrySinkShip(List<ShipData> ships, Vector2Int attackingCoordinate)
        {
            bool sunkShip = false;
            ShipData shipToRemove = null;
            foreach (var ship in ships)
            {
                if (ship.ShipCoordinates.Contains(attackingCoordinate))
                {
                    ship.ShipCoordinates.Remove(attackingCoordinate);
                    if (ship.Sunk)
                    {
                        sunkShip = true;
                        shipToRemove = ship;
                    }
                }
            }
            ships.Remove(shipToRemove);

            return sunkShip;
        }

        private bool ValidateGameBoard(UserData userData, List<int> presetShipSizes)
        {
            if (userData.Grid.GetLength(1) != _gridSize.x || userData.Grid.GetLength(0) != _gridSize.y)
            {
                Debug.Log("Not Valid: GridSize");
                return false;
            }
            foreach (var ship in userData.ShipData)
            {
                if (!presetShipSizes.Contains(ship.Length))
                {
                    Debug.Log("Not Valid: Found Ship of Irregular Length");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateName(IBattleship playerInterface)
        {
            if (playerInterface == null)
            {
                Debug.Log("Not Valid: UserData");
                return false;
            }
            if (string.IsNullOrEmpty(playerInterface.GetName()))
            {
                Debug.Log("Not Valid: UserName");
                return false;
            }
            return true;
        }

        private (Vector2Int, bool, bool) Attack(Vector2Int attackingCoordinate, UserData defendingUser)
        {
            (Vector2Int, bool, bool) attackData;
            attackData.Item1 = attackingCoordinate;
            attackData.Item2 = defendingUser.Grid[attackingCoordinate.x, attackingCoordinate.y];
            attackData.Item3 = TrySinkShip(defendingUser.ShipData, attackingCoordinate);

            return attackData;
        }

        private List<ShipData> FindShips(bool[,] grid)
        {
            List<ShipData> ships = new();
            List<Vector2Int> searchedSquares = new();
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    if (!searchedSquares.Contains(new Vector2Int(x, y)) && grid[x, y])
                    {
                        var shipCoordinates = FindNeighboringShipCells(grid, new Vector2Int(x, y));
                        searchedSquares.AddRange(shipCoordinates);
                        ships.Add(new ShipData(shipCoordinates));
                        //Debug.Log("Added ship of " + shipCoordinates.Count + " size");
                    }
                }
            }

            return ships;
        }

        private List<Vector2Int> FindNeighboringShipCells(bool[,] grid, Vector2Int start)
        {
            Queue<Vector2Int> frontier = new();
            frontier.Enqueue(start);

            List<Vector2Int> reached = new()
            {
                start
            };

            while (frontier.Count > 0)
            {
                Vector2Int current = frontier.Dequeue();
                var neighbors = GetNeighbors(grid, current);
                foreach (Vector2Int next in neighbors)
                {
                    if (!reached.Contains(next) && grid[next.x, next.y])
                    {
                        frontier.Enqueue(next);
                        reached.Add(next);
                    }
                }
            }

            //Debug.Log("Found ship of " + reached.Count + " length");
            return reached;
        }

        private List<Vector2Int> GetNeighbors(bool[,] grid, Vector2Int selectedCell)
        {
            List<Vector2Int> list = new();
            Vector2Int gridSize = new(grid.GetLength(1), grid.GetLength(0));
            for (int i = 0; i < 4; i++)
            {
                var targetCell = new Vector2Int(selectedCell.x, selectedCell.y) + Helper.VectorDirection((Direction)i);
                if (Helper.IsCoordinateInsideGrid(gridSize, targetCell))
                {
                    list.Add(targetCell);
                }
            }
            return list;
        }
    }
}
