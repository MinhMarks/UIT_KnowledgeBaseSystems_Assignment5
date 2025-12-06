using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SortingVisualizer.Algorithms;

namespace SortingVisualizer
{
    public partial class MainWindow : Window
    {
        private int[] _array = Array.Empty<int>();
        private int[] _originalArray = Array.Empty<int>();
        private List<SortStep> _steps = new();
        private int _currentStepIndex = 0;
        private bool _isRunning = false;
        private bool _isPaused = false;
        private CancellationTokenSource? _cts;

        // Statistics
        private int _comparisons = 0;
        private int _swaps = 0;

        // Colors
        private readonly SolidColorBrush _normalColor = new(Color.FromRgb(137, 180, 250));    // Blue
        private readonly SolidColorBrush _comparingColor = new(Color.FromRgb(243, 139, 168)); // Red
        private readonly SolidColorBrush _sortedColor = new(Color.FromRgb(166, 227, 161));    // Green
        private readonly SolidColorBrush _pivotColor = new(Color.FromRgb(249, 226, 175));     // Yellow
        private readonly SolidColorBrush _swappingColor = new(Color.FromRgb(30, 30, 46));     // Black (dark)
        private readonly SolidColorBrush _minColor = new(Color.FromRgb(203, 166, 247));       // Purple (cho minIdx)

        // Pseudocode
        private List<TextBlock> _pseudocodeLines = new();
        private readonly SolidColorBrush _pseudoNormal = new(Color.FromRgb(166, 173, 200));
        private readonly SolidColorBrush _pseudoHighlight = new(Color.FromRgb(249, 226, 175));
        private readonly SolidColorBrush _pseudoHighlightBg = new(Color.FromRgb(69, 71, 90));

        public MainWindow()
        {
            InitializeComponent();
            sliderSpeed.ValueChanged += SliderSpeed_ValueChanged;
            GenerateRandomArray(12);
            UpdatePseudocode();
        }

