using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pleh.Views
{
    public class WorkspaceView : UserControl
    {
        public WorkspaceView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
