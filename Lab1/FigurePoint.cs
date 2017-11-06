using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class FigurePoint
    {
        public int x;
        public int y;
        public LineDrawType lineType;

        public FigurePoint(int x, int y, LineDrawType lineType)
        {
            this.x = x;
            this.y = y;
            this.lineType = lineType;
        }

        public FigurePoint(Point point, LineDrawType lineType)
        {
            x = point.X;
            y = point.Y;
            this.lineType = lineType;
        }

        public FigurePoint(Point point)
        {
            x = point.X;
            y = point.Y;
            this.lineType = LineDrawType.Solid;
        }

        public Point ToPoint()
        {
            return new Point(x, y);
        }

        public Point VectorToPoint(FigurePoint point)
        {
            var dx = point.x - x;
            var dy = point.y - y;
            return new Point(dx, dy);
        }

        public bool Equals(FigurePoint fp)
        {
            return x == fp.x && y == fp.y;
        }
    }
}
