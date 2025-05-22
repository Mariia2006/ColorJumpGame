using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ColorJump
{
    public class MainMenuScreen: BaseScreen
    {
        private readonly BaseButton _playButton;
        private readonly BaseButton _shopButton;
        private readonly BaseButton _exitButton;
        private readonly BaseButton _soundButton;
        private int _highScore;

        public MainMenuScreen(Control parent) : base(parent)
        {
            _highScore = RecordManager.LoadRecord();

            _playButton = new BaseButton(290, 280, 160, 60, ImageManager.Play);
            _shopButton = new BaseButton(290, 360, 160, 60, ImageManager.Shop);
            _exitButton = new BaseButton(290, 440, 160, 60, ImageManager.Exit);
            _soundButton = new BaseButton(10, 515, 80, 80, ImageManager.Sound);

            Start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_playButton.InRegion(e.X, e.Y))
            {
                _playButton.Pressed = true;
            }
            else if (_shopButton.InRegion(e.X, e.Y))
            {
                _shopButton.Pressed = true;
            }
            else if (_exitButton.InRegion(e.X, e.Y))
            {
                _exitButton.Pressed = true;
            }
            else if (_soundButton.InRegion(e.X, e.Y))
            {
                _soundButton.Pressed = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_playButton.InRegion(e.X, e.Y) && _playButton.Pressed)
            {
                Stop();
                (Parent as Form1)?.ChangeScreen(new GameScreen(Parent));
            }
            else if (_shopButton.InRegion(e.X, e.Y) && _shopButton.Pressed)
            {
                Stop();
                (Parent as Form1)?.ChangeScreen(new ShopScreen(Parent));
            }
            else if (_soundButton.InRegion(e.X, e.Y) && _soundButton.Pressed)
            {
                Stop();
                AudioManager.ToggleMusic();
            }
            else if (_exitButton.InRegion(e.X, e.Y) && _exitButton.Pressed)
            {
                Stop();
                Application.Exit();
            }

            _playButton.Pressed = false;
            _shopButton.Pressed = false;
            _exitButton.Pressed = false;
            _soundButton.Pressed = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            var logo = ImageManager.Logo;
            var diamond = ImageManager.Diamond;

            base.OnPaint(pe);
            _playButton.Draw(pe.Graphics);
            _shopButton.Draw(pe.Graphics);
            _exitButton.Draw(pe.Graphics);
            _soundButton.Draw(pe.Graphics);
            pe.Graphics.DrawImage(logo, 130, 70);
            pe.Graphics.DrawImage(diamond, 10, 10, 40, 40);

            using (Font font = new Font("Arial", 16, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                pe.Graphics.DrawString($"{_highScore}", font, brush, new PointF(60, 20));
            }
        }
    }
}
