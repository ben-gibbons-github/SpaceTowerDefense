using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerMarker : BasicMarker
    {
        public bool UseRTrig = true;
        public bool UseLTrig = true;

        public PlayerMarker(PlayerProfile MyPlayer)
            : base(MyPlayer)
        {

        }

        public PlayerMarker(PlayerProfile MyPlayer, Color MyColor)
            : base(MyPlayer, MyColor)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (FormChildren != null)
            {
                Vector2 MStick = MyPlayer.MyController.MenuStick(UseLTrig, UseRTrig, true, false, false);

                if (MStick.Length() > 0.1f)
                {
                    if (CurrentForm == null || CurrentForm.MarkerMove(MStick))
                    {
                        if (Math.Abs(MStick.Y) < Math.Abs(MStick.X))
                        {
                            if (MStick.X < 0)
                                MoveMark((float)Math.PI * 1.5f);
                            else
                                MoveMark((float)Math.PI * 0.5f);
                        }
                        else
                        {
                            if (MStick.Y > 0)
                                MoveMark(0);
                            else
                                MoveMark((float)Math.PI);
                        }
                    }
                }

                if (MyPlayer.MyController.AButton() && !MyPlayer.MyController.AButtonPrevious())
                    TriggerCurrent();
            }

            base.Update(gameTime);
        }

        private void MoveMark(float Theta)
        {
            if (BasicMarker.MoveSound != null)
                BasicMarker.MoveSound.Play(BasicMarker.MoveVolume, 0, 0);

            float BestDistance = 100000;

            BasicGameForm BestForm = null;

            foreach (BasicGameForm form in FormChildren)
                if (form != CurrentForm && Vector2.Distance(form.Position.get(), Position.get()) > 32)
                {
                    float d = (400 + Vector2.Distance(form.Position.get(), Position.get())) / 1000 *
                        (float)Math.Pow(1 + (float)Math.Abs(MathHelper.WrapAngle(Theta - Logic.ToAngle(form.Position.get() - Position.get()))), 1);

                    if (d < BestDistance && 
                        Math.Abs(MathHelper.WrapAngle(Theta - Logic.ToAngle(form.Position.get() - Position.get()))) < 1)
                    {
                        BestDistance = d;
                        BestForm = form;
                    }
                }

            if (BestForm != null)
                SetCurrentForm(BestForm);
        }

    }
}
