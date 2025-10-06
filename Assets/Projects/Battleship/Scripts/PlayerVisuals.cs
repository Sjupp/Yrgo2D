using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battleship
{
    public class PlayerVisuals : MonoBehaviour
    {
        private int _playerID = 0;
        Dictionary<Vector2Int, GameTile> _coordinateDisplay = new();

        [SerializeField] private GameTile _gameTilePrefab = null;
        [SerializeField] private Transform _boardHolder = null;
        [SerializeField] private SpriteRenderer _expressionRenderer = null;
        [SerializeField] private List<Sprite> _playerExpressions = new List<Sprite>();
        [SerializeField] private GameObject _explosionVFX = null;
        [SerializeField] private GameObject _waterVFX = null;
        [SerializeField] private TextMeshProUGUI _nameLabel = null;
        [SerializeField] private Image _healthBarFill = null;

        private float _gridCellScale = 1f;
        private float _gridSpacing = 1f;
        public void SetupReplayVisuals(BattleshipReplay battleshipReplay, GameData gameData, int playerID, float gridCellScale, float gridSpacing)
        {
            _nameLabel.text = playerID == 0 ? gameData.User0.UserName : gameData.User1.UserName;
            _playerID = playerID == 0 ? gameData.User0.PlayerID : gameData.User1.PlayerID;
            battleshipReplay.TurnEvent += OnTurnEvent;
            _gridCellScale = gridCellScale;
            _gridSpacing = gridSpacing;

            Vector2Int gridSize = new(gameData.User0.Grid.GetLength(1), gameData.User0.Grid.GetLength(0));
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var obj = Instantiate(_gameTilePrefab, _boardHolder.transform);
                    obj.transform.localPosition = new Vector2(x * gridCellScale + x * gridSpacing, y * gridCellScale + y * gridSpacing);
                    obj.Init(gridCellScale, x, y);
                    _coordinateDisplay.TryAdd(new Vector2Int(x, y), obj);
                }
            }
        }

        private void OnTurnEvent(Turn turn)
        {
            if (turn.TurnOwnerID == _playerID)
            {
                if (turn.Hit)
                {
                    _expressionRenderer.sprite = _playerExpressions[2]; // smug
                }
                else
                {

                }
            }
            else
            {
                _healthBarFill.fillAmount = (1f / 13f) * turn.ShipCellsRemaining;
                _coordinateDisplay[turn.FireTarget].Marked();

                if (turn.Hit)
                {
                    _expressionRenderer.sprite = _playerExpressions[1]; // sad
                }
                else
                {
                    _expressionRenderer.sprite = _playerExpressions[0]; // content
                }

                BoardEffects(turn.FireTarget, turn.Hit);
            }
        }

        private void BoardEffects(Vector2Int coordinate, bool hit)
        {
            if (hit)
            {
                var obj = Instantiate(_explosionVFX, _coordinateDisplay[coordinate].transform.position, Quaternion.identity);
                obj.transform.localScale = Vector3.one * _gridCellScale;
            }
            else
            {
                var obj = Instantiate(_waterVFX, _coordinateDisplay[coordinate].transform.position, Quaternion.identity);
                obj.transform.localScale = Vector3.one * _gridCellScale;
            }
        }
    }
}