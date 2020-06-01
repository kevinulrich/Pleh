using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pleh.Views
{
    public class TopBarView : UserControl
    {
        public TopBarView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
