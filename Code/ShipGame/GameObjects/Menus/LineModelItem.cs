using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class LineModelItem
    {
        public List<Vector3> Points = new List<Vector3>();
        public List<Vector3> BuiltPoints = new List<Vector3>();
        public LineModelFlare[] Flares;
        public LineModelRandomFlare[] RandomFlares;
        public float Distance;

        public LineModelItem(Model model, int FlareCount, int RandomFlareCount)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];

                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 p = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                        Points.Add(p);
                        Distance += p.Length();
                    }
                }
            }

            Flares = new LineModelFlare[FlareCount];
            for (int i = 0; i < FlareCount; i++)
                Flares[i] = new LineModelFlare(this, Points.Count * i / FlareCount, Distance / 50);

            RandomFlares = new LineModelRandomFlare[RandomFlareCount];
            for (int i = 0; i < RandomFlareCount; i++)
                RandomFlares[i] = new LineModelRandomFlare(this, Distance / 50);


            Distance /= Points.Count;
        }

        public void Update(GameTime gameTime)
        {
            foreach (Vector3 v in BuiltPoints)
            {
                ParticleManager.CreateParticle(v, Vector3.Zero, Color.White, Distance / 20f, 1);
                ParticleManager.CreateParticle(v, Vector3.Zero, Color.White * 0.1f, Distance / 2f, 1);
            }
            for (int i = 0; i < Flares.Length; i++)
                Flares[i].Update(gameTime);
            for (int i = 0; i < RandomFlares.Length; i++)
                RandomFlares[i].Update(gameTime);
        }
    }
}
