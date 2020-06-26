using System;
using System.Collections.Generic;
using System.Text;
using Pleh.Services;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Pleh.Models;

namespace Pleh.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public WorkspaceViewModel Workspace { get; }

        public MainWindowViewModel()
        {
            Workspace = new WorkspaceViewModel();
        }

        public void DeletePress()
        {
            Workspace.DeletePress();
        }
    }
}
