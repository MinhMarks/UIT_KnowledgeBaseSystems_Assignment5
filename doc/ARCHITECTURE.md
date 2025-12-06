# 📐 Kiến trúc & Luồng hoạt động - Sorting Visualizer

## 1. Cấu trúc Project

```
SortingVisualizer/
├── SortingVisualizer.csproj    # Cấu hình project .NET
├── App.xaml / App.xaml.cs      # Entry point của ứng dụng
├── MainWindow.xaml             # Giao diện người dùng (XAML)
├── MainWindow.xaml.cs          # Logic xử lý sự kiện & điều khiển
├── Algorithms/
│   ├── SortStep.cs             # Model định nghĩa 1 bước sắp xếp
│   ├── SelectionSortAlgorithm.cs   # Thuật toán Selection Sort
│   └── QuickSortAlgorithm.cs       # Thuật toán Quick Sort
├── publish.bat                 # Script đóng gói exe
└── README.md                   # Hướng dẫn sử dụng
```

---

## 2. Các thành phần chính

### 2.1. SortStep (Model)
```
Algorithms/SortStep.cs
```

Định nghĩa một "bước" trong quá trình sắp xếp:

| Property | Kiểu | Mô tả |
|----------|------|-------|
| `Type` | `StepType` | Loại bước: Compare, Swap, Pivot, Complete |
| `Index1` | `int` | Vị trí phần tử thứ nhất |
| `Index2` | `int` | Vị trí phần tử thứ hai |
| `PivotIndex` | `int` | Vị trí pivot (Quick Sort) |
| `SortedIndices` | `HashSet<int>` | Các vị trí đã sắp xếp xong |
| `Description` | `string` | Mô tả bước hiện tại |

### 2.2. Thuật toán (Algorithms)

Mỗi thuật toán có method `GenerateSteps(int[] array)`:
- **Input:** Mảng số nguyên
- **Output:** `List<SortStep>` - Danh sách tất cả các bước

Thuật toán **KHÔNG** thực hiện sắp xếp trực tiếp trên UI, mà **tạo trước** toàn bộ các bước, sau đó UI sẽ "phát lại" từng bước.

### 2.3. MainWindow (Controller)

Quản lý:
- Giao diện người dùng
- Điều khiển quá trình sắp xếp
- Vẽ biểu đồ thanh
- Thống kê (so sánh, hoán đổi)

---

## 3. Luồng hoạt động chi tiết

### 3.1. Khởi động ứng dụng

```
App.xaml (StartupUri="MainWindow.xaml")
    │
    ▼
MainWindow.xaml.cs → Constructor
    │
    ├── InitializeComponent()     // Load giao diện XAML
    ├── Đăng ký sự kiện slider
    └── GenerateRandomArray(15)   // Tạo mảng ngẫu nhiên mặc định
            │
            ▼
        DrawArray()               // Vẽ biểu đồ thanh lên Canvas
```

### 3.2. Nhập dữ liệu

```
┌─────────────────────────────────────────────────────────┐
│  Cách 1: Nhập thủ công                                  │
│  ─────────────────────                                  │
│  User nhập "64, 34, 25, 12" → Click "Áp dụng"          │
│      │                                                  │
│      ▼                                                  │
│  BtnApplyManual_Click()                                │
│      │                                                  │
│      ├── Parse string → int[]                          │
│      ├── Lưu vào _array và _originalArray              │
│      ├── ResetState()                                  │
│      └── DrawArray()                                   │
├─────────────────────────────────────────────────────────┤
│  Cách 2: Tạo ngẫu nhiên                                │
│  ──────────────────────                                │
│  User nhập số lượng → Click "Tạo ngẫu nhiên"           │
│      │                                                  │
│      ▼                                                  │
│  BtnRandom_Click()                                     │
│      │                                                  │
│      └── GenerateRandomArray(count)                    │
│              │                                          │
│              ├── Random.Next(5, 100) cho mỗi phần tử   │
│              ├── Lưu vào _array và _originalArray      │
│              └── DrawArray()                           │
└─────────────────────────────────────────────────────────┘
```

### 3.3. Chạy tự động (Start)

