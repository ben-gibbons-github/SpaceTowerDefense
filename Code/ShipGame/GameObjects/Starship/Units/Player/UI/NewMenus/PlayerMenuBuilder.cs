using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class PlayerMenuBuilder
    {
        static FormFrame CurrentFrame;

        public static void BuildPauseMenu(FormFrame frame)
        {
            CurrentFrame = frame;

            Add(new FormButton()).SetValues(
                new Vector2(0, 0), "Resume", "");

            Add(new FormButton()).SetValues(
                new Vector2(0, 60), "Controller Options", "Controller");

            Add(new FormButton()).SetValues(
                new Vector2(0, 120), "Screen Options", "Screen");

            Add(new FormButton()).SetValues(
                new Vector2(0, 180), "Leave Game", DropPlayer);

            frame.Commit("PauseMenu", true);
        }

        public static void BuildControllerOptions(FormFrame frame)
        {
            CurrentFrame = frame;

            Add(new FormButton()).SetValues(
                new Vector2(0, 0), "Return", "PauseMenu");

            frame.Commit("Controller", true);
        }

        public static void BuildScreenOptions(FormFrame frame)
        {
            CurrentFrame = frame;

            Add(new FormButton()).SetValues(
                new Vector2(0, 0), "Return", "PauseMenu");

            Add(new FormSlider()).SetValues(new Vector2(0, 80), "Brightness:", 0, 10, ShipGameSettings.BrightnessValue);

            Add(new FormSlider()).SetValues(new Vector2(0, 160), "Contrast:", 0, 10, ShipGameSettings.ContrastValue);


            frame.Commit("Screen", true);
        }
        
        static bool DropPlayer(BasicGameForm form, BasicMarker trigger)
        {
            PlayerProfile.RemovePlayer(trigger.MyPlayer);

            return true;
        }

        static BasicGameForm Add(BasicGameForm Form)
        {
            GameManager.GetLevel().AddObject(Form);
            CurrentFrame.Add(Form);
            return Form;
        }
    }
}
