using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace BadRabbit.Carrot
{
    struct FlareVertex
    {
        public Short2 Corner;
        public Vector3 Position;
        public Color ParticleColor;
        public float Size;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Short2,
                                 VertexElementUsage.Position, 0),

            new VertexElement(4, VertexElementFormat.Vector3,
                                 VertexElementUsage.Position, 1),

            new VertexElement(16, VertexElementFormat.Color,
                                  VertexElementUsage.Color, 0),

            new VertexElement(20, VertexElementFormat.Single,
                                  VertexElementUsage.TextureCoordinate, 0)
        );

        public const int SizeInBytes = 24;
    }
}
