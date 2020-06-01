using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pleh.Views
{
    public class PlaylistView : UserControl
    {
        public PlaylistView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
