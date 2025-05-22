using ColorJump;
using System.Drawing;

namespace ColorJump
{
    public abstract class ObstacleBase
    {
        public int Y { get; set; }
        public float Angle { get; set; } = 0f;
        public float RotationSpeed { get; set; } = 2f;
        public int Radius { get; set; } = 65;

        protected readonly Color[] SegmentColors = new[]
        {
            Color.DeepSkyBlue, Color.Yellow, Color.MediumOrchid, Color.HotPink
        };

        public bool ColorChanged = false;
        public int ColorChangeY => Y + 200;

        public virtual void Update()
        {
            Angle += RotationSpeed;
            if (Angle > 360f) Angle -= 360f;
        }
        public abstract void Draw(Graphics g);
        public abstract bool CheckCollision(Ball ball);
        public virtual bool ColorsAreEqual(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }
    }
}