using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class OverTeamBar : SizeNodeObject
    {
        public static List<BarTeam> BarTeams = new List<BarTeam>();
        public static float SizeBonus = 0;
        public static float Alpha = 0;
        static OverTeamBar self;

        SizeNode RectL;
        SizeNode RectR;

        SizeNode LargeRectL;
        SizeNode LargeRectR;

        Vector2 InterpolatedPositionL;
        Vector2 InterpolatedPositionR;
        Vector2 InterpolatedDifference;

        public OverTeamBar()
        {
            self = this;
        }

        public override void Create()
        {
            AddTag(GameObjectTag.OverDrawViews);
            AddTag(GameObjectTag.Update);

            RectL = AddSizeNode(new SizeNode(new Vector2(150, 500)));
            RectR = AddSizeNode(new SizeNode(new Vector2(200, 500)));

            LargeRectL = AddSizeNode(new SizeNode(new Vector2(400, 500)));
            LargeRectR = AddSizeNode(new SizeNode(new Vector2(600, 500)));

            base.Create();
        }

        public static void AddBarTeam(BarTeam team)
        {
            BarTeams.Add(team);

            self.RectL.TargetPosition.Y -= 25;
            self.RectR.TargetPosition.Y += 25;
            self.LargeRectL.TargetPosition.Y -= 50;
            self.LargeRectR.TargetPosition.Y += 50;
        }

        public static void Clear()
        {
            self.RectL.TargetPosition.Y = 500;
            self.RectR.TargetPosition.Y = 500;
            self.LargeRectL.TargetPosition.Y = 500;
            self.LargeRectR.TargetPosition.Y = 500;
            BarTeams.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (BarTeam team in BarTeams)
                team.Update(gameTime);

            InterpolatedPositionL = (RectL.Position + (LargeRectL.Position - RectL.Position) * SizeBonus) /
                OverFormat.BaseScreenSize * MasterManager.FullScreenSize;
            InterpolatedPositionR = (RectR.Position + (LargeRectR.Position - RectR.Position) * SizeBonus) /
                OverFormat.BaseScreenSize * MasterManager.FullScreenSize;
            InterpolatedDifference = InterpolatedPositionR - InterpolatedPositionL;

            base.Update(gameTime);
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (BarTeams.Count == 1)
                return;

            Render.DrawSolidRect(InterpolatedPositionL, InterpolatedPositionR, new Color(0, 0, 0, 0.5f));

            Vector2 BarPositionL = Vector2.Zero;
            Vector2 BarPositionR = Vector2.Zero;

            for (int i = 0; i < BarTeams.Count; i++)
            {
                BarPositionL = InterpolatedPositionL + 
                    InterpolatedDifference / 50 + new Vector2(0, InterpolatedDifference.Y / BarTeams.Count * BarTeams[i].ListPosition);
                BarPositionR = InterpolatedPositionL + 
                    new Vector2(InterpolatedDifference.X * 0.98f, InterpolatedDifference.Y / BarTeams.Count * (BarTeams[i].ListPosition + 0.98f));

                Render.DrawSolidRect(BarPositionL, BarPositionR, TeamInfo.Colors[BarTeams[i].Team]);
            }

                base.Draw2D(DrawTag);
        }
    }
}
