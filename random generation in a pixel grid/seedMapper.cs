using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace random_generation_in_a_pixel_grid
{
    internal class seedMapper
    {
        int[,] _values;
        int[,] _heights;
        WeightedRandom random;
        int height;
        int width;
        public seedMapper(int width, int height, int[] weights, int maxHeight, int? seed = null)
        {
            this.height = height;
            this.width = width;

            Random pureRandom;
            if (seed != null)
            {
                this.random = new WeightedRandom(weights, seed);
                pureRandom = new Random((int)seed);

            }
            else
            {
                this.random = new WeightedRandom(weights);
                pureRandom = new Random();
            }
            _values = new int[width, height];
            _heights = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _values[x, y] = random.Next();
                    _heights[x, y] = pureRandom.Next(maxHeight);
                }
            }


        }
        public unsafe void draw(SpriteBatch spritebatch, Point Position, int pixelWidth, int pixelHeight, Texture2D texture, Color[] colors)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = colors[_values[x, y]];
                    color = Color.Lerp(color, Color.White, _heights[x, y] / 50f);
                    spritebatch.Begin();
                    spritebatch.Draw(
                        texture,
                        new Rectangle(Position.X + pixelWidth * x, Position.Y + pixelHeight * y, pixelWidth, pixelHeight),
                        color
                        );
                    spritebatch.End();
                }
            }
        }
        private int CountLandNeighbhors(int x, int y)
        {
            int count = 0;
            int xChecking;
            int yChecking;

            int xMin = Math.Min(x - 1, 0);
            int yMin = Math.Min(y - 1, 0);
            int xMax = Math.Min(x + 1, width);
            int yMax = Math.Min(y + 1, height);

            for (int xoffset = -1; xoffset < 2; xoffset++)
            {
                for (int yoffset = -1; yoffset < 2; yoffset++)
                {
                    xChecking = x + xoffset;
                    yChecking = y + yoffset;
                    if (yChecking > 0 && xChecking > 0 && yChecking < height && xChecking < width && _values[xChecking, yChecking] != 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public void SmoothenHeights(int Level)
        {
            int[,] newHeights = new int[width, height];
            int xChecking;
            int yChecking;
            int LowRange = -Level;
            int HighRange = Level + 1;
            int amountPerCheck = Level * 2 + 1;
            int average = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_values[x, y] == 0)
                    {
                        _heights[x, y] = 0;
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    average = 0;
                    for (int xOffset = LowRange; xOffset < HighRange; xOffset++)
                    {
                        xChecking = x + xOffset;
                        if (xChecking > 0 && xChecking < width)
                        {
                            average += _heights[xChecking, y];
                        }
                    }
                    average /= amountPerCheck;
                    newHeights[x, y] = average;
                }
            }
            _heights = newHeights;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    average = 0;
                    for (int yOffset = LowRange; yOffset < HighRange; yOffset++)
                    {
                        yChecking = y + yOffset;
                        if (yChecking > 0 && yChecking < height)
                        {
                            average += _heights[x, yChecking];
                        }
                    }
                    average /= amountPerCheck;
                    newHeights[x, y] = average;
                }
            }
            _heights = newHeights;
        }
        public void SmoothenTerrain()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (CountLandNeighbhors(x, y) > 4)
                    {
                        _values[x, y] = 1;
                    }
                    else
                    {
                        _values[x, y] = 0;
                    }
                }
            }
        }
    }
}
