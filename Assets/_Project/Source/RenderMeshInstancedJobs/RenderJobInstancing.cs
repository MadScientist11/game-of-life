using ConwaysGameOfLife.Source.Infrastructure;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ConwaysGameOfLife.Source.RenderMeshInstancedJobs
{
    [BurstCompile]
    public struct RenderJobInstancing : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Cell> Cells;
        [WriteOnly] public NativeQueue<Matrix4x4>.ParallelWriter RenderData;
        public float CellSize;

        public void Execute(int index)
        {
            Cell cell = Cells[index];
            if (cell.Populated)
                RenderData.Enqueue(ConstructMatrix(cell));
        }

        private Matrix4x4 ConstructMatrix(Cell cell)
        {
            Vector2 position = cell.Position - new Vector2(CellSize / 2, CellSize / 2);
            return Matrix4x4.TRS(position, Quaternion.identity, CellSize * Vector3.one);
        }
    }
}