using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class NeutralSpawn : Basic2DObject
    {
#if EDITOR && WINDOWS
        private static Texture2D SpawnIcon;
#endif
        private static Vector2 MaxOffset = new Vector2(100);

        public Vector2 Offset = -MaxOffset;

        public override void Create()
        {
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                if (SpawnIcon == null)
                    SpawnIcon = AssetManager.Load<Texture2D>("Textures/Shipgame/SpawnIcon2D");
                AddTag(GameObjectTag._2DForward);
            }
#endif
            base.Create();
        }

        public override void CreateInGame()
        {
            int max = NeutralManager.SpawnList.Count;
            NeutralManager.SpawnList.AddLast(this);
            base.CreateInGame();
        }

        public void UpdateOffset(Vector2 Amount)
        {
            Offset.X += Amount.X;
            if (Offset.X > MaxOffset.X)
            {
                Offset.Y += Amount.Y;
                Offset.X = -MaxOffset.X;

                if (Offset.Y > MaxOffset.Y)
                {
                    Offset.Y = -MaxOffset.Y;
                }
            }
        }

#if EDITOR && WINDOWS
        public override void Draw2D(GameObjectTag DrawTag)
        {
            Render.DrawSprite(SpawnIcon, Position, Size, Rotation);
            base.Draw2D(DrawTag);
        }
#endif
    }
}
