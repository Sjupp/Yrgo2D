using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battleship
{
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
            Dictionary<string, int> scoreTally = new();
            List<GameData> recordedGames = new();
            
            for (int i = 0; i < _numberOfTestsToRun; i++)
            {
                GameData preparedGameData = SetupGame(new OttWen(), new OttWen());
                GameData gameResult = null;

                if (preparedGameData == null) // failed preparation
                {

                }
                else if (!string.IsNullOrEmpty(preparedGameData.Winner)) // one sided validation, winner already decided
                {
                    gameResult = preparedGameData;
                }
                else
                {
                    gameResult = RunGame(preparedGameData);
                    recordedGames.Add(gameResult);
                }


                if (scoreTally.ContainsKey(gameResult.Winner))
                {
                    scoreTally[gameResult.Winner] += 1;
                }
                else
                {
                    scoreTally.Add(gameResult.Winner, 1);
                }

            }

            // pick the most close game and replay it
            if (scoreTally.Keys.Count > 0)
            {
                var maxKey = scoreTally.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                var lowestDiscrepancyReplay = recordedGames.Where(x => x.Winner == maxKey).OrderBy(x => x.GameResultDiscrepancy).FirstOrDefault();
                var highestDiscrepancyReplay = recordedGames.Where(x => x.Winner == maxKey).OrderByDescending(x => x.GameResultDiscrepancy).FirstOrDefault();
                Debug.Log("Winner: " + maxKey + " with " + scoreTally[maxKey] + " wins, ship discrepancies, highest/lowest: " + highestDiscrepancyReplay.GameResultDiscrepancy + "/" + lowestDiscrepancyReplay.GameResultDiscrepancy);
                if (lowestDiscrepancyReplay != null)
                {
                    BattleshipReplay.Instance.ReplayGame(lowestDiscrepancyReplay);
                }
            }
        }

        private GameData SetupGame(IBattleship user0, IBattleship user1)
        {
            int validations = 0;
            var userData0 = BuildUserData(user0, 0);
            bool user0Validated = ValidateData(userData0, _presetShipSizes);
            validations += user0Validated ? 1 : 0;

            var userData1 = BuildUserData(user1, 1);
            bool user1Validated = ValidateData(userData1, _presetShipSizes);
            validations += user1Validated ? 1 : 0;

            var gameData = new GameData();
            gameData.SetUsers(userData0, userData1);

            if (validations == 0)
            {
                return null;
            }
            else if (validations == 1)
            {
                if (user0Validated)
                {
                    gameData.Winner = gameData.User0.UserName;
                }
                else
                {
                    gameData.Winner = gameData.User1.UserName;
                }
                return gameData;
            }
            else
            {
                return gameData;
            }
        }

        private UserData BuildUserData(IBattleship user, int userIndex)
        {
            var userData = new UserData
            {
                PlayerID = userIndex
            };

            if (userIndex == 0)
            {
                userData.UserName = string.IsNullOrEmpty(_overrideNameOfPlayerA) ? user.GetName() : _overrideNameOfPlayerA;
            }
            else if (userIndex == 1)
            {
                userData.UserName = string.IsNullOrEmpty(_overrideNameOfPlayerB) ? user.GetName() : _overrideNameOfPlayerB;
            }

            userData.Grid = user.NewGame(_gridSize, "");
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
                attackingPlayer.InterfaceRef.Result(attackResult.targetCoordinate, attackResult.hit, attackResult.sunkShip);

                gameOver = defendingPlayer.NoRemainingShips;
                gameData.TurnHistory.Add(new Turn(turn % 2, attackResult.targetCoordinate, attackResult.hit, defendingPlayer.RemainingCells, gameOver));

                if (gameOver)
                {
                    gameData.Winner = attackingPlayer.UserName;
                    gameData.Loser = defendingPlayer.UserName;
                    gameData.GameResultDiscrepancy = attackingPlayer.RemainingCells - defendingPlayer.RemainingCells;
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
            foreach (var ship in ships)
            {
                if (ship.ShipCoordinates.Contains(attackingCoordinate))
                {
                    if (!ship.DamagedCoordinates.Contains(attackingCoordinate))
                    {
                        ship.DamagedCoordinates.Add(attackingCoordinate);
                    }

                    if (ship.Sunk)
                    {
                        sunkShip = true;
                    }
                }
            }
            
            return sunkShip;
        }

        private bool ValidateData(UserData userData, List<int> presetShipSizes)
        {
            if (string.IsNullOrEmpty(userData.UserName))
            {
                Debug.Log("Not Valid: UserName is empty");
            }

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

        private (Vector2Int targetCoordinate, bool hit, bool sunkShip) Attack(Vector2Int attackingCoordinate, UserData defendingUser)
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
                foreach (Vector2Int nextNeighbor in neighbors)
                {
                    if (!reached.Contains(nextNeighbor) && grid[nextNeighbor.x, nextNeighbor.y])
                    {
                        frontier.Enqueue(nextNeighbor);
                        reached.Add(nextNeighbor);
                    }
                }
            }

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
