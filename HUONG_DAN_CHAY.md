# 🚀 Hướng dẫn chạy ứng dụng

## Cách 1: Chạy file exe (Dành cho người dùng)

1. Vào folder `publish`
2. Double-click file `SortingVisualizer.exe`
3. Không cần cài đặt gì!

**Lưu ý:** Nếu Windows Defender cảnh báo, chọn "More info" → "Run anyway"

---

## Cách 2: Chạy từ source code (Dành cho developer)

### Yêu cầu:
- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0

### Các bước:

**Cách 2.1: Dùng Command Line**
```bash
cd SortingVisualizer
dotnet run
```

**Cách 2.2: Dùng Visual Studio 2022**
1. Mở Visual Studio 2022
2. File → Open → Folder
3. Chọn folder `SortingVisualizer`
4. Nhấn F5 để chạy

**Cách 2.3: Dùng Visual Studio Code**
1. Mở VS Code
2. Mở folder `SortingVisualizer`
3. Nhấn F5 (hoặc chạy lệnh `dotnet run` trong terminal)

---

## Cách 3: Build lại file exe

```bash
cd SortingVisualizer
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

File exe sẽ nằm trong folder `publish`

---

## ⚠️ Troubleshooting

### Lỗi: "Cannot locate resource 'app.ico'"
**Giải pháp:** Copy file `app.ico` vào folder chứa exe

### Lỗi: App không mở
**Giải pháp:** 
1. Kiểm tra Task Manager xem có process `SortingVisualizer.exe` đang chạy không
2. Nếu có, kill process đó và chạy lại
3. Tắt antivirus tạm thời
4. Chạy bằng Visual Studio để xem lỗi chi tiết

### App chạy nhưng không thấy cửa sổ
**Giải pháp:**
1. Nhấn `Alt + Tab` để xem các cửa sổ đang mở
2. Kiểm tra xem có nhiều màn hình không (app có thể mở ở màn hình khác)
3. Nhấn `Windows + D` để minimize tất cả rồi chạy lại

---

## 📁 Cấu trúc project

```
SortingVisualizer/
├── HomePage.xaml/cs          # Trang chủ
├── MainWindow.xaml/cs         # Trang minh họa
├── App.xaml/cs                # Entry point
├── Algorithms/
│   ├── SelectionSortAlgorithm.cs
│   ├── QuickSortAlgorithm.cs
│   └── SortStep.cs
├── app.ico                    # Logo
├── publish/                   # Folder chứa exe
│   ├── SortingVisualizer.exe
│   └── HUONG_DAN.txt
└── README.md
```

---

## 🎯 Tính năng

- ✅ Minh họa Selection Sort và Quick Sort
- ✅ Hiển thị mã giả với highlight từng bước
- ✅ Điều khiển: Start, Pause, Next Step, Speed
- ✅ Thống kê số lần so sánh và hoán đổi
- ✅ Màu sắc trực quan
- ✅ Nhập dãy số thủ công hoặc random

---

## 👥 Nhóm 5

1. Nguyễn Thái Sơn
2. Trần Lê Minh Nhật
3. Đặng Quang Vinh

**GVHD:** Nguyễn Đình Hiển

---

© 2024 - Trường ĐH Công nghệ Thông tin - ĐHQG TP.HCM
