using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CloakingField : BasicField
    {
        public CloakingField()
            : base(-1)
        {

        }

        public override void Create()
        {
            fieldState = FieldState.Cloaked;
            ShipEffectTime = 5000;

            base.Create();
            Size.set(new Vector2(200, 200));
            CircleColor = new Color(0.2f, 0.2f, 1);
        }

        public override void EffectUnit(UnitBasic s, GameTime gameTime)
        {
            base.EffectUnit(s, gameTime);
        }
    }
}
