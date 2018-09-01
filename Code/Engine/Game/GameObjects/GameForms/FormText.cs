using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class FormText : BasicGameForm
    {
        public SpriteFontValue Font;
        public StringValue Text;
        public ColorValue MyColor;
        public FloatValue SizeMult;

        public override void Create()
        {
            Font = new SpriteFontValue("Font", "Fonts/FormTextFont");
            Text = new StringValue("Text", "Text");
            MyColor = new ColorValue("Color", Vector4.One);
            SizeMult = new FloatValue("Size Mult", 1.5f);

            Text.ChangeEvent = TextChange;
            Font.ChangeEvent = TextChange;

            base.Create();
        }

        void TextChange()
        {
            if (Size != null && Font.get() != null)
                Size.set(Font.get().MeasureString(Text.get()) * SizeMult.get());
        }

        public override void DrawAsForm(Vector2 Position, Vector2 Size)
        {
            Render.DrawShadowedText(Font.get(), Text.get(), Position - Size / 2,
                Vector2.Zero, MyColor.getAsColor() * Alpha, Color.Black * Alpha);

            base.DrawAsForm(Position, Size);
        }
    }
}
