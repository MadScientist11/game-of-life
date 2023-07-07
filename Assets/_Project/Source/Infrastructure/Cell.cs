using Freya;
using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    public struct Cell
    {
        public Vector2 Position => Box.center;
        public Box2D Box;
        public bool Populated;

        public Cell(Box2D box)
        {
            Box = box;
            Populated = false;
        }
        
        public static int GetSize()
        {
            return sizeof(float) * 13 + sizeof(int) * 4;
        }
    }
}