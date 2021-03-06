﻿using System;

namespace Bonsai.ONIX
{
    public abstract class DataBlock
    {
        protected int SamplesPerBlock;
        protected int index = 0;

        ulong[] frame_clock;
        ulong[] data_clock;

        public DataBlock(int samples_per_block)
        {
            SamplesPerBlock = samples_per_block;

            AllocateArray1D(ref frame_clock, samples_per_block);
            AllocateArray1D(ref data_clock, samples_per_block);
        }

        protected abstract void FillFromData(ushort[] data);

        public bool FillFromFrame(oni.Frame frame)
        {
            if (index >= SamplesPerBlock)
                throw new IndexOutOfRangeException();

            var data = frame.Data<ushort>();

            frame_clock[index] = frame.Clock();
            data_clock[index] = ((ulong)data[0] << 48) | ((ulong)data[1] << 32) | ((ulong)data[2] << 16) | ((ulong)data[3] << 0);

            FillFromData(data);

            return ++index == SamplesPerBlock;
        }

        // Allocates memory for a 1-D array of integers.
        protected void AllocateArray1D<T>(ref T[] array1D, int xSize)
        {
            Array.Resize(ref array1D, xSize);
        }

        // Allocates memory for a 2-D array of ushorts.
        protected void AllocateArray2D<T>(ref T[,] array2D, int xSize, int ySize)
        {
            array2D = new T[xSize, ySize];
        }

        /// <summary>
        /// Gets the array of 64-bit unsigned frame clock.
        /// </summary>
        public ulong[] FrameClock
        {
            get { return frame_clock; }
        }

        /// <summary>
        /// Gets the array of 64-bit unsigned local hardware clock.
        /// </summary>
        public ulong[] DataClock
        {
            get { return data_clock; }
        }
    }
}
