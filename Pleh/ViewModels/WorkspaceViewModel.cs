using System;
using System.Collections.Generic;
using System.Text;
using Pleh.Models;
using System.Collections.ObjectModel;
using Pleh.Services;
using System.Diagnostics;
using ReactiveUI;
using System.Timers;
using Avalonia.Controls;
using Pleh.Views;

namespace Pleh.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase
    {
        private ClipService ClipService = new ClipService();
        private AutoplayService AutoplayService;

        public ObservableCollection<PlayerViewModel> Players { get; private set; }
        public PlaylistViewModel Playlist { get; private set; }

        private Timer Clock;

        private string timeFormat = "--:--:--";
        public string TimeFormat
        {
            get => timeFormat;
            set => this.RaiseAndSetIfChanged(ref timeFormat, value);
        }

        private bool autoActive = false;
        public bool AutoActive
        {
            get => autoActive;
            set => this.RaiseAndSetIfChanged(ref autoActive, value);
        }

        public WorkspaceViewModel()
        {
            IEnumerable<PlayerViewModel> playerList = new []
            {
                new PlayerViewModel(new Player(1)),
                new PlayerViewModel(new Player(2))
            };

            Players = new ObservableCollection<PlayerViewModel>(playerList);

            Playlist playlist = new Playlist();

            Playlist = new PlaylistViewModel(playlist);

            AutoplayService = new AutoplayService(this);

            Clock = new Timer(1000);

            Clock.Elapsed += (s, e) => {
                Clock.Stop();
                TimeFormat = DateTime.UtcNow.ToString("HH:mm:ss");
                Clock.Start();
            };

            Clock.Start();
        }

        public void LoadClip(Clip clip)
        {
            foreach(PlayerViewModel player in Players)
            {
                if(player.SetClip(clip))
                {
                    break;
                }
            }
        }

        public void EditClipMeta(Clip clip)
        {
            MetaWindow window = new MetaWindow();

            window.DataContext = new MetaWindowViewModel(clip, window);

            window.Width = 400;
            window.Height = 400;

            window.Show();
        }

        public void ActivateAuto()
        {
            AutoplayService.Enable();
            AutoActive = true;
        }

        public void DeactivateAuto()
        {
            AutoplayService.Disable();
            AutoActive = false;
        }

        public async void AddToPlaylist(Avalonia.Controls.Window window)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.AllowMultiple = true;

            string[] files = await fileDialog.ShowAsync(window);

            foreach(string file in files)
            {
                Playlist.AddClip(ClipService.CreateClipFromSourceFile(file));
            }
        }
    }
}
