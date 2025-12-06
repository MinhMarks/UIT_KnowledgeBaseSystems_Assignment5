namespace SortingVisualizer.Algorithms
{
    public static class QuickSortAlgorithm
    {
        public static List<SortStep> GenerateSteps(int[] array)
        {
            var steps = new List<SortStep>();
            var sortedIndices = new HashSet<int>();
            
            QuickSort(array, 0, array.Length - 1, steps, sortedIndices);
            
            return steps;
        }

        private static void QuickSort(int[] array, int low, int high, List<SortStep> steps, HashSet<int> sortedIndices)
        {
            if (low < high)
            {
                int pivotIndex = Partition(array, low, high, steps, sortedIndices);
                
                // Mark pivot as sorted
                sortedIndices.Add(pivotIndex);
                
                QuickSort(array, low, pivotIndex - 1, steps, sortedIndices);
                QuickSort(array, pivotIndex + 1, high, steps, sortedIndices);
            }
            else if (low == high)
            {
                // Single element is sorted
                sortedIndices.Add(low);
            }
        }

        private static int Partition(int[] array, int low, int high, List<SortStep> steps, HashSet<int> sortedIndices)
        {
            int pivot = array[high];
            int pivotIdx = high;
            
            // Show pivot selection
            steps.Add(new SortStep
            {
                Type = StepType.Pivot,
                PivotIndex = pivotIdx,
                SortedIndices = new HashSet<int>(sortedIndices),
                Description = $"Chọn pivot = {pivot} tại vị trí {pivotIdx}"
            });

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                // Compare with pivot
                steps.Add(new SortStep
                {
                    Type = StepType.Compare,
                    Index1 = j,
                    Index2 = pivotIdx,
                    PivotIndex = pivotIdx,
                    SortedIndices = new HashSet<int>(sortedIndices),
                    Description = $"So sánh arr[{j}]={array[j]} với pivot={pivot}"
                });

                if (array[j] <= pivot)
                {
                    i++;
                    
                    if (i != j)
                    {
                        // Swap step
                        steps.Add(new SortStep
                        {
                            Type = StepType.Swap,
                            Index1 = i,
                            Index2 = j,
                            PivotIndex = pivotIdx,
                            SortedIndices = new HashSet<int>(sortedIndices),
                            Description = $"Hoán đổi arr[{i}]={array[i]} với arr[{j}]={array[j]}"
                        });

                        (array[i], array[j]) = (array[j], array[i]);
                    }
                }
            }

            // Place pivot in correct position
            if (i + 1 != high)
            {
                steps.Add(new SortStep
                {
                    Type = StepType.Swap,
                    Index1 = i + 1,
                    Index2 = high,
                    PivotIndex = i + 1, // New pivot position
                    SortedIndices = new HashSet<int>(sortedIndices),
                    Description = $"Đặt pivot vào vị trí đúng: hoán đổi arr[{i + 1}]={array[i + 1]} với arr[{high}]={array[high]}"
                });

                (array[i + 1], array[high]) = (array[high], array[i + 1]);
            }

            return i + 1;
        }
    }
}
