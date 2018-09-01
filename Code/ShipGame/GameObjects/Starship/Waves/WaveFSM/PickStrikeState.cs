using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class PickStrikeState : WaveState
    {
        public static PickStrikeState self;

        static PickStrikeState()
        {
            self = new PickStrikeState();
        }

        float FadeAlpha = 0;
        float FadeAlphaChange = 0;
        bool FadingOut = true;
        bool HasDoneStrike = false;

        Vector2 StrikePosition;

        public override void Enter()
        {
            HasDoneStrike = false;
            FadingOut = true;
            FadeAlpha = 0;
            FadeAlphaChange = 0.01f;

            UnitTurret BestTurret = null;
            int BestKills = -1;

            foreach(UnitTurret t in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(UnitTurret)))
                if (t.GetTeam() == WaveManager.ActiveTeam && t.Kills > BestKills)
                {
                    BestTurret = t;
                }

            if (BestTurret != null)
            {
                StrikePosition = BestTurret.getPosition();
            }

            base.Enter();
        }

        public override void Update(GameTime gameTime)
        {
            if (FadingOut)
            {
                FadeAlpha += FadeAlphaChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                if (FadeAlpha > 1)
                {
                    FadeAlpha = 1;
                    FadingOut = false;
                    if (HasDoneStrike)
                    {
                        SceneObject s = GameManager.GetLevel().getCurrentScene();
                        Camera3DObject WorldCamera = (Camera3DObject)s.FindObject(typeof(Camera3DObject));

                        WorldCamera.RemoveTag(GameObjectTag.WorldViewer);
                        GameManager.GetLevel().getCurrentScene().WorldViewerChildren.Remove(WorldCamera);

                        foreach (PlayerShip p in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(PlayerShip)))
                        {
                            p.AddTag(GameObjectTag.WorldViewer);
                        }

                        WaveManager.SetState(FadeInState.self);
                        FadeInState.SetTargetState(WaveStepState.self);
                    }
                    else
                    {
                        SceneObject s = GameManager.GetLevel().getCurrentScene();
                        Camera3DObject WorldCamera = (Camera3DObject)s.FindObject(typeof(Camera3DObject));
                        WorldCamera.AddTag(GameObjectTag.WorldViewer);
                        foreach (PlayerShip p in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(PlayerShip)))
                        {
                            p.RemoveTag(GameObjectTag.WorldViewer);
                            GameManager.GetLevel().getCurrentScene().WorldViewerChildren.Remove(p);
                        }
                        WorldCamera.MyCamera.SetLookAt(new Vector3(StrikePosition.X, 1000, StrikePosition.Y), new Vector3(StrikePosition.X, 0, StrikePosition.Y), new Vector3(0, 0, -1));
                    }
                }
            }
            else
            {
                if (FadeAlpha > 0)
                {
                    FadeAlpha -= FadeAlphaChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                    if (FadeAlpha < 0)
                        FadeAlpha = 0;
                }
                else
                {
                    if (!HasDoneStrike)
                    {
                        HasDoneStrike = true;
                        WaveManager.CurrentStrike.Trigger(StrikePosition);
                    }
                    else if (WaveManager.CurrentStrike.UpdateStrike(gameTime))
                    {
                        FadingOut = true;
                    }
                }
            }
            FadeManager.SetFadeColor(new Vector4(0, 0, 0, FadeAlpha));
        }
    }
}
