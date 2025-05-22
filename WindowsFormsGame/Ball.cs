using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ColorJump
{
    public class Ball
    {
        private static readonly Color[] AvailableColors =
        {
            Color.DeepSkyBlue,
            Color.Yellow,
            Color.MediumOrchid,
            Color.HotPink
        };

        private readonly Queue<TrailPoint> _trail = new Queue<TrailPoint>();
        private const int MaxTrailLength = 8;
        private readonly Random _rand = new Random();

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; } = 20;
        public int Height { get; } = 20;
        public Color Color { get; private set; }

        public Ball(int x, int y)
        {
            X = x;
            Y = y;
            RandomizeColor();
        }

        public void Draw(Graphics g)
        {
            _trail.Enqueue(new TrailPoint(X, Y, 1f));

            while (_trail.Count > MaxTrailLength)
                _trail.Dequeue();

            float fadeStep = 1f / MaxTrailLength;
            int i = 0;
            foreach (var tp in _trail)
            {
                float alpha = 0.1f + fadeStep * i;
                Color lighter = BlendWithWhite(Color, 0.8f);
                using (Brush trailBrush = new SolidBrush(Color.FromArgb((int)(alpha * 255), lighter)))
                {
                    DrawShape(g, trailBrush, tp.X, tp.Y);
                }
                i++;
            }

            using (Brush brush = new SolidBrush(Color))
            {
                DrawShape(g, brush, X, Y);
            }
        }

        private void DrawShape(Graphics g, Brush brush, int x, int y)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            switch (BallState.SelectedShape)
            {
                case BallShape.Star:
                    g.FillPolygon(brush, GetStarPoints(x, y));
                    break;
                case BallShape.Diamond:
                    g.FillPolygon(brush, GetDiamondPoints(x, y));
                    break;
                default:
                    g.FillEllipse(brush, x, y, Width, Height);
                    break;
            }
        }

        private PointF[] GetStarPoints(int x, int y)
        {
            PointF[] points = new PointF[10];
            double angle = -Math.PI / 2;
            double delta = Math.PI / 5;

            float cx = x + Width / 2f;
            float cy = y + Height / 2f;
            float r1 = Width * 0.65f;
            float r2 = r1 * 0.5f;

            for (int i = 0; i < 10; i++)
            {
                float r = i % 2 == 0 ? r1 : r2;
                points[i] = new PointF(
                    cx + (float)(r * Math.Cos(angle)),
                    cy + (float)(r * Math.Sin(angle)));
                angle += delta;
            }

            return points;
        }

        private Point[] GetDiamondPoints(int x, int y)
        {
            float scale = 1.2f;

            int centerX = x + Width / 2;
            int centerY = y + Height / 2;
            int halfWidth = (int)(Width / 2 * scale);
            int halfHeight = (int)(Height / 2 * scale);

            return new Point[]
            {
                new Point(centerX, centerY - halfHeight),
                new Point(centerX + halfWidth, centerY),
                new Point(centerX, centerY + halfHeight),
                new Point(centerX - halfWidth, centerY)
            };
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        public void RandomizeColor()
        {
            Color = AvailableColors[_rand.Next(AvailableColors.Length)];
        }

        private Color BlendWithWhite(Color baseColor, float blendAmount)
        {
            int r = (int)(baseColor.R + (255 - baseColor.R) * blendAmount);
            int g = (int)(baseColor.G + (255 - baseColor.G) * blendAmount);
            int b = (int)(baseColor.B + (255 - baseColor.B) * blendAmount);
            return Color.FromArgb(r, g, b);
        }
    }
}