        private void SliderSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtSpeed != null)
                txtSpeed.Text = $"{(int)sliderSpeed.Value}ms";
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            StopSorting();
            var homePage = new HomePage();
            homePage.Show();
            this.Close();
        }

        private void CboAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Chỉ update khi đã khởi tạo xong
            if (pseudocodePanel != null && _pseudocodeLines != null)
            {
                UpdatePseudocode();
            }
        }

        private void UpdatePseudocode()
        {
            if (pseudocodePanel == null || cboAlgorithm == null) return;
            
            pseudocodePanel.Children.Clear();
            _pseudocodeLines.Clear();

            var algorithmName = ((ComboBoxItem)cboAlgorithm.SelectedItem)?.Content?.ToString() ?? "Selection Sort";
            var lines = GetPseudocodeLines(algorithmName);

            for (int i = 0; i < lines.Length; i++)
            {
                var border = new Border
                {
                    Padding = new Thickness(5, 3, 5, 3),
                    Margin = new Thickness(0, 1, 0, 1),
                    CornerRadius = new CornerRadius(3)
                };

                var textBlock = new TextBlock
                {
                    Text = lines[i],
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    Foreground = _pseudoNormal,
                    TextWrapping = TextWrapping.Wrap
                };

                border.Child = textBlock;
                pseudocodePanel.Children.Add(border);
                _pseudocodeLines.Add(textBlock);
            }
        }

        private string[] GetPseudocodeLines(string algorithm)
        {
            if (algorithm == "Selection Sort")
            {
                return new[]
                {
                    "procedure SelectionSort(A)",
                    "  n = length(A)",
                    "  for i = 0 to n-2 do",
                    "    minIdx = i",
                    "    for j = i+1 to n-1 do",
                    "      if A[j] < A[minIdx] then",
                    "        minIdx = j",
                    "      end if",
                    "    end for",
                    "    if minIdx ≠ i then",
                    "      swap(A[i], A[minIdx])",
                    "    end if",
                    "  end for",
                    "end procedure"
                };
            }
            else // Quick Sort
            {
                return new[]
                {
                    "procedure QuickSort(A, low, high)",
                    "  if low < high then",
                    "    pi = Partition(A, low, high)",
                    "    QuickSort(A, low, pi-1)",
                    "    QuickSort(A, pi+1, high)",
                    "  end if",
                    "end procedure",
                    "",
                    "procedure Partition(A, low, high)",
                    "  pivot = A[high]",
                    "  i = low - 1",
                    "  for j = low to high-1 do",
                    "    if A[j] ≤ pivot then",
                    "      i = i + 1",
                    "      swap(A[i], A[j])",
                    "    end if",
                    "  end for",
                    "  swap(A[i+1], A[high])",
                    "  return i + 1",
                    "end procedure"
                };
            }
        }

        private void HighlightPseudocodeLine(int lineIndex)
        {
            // Reset all lines
            for (int i = 0; i < _pseudocodeLines.Count; i++)
            {
                _pseudocodeLines[i].Foreground = _pseudoNormal;
                _pseudocodeLines[i].FontWeight = FontWeights.Normal;
                if (_pseudocodeLines[i].Parent is Border border)
                {
                    border.Background = Brushes.Transparent;
                }
            }

            // Highlight current line
            if (lineIndex >= 0 && lineIndex < _pseudocodeLines.Count)
            {
                _pseudocodeLines[lineIndex].Foreground = _pseudoHighlight;
                _pseudocodeLines[lineIndex].FontWeight = FontWeights.Bold;
                if (_pseudocodeLines[lineIndex].Parent is Border border)
                {
                    border.Background = _pseudoHighlightBg;
                }
            }
        }

        private int GetPseudocodeLineForStep(SortStep step, string algorithm)
        {
            if (algorithm == "Selection Sort")
            {
                return step.Type switch
                {
                    StepType.Compare => 5,  // if A[j] < A[minIdx]
                    StepType.Swap => 10,    // swap(A[i], A[minIdx])
                    StepType.Complete => 13,
                    _ => -1
                };
            }
            else // Quick Sort
            {
                return step.Type switch
                {
                    StepType.Pivot => 9,    // pivot = A[high]
                    StepType.Compare => 12, // if A[j] ≤ pivot
                    StepType.Swap => step.PivotIndex == step.Index1 || step.PivotIndex == step.Index2 ? 17 : 14,
                    StepType.Complete => 6,
                    _ => -1
                };
            }
        }

        private void BtnApplyManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var input = txtInput.Text.Trim();
                var parts = input.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                _array = parts.Select(p => int.Parse(p.Trim())).ToArray();
                _originalArray = (int[])_array.Clone();
                ResetState();
                DrawArray();
            }
            catch
            {
                MessageBox.Show("Vui lòng nhập dãy số hợp lệ (ví dụ: 64, 34, 25, 12)", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnRandom_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCount.Text, out int count) && count > 0 && count <= 50)
            {
                GenerateRandomArray(count);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số lượng từ 1 đến 50", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void GenerateRandomArray(int count)
        {
            var random = new Random();
            _array = Enumerable.Range(0, count).Select(_ => random.Next(5, 100)).ToArray();
            _originalArray = (int[])_array.Clone();
            txtInput.Text = string.Join(", ", _array);
            ResetState();
            DrawArray();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            StopSorting();
            _array = (int[])_originalArray.Clone();
            ResetState();
            DrawArray();
        }

        private void ResetState()
        {
            _steps.Clear();
            _currentStepIndex = 0;
            _comparisons = 0;
            _swaps = 0;
            _isRunning = false;
            _isPaused = false;
            UpdateStatistics();
            UpdateButtons();
            txtStatus.Text = "Sẵn sàng";
            txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(166, 227, 161));
            HighlightPseudocodeLine(-1);
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_isPaused)
            {
                _isPaused = false;
                _isRunning = true;
                UpdateButtons();
                txtStatus.Text = "Đang chạy...";
                await RunSortingAsync();
                return;
            }

            _array = (int[])_originalArray.Clone();
            _steps.Clear();
            _currentStepIndex = 0;
            _comparisons = 0;
            _swaps = 0;

            GenerateSortSteps();

            if (_steps.Count == 0)
            {
                MessageBox.Show("Không có bước nào để thực hiện!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _isRunning = true;
            _isPaused = false;
            UpdateButtons();
            txtStatus.Text = "Đang chạy...";
            txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(249, 226, 175));

            await RunSortingAsync();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = true;
            _isRunning = false;
            UpdateButtons();
            txtStatus.Text = "Tạm dừng";
            txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(250, 179, 135));
        }

        private void BtnNextStep_Click(object sender, RoutedEventArgs e)
        {
            if (_steps.Count == 0)
            {
                _array = (int[])_originalArray.Clone();
                GenerateSortSteps();
            }

            if (_currentStepIndex < _steps.Count)
            {
                ExecuteStep(_steps[_currentStepIndex]);
                _currentStepIndex++;

                if (_currentStepIndex >= _steps.Count)
                {
                    MarkAllSorted();
                    txtStatus.Text = "Hoàn thành!";
                    txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(166, 227, 161));
                }
            }
        }

        private void GenerateSortSteps()
        {
            var algorithmName = ((ComboBoxItem)cboAlgorithm.SelectedItem).Content.ToString();
            txtAlgorithmName.Text = algorithmName;

            var arrayCopy = (int[])_originalArray.Clone();

            _steps = algorithmName switch
            {
                "Selection Sort" => SelectionSortAlgorithm.GenerateSteps(arrayCopy),
                "Quick Sort" => QuickSortAlgorithm.GenerateSteps(arrayCopy),
                _ => new List<SortStep>()
            };
        }

        private async Task RunSortingAsync()
        {
            _cts = new CancellationTokenSource();

            try
            {
                while (_currentStepIndex < _steps.Count && _isRunning)
                {
                    ExecuteStep(_steps[_currentStepIndex]);
                    _currentStepIndex++;

                    await Task.Delay((int)sliderSpeed.Value, _cts.Token);
                }

                if (_currentStepIndex >= _steps.Count && _isRunning)
                {
                    MarkAllSorted();
                    _isRunning = false;
                    txtStatus.Text = "Hoàn thành!";
                    txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(166, 227, 161));
                }
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                UpdateButtons();
            }
        }

        private void ExecuteStep(SortStep step)
        {
            if (step.Type == StepType.Swap)
            {
                (_array[step.Index1], _array[step.Index2]) = (_array[step.Index2], _array[step.Index1]);
                _swaps++;
            }
            
            if (step.Type == StepType.Compare || step.Type == StepType.Swap)
            {
                _comparisons++;
            }

            UpdateStatistics();
            DrawArray(step);

            // Highlight pseudocode
            var algorithmName = ((ComboBoxItem)cboAlgorithm.SelectedItem).Content.ToString() ?? "";
            int lineIndex = GetPseudocodeLineForStep(step, algorithmName);
            HighlightPseudocodeLine(lineIndex);
        }

        private void StopSorting()
        {
            _isRunning = false;
            _isPaused = false;
            _cts?.Cancel();
        }

        private void UpdateButtons()
        {
            btnStart.IsEnabled = !_isRunning;
            btnPause.IsEnabled = _isRunning;
            btnNextStep.IsEnabled = !_isRunning;
            btnRandom.IsEnabled = !_isRunning;
            btnApplyManual.IsEnabled = !_isRunning;
            btnReset.IsEnabled = true;
            cboAlgorithm.IsEnabled = !_isRunning && !_isPaused;

            btnStart.Content = _isPaused ? "▶ Resume" : "▶ Start";
        }

        private void UpdateStatistics()
        {
            txtComparisons.Text = _comparisons.ToString();
            txtSwaps.Text = _swaps.ToString();
        }

        private void DrawArray(SortStep? currentStep = null)
        {
            canvasVisualization.Children.Clear();

            if (_array.Length == 0) return;

            double canvasWidth = canvasVisualization.ActualWidth > 0 ? canvasVisualization.ActualWidth : 700;
            double canvasHeight = canvasVisualization.ActualHeight > 0 ? canvasVisualization.ActualHeight : 350;

            double barWidth = (canvasWidth - 20) / _array.Length - 4;
            barWidth = Math.Min(barWidth, 50);
            double maxValue = _array.Max();
            double heightRatio = (canvasHeight - 50) / maxValue;

            double totalWidth = _array.Length * (barWidth + 4);
            double startX = (canvasWidth - totalWidth) / 2;

            for (int i = 0; i < _array.Length; i++)
            {
                double barHeight = _array[i] * heightRatio;
                double x = startX + i * (barWidth + 4);
                double y = canvasHeight - barHeight - 25;

                SolidColorBrush color = _normalColor;

                if (currentStep != null)
                {
                    if (currentStep.SortedIndices.Contains(i))
                    {
                        color = _sortedColor;
                    }
                    else if (currentStep.Type == StepType.Swap && (i == currentStep.Index1 || i == currentStep.Index2))
                    {
                        // Màu đen khi đang swap
                        color = _swappingColor;
                    }
                    else if (currentStep.PivotIndex == i)
                    {
                        color = _pivotColor;
                    }
                    else if (currentStep.MinIndex == i && currentStep.MinIndex != -1)
                    {
                        // Màu tím cho minIndex trong Selection Sort
                        color = _minColor;
                    }
                    else if (i == currentStep.Index1 || i == currentStep.Index2)
                    {
                        color = _comparingColor;
                    }
                }

                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = color,
                    Stroke = color == _swappingColor ? new SolidColorBrush(Color.FromRgb(243, 139, 168)) : null,
                    StrokeThickness = color == _swappingColor ? 2 : 0,
                    RadiusX = 4,
                    RadiusY = 4
                };

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                canvasVisualization.Children.Add(rect);

                var text = new TextBlock
                {
                    Text = _array[i].ToString(),
                    Foreground = new SolidColorBrush(Color.FromRgb(205, 214, 244)),
                    FontSize = barWidth > 20 ? 11 : 9,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };

                Canvas.SetLeft(text, x);
                Canvas.SetTop(text, canvasHeight - 20);
                canvasVisualization.Children.Add(text);
            }
        }

        private void MarkAllSorted()
        {
            var finalStep = new SortStep
            {
                Type = StepType.Complete,
                SortedIndices = Enumerable.Range(0, _array.Length).ToHashSet()
            };
            DrawArray(finalStep);
            HighlightPseudocodeLine(-1);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            DrawArray();
        }
    }
}
