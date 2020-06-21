using System;
using System.Collections.Generic;
using System.Text;
using Pleh.ViewModels;
using System.Timers;
using Pleh.Models;

namespace Pleh.Services
{
    class AutoplayService
    {
        private readonly WorkspaceViewModel Workspace;
        private Timer Timer;
        private PlayerViewModel CurrentPlayer;

        public bool Enabled { get; private set; } = false;

        public AutoplayService(WorkspaceViewModel workspace)
        {
            Workspace = workspace;

            Timer = new Timer(100);

            Timer.Elapsed += (s, e) => {
                Timer.Stop();
                RunObservation();
                Timer.Start();
            };
        }

        private void RunObservation()
        {
            PlayerViewModel currentPlayer = GetCurrentPlayer();

            if(currentPlayer == null)
            {
                return;
            }

            if(currentPlayer.GetSecondsRemaining() < 30)
            {
                ReadyNextClip();
            }

            if( GetCurrentClip().Type == ClipType.Music 
                && GetNextClip().Type == ClipType.Voicetrack 
                && currentPlayer.GetSecondsProgress() >= (GetCurrentClip().FadeOutStart - GetNextClip().RampIn) 
                && currentPlayer.GetSecondsProgress() >= GetCurrentClip().RampOut)
            {
                PlayNextClip();
                return;
            }

            if (    GetCurrentClip().Type == ClipType.Voicetrack 
                    && GetNextClip().Type == ClipType.Music 
                    && currentPlayer.GetSecondsProgress() >= GetCurrentClip().RampOut
                    && (GetCurrentClip().RampOut - currentPlayer.GetSecondsProgress()) <= GetNextClip().RampIn)
            {
                PlayNextClip();
                return;
            }

            if (currentPlayer.GetSecondsProgress() >= GetCurrentClip().FadeOutStart)
            {
                PlayNextClip();
            }
        }

        private int GetClipIndex(Clip needleClip)
        {
            return Workspace.Playlist.Clips.IndexOf(needleClip);
        }

        private PlayerViewModel GetCurrentPlayer()
        {
            if(CurrentPlayer != null)
            {
                return CurrentPlayer;
            }

            double maxTimeRemaining = 0;
            PlayerViewModel activePlayer = null;

            foreach (PlayerViewModel player in Workspace.Players)
            {
                if (player.State == PlayerState.Playing && player.GetSecondsRemaining() > maxTimeRemaining)
                {
                    maxTimeRemaining = player.GetSecondsRemaining();
                    activePlayer = player;
                }
            }

            return activePlayer;
        }

        private Clip GetCurrentClip()
        {
            PlayerViewModel activePlayer = GetCurrentPlayer();

            if(activePlayer == null)
            {
                return null;
            }

            return activePlayer.Player.Clip;
        }

        private Clip GetNextClip()
        {
            Clip activeClip = GetCurrentClip();

            if(activeClip == null)
            {
                return Workspace.Playlist.Clips[0];
            }

            int activeClipIndex = Workspace.Playlist.Clips.IndexOf(activeClip);

            if(Workspace.Playlist.Clips.Count > activeClipIndex + 1)
            {
                return Workspace.Playlist.Clips[activeClipIndex + 1];
            }

            return Workspace.Playlist.Clips[0];
        }

        private void ReadyNextClip()
        {
            bool found = false;

            foreach (PlayerViewModel player in Workspace.Players)
            {
                if (player.State == PlayerState.Paused && player.Player.Clip == GetNextClip())
                {
                    found = true;
                }
            }

            if(!found)
            {
                Workspace.LoadClip(GetNextClip());
            }
        }

        private void PlayNextClip()
        {
            foreach (PlayerViewModel player in Workspace.Players)
            {
                if (player.State == PlayerState.Paused && player.Player.Clip == GetNextClip())
                {
                    player.FadeIn();
                    CurrentPlayer = player;
                }
            }
        }

        public void Enable()
        {
            Timer.Start();
            Enabled = true;
        }

        public void Disable()
        {
            Timer.Stop();
            Enabled = false;
            CurrentPlayer = null;
        }
    }
}
