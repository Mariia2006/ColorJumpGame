using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ColorJump
{
    public class ObstacleSquare : ObstacleBase
    {
        public int X;

        public ObstacleSquare(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float side = Radius * 1.9f;  
            float thickness = 10f;                      
            float overlap = 5f;                         

            PointF center = new PointF(X, Y);

            PointF[] offsets = new PointF[]
            {
                new PointF(0, -side / 2), 
                new PointF(side / 2, 0),  
                new PointF(0, side / 2), 
                new PointF(-side / 2, 0)  
            };

            using (Matrix rotation = new Matrix())
            {
                rotation.RotateAt(Angle, center);

                for (int i = 0; i < 4; i++)
                {
                    PointF offset = offsets[i];
                    PointF sideCenter = new PointF(center.X + offset.X, center.Y + offset.Y);

                    RectangleF sideRect;

                    if (i % 2 == 0) 
                    {
                        sideRect = new RectangleF(
                            sideCenter.X - side / 2 - overlap,
                            sideCenter.Y - thickness / 2,
                            side + 2 * overlap,
                            thickness
                        );
                    }
                    else 
                    {
                        sideRect = new RectangleF(
                            sideCenter.X - thickness / 2,
                            sideCenter.Y - side / 2 - overlap,
                            thickness,
                            side + 2 * overlap
                        );
                    }

                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddRectangle(sideRect);
                        path.Transform(rotation);

                        using (Brush brush = new SolidBrush(SegmentColors[i]))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                }
            }
        }

        public override bool CheckCollision(Ball ball)
        {
            float dx = ball.X - X;
            float dy = ball.Y - Y;

            double rad = -Angle * Math.PI / 180.0;
            float rotatedX = (float)(dx * Math.Cos(rad) - dy * Math.Sin(rad));
            float rotatedY = (float)(dx * Math.Sin(rad) + dy * Math.Cos(rad));

            int halfSide = Radius;

            float buffer = 12; 
            bool inXRange = Math.Abs(rotatedX) <= halfSide + buffer;
            bool inYRange = Math.Abs(rotatedY) <= halfSide + buffer;

            if (inXRange && inYRange &&
                (Math.Abs(Math.Abs(rotatedX) - halfSide) <= buffer || Math.Abs(Math.Abs(rotatedY) - halfSide) <= buffer))
            {
                int segmentIndex = -1;

                if (Math.Abs(rotatedY + halfSide) <= buffer) segmentIndex = 0; 
                else if (Math.Abs(rotatedX - halfSide) <= buffer) segmentIndex = 1; 
                else if (Math.Abs(rotatedY - halfSide) <= buffer) segmentIndex = 2; 
                else if (Math.Abs(rotatedX + halfSide) <= buffer) segmentIndex = 3; 

                if (segmentIndex != -1)
                {
                    Color segmentColor = SegmentColors[segmentIndex];
                    if (!ColorsAreEqual(ball.Color, segmentColor))
                        return true; 
                }
            }

            return false;
        }
    }
}
