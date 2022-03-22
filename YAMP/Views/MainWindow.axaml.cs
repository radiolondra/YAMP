using Avalonia.Controls;
using YAMP.ViewModels;

namespace YAMP.Views
{
    public partial class MainWindow : Window
    {
        private static MainWindow _this;
        MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            _this = this;
        }

        public static MainWindow GetInstance()
        {
            return _this;
        }
    }
}
