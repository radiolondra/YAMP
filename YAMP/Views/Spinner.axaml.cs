using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace YAMP.Views
{
    public partial class Spinner : UserControl
    {
        public Spinner()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
