using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ColorJump
{
    public class ObstacleRing : ObstacleBase
    {
        public int X;

        public ObstacleRing(int x, int y)
        {
            X = x;
            Y = y;
            Radius = 80;
        }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color.White, 10))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(X - Radius, Y - Radius, Radius * 2, Radius * 2);

                for (int i = 0; i < 4; i++)
                {
                    pen.Color = SegmentColors[i];
                    g.DrawArc(pen, rect, Angle + i * 90, 90);
                }
            }
        }

        public override bool CheckCollision(Ball ball)
        {
            float dx = ball.X - X;
            float dy = ball.Y - Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance >= Radius - 12 && distance <= Radius + 12)
            {
                float angle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
                if (angle < 0) angle += 360;

                angle -= Angle;
                if (angle < 0) angle += 360;

                int segmentIndex = (int)(angle / 90) % 4;
                Color segmentColor = SegmentColors[segmentIndex];

                if (!ColorsAreEqual(ball.Color, segmentColor))
                    return true;
            }

            return false;
        }
    }
}