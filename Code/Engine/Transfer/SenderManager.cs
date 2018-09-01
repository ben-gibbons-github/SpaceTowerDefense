#if EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace BadRabbit.Carrot
{
    public class SenderManager
    {
        public static void Create()
        {
            if (Gamer.SignedInGamers.Count == 0)
                Guide.ShowSignIn(1, false);
            else
                NetworkManager.CreateSession();
        }

        public static void Update(GameTime gameTime)
        {
            if (NetworkManager.networkSession != null)
            {
                NetworkManager.Update(gameTime);
            }
        }

        public static string getStatus()
        {
            return NetworkManager.NetworkStatus;
        }
    }
}

#endif