using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorJump
{
    public struct TrailPoint
    {
        public int X;
        public int Y;
        public float Alpha;

        public TrailPoint(int x, int y, float alpha)
        {
            X = x;
            Y = y;
            Alpha = alpha;
        }
    }
}
