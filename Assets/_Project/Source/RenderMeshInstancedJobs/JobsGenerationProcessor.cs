using ConwaysGameOfLife.Source.Infrastructure;
using Unity.Collections;
using Unity.Jobs;

namespace ConwaysGameOfLife.Source.RenderMeshInstancedJobs
{
    public class JobsGenerationProcessor : IGenerationProcessor
    {
        private readonly Grid _grid;


        public JobsGenerationProcessor(Grid grid)
        {
            _grid = grid;
        }

        public void RunGeneration()
        {
            NativeArray<Cell> nextGenCells =
                new NativeArray<Cell>(_grid.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            NewGenerationJob job = new NewGenerationJob()
            {
                Height = _grid.Height,
                Width = _grid.Width,
                Cells = _grid.Cells,
                NextGenCells = nextGenCells
            };

            JobHandle jobHandle = job.Schedule(_grid.Length, 64);
            jobHandle.Complete();


            nextGenCells.CopyTo(_grid.Cells);

            nextGenCells.Dispose();
        }
    }
}