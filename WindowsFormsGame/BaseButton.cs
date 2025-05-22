using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ColorJump
{
    public class BaseButton
    {
        public BaseButton(int left, int top, int width, int height, Image image)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            this.image = image;
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Pressed { get; set; }
        public Image image { get; set; }

        public bool InRegion(int x, int y)
        {
            return x >= Left && x <= Left + Width && y >= Top && y <= Top + Height;
        }

        public void Draw(Graphics g)
        {
            if (image != null)
            {
                g.DrawImage(image, Left, Top, Width, Height);
            }
        }
    }
}