using System.Collections.Generic;
using Freya;
using Unity.Collections;
using UnityEngine;
using Random = Freya.Random;


namespace ConwaysGameOfLife.Source.Infrastructure
{
    public class Grid
    {
        public int Length => Cells.Length;
        public int Width => (int)_grid.Width;
        public int Height => (int)_grid.Height;

        private Box2D _grid;

        private readonly float _cellSize;
        private readonly List<Cell> _tempNeighbours = new(8);

        private int _gridWidth;
        public NativeArray<Cell> Cells;

        public Cell this[int i, int j]
        {
            get => Cells[i * _gridWidth + j];
            set => Cells[i * _gridWidth + j] = value;
        }

        public Grid(int width, int height, float cellSize)
        {
            _cellSize = cellSize;

            _grid = new Box2D
            {
                center = Vector2.zero,
                extents = new Vector2(width / 2, height / 2)
            };
            _gridWidth = Width;

            Cells = new NativeArray<Cell>(width * height, Allocator.Persistent);
        }

        public Grid(int width, int height, float cellSize, Vector2 center)
        {
            _cellSize = cellSize;

            _grid = new Box2D
            {
                center = center,
                extents = new Vector2(width / 2, height / 2)
            };
            _gridWidth = Width;

            Cells = new NativeArray<Cell>(width * height, Allocator.Persistent);
        }

        public void Initialize()
        {
            int columns = (int)_grid.Width;
            int rows = (int)_grid.Height;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int index = i * columns + j;
                    Cells[index] = CreateCell(i, j);

                    if (Random.Value > 0.9f)
                    {
                        Cell cell = Cells[index];
                        cell.Populated = 1;
                        Cells[index] = cell;
                    }
                }
            }
        }

        private int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        public List<Cell> GetNeighbours(int x, int y)
        {
            _tempNeighbours.Clear();

            for (int i = 0; i < dx.Length; i++)
            {
                int newRow = x + dx[i];
                int newCol = y + dy[i];

                if (newRow >= 0 && newRow < _grid.Height && newCol >= 0 && newCol < _grid.Width)
                    _tempNeighbours.Add(this[newRow, newCol]);
            }

            return _tempNeighbours;
        }

        public Matrix4x4 GetCellTransformMatrix(int i, int j, bool centerPosition)
        {
            Vector2 position = centerPosition
                ? this[i, j].Position
                : this[i, j].Position - new Vector2(_cellSize / 2, _cellSize / 2);
            return Matrix4x4.TRS(position, Quaternion.identity, _cellSize * Vector3.one);
        }

        public void PopulateCell(int x, int y)
        {
            Cell cell = this[x, y];

            if (cell.Populated == 1)
                return;

            cell.Populated = 1;
            this[x, y] = cell;
        }

        public void KillCell(int x, int y)
        {
            Cell cell = this[x, y];

            if (cell.Populated == 0)
                return;

            cell.Populated = 0;
            this[x, y] = cell;
        }


        public bool TryGetBoundedCellCoords(Vector2 point, out Vector2Int coords)
        {
            coords = default;

            for (int i = 0; i < Length; i++)
            {
                if (Cells[i].Contains(point))
                {
                    coords = new Vector2Int(i / Width, i % Width);
                    return true;
                }
            }

            return false;
        }

        private Cell CreateCell(int i, int j)
        {
            Box2D cellBox = new Box2D()
            {
                center = new Vector2(j * _cellSize, i * _cellSize) + new Vector2(_cellSize / 2, _cellSize / 2) -
                         _grid.extents * _cellSize,
                extents = new Vector2(_cellSize / 2, _cellSize / 2)
            };
            return new Cell(cellBox.center, cellBox.extents);
        }


    }
}