using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battleship
{
    public class PlayerVisuals : MonoBehaviour
    {
        private int _playerID = 0;
        Dictionary<Vector2Int, GameTile> _tileDictionary = new();

        [SerializeField] private GameTile _gameTilePrefab = null;
        [SerializeField] private Transform _boardHolder = null;
        [SerializeField] private SpriteRenderer _expressionRenderer = null;
        [SerializeField] private List<Sprite> _playerExpressions = new List<Sprite>();
        [SerializeField] private List<Sprite> _shipSprites = new List<Sprite>();
        [SerializeField] private GameObject _explosionVFX = null;
        [SerializeField] private GameObject _waterVFX = null;
        [SerializeField] private GameObject _fireVFX = null;
        [SerializeField] private TextMeshProUGUI _nameLabel = null;
        [SerializeField] private Image _healthBarFill = null;
        [SerializeField] private float _punchStrength = 3f;

        private float _gridCellScale = 1f;
        private float _gridSpacing = 1f;
        public void SetupReplayVisuals(BattleshipReplay battleshipReplay, GameData gameData, int playerID, float gridCellScale, float gridSpacing)
        {
            var user = playerID == 0 ? gameData.User0 : gameData.User1;
            _playerID = user.PlayerID;
            _nameLabel.text = user.UserName;
            battleshipReplay.TurnEvent += OnTurnEvent;
            _gridCellScale = gridCellScale;
            _gridSpacing = gridSpacing;

            Vector2Int gridSize = new(user.Grid.GetLength(1), user.Grid.GetLength(0));
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var obj = Instantiate(_gameTilePrefab, _boardHolder.transform);
                    _tileDictionary.TryAdd(new Vector2Int(x, y), obj);
                    obj.transform.localPosition = new Vector2(x * gridCellScale + x * gridSpacing, y * gridCellScale + y * gridSpacing);
                    obj.Init(gridCellScale, x, y);
                }
            }

            ApplyShipSprites(user);
        }

        private void ApplyShipSprites(UserData user)
        {
            foreach (var shipData in user.ShipData)
            {
                var start = shipData.ShipCoordinates[0];
                var end = shipData.ShipCoordinates[shipData.ShipCoordinates.Count - 1];
                var dir = end - start;
                float pieceRotation = 0f;
                if (dir.x != 0)
                {
                    if (dir.x > 0)
                        pieceRotation = 0f;
                    else
                        pieceRotation = 180f;
                }
                else
                {
                    if (dir.y > 0)
                        pieceRotation = 90f;
                    else
                        pieceRotation = 270f;
                }

                if (shipData.ShipCoordinates.Count == 1)
                {
                    _tileDictionary[shipData.ShipCoordinates[0]].UpdateTile(_shipSprites[3], 0f);
                }
                else
                {
                    foreach (var shipCoordinate in shipData.ShipCoordinates)
                    {
                        if (shipCoordinate == start)
                        {
                            _tileDictionary[shipCoordinate].UpdateTile(_shipSprites[0], pieceRotation);
                        }
                        else if (shipCoordinate == end)
                        {
                            _tileDictionary[shipCoordinate].UpdateTile(_shipSprites[2], pieceRotation);
                        }
                        else
                        {
                            _tileDictionary[shipCoordinate].UpdateTile(_shipSprites[1], pieceRotation);
                        }
                    }
                }
            }
        }

        private void OnTurnEvent(Turn turn)
        {
            bool yourTurn = turn.TurnOwnerID == _playerID;
            if (yourTurn)
            {
                if (turn.Hit)
                {
                    _expressionRenderer.sprite = _playerExpressions[2]; // smug
                }
            }
            else
            {
                _healthBarFill.fillAmount = (1f / 13f) * turn.ShipCellsRemaining; // magical number, bad
                _tileDictionary[turn.FireTarget].Marked();

                var targetPosition = _tileDictionary[turn.FireTarget].transform.position;
                foreach (var entry in _tileDictionary)
                {
                    var dir = (entry.Value.transform.position - targetPosition).normalized;
                    entry.Value.PunchTile(dir, _punchStrength - Mathf.Clamp((entry.Value.transform.position - targetPosition).sqrMagnitude, 0, _punchStrength));
                }

                BoardEffects(turn.FireTarget, turn.Hit);

                if (turn.Hit)
                {
                    if (turn.ShipCellsRemaining == 1)
                    {
                        _expressionRenderer.sprite = _playerExpressions[3]; // look
                    }
                    else
                    {
                        _expressionRenderer.sprite = _playerExpressions[1]; // sad
                    }
                }
                else
                {
                    if (turn.ShipCellsRemaining == 1)
                    {
                        _expressionRenderer.sprite = _playerExpressions[3]; // look
                    }
                    else
                    {
                        _expressionRenderer.sprite = _playerExpressions[0]; // content
                    }
                }

            }
        }

        private void BoardEffects(Vector2Int coordinate, bool hit)
        {
            if (hit)
            {
                var explosion = Instantiate(_explosionVFX, _tileDictionary[coordinate].transform.position, Quaternion.identity);
                explosion.transform.localScale = Vector3.one * _gridCellScale;
                var fire = Instantiate(_fireVFX, _tileDictionary[coordinate].transform.position, Quaternion.identity);
                fire.transform.localScale = Vector3.one * _gridCellScale;
            }
            else
            {
                var obj = Instantiate(_waterVFX, _tileDictionary[coordinate].transform.position, Quaternion.identity);
                obj.transform.localScale = Vector3.one * _gridCellScale;
            }
        }
    }
}