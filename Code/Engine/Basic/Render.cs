using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Render
    {
        public static SceneView CurrentView;
        public static int ViewIndex;
        public static WorldViewer3D CurrentWorldViewer3D;
        public static int ViewWidth;
        public static int ViewHeight;
        public static Texture2D BlankTexture;
        public static Texture2D TransparentTexture;
        public static SpriteFont BasicFont;
        public static bool AdditiveBlending = false;

        public static _3DEffect WhiteEffectHolder;
#if EDITOR
        public static int DrawCalls = 0;
        public static int EffectUpdateCalls = 0;
        public static StopwatchWrapper RenderTime;
#endif

        public static void Load()
        {
#if EDITOR
            RenderTime = new StopwatchWrapper("RenderTime",false);
#endif
            BasicFont = Game1.content.Load<SpriteFont>("Fonts/BasicFont");
            BlankTexture = Game1.content.Load<Texture2D>("Textures/blank");
            TransparentTexture = Game1.content.Load<Texture2D>("Editor/Transparent");

            WhiteEffectHolder = (_3DEffect)new _3DEffect().Create("Effects/WhiteEffect");
        }

        public static void DrawShadowedText(string Text, Vector2 Position)
        {
            DrawShadowedText(BasicFont, Text, Position, Vector2.One);
        }

        public static void DrawShadowedText(string Text, Vector2 Position, Color color)
        {
            DrawShadowedText(BasicFont, Text, Position, Vector2.One, color, Color.Black * (color.A / 255f));
        }

        public static void DrawShadowedText(SpriteFont Font, string Text, Vector2 Position, Vector2 ShadowOffset)
        {
            Game1.spriteBatch.DrawString(Font, Text, Position + ShadowOffset, Color.Black);
            Game1.spriteBatch.DrawString(Font, Text, Position, Color.White);
        }

        public static void DrawShadowedText(SpriteFont Font, string Text, Vector2 Position, Vector2 ShadowOffset, Color color, Color ShadowColor)
        {
            Game1.spriteBatch.DrawString(Font, Text, Position + ShadowOffset, ShadowColor);
            Game1.spriteBatch.DrawString(Font, Text, Position, color);
        }

        public static void DrawLine(Vector2 StartPos, Vector2 EndPos, Color color)
        {
            Game1.spriteBatch.Draw(
                BlankTexture,
                StartPos,
                null,
                color,
                (float)Math.Atan2(StartPos.Y - EndPos.Y, StartPos.X - EndPos.X) - (float)Math.PI,
                Vector2.Zero,
                new Vector2(Vector2.Distance(StartPos, EndPos), 1),
                SpriteEffects.None,
                0);
        }

        public static void DrawLine(Vector2 StartPos, Vector2 EndPos, Color color, float Width)
        {
            Game1.spriteBatch.Draw(
                BlankTexture,
                StartPos,
                null,
                color,
                (float)Math.Atan2(StartPos.Y - EndPos.Y, StartPos.X - EndPos.X) - (float)Math.PI,
                Vector2.Zero,
                new Vector2(Vector2.Distance(StartPos, EndPos), Width),
                SpriteEffects.None,
                0);
        }

        public static void DrawSquare(Vector2 StartPos, Vector2 EndPos, int Width, Texture2D Texture, Color color)
        {
            Game1.spriteBatch.Draw(
                Texture,
                new Rectangle((int)StartPos.X, (int)StartPos.Y, (int)Vector2.Distance(StartPos, EndPos), Width),
                null,
                color,
                (float)Math.Atan2(StartPos.Y - EndPos.Y, StartPos.X - EndPos.X) - (float)Math.PI,
                new Vector2(0, Texture.Height / 2),
                SpriteEffects.None,
                0
                );
        }


        public static void DrawSprite(Texture2D tex, Vector2Value Position, Vector2Value Size, FloatValue Rotation)
        {
            DrawSprite(tex, Position.get(), Size.get(), Rotation.getAsRadians());
        }

        public static void DrawSprite(Texture2D tex, Vector2Value Position, Vector2Value Size, FloatValue Rotation, Color color)
        {
            DrawSprite(tex, Position.get(), Size.get(), Rotation.getAsRadians(), color);
        }

        public static void DrawBar(Vector2 Pos1, Vector2 Pos2, float Mult, Color BackColor, Color InnerColor)
        {
            DrawSolidRect(Pos1, Pos2, BackColor);
            DrawSolidRect(Pos1 + Vector2.One, new Vector2(Pos1.X + (Pos2.X - Pos1.X) * Mult - 2, Pos2.Y - 1), InnerColor);
        }

        public static void DrawSprite(Texture2D tex, Vector2 Position, Vector2 Size, float Rotation)
        {
//#if (EDITOR && WINDOWS) || AUTO
            if (tex == null)
                tex = BlankTexture;
//#endif
            Vector2 TextureSize = new Vector2(tex.Width, tex.Height);
            Game1.spriteBatch.Draw(tex, Position, null, Color.White, Rotation, TextureSize / 2, Size / TextureSize, SpriteEffects.None, 0);
        }

        public static void DrawSprite(Texture2D tex, Vector2 Position, Vector2 Size, float Rotation,Color color)
        {
//#if (EDITOR && WINDOWS) || AUTO
            if (tex == null)
                tex = BlankTexture;
//#endif
            Vector2 TextureSize = new Vector2(tex.Width, tex.Height);
            Game1.spriteBatch.Draw(tex, Position, null, color, Rotation, TextureSize / 2, Size / TextureSize, SpriteEffects.None, 0);
        }

        public static void DrawLine(int x1, int y1, int x2, int y2, Color color, float width)
        {
            Game1.spriteBatch.Draw(BlankTexture, new Vector2(x1, y1), null, color, (float)Math.Atan2(y1 - y2, x1 - x2) - (float)Math.PI,
                Vector2.Zero, new Vector2((float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)), width), SpriteEffects.None, 0);
        }

        public static void DrawSolidRect(Vector2 Pos1, Vector2 Pos2, Color color)
        {
            Game1.spriteBatch.Draw(BlankTexture, Pos1, null, color, 0, Vector2.Zero, Pos2 - Pos1, SpriteEffects.None, 0);
        }

        public static void DrawSolidRect(int x1, int y1, int x2, int y2, Color color)
        {
            Game1.spriteBatch.Draw(BlankTexture,
                new Rectangle(x1, y1, x2 - x1, y2 - y1)
                , color);
        }

        public static void DrawSolidRect(Rectangle rectangle, Color color)
        {
            Game1.spriteBatch.Draw(BlankTexture, rectangle, color);
        }

        public static void DrawOutlineRect(Rectangle rectangle, int LineWidth, Color color)
        {
            DrawSolidRect(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + LineWidth, color);
            DrawSolidRect(rectangle.X, rectangle.Y + rectangle.Height, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height + LineWidth, color);
            DrawSolidRect(rectangle.X - LineWidth, rectangle.Y, rectangle.X, rectangle.Y + rectangle.Height, color);
            DrawSolidRect(rectangle.X + rectangle.Width - LineWidth, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, color);
        }

        public static void DrawOutlineRect(Vector2 Pos1, Vector2 Pos2, float LineWidth, Color color)
        {
            DrawSolidRect(Pos1, new Vector2(Pos2.X, Pos1.Y + LineWidth), color);
            DrawSolidRect(Pos1, new Vector2(Pos1.X + LineWidth, Pos2.Y), color);
            DrawSolidRect(new Vector2(Pos1.X, Pos2.Y), new Vector2(Pos2.X + LineWidth / 2, Pos2.Y + LineWidth), color);
            DrawSolidRect(new Vector2(Pos2.X, Pos1.Y), new Vector2(Pos2.X + LineWidth, Pos2.Y + LineWidth / 2), color);
        }

        public static void DrawOutlineRect(Rectangle rectangle, float LineWidth, Color color)
        {
            DrawLine(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y, color, LineWidth);
            DrawLine(rectangle.X, rectangle.Y + rectangle.Height, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, color, LineWidth);
            DrawLine(rectangle.X, rectangle.Y, rectangle.X, rectangle.Y + rectangle.Height, color, LineWidth);
            DrawLine(rectangle.X + rectangle.Width, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, color, LineWidth);
        }

        public static void DrawModel(Model model, Effect effect)
        {
#if EDITOR
            RenderTime.Continue();
            DrawCalls++;
#endif
            if (model != null)
                foreach (ModelMesh mesh in model.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Game1.graphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
                        Game1.graphicsDevice.Indices = part.IndexBuffer;
                        effect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
#if EDITOR
            RenderTime.Stop();
#endif
        }

        public static void DrawModel(Model model, EffectValue effect, Camera3D camera, Basic3DObject obj)
        {
            if (model != null && effect.get() != null)
            {
                _3DEffect effect3D = (_3DEffect)effect.Holder;
                effect3D.SetFromObject(obj);
                effect3D.SetFromCamera(camera);
                DrawModel(model, effect3D.MyEffect);
            }
        }

        public static void DrawModel(ModelValue model, EffectValue effect, Camera3D camera, Basic3DObject obj)
        {
            DrawModel(model.get(), effect, camera, obj);
        }

    }
}
