using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battleship
{
    public class BattleshipReplay : MonoBehaviour
    {
        public static BattleshipReplay Instance = null;

        [SerializeField] private GameTile _gameTilePrefab = null;
        [SerializeField] private GameObject _explosionVFX = null;
        [SerializeField] private GameObject _waterVFX = null;

        [SerializeField] private float _gridSize = 1f;
        [SerializeField] private float _gridSpacing = 0.1f;
        [SerializeField] private float _turnsPerSecond = 1f;

        [SerializeField] private PlayerVisuals _player0Visuals = null;
        [SerializeField] private PlayerVisuals _player1Visuals = null;

        Dictionary<Vector2Int, GameTile> _coordinateDisplay0 = new();
        Dictionary<Vector2Int, GameTile> _coordinateDisplay1 = new();

        //public Action<int, >

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void ReplayGame(GameData gameData)
        {
            _player0Visuals.SetupReplayVisuals(gameData, _gridSize, _gridSpacing);
            _player1Visuals.SetupReplayVisuals(gameData, _gridSize, _gridSpacing);

            StartCoroutine(RunSimulation(gameData));
        }

        private IEnumerator RunSimulation(GameData gameData)
        {
            for (int i = 0; i < gameData.TurnHistory.Count; i++)
            {
                var playerRef = i % 2 == 0 ? _player0Visuals : _player1Visuals;
                var turnData = gameData.TurnHistory[i];

                playerRef.GetAttacked(turnData.FireTarget, turnData.Hit);

                float turnsPerSecond = 1f / Mathf.Max(_turnsPerSecond, 0.001f);
                yield return new WaitForSeconds(turnsPerSecond);
            }
        }
    }

}