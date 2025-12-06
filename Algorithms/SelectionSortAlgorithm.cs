namespace SortingVisualizer.Algorithms
{
    public static class SelectionSortAlgorithm
    {
        public static List<SortStep> GenerateSteps(int[] array)
        {
            var steps = new List<SortStep>();
            var sortedIndices = new HashSet<int>();
            int n = array.Length;

            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;

                // Find minimum element in unsorted portion
                for (int j = i + 1; j < n; j++)
                {
                    // Compare step - đánh dấu minIndex bằng MinIndex property
                    steps.Add(new SortStep
                    {
                        Type = StepType.Compare,
                        Index1 = minIndex,
                        Index2 = j,
                        MinIndex = minIndex, // Đánh dấu vị trí min hiện tại
                        SortedIndices = new HashSet<int>(sortedIndices),
                        Description = $"So sánh arr[{minIndex}]={array[minIndex]} với arr[{j}]={array[j]}"
                    });

                    if (array[j] < array[minIndex])
                    {
                        minIndex = j;
                    }
                }

                // Swap if needed
                if (minIndex != i)
                {
                    // Swap step
                    steps.Add(new SortStep
                    {
                        Type = StepType.Swap,
                        Index1 = i,
                        Index2 = minIndex,
                        MinIndex = minIndex,
                        SortedIndices = new HashSet<int>(sortedIndices),
                        Description = $"Hoán đổi arr[{i}]={array[i]} với arr[{minIndex}]={array[minIndex]}"
                    });

                    (array[i], array[minIndex]) = (array[minIndex], array[i]);
                }

                // Mark as sorted
                sortedIndices.Add(i);
            }

            // Last element is automatically sorted
            sortedIndices.Add(n - 1);

            return steps;
        }
    }
}
