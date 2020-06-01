using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Pleh.Models;

namespace Pleh.Views
{
    public class PlayerView : UserControl
    {
        public PlayerView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
        }
    }
}