```
User click "Start"
    │
    ▼
BtnStart_Click()
    │
    ├── Copy _originalArray → _array (reset mảng)
    │
    ├── GenerateSortSteps()
    │       │
    │       ├── Đọc thuật toán từ ComboBox
    │       │
    │       └── Gọi Algorithm.GenerateSteps(_originalArray)
    │               │
    │               └── Trả về List<SortStep> (tất cả các bước)
    │
    └── RunSortingAsync()  ←─────────────────────┐
            │                                     │
            ▼                                     │
        ┌─────────────────────────────────┐      │
        │  LOOP: while có bước & đang chạy │      │
        │  ────────────────────────────── │      │
        │  1. ExecuteStep(step)           │      │
        │       │                         │      │
        │       ├── Nếu Swap: đổi vị trí  │      │
        │       ├── Cập nhật thống kê     │      │
        │       └── DrawArray(step)       │      │
        │                                 │      │
        │  2. await Task.Delay(speed)     │      │
        │                                 │      │
        │  3. _currentStepIndex++         │      │
        └─────────────────────────────────┘      │
                    │                            │
                    ▼                            │
            Hết bước? ──No──────────────────────┘
                │
               Yes
                │
                ▼
        MarkAllSorted()  // Tô xanh tất cả
        txtStatus = "Hoàn thành!"
```

### 3.4. Chạy từng bước (Next Step)

```
User click "Next Step"
    │
    ▼
BtnNextStep_Click()
    │
    ├── Nếu chưa có steps → GenerateSortSteps()
    │
    └── ExecuteStep(_steps[_currentStepIndex])
            │
            ├── Thực hiện swap (nếu có)
            ├── Cập nhật thống kê
            ├── DrawArray(step)  // Vẽ với màu highlight
            └── _currentStepIndex++
```

### 3.5. Pause / Resume

```
┌─────────────────────────────────────────┐
│  PAUSE                                  │
│  ─────                                  │
│  User click "Pause"                     │
│      │                                  │
│      ▼                                  │
│  _isPaused = true                       │
│  _isRunning = false                     │
│  → Loop trong RunSortingAsync() dừng   │
├─────────────────────────────────────────┤
│  RESUME                                 │
│  ──────                                 │
│  User click "Resume" (nút Start đổi)   │
│      │                                  │
│      ▼                                  │
│  _isPaused = false                      │
│  _isRunning = true                      │
│  → Gọi lại RunSortingAsync()           │
│  → Tiếp tục từ _currentStepIndex       │
└─────────────────────────────────────────┘
```

---

## 4. Cách vẽ biểu đồ (DrawArray)

```
DrawArray(SortStep? currentStep)
    │
    ├── Xóa Canvas
    │
    ├── Tính toán kích thước:
    │       barWidth = (canvasWidth - 20) / array.Length - 4
    │       heightRatio = (canvasHeight - 50) / maxValue
    │
    └── FOR mỗi phần tử i trong array:
            │
            ├── Xác định màu:
            │       │
            │       ├── sortedIndices.Contains(i) → 🟢 Xanh lá
            │       ├── pivotIndex == i           → 🟡 Vàng
            │       ├── i == Index1 hoặc Index2   → 🔴 Đỏ
            │       └── Còn lại                   → 🔵 Xanh dương
            │
            ├── Vẽ Rectangle (thanh)
            │       Height = array[i] * heightRatio
            │
            └── Vẽ TextBlock (giá trị số)
```

---

## 5. Thuật toán Selection Sort

```
GenerateSteps(array):
    │
    FOR i = 0 → n-2:
        │
        ├── minIndex = i
        │
        ├── FOR j = i+1 → n-1:
        │       │
        │       ├── Thêm step COMPARE(minIndex, j)
        │       │
        │       └── Nếu array[j] < array[minIndex]:
        │               minIndex = j
        │
        ├── Nếu minIndex != i:
        │       │
        │       ├── Thêm step SWAP(i, minIndex)
        │       └── Đổi array[i] ↔ array[minIndex]
        │
        └── Đánh dấu i đã sorted
```

**Ví dụ:** Mảng [64, 25, 12, 22]

