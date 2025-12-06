using System.Windows;

namespace SortingVisualizer
{
    public partial class HomePage : Window
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ẩn HomePage thay vì đóng
                this.Hide();
                
                // Tạo và hiển thị MainWindow
                var mainWindow = new MainWindow();
                mainWindow.Closed += (s, args) => this.Close(); // Khi MainWindow đóng thì đóng luôn app
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Show(); // Hiện lại HomePage nếu lỗi
            }
        }
    }
}
