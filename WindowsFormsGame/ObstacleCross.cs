using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ColorJump
{
    public class ObstacleCross : ObstacleBase
    {
        public int ScreenWidth;
        private int leftX;
        private int rightX;

        public ObstacleCross(int y)
        {
            Y = y;
            leftX = 310;
            rightX = 430;
        }

        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawSingleCross(g, leftX, mirrored: false); 
            DrawSingleCross(g, rightX, mirrored: true); 
        }

        private void DrawSingleCross(Graphics g, int centerX, bool mirrored)
        {
            float length = Radius * 2.1f;
            float thickness = 12f;

            PointF center = new PointF(centerX, Y);
            float[] angles = { 0, 90 };

            for (int i = 0; i < angles.Length; i++)
            {
                float angle = Angle + angles[i];
                if (mirrored)
                    angle = 180 - angle;

                float rad = angle * (float)Math.PI / 180f;
                float dx = (float)Math.Cos(rad);
                float dy = (float)Math.Sin(rad);

                PointF mid1 = new PointF(center.X - dx * length / 10f, center.Y - dy * length / 10f);
                PointF mid2 = new PointF(center.X + dx * length / 10f, center.Y + dy * length / 10f);

                PointF end1 = new PointF(center.X - dx * length / 2, center.Y - dy * length / 2);
                PointF end2 = new PointF(center.X + dx * length / 2, center.Y + dy * length / 2);

                using (Pen pen = new Pen(SegmentColors[i * 2], thickness)
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round
                })
                {
                    g.DrawLine(pen, end1, mid1);
                }

                using (Pen pen = new Pen(SegmentColors[i * 2 + 1], thickness)
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round
                })
                {
                    g.DrawLine(pen, mid2, end2);
                }
            }
        }

        public override bool CheckCollision(Ball ball)
        {
            return CheckSingleCrossCollision(ball, leftX, mirrored: false)
                || CheckSingleCrossCollision(ball, rightX, mirrored: true);
        }

        private bool CheckSingleCrossCollision(Ball ball, int centerX, bool mirrored)
        {
            float length = Radius * 2f;
            float thickness = 12f;

            PointF center = new PointF(centerX, Y);
            float[] angles = { 0, 90 };

            for (int i = 0; i < angles.Length; i++)
            {
                float angle = Angle + angles[i];
                if (mirrored)
                    angle = 180 - angle;

                float rad = angle * (float)Math.PI / 180f;
                float dx = (float)Math.Cos(rad);
                float dy = (float)Math.Sin(rad);

                PointF mid1 = new PointF(center.X - dx * length / 4, center.Y - dy * length / 4);
                PointF mid2 = new PointF(center.X + dx * length / 4, center.Y + dy * length / 4);

                PointF end1 = new PointF(center.X - dx * length / 2, center.Y - dy * length / 2);
                PointF end2 = new PointF(center.X + dx * length / 2, center.Y + dy * length / 2);

                RectangleF rect1 = MakeCapsuleRect(end1, mid1, thickness);
                RectangleF rect2 = MakeCapsuleRect(mid2, end2, thickness);

                int colorIndex1 = i * 2;
                int colorIndex2 = i * 2 + 1;

                if (rect1.Contains(ball.X, ball.Y) && !ColorsAreEqual(ball.Color, SegmentColors[colorIndex1]))
                    return true;

                if (rect2.Contains(ball.X, ball.Y) && !ColorsAreEqual(ball.Color, SegmentColors[colorIndex2]))
                    return true;
            }

            return false;
        }

        private RectangleF MakeCapsuleRect(PointF a, PointF b, float thickness)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            float nx = dx / length;
            float ny = dy / length;

            float ox = -ny * thickness / 2;
            float oy = nx * thickness / 2;

            PointF[] corners = new PointF[]
            {
                new PointF(a.X + ox, a.Y + oy),
                new PointF(a.X - ox, a.Y - oy),
                new PointF(b.X - ox, b.Y - oy),
                new PointF(b.X + ox, b.Y + oy)
            };

            return GetBoundingRect(corners);
        }

        private RectangleF GetBoundingRect(PointF[] points)
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (var pt in points)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y > maxY) maxY = pt.Y;
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}