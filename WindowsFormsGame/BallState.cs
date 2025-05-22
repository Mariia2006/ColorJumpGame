using System;
using System.IO;

namespace ColorJump
{
    public enum BallShape
    {
        Circle,
        Star,
        Diamond
    }

    public static class BallState
    {
        private static readonly string shapeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "selected_shape.dat");

        public static BallShape SelectedShape { get; private set; } = LoadShape();

        private static BallShape LoadShape()
        {
            try
            {
                if (!File.Exists(shapeFilePath))
                {
                    SaveShape(BallShape.Circle);
                    return BallShape.Circle;
                }

                string shapeText = File.ReadAllText(shapeFilePath);
                if (Enum.TryParse(shapeText, out BallShape shape))
                {
                    return shape;
                }
            }
            catch
            {

            }

            return BallShape.Circle;
        }

        public static void SaveShape(BallShape shape)
        {
            try
            {
                File.WriteAllText(shapeFilePath, shape.ToString());
                SelectedShape = shape;
            }
            catch
            {
                
            }
        }
    }
}