| Bước | Loại | Index1 | Index2 | Mô tả |
|------|------|--------|--------|-------|
| 1 | Compare | 0 | 1 | So sánh 64 với 25 |
| 2 | Compare | 1 | 2 | So sánh 25 với 12 |
| 3 | Compare | 2 | 3 | So sánh 12 với 22 |
| 4 | Swap | 0 | 2 | Hoán đổi 64 ↔ 12 |
| ... | ... | ... | ... | ... |

---

## 6. Thuật toán Quick Sort

```
GenerateSteps(array):
    │
    └── QuickSort(array, 0, n-1)

QuickSort(array, low, high):
    │
    ├── Nếu low < high:
    │       │
    │       ├── pivotIndex = Partition(array, low, high)
    │       ├── Đánh dấu pivot đã sorted
    │       ├── QuickSort(array, low, pivotIndex - 1)
    │       └── QuickSort(array, pivotIndex + 1, high)
    │
    └── Nếu low == high: Đánh dấu sorted

Partition(array, low, high):
    │
    ├── pivot = array[high]
    ├── Thêm step PIVOT(high)
    │
    ├── i = low - 1
    │
    ├── FOR j = low → high-1:
    │       │
    │       ├── Thêm step COMPARE(j, pivotIndex)
    │       │
    │       └── Nếu array[j] <= pivot:
    │               i++
    │               Nếu i != j:
    │                   Thêm step SWAP(i, j)
    │                   Đổi array[i] ↔ array[j]
    │
    ├── Thêm step SWAP(i+1, high)  // Đặt pivot vào đúng vị trí
    ├── Đổi array[i+1] ↔ array[high]
    │
    └── Return i+1
```

---

## 7. Sơ đồ tổng quan

```
┌─────────────────────────────────────────────────────────────────┐
│                         USER INTERFACE                          │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  Input: [Nhập số] [Tạo ngẫu nhiên] [Reset]               │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │  Control: [Algorithm ▼] [Start] [Pause] [Next] [Speed]   │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │                                                          │  │
│  │                    CANVAS (Biểu đồ)                      │  │
│  │         ██                                               │  │
│  │    ██   ██   ██        ██                                │  │
│  │    ██   ██   ██   ██   ██   ██                           │  │
│  │   [5]  [8]  [3]  [2]  [9]  [1]                           │  │
│  │                                                          │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │  Stats: Algorithm | Comparisons | Swaps | Status         │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      MAINWINDOW.XAML.CS                         │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐    │
│  │ _array[]       │  │ _steps[]       │  │ _currentStep   │    │
│  │ _originalArray │  │ List<SortStep> │  │ Index          │    │
│  └────────────────┘  └────────────────┘  └────────────────┘    │
│                              │                                  │
│  Methods:                    │                                  │
│  - GenerateSortSteps()  ─────┘                                  │
│  - RunSortingAsync()                                            │
│  - ExecuteStep()                                                │
│  - DrawArray()                                                  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                         ALGORITHMS                              │
│  ┌─────────────────────┐      ┌─────────────────────┐          │
│  │  SelectionSort      │      │  QuickSort          │          │
│  │  ───────────────    │      │  ─────────          │          │
│  │  GenerateSteps()    │      │  GenerateSteps()    │          │
│  │       │             │      │       │             │          │
│  │       ▼             │      │       ▼             │          │
│  │  List<SortStep>     │      │  List<SortStep>     │          │
│  └─────────────────────┘      └─────────────────────┘          │
└─────────────────────────────────────────────────────────────────┘
```

---

## 8. Màu sắc trong ứng dụng

| Màu | Hex Code | Ý nghĩa |
|-----|----------|---------|
| 🔵 Xanh dương | `#89b4fa` | Phần tử bình thường |
| 🔴 Đỏ/Hồng | `#f38ba8` | Đang so sánh |
| 🟢 Xanh lá | `#a6e3a1` | Đã sắp xếp xong |
| 🟡 Vàng | `#f9e2af` | Pivot (Quick Sort) |
| ⬛ Nền | `#1e1e2e` | Background chính |
| ⬜ Panel | `#313244` | Background panel |

*Sử dụng bảng màu Catppuccin Mocha*
