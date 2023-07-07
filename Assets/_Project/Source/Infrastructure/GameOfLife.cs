namespace ConwaysGameOfLife.Source.Infrastructure
{
    public class GameOfLife
    {
        public Grid Grid => _grid;
        private readonly Grid _grid;
        private readonly IGenerationProcessor _generationProcessor;

        public GameOfLife(Grid grid, IGenerationProcessor generationProcessor)
        {
            _generationProcessor = generationProcessor;
            _grid = grid;
        }


        public void RunGeneration() =>
            _generationProcessor.RunGeneration();


        public void Dispose()
        {
            _grid.Cells.Dispose();
        }


  
    }
}