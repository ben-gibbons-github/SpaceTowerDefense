using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public enum DeferredMode
    {
        Composite,
        Basic,
        Lighting,
        Normal,
        Depth
    }

    public class DeferredControls : Form
    {

        public DeferredMode deferredMode = DeferredMode.Composite;
        #if EDITOR && WINDOWS
        private List<Form> MyForms = new List<Form>();
        private Vector2 LowerRightCorner;

        public static Texture2D CompositeIcon;
        public static Texture2D BasicIcon;
        public static Texture2D LightingIcon;
        public static Texture2D NormalIcon;
        public static Texture2D DepthIcon;
        public static bool Loaded = false;

        public void Cycle()
        {
            deferredMode++;
            if (deferredMode == DeferredMode.Depth)
                deferredMode = DeferredMode.Composite;
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            Vector2 TargetCorner = Updater.Size;
            if (Updater.Size.Y != LowerRightCorner.Y)
            {
                LowerRightCorner.Y = Updater.Size.Y;
                CreateForms();
            }

            base.Update(gameTime, Updater);
        }

        public override void Create(FormHolder Parent)
        {
            base.Create(Parent);
            Load();
            CreateForms();
            SetComposite((Button)MyForms[0]);
        }

        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                CompositeIcon = AssetManager.Load<Texture2D>("Editor/CompositeIcon");
                BasicIcon = AssetManager.Load<Texture2D>("Editor/BasicIcon");
                LightingIcon = AssetManager.Load<Texture2D>("Editor/LightingIcon");
                NormalIcon = AssetManager.Load<Texture2D>("Editor/NormalIcon");
                DepthIcon = AssetManager.Load<Texture2D>("Editor/DepthIcon");
            }
        }

        public void CreateForms()
        {
            foreach (Form form in MyForms)
                Parent.RemoveForm(form);
            MyForms.Clear();

            int ButtonX = 0;
            int Margin = -23;
            int ButtonY = (int)LowerRightCorner.Y + Margin;
            int ButtonSize = 16;

            AddForm(new Button(SetComposite, CompositeIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetBasic, BasicIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetLighting, LightingIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetNormal, NormalIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetDepth, DepthIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));
        }

        public void SetComposite(Button button)
        {
            deferredMode = DeferredMode.Composite;
            Switchselect(button);
        }

        public void SetBasic(Button button)
        {
            deferredMode = DeferredMode.Basic;
            Switchselect(button);
        }

        public void SetLighting(Button button)
        {
            deferredMode = DeferredMode.Lighting;
            Switchselect(button);
        }

        public void SetNormal(Button button)
        {
            deferredMode = DeferredMode.Normal;
            Switchselect(button);
        }

        public void SetDepth(Button button)
        {
            deferredMode = DeferredMode.Depth;
            Switchselect(button);
        }

        private void Switchselect(Button b)
        {
            foreach (Button but in MyForms)
                but.Selected = false;
            b.Selected = true;
        }

        new public void AddForm(Form NewForm)
        {
            MyForms.Add(NewForm);
            Parent.AddForm(NewForm);
        }
#endif
    }
}
