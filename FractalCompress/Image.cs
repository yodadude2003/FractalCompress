﻿/*
 *  Copyright (C) 2020 Chosen Few Software
 *  This file is part of FractalCompress.
 *
 *  FractalCompress is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  FractalCompress is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with FractalCompress.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;

namespace FractalCompress
{
    public class Image
    {
        public float[,] Data { get; }

        public int Width => Data.GetLength(1);
        public int Height => Data.GetLength(0);

        public Image(int height, int width)
        {
            Data = new float[height, width];
        }

        public Image(float[,] data)
        {
            Data = data;
        }

        public Image Copy()
        {
            float[,] data = new float[Data.GetLength(0), Data.GetLength(1)];
            Array.Copy(Data, data, Data.Length);
            return new Image(data);
        }

        public Image Reduce(int factor)
        {
            Image newImage = new Image(Height / factor, Width / factor);
            for (int y = 0; y < newImage.Height; y++)
            {
                for (int x = 0; x < newImage.Width; x++)
                {
                    newImage.Data[y, x] = Data[y * factor, x * factor];
                }
            }
            return newImage;
        }

        public Image Reflect(Reflection reflection)
        {
            switch (reflection)
            {
                case Reflection.HorizontalReflection:
                    {
                        Image newImage = new Image(Height, Width);
                        for (int y = 0; y < Height; y++)
                        {
                            for (int x = 0; x < Width; x++)
                            {
                                newImage.Data[y, Width - (x + 1)] = Data[y, x];
                            }
                        }
                        return newImage;
                    }
                default:
                    return Copy();
            }
        }

        public Image Rotate(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.PosQuarterRotation:
                    {
                        Image newImage = new Image(Height, Width);
                        for (int y = 0; y < Height; y++)
                        {
                            for (int x = 0; x < Width; x++)
                            {
                                newImage.Data[x, Height - (y + 1)] = Data[y, x];
                            }
                        }
                        return newImage;
                    }
                case Rotation.HalfRotation:
                    {
                        Image newImage = new Image(Height, Width);
                        for (int y = 0; y < Height; y++)
                        {
                            for (int x = 0; x < Width; x++)
                            {
                                newImage.Data[Height - (y + 1), Width - (x + 1)] = Data[y, x];
                            }
                        }
                        return newImage;
                    }
                case Rotation.NegQuarterRotation:
                    {
                        Image newImage = new Image(Height, Width);
                        for (int y = 0; y < Height; y++)
                        {
                            for (int x = 0; x < Width; x++)
                            {
                                newImage.Data[Width - (x + 1), y] = Data[y, x];
                            }
                        }
                        return newImage;
                    }
                default:
                    return Copy();
            }
        }

        public Image ApplyTransform(Transform transform)
        {
            return Reflect(transform.Reflection)
                .Rotate(transform.Rotation);
        }

        public Image Multiply(float s)
        {
            Image newImage = Copy();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    newImage.Data[y, x] *= s;
                }
            }
            return newImage;
        }

        public Image Add(float b)
        {
            Image newImage = Copy();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    newImage.Data[y, x] += b;
                }
            }
            return newImage;
        }

        public Image Add(Image other)
        {
            Image newImage = Copy();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    newImage.Data[y, x] += other.Data[y, x];
                }
            }
            return newImage;
        }

        public Image Multiply(Image other)
        {
            Image newImage = Copy();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    newImage.Data[y, x] *= other.Data[y, x];
                }
            }
            return newImage;
        }

        public Image SubImage(int startX, int startY, int endX, int endY)
        {
            Image newImage = new Image(endY - startY, endX - startX);
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    newImage.Data[y - startY, x - startX] = Data[y, x];
                }
            }
            return newImage;
        }

        public Image[,] ExtractBlocksOfSize(int blockSize)
        {
            int numBlocksY = Height / blockSize;
            int numBlocksX = Width / blockSize;
            Image[,] blocks = new Image[numBlocksY, numBlocksX];
            for (int y = 0; y < numBlocksY; y++)
            {
                for (int x = 0; x < numBlocksX; x++)
                {
                    blocks[y, x] = SubImage(x * blockSize, y * blockSize, (x + 1) * blockSize, (y + 1) * blockSize);
                }
            }
            return blocks;
        }

        public void Insert(int startX, int startY, Image other)
        {
            int endX = startX + other.Width;
            int endY = startY + other.Height;

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    Data[y, x] = other.Data[y - startY, x - startX];
                }
            }
        }

        public float DistanceTo(Image other)
        {
            float sum = 0.0f;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float diff = Data[y, x] - other.Data[y, x];
                    sum += diff * diff;
                }
            }
            return sum;
        }

        public float Average()
        {
            float sum = 0.0f;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    sum += Data[y, x];
            return sum / Data.Length;
        }

        public static Image Random(int height, int width)
        {
            Random rand = new Random();
            Image newImage = new Image(height, width);
            for (int y = 0; y < newImage.Height; y++)
                for (int x = 0; x < newImage.Width; x++)
                    newImage.Data[y, x] = (float)rand.NextDouble();
            return newImage;
        }
    }
}
