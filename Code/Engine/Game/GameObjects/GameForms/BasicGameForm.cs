using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public delegate bool EventTrigger(BasicGameForm form, BasicMarker trigger);

    public class BasicGameForm : Basic2DObject
    {
        BasicMarker[] Markers = new BasicMarker[4];
        protected int MarkerCount = 0;

        public int RestrictedView = -1;
        protected FormFrame ParentFrame;
        protected FormSection ParentSection;
        protected bool FrameLess = true;

        public float Alpha = 1;
        protected float AlphaChange = 0.05f;
        protected float MinAlpha = 0.4f;
        protected float MaxAlpha = 1;

        protected float OffsetPosition;
        protected float OffsetPositionMult = 1;
        protected float MoveSpeed = 25;

        public BoolValue StartingForm;
        public BoolValue Active;

        Vector2 DrawPosition;
        Vector2 DrawSize;

        public override void Create()
        {
            StartingForm = new BoolValue("Starting Form", false);

            base.Create();
            AddTag(GameObjectTag.Form);
            AddTag(GameObjectTag._2DForward);
            AddTag(GameObjectTag.Update);
        }

        public virtual void SetValues(Vector2 Position, string Text, float Min, float Max, FloatValue SliderValue)
        {
            this.Position.set(Position);
        }

        public virtual void SetValues(Vector2 Position, string Text, EventTrigger Event)
        {
            this.Position.set(Position);
        }

        public virtual void SetValues(Vector2 Position, string Text, string TargetMenu)
        {
            this.Position.set(Position);
        }

        public virtual void AddToFrame(FormFrame f)
        {
            RestrictedView = f.RestrictedView;
            ParentFrame = f;
            FrameLess = false;
        }

        public virtual void RemoveFromFrame(FormFrame f)
        {
            if (ParentFrame == f)
            {
                ParentFrame = null;
                MarkerCount = 0;
                for (int i = 0; i < 4; i++)
                    Markers[i] = null;
            }
        }

#if EDITOR && WINDOWS
        public override void UpdateEditor(GameTime gameTime)
        {
            Update(gameTime);
            base.UpdateEditor(gameTime);
        }
#endif

        public void SetSection(FormSection s)
        {
            ParentSection = s;
        }

        public override void Update(GameTime gameTime)
        {
            if (FrameLess)
            {
                if (ParentSection == null)
                {
                    DrawPosition = Position.get();
                    DrawSize = Size.get();
                }
                else
                {
                    DrawPosition = (Position.get() - ParentSection.Position.get()) * ParentSection.SizeMult + 
                        ParentSection.Position.get();
                    DrawSize = Size.get() * ParentSection.Size.get();
                }
            }
            else if (ParentFrame != null)
            {
                OffsetPosition -= MoveSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                if (OffsetPosition < 0)
                    OffsetPosition = 0;

                Vector2 ProjectedSize = Size.get() / ParentFrame.FrameSize / ParentFrame.CameraZoom;
                DrawPosition = (Position.get() + new Vector2(-OffsetPosition, 0) - ParentFrame.CameraPosition + 
                    ParentFrame.ScreenOffset) / ParentFrame.FrameSize * ParentFrame.CameraZoom +
                    new Vector2(Game1.ResolutionX, Game1.ResolutionY) / ParentFrame.FrameSize / 2;
                DrawSize = Vector2.Normalize(Size.get()) * ProjectedSize.Length();
            }
            else
            {
                OffsetPosition = OffsetPositionMult * Position.X();
            }

            float TargetAlpha = FrameLess || ParentFrame != null ? (MarkerCount > 0 ? MaxAlpha : MinAlpha) : 0;
            float AlphaChangeAmount = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;

            if (Math.Abs(Alpha - TargetAlpha) > AlphaChangeAmount)
            {
                if (Alpha > TargetAlpha)
                    Alpha -= AlphaChangeAmount;
                else
                    Alpha += AlphaChangeAmount;
            }
            else
                Alpha = TargetAlpha;

            base.Update(gameTime);
        }

        public virtual void MarkerEnter(BasicMarker Marker)
        {
            for (int i = 0; i < 4; i++)
                if (Marker == Markers[i])
                    return;
            
            Markers[MarkerCount] = Marker;
            MarkerCount++;
        }

        public virtual void MarkerLeave(BasicMarker Marker)
        {
            for (int i = 0; i < MarkerCount; i++)
            {
                if (Markers[i] == Marker)
                {
                    BasicMarker temp = Markers[MarkerCount - 1];
                    Markers[MarkerCount - 1] = Markers[i];
                    Markers[i] = temp;
                    MarkerCount--;
                    Markers[MarkerCount] = null;
                    return;
                }
            }
        }

        public virtual bool MarkerMove(Vector2 MoveAmount)
        {
            return true;
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (Alpha < 0.1f || (RestrictedView != -1 && Render.ViewIndex - 1 != RestrictedView))
                return;

            DrawAsForm(DrawPosition, DrawSize);

            base.Draw2D(DrawTag);
        }

        public virtual void DrawAsForm(Vector2 Position, Vector2 Size)
        {

        }

        public virtual bool TriggerAsCurrent(BasicMarker m)
        {
            return false;
        }
    }
}
