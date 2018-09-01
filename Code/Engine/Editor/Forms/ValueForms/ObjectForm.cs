#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot.ValueForms
{
    public class ObjectForm : ValueForm
    {
        public TextField ValueField;
        public GameObject Value;
        public Type ObjectType;
        public bool NoValue = false;

        public ObjectForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            ValueField = (TextField)AddForm(new TextField(Vector2.Zero, (int)Font.MeasureString("").X, Font,"", GetValueFromField));
            ValueField.DraggedEvent = DragObject;

            GetValueFromReferences();
        }

        private void DragObject(GameObject g)
        {
            if (g != null && (g.GetType().Equals(ObjectType) || g.GetType().IsSubclassOf(ObjectType)))
            {
                foreach (ObjectValue val in ReferenceValues)
                    val.set(g);
                GetValueFromReferences();
            }
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            ValueField.BorderColor = FormFormat.BorderColor;

            if (MouseManager.DraggedObject != null && MouseManager.DraggedObject.GetType().IsSubclassOf(typeof(GameObject)))
            {
                GameObject g = (GameObject)MouseManager.DraggedObject;
                if (g != null && (g.GetType().Equals(ObjectType) || g.GetType().IsSubclassOf(ObjectType)))
                    if (ValueField.MyRectangle.Contains(Updater.RelativeMousePoint))
                        ValueField.BorderColor = Color.White;
            }
            base.Update(gameTime, Updater);
        }

        public override void Create(FormHolder Parent)
        {
            SetSize(ValueField.Size + new Vector2(Font.MeasureString(Name).X, 0));

            base.Create(Parent);
        }

        public override void SetPosition(Vector2 Position)
        {
            ValueField.SetPosition(Position + new Vector2(Font.MeasureString(Name.ToString()).X + Buffer, 0));
            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            foreach (ObjectValue val in ReferenceValues)
            {
                if (val == ReferenceValues.First.Value)
                {
                    Name = val.Name;
                    Value = val.get();
                    ObjectType = val.getObjectType();
                }
                else if (val.get() != Value)
                    NoValue = true;
            }

            ValueField.SetText(NoValue || Value == null ? "" : Value.Name.get());
        }

        public override void GetValueFromField()
        {

            BadRabbit.Carrot.Value.ChangeFromForm = true;

            if (ReferenceValues.First.Value.Parent.ParentScene != null)
                Value = ReferenceValues.First.Value.Parent.ParentScene.FindObject(ValueField.Text, ObjectType);
            if (Value == null)
                Value = GameManager.GetLevel().FindObject(ValueField.Text, ObjectType);
            if (Value != null)
            {
                NoValue = false;

                foreach (ObjectValue val in ReferenceValues)
                    val.set(Value);
            }
            else if (ValueField.Text.Equals(""))
            {
                NoValue = false;
                foreach (ObjectValue val in ReferenceValues)
                    val.clear();
            }
            else
                NoValue = true;


            BadRabbit.Carrot.Value.ChangeFromForm = false;

            base.GetValueFromField();
        }


        public override void Draw()
        {
            Game1.spriteBatch.DrawString(Font, Name, Position, TextColor);
            base.Draw();
        }

    }
}
#endif