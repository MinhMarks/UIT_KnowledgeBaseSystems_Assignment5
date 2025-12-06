# 🔢 Sorting Algorithm Visualizer

Chương trình minh họa trực quan thuật toán sắp xếp Selection Sort và Quick Sort.

## 📋 Tính năng

- **Thuật toán hỗ trợ:**
  - Selection Sort (Sắp xếp chọn)
  - Quick Sort (Sắp xếp nhanh)

- **Đầu vào:**
  - Nhập thủ công dãy số
  - Tạo ngẫu nhiên (1-50 phần tử)

- **Minh họa trực quan:**
  - Biểu diễn bằng thanh cột
  - Tô màu nổi bật các phần tử đang xử lý
  - Hiển thị từng bước

- **Điều khiển:**
  - Start/Pause
  - Next Step (tiến từng bước)
  - Speed Control (điều chỉnh tốc độ)

- **Thống kê:**
  - Số lần so sánh
  - Số lần hoán đổi

## 🎨 Màu sắc

| Màu | Ý nghĩa |
|-----|---------|
| 🔵 Xanh dương | Phần tử bình thường |
| 🔴 Đỏ | Phần tử đang so sánh |
| 🟢 Xanh lá | Phần tử đã sắp xếp xong |
| 🟡 Vàng | Pivot (Quick Sort) |

## 🚀 Cách chạy

### Cách 1: Chạy file exe (Khuyên dùng)
1. Tải file `SortingVisualizer.exe` từ folder `publish`
2. Double-click để chạy
3. Không cần cài đặt gì thêm!

### Cách 2: Build từ source code
Yêu cầu: .NET 8 SDK

```bash
cd SortingVisualizer
dotnet run
```

### Cách 3: Build Single File Executable
```bash
cd SortingVisualizer
publish.bat
```
File exe sẽ được tạo trong folder `publish`

## 📖 Hướng dẫn sử dụng

1. **Nhập dữ liệu:**
   - Nhập dãy số vào ô "Nhập dãy số" (cách nhau bởi dấu phẩy) → Click "Áp dụng"
   - Hoặc nhập số lượng → Click "Tạo ngẫu nhiên"

2. **Chọn thuật toán:** Selection Sort hoặc Quick Sort

3. **Điều khiển:**
   - Click "Start" để bắt đầu tự động
   - Click "Pause" để tạm dừng
   - Click "Next Step" để tiến từng bước
   - Kéo thanh "Tốc độ" để điều chỉnh

4. **Reset:** Click "Reset" để quay về trạng thái ban đầu

## 👥 Nhóm thực hiện

- [Họ tên] - [MSSV] - [Lớp]
- [Họ tên] - [MSSV] - [Lớp]

## 📝 License

MIT License
