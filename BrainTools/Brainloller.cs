﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;

namespace Brainfuck
{
    static class Brainloller
    {
        private enum Direction
        {
            east, west, north, south
        };

        public static Bitmap Encode(string code, int width, Color gapFiller)
        {
            double _height = (double)code.Length / (width - 2);
            int height = 0;
            if (_height == (int)_height)
                height = (int)_height;
            else
                height = (int)_height + 1;

            Bitmap newBmp = new Bitmap(width, height);
            int length = code.Length;

            int curX = 1, curY = 0;
            Direction dir = Direction.east;

            for (int i = 0; i < code.Length; i++)
            {
                Color clr = Color.FromArgb(0, 0, 0);

                if (curX == newBmp.Width - 1 && dir == Direction.east)
                {
                    newBmp.SetPixel(curX, curY, Color.FromArgb(0, 255, 255));
                    newBmp.SetPixel(curX, curY + 1, Color.FromArgb(0, 255, 255));
                    curX--;
                    curY++;
                    dir = Direction.west;
                }
                else if (curX == 0 && dir == Direction.west)
                {
                    newBmp.SetPixel(curX, curY, Color.FromArgb(0, 128, 128));
                    newBmp.SetPixel(curX, curY + 1, Color.FromArgb(0, 128, 128));
                    curX++;
                    curY++;
                    dir = Direction.east;
                }

                switch (code[i])
                {
                    case '>':
                        clr = Color.FromArgb(255, 0, 0);
                        break;
                    case '<':
                        clr = Color.FromArgb(128, 0, 0);
                        break;
                    case '+':
                        clr = Color.FromArgb(0, 255, 0);
                        break;
                    case '-':
                        clr = Color.FromArgb(0, 128, 0);
                        break;
                    case '.':
                        clr = Color.FromArgb(0, 0, 255);
                        break;
                    case ',':
                        clr = Color.FromArgb(0, 0, 128);
                        break;
                    case '[':
                        clr = Color.FromArgb(255, 255, 0);
                        break;
                    case ']':
                        clr = Color.FromArgb(128, 128, 0);
                        break;
                    default:
                        continue;
                }

                newBmp.SetPixel(curX, curY, clr);

                if (dir == Direction.east)
                    curX++;
                if (dir == Direction.west)
                    curX--;
            }

            if (dir == Direction.east)
                for (int i = curX; i < newBmp.Width; i++)
                    newBmp.SetPixel(i, curY, gapFiller);
            else if (dir == Direction.west)
                for (int i = curX; i > -1; i--)
                    newBmp.SetPixel(i, curY, gapFiller);

            return newBmp;
        }

        public static string Decode(Bitmap bmp)
        {
            string code = "";
            Direction dir = Direction.east;
            int curX = 0, curY = 0;

            while ((curX >= 0 && curX < bmp.Width) && (curY >= 0 && curY < bmp.Height))
            {
                Color clr = bmp.GetPixel(curX, curY);

                if (clr.R == 255 && clr.G == 0 && clr.B == 0)
                    code += ">";
                if (clr.R == 128 && clr.G == 0 && clr.B == 0)
                    code += "<";
                if (clr.R == 0 && clr.G == 255 && clr.B == 0)
                    code += "+";
                if (clr.R == 0 && clr.G == 128 && clr.B == 0)
                    code += "-";
                if (clr.R == 0 && clr.G == 0 && clr.B == 255)
                    code += ".";
                if (clr.R == 0 && clr.G == 0 && clr.B == 128)
                    code += ",";
                if (clr.R == 255 && clr.G == 255 && clr.B == 0)
                    code += "[";
                if (clr.R == 128 && clr.G == 128 && clr.B == 0)
                    code += "]";

                if (clr.R == 0 && clr.G == 255 && clr.B == 255)
                {
                    switch (dir)
                    {
                        case Direction.east:
                            dir = Direction.south;
                            break;
                        case Direction.south:
                            dir = Direction.west;
                            break;
                        case Direction.west:
                            dir = Direction.north;
                            break;
                        case Direction.north:
                            dir = Direction.east;
                            break;
                        default:
                            break;
                    }
                }
                if (clr.R == 0 && clr.G == 128 && clr.B == 128)
                {
                    switch (dir)
                    {
                        case Direction.east:
                            dir = Direction.north;
                            break;
                        case Direction.south:
                            dir = Direction.east;
                            break;
                        case Direction.west:
                            dir = Direction.south;
                            break;
                        case Direction.north:
                            dir = Direction.west;
                            break;
                        default:
                            break;
                    }
                }

                switch (dir)
                {
                    case Direction.east:
                        curX++;
                        break;
                    case Direction.west:
                        curX--;
                        break;
                    case Direction.north:
                        curY--;
                        break;
                    case Direction.south:
                        curY++;
                        break;
                    default:
                        break;
                }
            }

            return code;
        }

        public static Bitmap Reduce(Bitmap bmp, int pxDimension)
        {
            Bitmap newBmp = new Bitmap(bmp.Width / pxDimension, bmp.Height / pxDimension);

            for (int row = 0; row < bmp.Height; row += pxDimension)
                for (int col = 0; col < bmp.Width; col += pxDimension)
                    newBmp.SetPixel(col / 10, row / 10, bmp.GetPixel(col, row));

            return newBmp;
        }

        public static Bitmap Enlarge(Bitmap bmp, int pxDimension)
        {
            Bitmap newBmp = new Bitmap(bmp.Width * pxDimension, bmp.Height * pxDimension);
            Graphics g = Graphics.FromImage(newBmp);

            for (int row = 0; row < bmp.Height; row++)
            {
                for (int col = 0; col < bmp.Width; col++)
                {
                    Rectangle rect = new Rectangle(col * pxDimension, row * pxDimension, pxDimension, pxDimension);
                    Pen pen = new Pen(bmp.GetPixel(col, row));
                    SolidBrush brush = new SolidBrush(bmp.GetPixel(col, row));
                    g.DrawRectangle(pen, rect);
                    g.FillRectangle(brush, rect);
                }
            }

            g.Dispose();

            return newBmp;
        }
    }
}
