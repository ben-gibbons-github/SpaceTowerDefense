using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class PlayerProfile
    {
        private enum MessageType
        {
            SignIn,
            SignOut,
            NotAllowed,
        }

        public enum SignInState
        {
            Controller,
            Keyboard
        }

        public enum ControllerIndex
        {
            ControllerOne,
            ControllerTwo,
            ControllerThree,
            ControllerFour,
            Keyboard,
            Online,
            AI,
        }

        private static string[] AINames =
        {
            "PigSlayer360",
            "TusslePlate01",
            "SantasFather",
            "Joey1337Man",
            "F34RM3",
            "PenguinCannon",
            "RAGEquit"
        };
        
        private static Texture2D MessageTexture;
        private static float MessageBackAlpha = 0.25f;
        private static float MessageAlpha = 0;
        private static MessageType messageType = MessageType.NotAllowed;
        public static SignInState signInState = SignInState.Controller;
        private static List<PlayerProfile> Players = new List<PlayerProfile>();
        private static int MaxPlayers = 4;
        private static float AlphaChange = 0.005f;

        private static Texture2D KeyboardTexture;
        private static Texture2D[] XboxControllerTextures;
        private static Texture2D AITexture;
        private static Texture2D OnlineTexture;
        public static Texture2D XTexture;

#if WINDOWS
        public static bool KeyboardTaken = false;
#endif
        public static LinkedList<PlayerIndex> FreePlayerIndecies = new LinkedList<PlayerIndex>();
        private static LinkedList<PlayerIndex> RemovePlayerIndecies = new LinkedList<PlayerIndex>();

        public BasicController MyController;
        private SignedInGamer MyGamer;
        private ControllerIndex controllerIndex;
        public string PlayerName;
        public int PlayerNumber;

        public static void Load()
        {
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(TestSignIn);
            SignedInGamer.SignedOut +=new EventHandler<SignedOutEventArgs>(TestSignOut);
            for (int i = 0; i < 4; i++)
                FreePlayerIndecies.AddLast((PlayerIndex)i);

            KeyboardTexture = AssetManager.Load<Texture2D>("_Engine/KeyboardIcon");
            AITexture = AssetManager.Load<Texture2D>("_Engine/AIIcon");
            OnlineTexture = AssetManager.Load<Texture2D>("_Engine/OnlineIcon");
            XTexture = AssetManager.Load<Texture2D>("_Engine/X");
            XboxControllerTextures = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                XboxControllerTextures[i] = AssetManager.Load<Texture2D>("_Engine/XboxIcon" + (i + 1).ToString());
        }

        public static void Clear()
        {
            FreePlayerIndecies.Clear();
            for (int i = 0; i < 4; i++)
                FreePlayerIndecies.AddLast((PlayerIndex)i);
#if EDITOR && WINDOWS
            KeyboardTaken = false;
#endif
            Players.Clear();
        }

        public static BasicController getController(int Index)
        {
            return Players.Count > Index ? Players[Index].MyController : null;
        }

        public static PlayerIndex Convert(ControllerIndex c)
        {
            switch (c)
            {
                case ControllerIndex.ControllerOne:
                    return PlayerIndex.One;
                case ControllerIndex.ControllerTwo:
                    return PlayerIndex.Two;
                case ControllerIndex.ControllerThree:
                    return PlayerIndex.Three;
                case ControllerIndex.ControllerFour:
                    return PlayerIndex.Four;
            }

            return PlayerIndex.One;
        }

        public static ControllerIndex Convert(PlayerIndex p)
        {
            switch (p)
            {
                case PlayerIndex.One:
                    return ControllerIndex.ControllerOne;
                case PlayerIndex.Two:
                    return ControllerIndex.ControllerTwo;
                case PlayerIndex.Three:
                    return ControllerIndex.ControllerThree;
                case PlayerIndex.Four:
                    return ControllerIndex.ControllerFour;
            }

            return ControllerIndex.ControllerOne;
        }


        public static Texture2D ConvertToTexture(ControllerIndex c)
        {
            switch (c)
            {
                case ControllerIndex.ControllerOne:
                    return XboxControllerTextures[0];
                case ControllerIndex.ControllerTwo:
                    return XboxControllerTextures[1];
                case ControllerIndex.ControllerThree:
                    return XboxControllerTextures[2];
                case ControllerIndex.ControllerFour:
                    return XboxControllerTextures[3];
                case ControllerIndex.Keyboard:
                    return KeyboardTexture;
                case ControllerIndex.AI:
                    return AITexture;
                case ControllerIndex.Online:
                    return OnlineTexture;
            }

            return null;
        }


        private static void TestSignIn(object o, SignedInEventArgs e)
        {
            BasicController b = null;
            ControllerIndex c = ControllerIndex.Keyboard;
            
#if EDITOR && WINDOWS
            if(signInState == SignInState.Controller)
            {
#endif
                b = new XboxController(e.Gamer.PlayerIndex);
                c = Convert(e.Gamer.PlayerIndex);
#if EDITOR && WINDOWS
            }
            else
                b = new KeyboardController();
#endif

            AddPlayer(b, e.Gamer, c);
        }

        private static void TestSignOut(object o, SignedOutEventArgs e)
        {
            foreach (PlayerProfile p in Players)
                if (p.MyGamer == e.Gamer)
                    RemovePlayer(p);
        }

        private static string FindFreeAIName()
        {
            foreach (string s in AINames)
            {
                bool Free = true;
                foreach (PlayerProfile f in Players)
                    if (f.PlayerName.Equals(s))
                        Free = false;

                if (Free)
                    return s;
            }

            return "";
        }

        public static void RemovePlayer(PlayerProfile p)
        {
            Players.Remove(p);
            if (p.controllerIndex != ControllerIndex.Keyboard)
                if (!FreePlayerIndecies.Contains(Convert(p.controllerIndex)))
                    FreePlayerIndecies.AddLast(Convert(p.controllerIndex));

            for (int i = 0; i < Players.Count; i++)
                Players[i].SetPlayerNumber(i);


#if WINDOWS
            if (!GameManager.GetLevel().LevelForEditing)
#endif
                GameManager.GetLevel().PlayerQuitEvent(p);
        }

        public static PlayerProfile AddPlayer(BasicController controller, SignedInGamer gamer, ControllerIndex controllerIndex)
        {
            PlayerProfile newProfile = new PlayerProfile(gamer, controller, controllerIndex);

            bool Found = false;

            if (controllerIndex != ControllerIndex.AI && controllerIndex != ControllerIndex.Online)
                for (int i = 0; i < Players.Count; i++)
                    if (controllerIndex == Players[i].controllerIndex)
                    {
                        Players[i] = newProfile;
                        newProfile.SetPlayerNumber(i);
                        Found = true;
#if WINDOWS
                        if (!GameManager.GetLevel().LevelForEditing)
#endif
                            GameManager.GetLevel().PlayerJoinedEvent(newProfile);

                        PlayerJoinMessage(newProfile);
                        return newProfile;
                    }

            if (!Found && Players.Count < MaxPlayers)
            {
                newProfile.SetPlayerNumber(Players.Count);
                Players.Add(newProfile);
#if WINDOWS
                if (!GameManager.GetLevel().LevelForEditing)
#endif
                    GameManager.GetLevel().PlayerJoinedEvent(newProfile);

                PlayerJoinMessage(newProfile);
                return newProfile;
            }
            return null;
        }

        public PlayerProfile(SignedInGamer MyGamer, BasicController MyController, ControllerIndex controllerIndex)
        {
            if (MyGamer != null)
                PlayerName = MyGamer.DisplayName;

            this.MyGamer = MyGamer;
            this.MyController = MyController;
            this.controllerIndex = controllerIndex;
        }

        private void SetPlayerNumber(int i)
        {
            if (MyGamer == null)
            {
                if (controllerIndex == ControllerIndex.AI)
                    PlayerName = FindFreeAIName();
                PlayerName = "Player" + i;
            }

            PlayerNumber = i;
        }

        private void MemberUpdate(GameTime gameTime)
        {
            MyController.Update(gameTime);
        }

        public static void CallLevelEvents()
        {
#if WINDOWS
            if (!GameManager.GetLevel().LevelForEditing)
#endif
                foreach (PlayerProfile p in Players)
                    GameManager.GetLevel().PlayerJoinedEvent(p);
        }

        public static void Update(GameTime gameTime, bool AllowJoining)
        {
            MessageAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60.0f / 1000.0f * AlphaChange;

            if (AllowJoining)
            {
#if WINDOWS
                if (!KeyboardTaken && Keyboard.GetState().GetPressedKeys().Count() > 0)
                    if (AddPlayer(new KeyboardController(), null, ControllerIndex.Keyboard) != null)
                        KeyboardTaken = true;
#endif
                foreach (PlayerIndex i in FreePlayerIndecies)
                {
                    GamePadState g = GamePad.GetState(i);
                    if (g.IsConnected && XboxController.TestAny(g))
                        if (AddPlayer(new XboxController(i), null, Convert(i)) != null)
                            RemovePlayerIndecies.AddLast(i);
                }
                foreach (PlayerIndex i in RemovePlayerIndecies)
                    FreePlayerIndecies.Remove(i);
                RemovePlayerIndecies.Clear();
            }
            else
            {
                /*
#if WINDOWS
                if (!KeyboardTaken && Keyboard.GetState().GetPressedKeys().Count() > 0)
                    PlayerNoJoinMessage(ControllerIndex.Keyboard);
                else
                    foreach (PlayerIndex i in FreePlayerIndecies)
                {
                    GamePadState g = GamePad.GetState(i);
                    if (g.IsConnected && XboxController.TestAny(g))
                        PlayerNoJoinMessage(Convert(i));
                }
#endif
                 */
            }

            foreach (PlayerProfile p in Players)
                p.MemberUpdate(gameTime);
        }

        private static void PlayerJoinMessage(PlayerProfile p)
        {
            MessageTexture = ConvertToTexture(p.controllerIndex);
            MessageAlpha = 2;
            messageType = MessageType.SignIn;
        }

        private static void PlayerQuitMessage(PlayerProfile p)
        {
            MessageTexture = ConvertToTexture(p.controllerIndex);
            MessageAlpha = 2;
            messageType = MessageType.SignOut;
        }

        private static void PlayerNoJoinMessage(ControllerIndex c)
        {
            MessageTexture = ConvertToTexture(c);
            MessageAlpha = 2;
            messageType = MessageType.NotAllowed;
        }

        public static void Draw()
        {
            if (MessageAlpha > 0)
            {
                Game1.spriteBatch.Begin();

                switch (messageType)
                {
                    case MessageType.NotAllowed:
                        Render.DrawSolidRect(MasterManager.ProfileMessageRect, Color.Red * MessageBackAlpha * MessageAlpha);
                        Game1.spriteBatch.Draw(MessageTexture, MasterManager.ProfileMessageRect, Color.White * MessageAlpha);
                        break;
                    case MessageType.SignIn:
                        Render.DrawSolidRect(MasterManager.ProfileMessageRect, Color.Green * MessageBackAlpha * MessageAlpha);
                        Game1.spriteBatch.Draw(MessageTexture, MasterManager.ProfileMessageRect, Color.White * MessageAlpha);
                        break;
                    case MessageType.SignOut:
                        Render.DrawSolidRect(MasterManager.ProfileMessageRect, Color.White * MessageBackAlpha * MessageAlpha);
                        Game1.spriteBatch.Draw(MessageTexture, MasterManager.ProfileMessageRect, Color.White * MessageAlpha);
                        break;
                }
                Game1.spriteBatch.End();
            }
        }
    }
}
