using System.Collections.Generic;
using ConwaysGameOfLife.Source.Infrastructure;
using Matrix4x4 = UnityEngine.Matrix4x4;

namespace ConwaysGameOfLife.Source.RenderMeshInstanced
{
    public class InstancingGameRender : IGameRenderer
    {
        private readonly GameOfLife _gameOfLife;
        private readonly InstancingTileRenderer _tileRenderer;
        private readonly List<Matrix4x4> _renderTilesData = new();

        public InstancingGameRender(GameOfLife gameOfLife, InstancingTileRenderer tileRenderer)
        {
            _gameOfLife = gameOfLife;
            _tileRenderer = tileRenderer;
        }

        public void Render()
        {
            _renderTilesData.Clear();
            int columns = (int)_gameOfLife.Grid.Width;
            int rows = (int)_gameOfLife.Grid.Height;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (_gameOfLife.Grid[i, j].Populated)
                    {
                        _renderTilesData.Add(_gameOfLife.Grid.GetCellTransformMatrix(i, j, false));
                    }
                }
            }

            _tileRenderer.Render(_renderTilesData);
        }

        public void Dispose()
        {
            
        }
    }
}