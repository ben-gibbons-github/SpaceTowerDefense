#if EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using System.IO;

namespace BadRabbit.Carrot
{
    public class NetworkManager
    {
        public static string NetworkStatus = "";
        public static NetworkSession networkSession;
        private static PacketReader packetReader = new PacketReader();
        private static PacketWriter packetWriter = new PacketWriter();
        private static bool ShouldSendLevel = false;
#if XBOX
        private static TimeSpan RequestTime;
#endif

        public static void Update(GameTime gameTime)
        {
            networkSession.Update();
            if (networkSession == null)
                return;
#if XBOX
            RequestTime += gameTime.ElapsedGameTime;
#endif
            Recieve();
            Send();
        }

        private static void Recieve()
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                ReadPackets(gamer);
            }
        }

        private static void Send()
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                SendPackets(gamer);
            }
        }

        private static void SendPackets(LocalNetworkGamer gamer)
        {
#if WINDOWS
            if (ShouldSendLevel)
            {
                BinaryWriter b = new BinaryWriter(packetWriter.BaseStream);
                GameManager.GetLevel().Write(b, true);

                Console.WriteLine("Packetwriter Line:" + packetWriter.Position);
                gamer.SendData(packetWriter, SendDataOptions.ReliableInOrder);
                ShouldSendLevel = false;
            }
#endif
#if XBOX
            if (RequestTime > TimeSpan.FromSeconds(2))
            {
                RequestTime -= TimeSpan.FromSeconds(2);
                packetWriter.Write(true);
                gamer.SendData(packetWriter, SendDataOptions.ReliableInOrder);
            }
#endif
        }

        private static void ReadPackets(LocalNetworkGamer gamer)
        {
#if XBOX   
                while (gamer.IsDataAvailable)
                    if (GameManager.GetLevel() == null)
                {
                    NetworkGamer sender;

                    gamer.ReceiveData(packetReader, out sender);

                    Level NewLevel = new Level(false);
                    BinaryReader b = new BinaryReader(packetReader.BaseStream);
                    NewLevel.Read(b);
                    b.Close();

                    GameManager.SetLevel(NewLevel);
                    break;
                }
#endif
#if WINDOWS
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;
                gamer.ReceiveData(packetReader, out sender);
                ShouldSendLevel = true;
            }
#endif
        }

        public static void CreateSession()
        {
            if (networkSession != null)
            {
                networkSession.Dispose();
            }
            SetStatus("Creating Session");
            try
            {
                networkSession = NetworkSession.Create(NetworkSessionType.SystemLink,2, 2);
                HookSessionEvents();
                SetStatus("Successfully Created Session");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SetStatus(e.Message);
            }
        }

        public static bool JoinSession()
        {
            SetStatus("Joining Session");

            try
            {
                using (AvailableNetworkSessionCollection availableSessions =
                            NetworkSession.Find(NetworkSessionType.SystemLink,
                                                2, null))
                {
                    if (availableSessions.Count == 0)
                    {
                        SetStatus("No network sessions found.");
                        return false;
                    }

                    networkSession = NetworkSession.Join(availableSessions[0]);

                    HookSessionEvents();

                    SetStatus("Connected!");
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SetStatus(e.Message);
                return false;
            }
        }

        public static void SetStatus(string Status)
        {
            NetworkStatus = Status;
        }


        #region HookSessionEvents
        private static void HookSessionEvents()
        {
            networkSession.GamerJoined += GamerJoinedEventHandler;
            networkSession.SessionEnded += SessionEndedEventHandler;
        }

        private static void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {

        }

        private static void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            SetStatus(e.EndReason.ToString());

            networkSession.Dispose();
            networkSession = null;
        }
        #endregion
    }
}

#endif