using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace SafeFactory.VideoCapture
{
    internal class VideoPlayer : IDisposable
    {
        private readonly LibVLC vlc;
        private readonly MediaPlayer player;

        public MediaPlayer Player => player;

        public VideoPlayer()
        {
            vlc = new LibVLC(enableDebugLogs: true);
            vlc.Log += (s, e) => Debug.WriteLine(e.Message);
            player = new MediaPlayer(vlc);
        }

        public void Dispose()
        {
            vlc.Dispose();
            Player.Dispose();
        }

        public void Play(Uri filePath)
        {
            using var media = new Media(vlc, filePath);
            player.Play(media);
        }

        public void Stop()
        {
            player.Stop();
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Resume()
        {
            player.Play();
        }

        public void Seek(TimeSpan time)
        {
            player.SeekTo(time);
        }
    }
}
