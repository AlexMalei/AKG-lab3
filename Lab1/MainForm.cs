using System;
using System.Threading;
using System.Windows.Forms;
using SDL2;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Lab1
{
    public partial class MainForm : Form
    {
        const int SCREEN_WIDTH = 640;
        const int SCREEN_HEIGHT = 640;
        const int DY = 2;
        const int DX = 2;
        const double ANGLE = Math.PI / 90;

        private IntPtr renderer;

        public MainForm()
        {
            InitializeComponent();
            Thread thread = new Thread(() =>
            {
                SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
                IntPtr wnd = SDL.SDL_CreateWindow(
                    "АКГ: Лабораторная работа №1", 
                    100 , 100, 
                    SCREEN_WIDTH, SCREEN_HEIGHT, 
                    SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
                );
                renderer = SDL.SDL_CreateRenderer(wnd, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

                List<Point> list = new List<Point>();
                list.Add(new Point(200, 200));
                list.Add(new Point(300, 200));
                list.Add(new Point(300, 300));
                list.Add(new Point(200, 300));
                var figure = new PolygonFigure(list, new Point(250, 250));
                list.Clear();
                list.Add(new Point(300, 300));
                list.Add(new Point(400, 300));
                list.Add(new Point(400, 400));
                list.Add(new Point(300, 400));
                var figure2 = new PolygonFigure(list, new Point(350, 350));
                bool quit = false;
                while (!quit)
                {
                    SDL.SDL_Event sdlEvent;
                    SDL.SDL_PollEvent(out sdlEvent);
                    switch (sdlEvent.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                        {
                            quit = true;
                            break;
                        }
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                        {
                            int numkey;
                            var ptr = SDL.SDL_GetKeyboardState(out numkey);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_W) == 1) figure.MoveY(-DY);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_S) == 1) figure.MoveY(DY);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_A) == 1) figure.MoveX(-DX);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_D) == 1) figure.MoveX(DX);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_Q) == 1) figure.Rotate(-ANGLE);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_E) == 1) figure.Rotate(ANGLE);

                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_I) == 1) figure2.MoveY(-DY);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_K) == 1) figure2.MoveY(DY);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_J) == 1) figure2.MoveX(-DX);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_L) == 1) figure2.MoveX(DX);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_U) == 1) figure2.Rotate(-ANGLE);
                            if (Marshal.ReadByte(ptr + (int)SDL.SDL_Scancode.SDL_SCANCODE_O) == 1) figure2.Rotate(ANGLE);
                            break;
                        }
                    }
                    int num = 0;
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                    SDL.SDL_RenderClear(renderer);
                    figure.DrawFigure(DrawLine);
                    figure2.DrawClippedFigure(figure, DrawLine, DrawNotVisibleLine);
                    SDL.SDL_RenderPresent(renderer);
                }
                SDL.SDL_DestroyRenderer(renderer);
                SDL.SDL_DestroyWindow(wnd);
                SDL.SDL_Quit();

            });
            thread.Start();
            thread.Join();
        }

        private void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            SDL.SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x2 >= x1 ? 1 : -1;
            int sy = y2 >= y1 ? 1 : -1;

            if (dy <= dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;

                SDL.SDL_RenderDrawPoint(renderer, x1, y1);
                for (int x = x1 + sx, y = y1, i = 1; i <= dx; i++, x += sx)
                {
                    if (d > 0)
                    {
                        d += d2; y += sy;
                    }
                    else
                        d += d1;
                    SDL.SDL_RenderDrawPoint(renderer, x, y);
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;

                SDL.SDL_RenderDrawPoint(renderer, x1, y1);
                for (int x = x1, y = y1 + sy, i = 1; i <= dy; i++, y += sy)
                {
                    if (d > 0)
                    {
                        d += d2; x += sx;
                    }
                    else
                        d += d1;
                    SDL.SDL_RenderDrawPoint(renderer, x, y);
                }
            }
        }

        private void DrawNotVisibleLine(int x1, int y1, int x2, int y2, Color color)
        {
            SDL.SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x2 >= x1 ? 1 : -1;
            int sy = y2 >= y1 ? 1 : -1;
            int c = 1;

            if (dy <= dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;

                SDL.SDL_RenderDrawPoint(renderer, x1, y1);
                for (int x = x1 + sx, y = y1, i = 1; i <= dx; i++, x += sx)
                {
                    if (d > 0)
                    {
                        d += d2; y += sy;
                    }
                    else
                    {
                        d += d1;
                    }
                    c = ++c % 4;
                    if (c > 1)
                    {
                        SDL.SDL_RenderDrawPoint(renderer, x, y);
                    }
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;

                SDL.SDL_RenderDrawPoint(renderer, x1, y1);
                for (int x = x1, y = y1 + sy, i = 1; i <= dy; i++, y += sy)
                {
                    if (d > 0)
                    {
                        d += d2; x += sx;
                    }
                    else
                    {
                        d += d1;
                    }
                    c = ++c % 4;
                    if (c > 1)
                    {
                        SDL.SDL_RenderDrawPoint(renderer, x, y);
                    }
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Hide();
            Close();
        }
    }
}
