using ConwaysGameOfLife.Source.Helpers;
using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GameConfiguration _gameConfiguration;
        private GameOfLife _gameOfLife;
        private IGameRenderer _gameRenderer;
        private InstancingTileRenderer _tileRenderer;

        private float _currentTime;
        private float _delay = 0.5f;
        private bool _runTimer;

        private void Start()
        {
            _gameOfLife = GameFactory.CreateGameOfLife(_gameConfiguration, GameFactory.GameType.RenderMeshInstancedWithJobs);
            _gameRenderer =
                GameFactory.CreateGameRenderer(_gameOfLife, _gameConfiguration, GameFactory.GameType.RenderMeshInstancedWithJobs);
        }

        private void OnDestroy()
        {
            _gameOfLife.Dispose();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePositionWS = MousePointer.GetWorldPosition(Camera.main);
                Vector2 point = new Vector2(mousePositionWS.x, mousePositionWS.y);

                if (_gameOfLife.Grid.TryGetBoundedCellCoords(point, out Vector2Int cellCoords))
                {
                    _gameOfLife.Grid.PopulateCell(cellCoords.x, cellCoords.y);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                _runTimer = true;
                _currentTime = _delay;
            }

            if (_runTimer)
            {
                _currentTime -= Time.deltaTime;
                if (_currentTime <= 0)
                {
                    _gameOfLife.RunGeneration();
                    _currentTime = _delay;
                }
            }

            _gameRenderer.Render();
        }
    }
}