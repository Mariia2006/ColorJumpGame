using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorJump
{
    public class Diamond
    {
        public int X;
        public int Y;
        public bool Collected;
        private Image _starImage;

        public Diamond(int x, int y, Image starImage)
        {
            X = x;
            Y = y;
            Collected = false;
            _starImage = starImage;
        }

        public void Draw(Graphics g)
        {
            if (!Collected)
            {
                g.DrawImage(_starImage, X, Y, 30, 30);
            }
        }
        public Rectangle GetBounds() => new Rectangle(X, Y, 20, 20);
    }
}
