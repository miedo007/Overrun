namespace Mtl.Bonfire
{
    public struct LevelSummaryEvent
    {
        public int LevelId;
        public bool Completed;
        public string LevelName;
        public int? CheckpointCount;
        public int? CheckpointCleared;
    }
}