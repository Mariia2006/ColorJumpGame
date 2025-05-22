using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace ColorJump
{
    public static class AudioManager
    {
        private static WindowsMediaPlayer wmp;
        private static bool isPlaying = false;

        public static void PlayMusic()
        {
            if (wmp == null)
            {
                wmp = new WindowsMediaPlayer();
                wmp.URL = "actiontheme-v3.mp3";
                wmp.settings.setMode("loop", true);
            }
            if (!isPlaying)
            {
                wmp.controls.play();
                isPlaying = true;
            }
        }

        public static void StopMusic()
        {
            if (wmp != null)
            {
                wmp.controls.stop();
                isPlaying = false;
            }
        }

        public static void ToggleMusic()
        {
            if (isPlaying)
                StopMusic();
            else
                PlayMusic();
        }
    }
}
