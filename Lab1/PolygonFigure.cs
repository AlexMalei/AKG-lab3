using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class PolygonFigure
    {
        public List<FigurePoint> Points { get; private set; }
        public Point RotatePoint;
        public delegate void DrawLine(int x1, int y1, int x2, int y2, Color color);
        private double angle { get; set; }

        public PolygonFigure(List<Point> points, Point rotatePoint)
        {
            Points = points.Select(p => new FigurePoint(p)).ToList();
            RotatePoint = rotatePoint;
        }

        private PolygonFigure(List<Point> points)
        {
            Points = points.Select(p => new FigurePoint(p)).ToList();
        }

        public void MoveX(int dx)
        {
            Points.ForEach(p => p.x += dx);
            RotatePoint.X += dx;
        }

        public void MoveY(int dy)
        {
            Points.ForEach(p => p.y += dy);
            RotatePoint.Y += dy;
        }

        public void Rotate(double angle)
        {
            this.angle += angle % Math.PI*2;
        }

        public void DrawFigure(DrawLine visLineFunc)
        {
            List<FigurePoint> fp = new List<FigurePoint>();
            for (int j = 0; j < Points.Count; j++)
            {
                FigurePoint p = new FigurePoint(0, 0, Points[j].lineType);
                p.x = (int)Math.Round((Points[j].x - RotatePoint.X) * Math.Cos(angle) - (Points[j].y - RotatePoint.Y) * Math.Sin(angle) + RotatePoint.X);
                p.y = (int)Math.Round((Points[j].x - RotatePoint.X) * Math.Sin(angle) + (Points[j].y - RotatePoint.Y) * Math.Cos(angle) + RotatePoint.Y);
                fp.Add(p);
            }
            for (int i = 0; i < fp.Count; i++)
            {
                var j = (i + 1) % Points.Count;
                visLineFunc(fp[i].x, fp[i].y, fp[j].x, fp[j].y, Color.Black);
            }
        }

        public void DrawClippedFigure(PolygonFigure clippingFigure, DrawLine visLineFunc, DrawLine unvisLineFunc)
        {
            var figure = GenerateClippedFigure(clippingFigure);
            var points = figure.Points;
            for (int i = 0; i < points.Count; i++)
            {
                var j = (i + 1) % points.Count;
                if (points[i].lineType == LineDrawType.Solid)
                {
                    visLineFunc(points[i].x, points[i].y, points[j].x, points[j].y, Color.Black);
                }
                else
                {
                    unvisLineFunc(points[i].x, points[i].y, points[j].x, points[j].y, Color.Black);
                }   
            }
        }

        private PolygonFigure GenerateClippedFigure(PolygonFigure clippingFigure)
        {
            int i;
            List<FigurePoint> fp = new List<FigurePoint>();
            for (int j = 0; j < Points.Count; j++)
            {
                FigurePoint p = new FigurePoint(0, 0, Points[j].lineType);
                p.x = (int)Math.Round((Points[j].x - RotatePoint.X) * Math.Cos(angle) - (Points[j].y - RotatePoint.Y) * Math.Sin(angle) + RotatePoint.X);
                p.y = (int)Math.Round((Points[j].x - RotatePoint.X) * Math.Sin(angle) + (Points[j].y - RotatePoint.Y) * Math.Cos(angle) + RotatePoint.Y);
                fp.Add(p);
            }
            var clippedFigure = new PolygonFigure(fp.Select(p => p.ToPoint()).ToList());
            i = 0;
            while (i < clippedFigure.Points.Count)
            {
                int j = (i + 1) % clippedFigure.Points.Count;
                var cutPoints = FindCutPoints(clippedFigure.Points[i], clippedFigure.Points[j], clippingFigure);
                clippedFigure.Points.InsertRange(j, cutPoints);
                i += cutPoints.Count + 1;
            }
            for (i = 0; i < clippedFigure.Points.Count; i++)
            {
                if (IsInPoligon(clippedFigure.Points[i], clippingFigure)) clippedFigure.Points[i].lineType = LineDrawType.Broken;
                else clippedFigure.Points[i].lineType = LineDrawType.Solid;
            }
            for (i = 0; i < clippedFigure.Points.Count; i++)
            {
                if (IsOnBorder(clippedFigure.Points[i], clippingFigure))
                {
                    int j = i == 0 ? j = clippedFigure.Points.Count - 1 : i - 1;
                    int k = (i + 1) % clippedFigure.Points.Count;
                    if (clippedFigure.Points[j].lineType == clippedFigure.Points[k].lineType)
                    {
                        clippedFigure.Points[i].lineType = clippedFigure.Points[j].lineType;
                    }
                    else if (clippedFigure.Points[j].lineType == LineDrawType.Solid)
                    {
                        clippedFigure.Points[i].lineType = LineDrawType.Broken;
                    }
                    else
                    {
                        clippedFigure.Points[i].lineType = LineDrawType.Solid;
                    }

                }
            }
            return clippedFigure;
        }

        private List<FigurePoint> FindCutPoints(FigurePoint p1, FigurePoint p2, PolygonFigure pf)
        {
            List<FigurePoint> list = new List<FigurePoint>();
            for (int i = 0; i < pf.Points.Count; i++)
            {
                int j = (i + 1) % pf.Points.Count;
                list.Add(FindCutPoint(pf.Points[i], pf.Points[j], p1, p2));
            }

            if (p2.x < p1.x)
            {
                list.Sort(new DecPointComparer());
                list = list.Where(p => p.x != -1).ToList();
            }
            else
            {
                list.Sort(new IncPointComparer());
                list = list.Where(p => p.x != -1).ToList();
            }
            return list;
        }

        private FigurePoint FindCutPoint(FigurePoint cutterP1, FigurePoint cutterP2, FigurePoint figureP1, FigurePoint figureP2)
        {
            if (cutterP2.x < cutterP1.x)
            {
                var p = cutterP1;
                cutterP1 = cutterP2;
                cutterP2 = p;
            }
            if (figureP2.x < figureP1.x)
            {
                var p = figureP1;
                figureP1 = figureP2;
                figureP2 = p;
            }
            var lc = new LineCutter(cutterP1, cutterP2, figureP1, figureP2);
            return lc.getPoint();
        }

        private bool IsInPoligon(FigurePoint point, PolygonFigure pf)
        {
            int i1, i2, n, N, S, S1, S2, S3;
            bool flag = false;
            N = pf.Points.Count;
            var p = pf.Points;
            for (n = 0; n < N; n++)
            {
                flag = false;
                i1 = n < N - 1 ? n + 1 : 0;
                while (!flag)
                {
                    i2 = i1 + 1;
                    if (i2 >= N)
                        i2 = 0;
                    if (i2 == (n < N - 1 ? n + 1 : 0))
                        break;
                    S = Math.Abs(p[i1].x * (p[i2].y - p[n].y) +
                             p[i2].x * (p[n].y - p[i1].y) +
                             p[n].x * (p[i1].y - p[i2].y));
                    S1 = Math.Abs(p[i1].x * (p[i2].y - point.y) +
                              p[i2].x * (point.y - p[i1].y) +
                              point.x * (p[i1].y - p[i2].y));
                    S2 = Math.Abs(p[n].x * (p[i2].y - point.y) +
                              p[i2].x * (point.y - p[n].y) +
                              point.x * (p[n].y - p[i2].y));
                    S3 = Math.Abs(p[i1].x * (p[n].y - point.y) +
                              p[n].x * (point.y - p[i1].y) +
                              point.x * (p[i1].y - p[n].y));
                    if (S == S1 + S2 + S3)
                    {
                        flag = true;
                        break;
                    }
                    i1 = i1 + 1;
                    if (i1 >= N)
                        i1 = 0;
                }
                if (!flag)
                    break;
            }
            return flag;
        }

        public bool IsOnBorder(FigurePoint p, PolygonFigure pf)
        {
            var points = pf.Points;
            bool onBorder = false;
            for (int i = 0; i < points.Count; i++)
            {
                int j = (i + 1) % points.Count;
                var p1 = points[i];
                var p2 = points[j];

                if (p1.x - p2.x == 0 && p1.x == p.x)
                {
                    onBorder = p1.y < p2.y ? (p.y >= p1.y && p.y <= p2.y) : (p.y >= p2.y && p.y <= p1.y);
                }
                else if (p1.y - p2.y == 0 && p1.y == p.y)
                {
                    onBorder = p1.x < p2.x ? (p.x >= p1.x && p.x <= p2.x) : (p.x >= p2.x && p.x <= p1.x);
                }
                else
                {
                    var lc = new LineCutter(points[i], points[j], p, p);
                    if (lc.SimpleCutYFormule(p.x) == p.y)
                    {
                        onBorder = p1.x < p2.x ? (p.x >= p1.x && p.x <= p2.x) : (p.x >= p2.x && p.x <= p1.x);
                    }
                }
                if (onBorder) return true;
            }
            return false;
        }

        private class IncPointComparer : Comparer<FigurePoint>
        {
            public IncPointComparer() { }

            public override int Compare(FigurePoint p1, FigurePoint p2)
            {
                return p1.x - p2.x;
            }
        }

        private class DecPointComparer : Comparer<FigurePoint>
        {
            public DecPointComparer() { }

            public override int Compare(FigurePoint p1, FigurePoint p2)
            {
                return p2.x - p1.x;
            }
        }
    }
}
