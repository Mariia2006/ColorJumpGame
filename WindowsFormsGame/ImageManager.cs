using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;

namespace ColorJump
{
    public static class ImageManager
    {
        public static Image Logo { get; private set; }
        public static Image Sound { get; private set; }
        public static Image Diamond { get; private set; }
        public static Image Play { get; private set; }
        public static Image Shop { get; private set; }
        public static Image Back { get; private set; }
        public static Image Pause { get; private set; }
        public static Image Exit { get; private set; }

        static ImageManager()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            Logo = LoadImage(basePath, "COLOR JUMP.png");
            Sound = LoadImage(basePath, "SOUND.png");
            Diamond = LoadImage(basePath, "DIAMOND.png");
            Play = LoadImage(basePath, "PLAY.png");
            Shop = LoadImage(basePath, "SHOP.png");
            Back = LoadImage(basePath, "BACK.png");
            Pause = LoadImage(basePath, "PAUSE.png");
            Exit = LoadImage(basePath, "EXIT.png");
        }

        private static Image LoadImage(string basePath, string fileName)
        {
            string fullPath = Path.Combine(basePath, fileName);
            return File.Exists(fullPath) ? Image.FromFile(fullPath) : null;
        }
    }
}
