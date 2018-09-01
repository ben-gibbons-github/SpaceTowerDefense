using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace BadRabbit.Carrot
{
    public abstract class FullscreenQuad
    {
        //Vertex Buffer
        static VertexBuffer vb;
        //Index Buffer
        static IndexBuffer ib;
        //Constructor
        static FullscreenQuad()
        {
            //Vertices
            VertexPositionTexture[] vertices =
            {
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
            };

            //Make Vertex Buffer
            vb = new VertexBuffer(Game1.graphicsDevice, VertexPositionTexture.VertexDeclaration,
              vertices.Length, BufferUsage.None);
            vb.SetData<VertexPositionTexture>(vertices);

            //Indices
            ushort[] indices = { 0, 1, 2, 2, 3, 0 };

            //Make Index Buffer
            ib = new IndexBuffer(Game1.graphicsDevice, IndexElementSize.SixteenBits,
            indices.Length, BufferUsage.None);
            ib.SetData<ushort>(indices);
        }
        //DrawAll and Set Buffers
        public static void Draw()
        {
#if EDITOR
            Render.DrawCalls++;
            Render.RenderTime.Continue();
#endif
            //Set Vertex Buffer
            Game1.graphicsDevice.SetVertexBuffer(vb);
            //Set Index Buffer
            Game1.graphicsDevice.Indices = ib;
            //DrawAll Quad
            Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

#if EDITOR
            Render.RenderTime.Stop();
#endif
        }
        //Set Buffers Onto GPU
        public static void ReadyBuffers(GraphicsDevice GraphicsDevice)
        {
            //Set Vertex Buffer
            GraphicsDevice.SetVertexBuffer(vb);
            //Set Index Buffer
            GraphicsDevice.Indices = ib;
        }    //DrawAll without S
        public static void JustDraw(GraphicsDevice GraphicsDevice)
        {
            //DrawAll Quad
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }
    }
}
