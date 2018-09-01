using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class TriggerBox : Basic3DObject
    {
        private static Vector4 drawColor = new Vector4(1, 1, 0, 1);

        public EventValue MyEvent;
        public ObjectValue TriggeringObject;
        public TypeValue TriggeringType;
        public BoolValue Used;
        public BoolValue AllowReset;
        public IntValue ResetDelay;
        private int ResetTime = 0;

        public override void Create()
        {
            MyEvent = new EventValue("Event");
            TriggeringObject = new ObjectValue("Trigger Object", typeof(Basic3DObject));
            TriggeringType = new TypeValue("Trigger Type", typeof(Basic3DObject));
            SetCollisionShape(new OrientedBoxShape());
            Used = new BoolValue("Used");
            AllowReset = new BoolValue("Allow Reset");
            ResetDelay = new IntValue("Reset Delay", 1000);

            base.Create();
            AddTag(GameObjectTag.Update);

#if EDITOR
            if (ParentLevel.LevelForEditing)
            {
                AddTag(GameObjectTag._3DForward);
                ShapeRenderer.Load();
                ApplyScale(Vector3.One * 200, Vector3.Zero, false);
            }
#endif
        }

        public override void Update(GameTime gameTime)
        {
            TestTrigger(gameTime);
            base.Update(gameTime);
        }

        private void TestTrigger(GameTime gameTime)
        {
            if (Used.get() && !AllowReset.get())
                return;
            if (TriggeringObject.get() != null && TestCollision((Basic3DObject)TriggeringObject.get()))
            {
                if (!Used.get())
                {
                    MyEvent.Trigger();
                    Used.set(true);
                }
                return;
            }

            if (Used.get())
            {
                ResetTime += gameTime.ElapsedGameTime.Milliseconds;
                if (ResetTime > ResetDelay.get())
                {
                    ResetTime = 0;
                    Used.set(false);
                }
            }
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            ShapeRenderer.DrawCube(WorldMatrix, camera, drawColor);
            base.Draw3D(camera, DrawTag);
        }
    }
}
