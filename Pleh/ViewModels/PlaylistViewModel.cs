using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Pleh.Models;
using System.Diagnostics;
using Pleh.Services;
using ReactiveUI;

namespace Pleh.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        public Playlist Playlist { get; private set; }
        public List<Clip> SelectedItems { get; private set; } = new List<Clip>();

        private ObservableCollection<Clip> clips;
        public ObservableCollection<Clip> Clips
        {
            get => clips;
            set => this.RaiseAndSetIfChanged(ref clips, value);
        }

        public PlaylistViewModel(Playlist playlist)
        {
            Playlist = playlist;

            Clips = new ObservableCollection<Clip>(playlist.List);
        }

        public void AddClip(Clip clip)
        {
            Clips.Add(clip);
            Playlist.List.Add(clip);
        }

        public void RemoveSelectedClip()
        {
            if(SelectedItems.Count == 0)
            {
                return;
            }

            for(int i = 0; i < SelectedItems.Count; i++)
            {
                Clips.Remove(SelectedItems[i]);
            }
        }
    }
}
