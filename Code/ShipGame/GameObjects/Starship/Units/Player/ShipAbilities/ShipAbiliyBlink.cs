using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ShipAbiliyBlink : ShipAbility
    {
        private static float DashSpeed = 10;
        public int MaxDashTime = 400;

        bool Dashing;
        int DashTime = 0;
        Vector2 DashVector = new Vector2(1, 0);

        public ShipAbiliyBlink()
        {
            MaxRechargeTime = 1500;
        }

        public override float GetCharge()
        {
            return Math.Min(1, RechargeTime / MaxRechargeTime);
        }

        public override bool ShipIsSolid()
        {
            return !Dashing;
        }

        public override bool ShipCanMove()
        {
            return !Dashing;  
        }

        public override bool ShipCanShoot()
        {
            return !Dashing;
        }

        public override bool ShipCanTakeDamage()
        {
            return !Dashing;
        }

        public override void Update(GameTime gameTime, BasicController MyController)
        {
            if (!Dashing)
            {
                RechargeTime += gameTime.ElapsedGameTime.Milliseconds;
                if (MyController.LeftStick().Length() > 0.1f)
                    DashVector = Vector2.Normalize(MyController.LeftStick() * new Vector2(1, -1));
            }
            else
            {
                ParentShip.AddPosition(DashVector * DashSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f * 60f);
                ParentShip.InvTime = Math.Max(ParentShip.InvTime, 400);
                DashTime += gameTime.ElapsedGameTime.Milliseconds;

                if (DashTime > MaxDashTime)
                {
                    bool PositionClear = true;
                    foreach (Basic2DObject s in ParentShip.Parent2DScene.quadGrids.First.Value.Enumerate(
                        ParentShip.Position.get(), new Vector2(ParentShip.PlayerSize)))
                        if (s != ParentShip)
                        {
                            if (s.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                            {
                                if (Vector2.Distance(ParentShip.getPosition(), s.getPosition()) < (ParentShip.PlayerSize + s.getSize().X) / 2)
                                {
                                    BasicShipGameObject b = (BasicShipGameObject)s;
                                    if (b.Solid)
                                    {
                                        PositionClear = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                WallNode n = (WallNode)s;
                                if (n.wallConnector != null)
                                {
                                    float MoveAmount = (ParentShip.PlayerSize + s.getSize().X * 0.75f) / 2 - Logic.DistanceLineSegmentToPoint(n.Position.get(), n.wallConnector.PositionNext, ParentShip.Position.get());

                                    if (MoveAmount > 0)
                                    {
                                        PositionClear = false;
                                        break;
                                    }
                                }
                            }
                        }

                    if (PositionClear && Dashing)
                    {
                        SoundManager.Play3DSound("PlayerDashReverse",
                            new Vector3(ParentShip.Position.X(), ParentShip.Y, ParentShip.Position.Y()), 0.2f, 300, 2);

                        Dashing = false;
                        Vector3 Position3 = new Vector3(ParentShip.Position.X(), ParentShip.Y, ParentShip.Position.Y());
                        ParticleManager.CreateParticle(Position3, Vector3.Zero, PlayerShip.TeleportColor2, ParentShip.Size.X() * 5, 4);
                        for (int i = 0; i < 30; i++)
                            ParticleManager.CreateParticle(Position3, Rand.V3() * 200, PlayerShip.TeleportColor2, 20, 5);
                    }
                }
            }

            base.Update(gameTime, MyController);
        }

        public override bool Trigger()
        {
            if (RechargeTime > MaxRechargeTime)
            {
                DashTime = 0;
                RechargeTime = 0;
                Dashing = true;

                SoundManager.Play3DSound("PlayerDash",
                    new Vector3(ParentShip.Position.X(), ParentShip.Y, ParentShip.Position.Y()), 0.4f, 300, 2);

                return true;
            }
            return false;
        }



    }
}
