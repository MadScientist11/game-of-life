using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        public int GridWidth;
        public int GridHeight;
        public float CellSize;

        public GameObject GridCell;
        public Material CellMaterial;
    }
}