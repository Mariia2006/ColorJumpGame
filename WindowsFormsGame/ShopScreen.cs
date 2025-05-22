using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ColorJump
{
    public class ShopScreen : BaseScreen
    {
        private readonly BaseButton _mainMenuButton;
        private int _highScore;
        private readonly Rectangle[] _shapeSlots;
        private readonly BallShape[] _shapes = { BallShape.Circle, BallShape.Star, BallShape.Diamond };
        private int _selectedIndex;

        public ShopScreen(Control parent) : base(parent)
        {
            _highScore = RecordManager.LoadRecord();
            _mainMenuButton = new BaseButton(540, 10, 160, 60, ImageManager.Back);

            _shapeSlots = new Rectangle[_shapes.Length];
            for (int i = 0; i < _shapes.Length; i++)
            {
                _shapeSlots[i] = new Rectangle(100 + i * 120, 150, 100, 100);
            }

            _selectedIndex = Array.IndexOf(_shapes, BallState.SelectedShape);
            if (_selectedIndex < 0) _selectedIndex = 0;

            Start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_mainMenuButton.InRegion(e.X, e.Y))
            {
                _mainMenuButton.Pressed = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_mainMenuButton.InRegion(e.X, e.Y) && _mainMenuButton.Pressed)
            {
                Stop();
                (Parent as Form1)?.ChangeScreen(new MainMenuScreen(Parent));
            }
            _mainMenuButton.Pressed = false;

            for (int i = 0; i < _shapeSlots.Length; i++)
            {
                if (_shapeSlots[i].Contains(e.Location) && IsShapeUnlocked(_shapes[i]))
                {
                    _selectedIndex = i;
                    BallState.SaveShape(_shapes[i]);
                    Invalidate();
                    break;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Graphics g = pe.Graphics;

            _mainMenuButton.Draw(g);
            g.DrawImage(ImageManager.Diamond, 10, 10, 40, 40);
            using (Font font = new Font("Arial", 16, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                g.DrawString($"{_highScore}", font, brush, new PointF(60, 20));
            }

            for (int i = 0; i < _shapeSlots.Length; i++)
            {
                Rectangle slot = _shapeSlots[i];

                using (Brush bgBrush = new SolidBrush(Color.White))
                    g.FillRoundedRectangle(bgBrush, slot, 15);

                if (i == _selectedIndex)
                {
                    using (Pen pen = new Pen(Color.LimeGreen, 4))
                        g.DrawRoundedRectangle(pen, slot, 15);
                }

                DrawShape(g, _shapes[i], slot);
                string label = GetShapeLabel(_shapes[i]);
                using (Font labelFont = new Font("Arial", 10, FontStyle.Regular))
                using (Brush labelBrush = new SolidBrush(Color.White))
                {
                    var textSize = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, labelBrush,
                        new PointF(slot.X + (slot.Width - textSize.Width) / 2, slot.Bottom + 5));
                }
            }

        }

        private void DrawShape(Graphics g, BallShape shape, Rectangle rect)
        {
            int x = rect.X + (rect.Width - 40) / 2;
            int y = rect.Y + (rect.Height - 40) / 2;
            Rectangle drawArea = new Rectangle(x, y, 40, 40);

            bool unlocked = IsShapeUnlocked(shape);
            float opacity = unlocked ? 1f : 0.3f;
            Color faded = Color.FromArgb((int)(opacity * 255), Color.DeepSkyBlue);
            using (Brush brush = new SolidBrush(faded))
            {
                switch (shape)
                {
                    case BallShape.Circle:
                        g.FillEllipse(brush, drawArea);
                        break;
                    case BallShape.Star:
                        DrawStar(g, brush, drawArea);
                        break;
                    case BallShape.Diamond:
                        DrawDiamond(g, brush, drawArea);
                        break;
                }
            }
        }

        private void DrawStar(Graphics g, Brush brush, Rectangle bounds)
        {
            PointF[] points = new PointF[10];
            double angle = -Math.PI / 2;
            double delta = Math.PI / 5;
            float cx = bounds.X + bounds.Width / 2f;
            float cy = bounds.Y + bounds.Height / 2f;
            float r1 = bounds.Width / 2f;
            float r2 = r1 * 0.5f;

            for (int i = 0; i < 10; i++)
            {
                float r = i % 2 == 0 ? r1 : r2;
                points[i] = new PointF(
                    cx + (float)(r * Math.Cos(angle)),
                    cy + (float)(r * Math.Sin(angle)));
                angle += delta;
            }

            g.FillPolygon(brush, points);
        }

        private void DrawDiamond(Graphics g, Brush brush, Rectangle bounds)
        {
            Point[] points = new Point[]
            {
                new Point(bounds.X + bounds.Width / 2, bounds.Y),
                new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height / 2),
                new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height),
                new Point(bounds.X, bounds.Y + bounds.Height / 2)
            };
            g.FillPolygon(brush, points);
        }

        private bool IsShapeUnlocked(BallShape shape)
        {
            if (shape == BallShape.Star)
                return _highScore >= 1000;
            else if (shape == BallShape.Diamond)
                return _highScore >= 500;
            else
                return true;
        }

        private string GetShapeLabel(BallShape shape)
        {
            if (IsShapeUnlocked(shape))
                return "Unlocked";

            if (shape == BallShape.Star)
                return "Need 1000";
            else if (shape == BallShape.Diamond)
                return "Need 500";
            else
                return "";
        }
    }
}
