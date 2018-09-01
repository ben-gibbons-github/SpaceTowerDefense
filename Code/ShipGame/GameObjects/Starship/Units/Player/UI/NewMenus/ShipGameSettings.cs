using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ShipGameSettings : GameObject
    {
        public static FloatValue BrightnessValue;
        public static FloatValue ContrastValue;

        public override void Create()
        {
            BrightnessValue = new FloatValue("Brightness", 5);
            ContrastValue = new FloatValue("Contrast", 5);

            BrightnessValue.ChangeEvent = BrightnessChange;
            ContrastValue.ChangeEvent = ContrastChange;

            base.Create();
        }

        void BrightnessChange()
        {
            BloomRenderer.SetIntensity(BrightnessValue.get());
        }

        void ContrastChange()
        {
            BloomRenderer.SetSaturation(ContrastValue.get());
        }
    }
}
