using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class LineCutter
    {
        FigurePoint Cp1 { get; set; }
        FigurePoint Cp2 { get; set; }
        FigurePoint Fp1 { get; set; }
        FigurePoint Fp2 { get; set; }
        int Cutdx { get; set; }
        int Cutdy { get; set; }
        double Cutk { get; set; }
        int Figdx { get; set; }
        int Figdy { get; set; }
        double Figk { get; set; }

        public LineCutter(FigurePoint cutterP1, FigurePoint cutterP2, FigurePoint figureP1, FigurePoint figureP2)
        {
            Cp1 = cutterP1;
            Cp2 = cutterP2;
            Fp1 = figureP1;
            Fp2 = figureP2;
            Cutdx = cutterP2.x - cutterP1.x;
            Cutdy = cutterP2.y - cutterP1.y;
            Cutk = (double)Cutdy / Cutdx;
            Figdx = figureP2.x - figureP1.x;
            Figdy = figureP2.y - figureP1.y;
            Figk = (double)Figdy / Figdx;
        }

        public FigurePoint getPoint()
        {
            if ((Cp1.x == Cp2.x && Cp1.y == Cp2.y) || (Fp1.x == Fp2.x && Fp1.y == Fp2.y)) return new FigurePoint(-1, -1, LineDrawType.Solid);

            var isParallel = (Cutdx != 0 && Figdx != 0
                             && Cutdy != 0 && Figdy != 0
                             && Cutdx % Figdx == 0 && Cutdx * Figdx > 0
                             && Cutdy % Figdy == 0 && Cutdy * Figdy > 0)
                             || (Cutdx == 0 && Figdx == 0)
                             || (Cutdy == 0 && Figdy == 0);
            if (isParallel) return new FigurePoint(-1, -1, LineDrawType.Solid);

            int x;
            int y;
            if (Figdx == 0 || Cutdx == 0)
            {
                if (Figdy == 0 || Cutdy == 0)
                {
                    x = VerticalAndHorizontalXFormule();
                    y = VerticalAndHorizontalYFormule();
                } 
                else
                {
                    if (Figdx == 0)
                    {
                        y = FigureVerticalYFormule();
                        x = SimpleCutXFormule(y);
                    }
                    else
                    {
                        y = CutterVerticalYFormule();
                        x = SimpleFigXFormule(y);
                    }          
                }
            }
            else if (Figdy == 0 || Cutdy == 0)
            {
                if (Figdx == 0)
                {
                    x = FigureHorizontalXFormule();
                    y = SimpleCutYFormule(x);
                }
                else
                {
                    x = CutterHorizontalXFormule();
                    y = SimpleFigYFormule(x);
                }
            }
            else
            {
                x = GeneralizedXFormule();
                y = SimpleCutYFormule(x);
            }

            var fp = new FigurePoint(x, y, LineDrawType.Solid);
            if (isValueNotInRange(x, Fp1.x, Fp2.x, Cp1.x, Cp2.x)) return new FigurePoint(-1, -1, LineDrawType.Solid);
            if (isValueNotInRange(y, Fp1.y, Fp2.y, Cp1.y, Cp2.y)) return new FigurePoint(-1, -1, LineDrawType.Solid);
            if (fp.Equals(Fp1) || fp.Equals(Fp2) || fp.Equals(Cp1) || fp.Equals(Cp2)) return new FigurePoint(-1, -1, LineDrawType.Solid);

            return fp;
        }

        public int SimpleCutYFormule(int x)
        {
            return (int)Math.Round((x - Cp1.x) * Cutk + Cp1.y);
        }

        public int SimpleFigYFormule(int x)
        {
            return (int)Math.Round((x - Fp1.x) * Figk + Fp1.y);
        }

        public int SimpleCutXFormule(int y)
        {
            return (int)Math.Round((y - Cp1.y) / Cutk + Cp1.x);
        }

        public int SimpleFigXFormule(int y)
        {
            return (int)Math.Round((y - Fp1.y) / Figk + Fp1.x);
        }

        private int VerticalAndHorizontalYFormule()
        {
            return Cp1.y == Cp2.y ? Cp1.y : Fp1.y;
        }

        private int GeneralizedXFormule()
        {
            var x1 = Fp1.x;
            var x2 = Fp2.x;
            var x3 = Cp1.x;
            var x4 = Cp2.x;
            var y1 = Fp1.y;
            var y2 = Fp2.y;
            var y3 = Cp1.y;
            var y4 = Cp2.y;
            return (int)Math.Round((double)(x1*(x4-x3)*(y2-y1)-x3*(y4-y3)*(x2-x1)+(y3-y1)*(x2-x1)*(x4-x3))/((x4-x3)*(y2-y1)-(y4-y3)*(x2-x1)));
        }

        private int CutterVerticalYFormule()
        {
            return (int)Math.Round((double)(Cp1.x - Fp1.x) * Figk + Fp1.y);
        }

        private int CutterHorizontalXFormule()
        {
            return (int)Math.Round((double)(Cp1.y - Fp1.y + Fp1.x * Figk) / Figk);
        }

        private int FigureVerticalYFormule()
        {
            return (int)Math.Round((double)(Fp1.x - Cp1.x) * Cutk + Cp1.y);
        }

        private int FigureHorizontalXFormule()
        {
            return (int)Math.Round((double)(Fp1.y - Cp1.y + Cp1.x * Cutk) / Cutk);
        }

        private int VerticalAndHorizontalXFormule()
        {
            return Cp1.x == Cp2.x ? Cp1.x : Fp1.x;
        }

        private bool isValueNotInRange(int value, int fp1v, int fp2v, int cp1v, int cp2v)
        {
            if (fp2v < fp1v)
            {
                var val = fp1v;
                fp1v = fp2v;
                fp2v = val;
            }
            if (cp2v < cp1v)
            {
                var val = cp1v;
                cp1v = cp2v;
                cp2v = val;
            }
            return value < fp1v || value < cp1v || value > fp2v || value > cp2v;
        }
    }
}
