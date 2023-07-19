using Freya;
using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    public struct Cell
    {
        public Vector2 Position;
        public Vector2 Extents;
        public bool Populated;

        public Cell(Vector2 position, Vector2 extents)
        {
            Position = position;
            Extents = extents;
            Populated = false;
        }
        
        
        public bool Contains(
            Vector2 point) => Mathfs.Abs(point.x - Position.x) - Extents.x <= 0 &&
                              Mathfs.Abs(point.y - Position.y) - Extents.y <= 0;

        public static int GetSize()
        {
            return sizeof(float) * 13 + sizeof(int) * 4;
        }
    }
}