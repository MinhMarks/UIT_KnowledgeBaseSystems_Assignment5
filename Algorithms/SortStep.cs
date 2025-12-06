namespace SortingVisualizer.Algorithms
{
    public enum StepType
    {
        Compare,
        Swap,
        Pivot,
        Complete
    }

    public class SortStep
    {
        public StepType Type { get; set; }
        public int Index1 { get; set; } = -1;
        public int Index2 { get; set; } = -1;
        public int PivotIndex { get; set; } = -1;
        public int MinIndex { get; set; } = -1; // For Selection Sort - vị trí min hiện tại
        public HashSet<int> SortedIndices { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public int PseudocodeLine { get; set; } = -1; // Line index in pseudocode
    }
}
