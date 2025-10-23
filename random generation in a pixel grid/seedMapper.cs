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
        public readonly int height;
        public readonly int width;
        public int GetValue(int x, int y) => _values[x, y];
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
        public void draw(SpriteBatch spritebatch, Point Position, int pixelWidth, int pixelHeight, Texture2D texture, Color[] colors)
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
        public void ApplySeaLevel(int seaLevel)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_heights[x, y] < seaLevel)
                    {
                        _values[x, y] = 0;
                    }
                    else
                    {
                        _values[x, y] = 1;
                    }
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
        // Cubic Bezier curve function
        (int x, int y) CubicBezier(float t,
            float x0, float x1, float x2, float x3,
            float y0, float y1, float y2, float y3
            )
        {
            double oneMinusT = 1 - t;

            int x = (int)(oneMinusT * (oneMinusT * (oneMinusT * x0 + t * x1)
                                    + t * (oneMinusT * x1 + t * x2))
                     + t * (oneMinusT * (oneMinusT * x1 + t * x2)
                                    + t * (oneMinusT * x2 + t * x3)));

            int y = (int)(oneMinusT * (oneMinusT * (oneMinusT * y0 + t * y1)
                                    + t * (oneMinusT * y1 + t * y2))
                     + t * (oneMinusT * (oneMinusT * y1 + t * y2)
                                    + t * (oneMinusT * y2 + t * y3)));

            return (x, y);
        }
        public void BezierSmoother(int Radius,
            float x0, float x1, float x2, float x3,
            float y0, float y1, float y2, float y3
            )
        {
            Random rnd = new Random();
            for (float t = 0; t <= 1; t += 0.01f)
            {
                var (x, y) = CubicBezier(t, x0, x1, x2, x3, y0, y1, y2, y3);
                CreateMound(x, y, rnd.Next(1, 20));
                //make it so it doesent make a mound if it was too close to the last point
            }
        }
        public void CreateMound(int posX, int posY, int Radius)
        {
            int MoundPeak = (int)Math.Pow(Radius * Radius + Radius * Radius, 0.4f);

            for (int x = posX - Radius; x < posX + Radius; x++)
            {
                for (int y = posY - Radius; y < posY + Radius; y++)
                {
                    if (x > 0 && y > 0 && x < width && y < height)
                    {
                        float num = x - posX;
                        float num2 = y - posY;
                        int AdditionalHeight = MoundPeak-(int)Math.Pow(num * num + num2 * num2, 0.4f);
                        _heights[x, y] += AdditionalHeight;
                    }
                }
            }
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
