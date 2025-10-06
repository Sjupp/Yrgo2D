using System.Collections.Generic;
using UnityEngine;

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

        private float _gridCellScale = 1f;
        private float _gridSpacing = 1f;

        public void SetupReplayVisuals(GameData gameData, float gridCellScale, float gridSpacing)
        {
            _gridCellScale = gridCellScale;
            _gridSpacing = gridSpacing;

            Vector2Int gridSize = new Vector2Int(gameData.User0.Grid.GetLength(1), gameData.User0.Grid.GetLength(0));
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

        public float GetAttacked(Vector2Int coordinate, bool hit)
        {
            float extraDelay = 0f;

            if (hit)
            {
                _expressionRenderer.sprite = _playerExpressions[1];
                var obj = Instantiate(_explosionVFX, _coordinateDisplay[coordinate].transform.position, Quaternion.identity);
                obj.transform.localScale = Vector3.one * _gridCellScale;
            }
            else
            {
                _expressionRenderer.sprite = _playerExpressions[0];
                var obj = Instantiate(_waterVFX, _coordinateDisplay[coordinate].transform.position, Quaternion.identity);
                obj.transform.localScale = Vector3.one * _gridCellScale;
            }

            return extraDelay;
        }
    }


}