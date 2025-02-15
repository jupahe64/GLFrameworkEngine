﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLFrameworkEngine
{
    public class UniformBlock
    {
        public BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw;

        public List<byte> Buffer = new List<byte>();

        public int Size => Buffer.Count * sizeof(byte);

        private int ID;
        private int Index = -1;

        public UniformBlock()
        {
            GL.GenBuffers(1, out ID);
        }

        public UniformBlock(int index)
        {
            GL.GenBuffers(1, out ID);
            Index = index;
        }

        public void Add(byte[] value)
        {
            Buffer.AddRange(value);
        }

        public void Add(uint[] value)
        {
            for (int i = 0; i < value.Length; i++)
                Buffer.AddRange(BitConverter.GetBytes(value[i]));
        }

        public void Add(int[] value)
        {
            for (int i = 0; i < value.Length; i++)
                Buffer.AddRange(BitConverter.GetBytes(value[i]));
        }

        public void Add(float[] value)
        {
            for (int i = 0; i < value.Length; i++)
                AddFloat(value[i]);
        }

        public void Add(Vector2[] value)
        {
            for (int i = 0; i < value.Length; i++)
                Add(value[i]);
        }

        public void Add(Vector3[] value)
        {
            for (int i = 0; i < value.Length; i++)
                Add(value[i]);
        }

        public void Add(Vector4[] value)
        {
            for (int i = 0; i < value.Length; i++)
                Add(value[i]);
        }

        public void Add(float value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void AddFloat(float value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
            Buffer.AddRange(new byte[12]); //Padding
        }

        public void AddInt(int value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
            Buffer.AddRange(new byte[12]); //Padding
        }

        public void Add(Vector2 value)
        {
            Add(value.X);
            Add(value.Y);
        }

        public void Add(Vector3 value)
        {
            Add(value.X);
            Add(value.Y);
            Add(value.Z);
            Buffer.AddRange(new byte[4]); //Buffer aligned so make sure it's 16 bytes size
        }

        public void Add(Vector4 value)
        {
            Add(value.X);
            Add(value.Y);
            Add(value.Z);
            Add(value.W);
        }

        public void Bind()
        {
            if (ID == -1)
                return;

            GL.BindBuffer(BufferTarget.UniformBuffer, ID);
        }

        public void RenderBuffer(int programID, string name, int binding = -1)
        {
            var index = GL.GetUniformBlockIndex(programID, name);
            RenderBuffer(programID, index, binding);
        }

        public void RenderBuffer(int programID, int index, int binding = -1)
        {
            if (index == -1)
                return;

            binding = binding != -1 ? binding : index;

            GL.UniformBlockBinding(programID, index, binding);
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, binding, ID);

            UpdateBufferData();
        }

        public void BindUniformBlock(int programID)
        {
            if (Index == -1)
                return;

            int binding = Index;

            GL.UniformBlockBinding(programID, Index, binding);
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, binding, ID);
        }   

        public void UpdateBufferData()
        {
            if (ID == -1)
                return;

            //Bind the data
            var buffer = Buffer.ToArray();

            Bind();
            GL.BufferData(BufferTarget.UniformBuffer, buffer.Length, buffer, BufferUsageHint);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(ID);
            Buffer.Clear();
        }
    }
}
