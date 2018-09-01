#if EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class RecieverManager
    {
        public static string RecieverMessage = "Press Start (>) to begin.";

        public static void Update(GameTime gameTime)
        {
            if (GameManager.GetLevel() == null)
            {
                if (PlayerProfile.getController(0) == null)
                {
                    PlayerProfile.Update(gameTime, true);
                    if (PlayerProfile.getController(0) != null)
                        setMessage("Press A to search for connection");
                }
                else
                {
                    PlayerProfile.Update(gameTime, false);
                    if (NetworkManager.networkSession == null)
                    {
                        BasicController b = PlayerProfile.getController(0);
                        if (b.AButton())
                        {
                            setMessage("Attempting to Join...");
                            if (NetworkManager.JoinSession())
                                setMessage("Sucess");
                            else
                                setMessage("Failure to Connect");
                        }
                    }
                    else
                    {
                        NetworkManager.Update(gameTime);
                    }
                }
            }
            else
            {
                if (!PlayerProfile.getController(0).BackButton())
                {
                    GameManager.Update(gameTime);
                    FPSCounter.Update(gameTime);
                }
                else
                    GameManager.ClearLevel();
            }
        }


        public static void setMessage(string Message)
        {
            RecieverMessage = Message;
        }

        public static void Draw()
        {
            if (GameManager.GetLevel() == null)
            {
                Game1.graphicsDevice.Clear(Color.DarkBlue);
                Game1.spriteBatch.Begin();
                Render.DrawShadowedText(RecieverMessage, new Vector2(128));
                Render.DrawShadowedText("Network Status:\n" + NetworkManager.NetworkStatus, new Vector2(128, 256));
                Game1.spriteBatch.End();
            }
            else
            {
                GameManager.Draw();
                FPSCounter.Draw();
            }
        }
    }
}
#endif