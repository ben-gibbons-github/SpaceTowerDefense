using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public class BasicMarker : Basic2DObject
    {
        public static SoundEffect MoveSound;
        public static float MoveVolume = 1;

        public static SoundEffect SelectSound;
        public static float SelectVolume = 1;

        static float AlphaChange = 0.1f;
        public static Texture2D FormBoxTexture;

        static BasicMarker()
        {
            FormBoxTexture = AssetManager.Load<Texture2D>("_Engine/HudOutlineBox");
        }

        public PlayerProfile MyPlayer;
        public LinkedList<GameObject> FormChildren;
        public BasicGameForm CurrentForm;

        public FloatValue MoveSpeed;
        public FloatValue ResizeSpeed;

        public bool SpecialColor = false;
        public Color MyColor;

        public int RestrictedView = -1;
        public bool Visible = true;

        protected FormFrame ParentFrame;
        protected bool FrameLess = true;

        protected float Alpha = 1;

        Vector2 DrawPosition;
        Vector2 DrawSize;

        public BasicMarker(PlayerProfile MyPlayer)
        {
            this.MyPlayer = MyPlayer;
        }

        public BasicMarker(PlayerProfile MyPlayer, Color MyColor)
        {
            this.MyPlayer = MyPlayer;
            this.MyColor = MyColor;
            this.SpecialColor = true;
        }

        public void AddToFrame(FormFrame f)
        {
            FormChildren = f.FormChildren;
            RestrictedView = f.RestrictedView;
            ParentFrame = f;
            FrameLess = false;

            if (f.FormChildren != null && f.FormChildren.First != null &&
                f.FormChildren.First.Value.GetType().IsSubclassOf(typeof(BasicGameForm)))
                SetCurrentForm((BasicGameForm)f.FormChildren.First.Value);
        }

        public void RemoveFromFrame(FormFrame f)
        {
            if (ParentFrame == f)
            {
                FormChildren = null;
                RestrictedView = -1;
                ParentFrame = null;
            }
        }

        public override void Create()
        {
            MoveSpeed = new FloatValue("Move Speed", 10);
            ResizeSpeed = new FloatValue("Resize Speed", 10);

            base.Create();

            AddTag(GameObjectTag.Update);
            AddTag(GameObjectTag._2DForward);
            FormChildren = ParentScene.GetList(GameObjectTag.Form);
        }

        public void SetCurrentForm(BasicGameForm Form)
        {
            if (this.CurrentForm != null)
                this.CurrentForm.MarkerLeave(this);

            this.CurrentForm = Form;

            if (Form != null)
                Form.MarkerEnter(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (FrameLess)
            {
                DrawPosition = Position.get();
                DrawSize = Size.get();
            }
            else if (ParentFrame != null)
            {
                Vector2 ProjectedSize = Size.get() * ParentFrame.FrameSize / ParentFrame.CameraZoom;
                DrawPosition = (Position.get() - ParentFrame.CameraPosition + ParentFrame.ScreenOffset) / ParentFrame.FrameSize * ParentFrame.CameraZoom + Render.CurrentView.Size / 2;
                DrawSize = Vector2.Normalize(Size.get()) * ProjectedSize.Length();
            }
            
            if (FormChildren != null)
            {
                Alpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (Alpha > 1)
                    Alpha = 1;

                if (CurrentForm != null)
                {
                    float MoveAmount = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * MoveSpeed.get();
                    float ReSizeAmount = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * ResizeSpeed.get();

                    if (Vector2.Distance(Position.get(), CurrentForm.Position.get()) < MoveAmount)
                        Position.set(CurrentForm.Position.get());
                    else
                        Position.add(Vector2.Normalize(CurrentForm.Position.get() - Position.get()) * MoveAmount);

                    if (Vector2.Distance(Size.get(), CurrentForm.Size.get()) < ReSizeAmount)
                        Size.set(CurrentForm.Size.get());
                    else
                        Size.add(Vector2.Normalize(CurrentForm.Size.get() - Size.get()) * ReSizeAmount);
                }
            }
            else
            {
                Alpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (Alpha < 0)
                    Alpha = 0;
            }

            base.Update(gameTime);
        }

        public virtual void DrawAsForm(Vector2 Position, Vector2 Size)
        {
            Color col = (SpecialColor ? MyColor : GameFormsManager.PlayerColors[MyPlayer.PlayerNumber]);
            Render.DrawSprite(FormBoxTexture, Position, Size * 1.2f, 0,
                col * (0.5f * Alpha));
            Render.DrawOutlineRect(Position - Size / 2, Position + Size / 2, 2,
                col * (1.5f * Alpha));
        }

        public virtual void TriggerCurrent()
        {
            if (FormChildren != null && CurrentForm != null)
                CurrentForm.TriggerAsCurrent(this);
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (!Visible || (RestrictedView != -1 && Render.ViewIndex - 1 != RestrictedView))
                return;

            /*
            if (FrameLess || ParentFrame == null)
                DrawAsForm(Position.get(), Size.get());
            else
            {
                Vector2 ProjectedSize = Size.get() * ParentFrame.FrameSize / ParentFrame.CameraZoom;
                Vector2 ProjectedPosition = (Position.get() - ParentFrame.CameraPosition + ParentFrame.ScreenOffset) / ParentFrame.FrameSize * ParentFrame.CameraZoom + Render.CurrentView.Size / 2;
                DrawAsForm(ProjectedPosition, Vector2.Normalize(Size.get()) * ProjectedSize.Length());
            }
            */
            DrawAsForm(DrawPosition, DrawSize);

            base.Draw2D(DrawTag);
        }
    }
}
