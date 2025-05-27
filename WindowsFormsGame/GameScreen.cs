using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace ColorJump
{
    public sealed class GameScreen : BaseScreen
    {
        private readonly BaseButton _mainMenuButton;
        private readonly BaseButton _pauseButton;

        private List<Diamond> _stars = new List<Diamond>();
        private List<ObstacleBase> _obstacles = new List<ObstacleBase>();

        private float _velocityY = 0;
        private const float JumpForce = -12f;
        private const float Gravity = 1.2f;

        private Ball _ball;
        private Rectangle _wellRect;

        private bool _isPaused = false;
        private bool _isGameOver = false;

        private int _score = 0;
        private int _totalScore;
        private int _worldOffsetY;
        private const int ObstacleSpacing = 400;


        public GameScreen(Control parent) : base(parent)
        {
            _pauseButton = new BaseButton(640, 10, 80, 80, ImageManager.Pause);
            _mainMenuButton = new BaseButton(440, 20, 160, 60, ImageManager.Back);

            DoubleBuffered = true;

            int wellWidth = 330;
            int wellHeight = parent.Height;
            int wellX = (parent.Width - wellWidth) / 2;
            int wellY = 0;
            _wellRect = new Rectangle(wellX, wellY, wellWidth, wellHeight);

            int ballX = _wellRect.Left + (_wellRect.Width - 20) / 2;
            int ballY = parent.Height / 2;
            _ball = new Ball(ballX, ballY);

            _totalScore = RecordManager.LoadRecord();

            GenerateInitialElements();
            Start();
        }

        private void GenerateInitialElements()
        {
            GenerateObstacles(_ball.Y, _ball.Y - 1000);
            GenerateStars();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_isGameOver)
            {
                if (_mainMenuButton.InRegion(e.X, e.Y))
                    _mainMenuButton.Pressed = true;
                return;
            }

            if (_pauseButton.InRegion(e.X, e.Y))
            {
                _isPaused = !_isPaused;
            }
            else if (_isPaused && _mainMenuButton.InRegion(e.X, e.Y))
            {
                _mainMenuButton.Pressed = true;
            }
            else if (!_isPaused)
            {
                _velocityY = JumpForce;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_mainMenuButton.InRegion(e.X, e.Y) && _mainMenuButton.Pressed)
            {
                Stop();
                if (_score > 0)
                    RecordManager.AddToRecord(_score);
                (Parent as Form1)?.ChangeScreen(new MainMenuScreen(Parent));
            }
            _mainMenuButton.Pressed = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pe.Graphics.FillRectangle(Brushes.Black, _wellRect);

            foreach (var obstacle in _obstacles)
                obstacle.Draw(pe.Graphics);

            foreach (var star in _stars)
                star.Draw(pe.Graphics);

            _ball.Draw(pe.Graphics);

            if (_isPaused)
            {
                using (Brush overlay = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                    pe.Graphics.FillRectangle(overlay, ClientRectangle);

                string pauseText = "Paused";
                Font pauseFont = new Font("Arial", 36, FontStyle.Bold);
                SizeF textSize = pe.Graphics.MeasureString(pauseText, pauseFont);
                PointF textPos = new PointF(
                    _wellRect.Left + (_wellRect.Width - textSize.Width) / 2,
                    _wellRect.Top + (_wellRect.Height - textSize.Height) / 2
                );
                pe.Graphics.DrawString(pauseText, pauseFont, Brushes.White, textPos);
                _mainMenuButton.Draw(pe.Graphics);
            }

            _pauseButton.Draw(pe.Graphics);

            if (_isGameOver)
            {
                using (Brush overlay = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                    pe.Graphics.FillRectangle(overlay, ClientRectangle);

                string pauseText = "Game Over!";
                Font pauseFont = new Font("Arial", 36, FontStyle.Bold);
                SizeF textSize = pe.Graphics.MeasureString(pauseText, pauseFont);
                PointF textPos = new PointF(
                    _wellRect.Left + (_wellRect.Width - textSize.Width) / 2,
                    _wellRect.Top + (_wellRect.Height - textSize.Height) / 2
                );
                pe.Graphics.DrawString(pauseText, pauseFont, Brushes.White, textPos);

                _mainMenuButton.Draw(pe.Graphics);
            }

            var diamond = ImageManager.Diamond;
            pe.Graphics.DrawImage(diamond, 10, 10, 40, 40);
            pe.Graphics.DrawString($"{_score}", new Font("Arial", 16), Brushes.White, 60, 20);
        }

        public override void Update(long tick)
        {
            if (_isPaused) return;

            _velocityY += Gravity;
            _ball.Y += (int)_velocityY;

            if (_ball.Y < 250)
            {
                int delta = 250 - _ball.Y;
                _ball.Y = 250;
                _worldOffsetY += delta;

                foreach (var star in _stars)
                    star.Y += delta;

                foreach (var obs in _obstacles)
                    obs.Y += delta;

                GenerateObstacles(_obstacles[_obstacles.Count - 1].Y, _obstacles[_obstacles.Count - 1].Y - 800);
                GenerateStars();
            }

            foreach (var star in _stars)
            {
                if (!star.Collected && _ball.GetBounds().IntersectsWith(star.GetBounds()))
                {
                    star.Collected = true;
                    _score++;
                }
            }

            foreach (var obs in _obstacles)
            {
                obs.Update();

                if (!obs.ColorChanged && _ball.Y <= obs.ColorChangeY)
                {
                    _ball.RandomizeColor();
                    obs.ColorChanged = true;
                }

                if (obs.CheckCollision(_ball))
                {
                    _isGameOver = true;
                    return;
                }
            }

            if (_ball.Y + 30 > _wellRect.Bottom)
            {
                _isGameOver = true;
                return;
            }
        }

        private void GenerateStars()
        {
            int starX = _wellRect.Left + (_wellRect.Width - 30) / 2;
            Image starImage = ImageManager.Diamond;

            foreach (var obstacle in _obstacles)
            {
                int y = obstacle.Y - 15;
                bool alreadyExists = _stars.Exists(s => Math.Abs(s.Y - y) < 10);
                if (!alreadyExists)
                {
                    _stars.Add(new Diamond(starX, y, starImage));
                }
            }
        }

        private void GenerateObstacles(int fromY, int toY)
        {
            int centerX = _wellRect.Left + _wellRect.Width / 2;
            Random rand = new Random();

            int startY = fromY - ObstacleSpacing;

            for (int y = startY; y >= toY; y -= ObstacleSpacing)
            {
                int choice = rand.Next(3);
                if (choice == 0)
                    _obstacles.Add(new ObstacleRing(centerX, y));
                else if (choice == 1)
                    _obstacles.Add(new ObstacleSquare(centerX, y));
                else
                    _obstacles.Add(new ObstacleCross(y));
            }
        }
    }
}