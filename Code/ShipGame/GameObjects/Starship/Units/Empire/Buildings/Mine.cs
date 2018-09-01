using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Mine : UnitBasic
    {
        MineLayer ParentLayer;
        UnitBasic AttackTarget = null;

        static int MaxRedFlashTime = 1000;
        static float RedFlashChange = 0.1f;

        float RedFlashAlpha = 0;
        int RedFlashTime = 0;

        public Mine(MineLayer ParentLayer, int FactionNumber) :
            base(FactionNumber)
        {
            this.ParentLayer = ParentLayer;
            ThreatLevel = 0;
            Solid = false;
            Acceleration = 1f;
            RotationSpeed = 1f;
        }

        public override bool CanBeTargeted()
        {
            return false;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(24));
        }

        public void SetAttackTarget(UnitBasic AttackTarget)
        {
            this.AttackTarget = AttackTarget;
        }

        public override void Update2(GameTime gameTime)
        {
            //TestCollision(gameTime);
            base.Update2(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (AttackTarget != null)
            {
                RedFlashTime += gameTime.ElapsedGameTime.Milliseconds * 2;

                if (Vector2.Distance(AttackTarget.Position.get(), Position.get()) > (Size.X() + AttackTarget.Size.X() + 4) / 2)
                    Accelerate(gameTime, AttackTarget.Position.get() - Position.get());
                else
                    Destroy();
            }
            else
                RedFlashTime += gameTime.ElapsedGameTime.Milliseconds;

            if (RedFlashTime > MaxRedFlashTime)
            {
                RedFlashTime -= MaxRedFlashTime;
                RedFlashAlpha = 1;
            }
            if (RedFlashAlpha > 1)
            {
                ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, Color.Red * RedFlashAlpha, Size.X() * 48, 1);
                RedFlashAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * RedFlashChange;
            }

            base.Update(gameTime);
        }

        public override void Collide(GameTime gameTime, BasicShipGameObject Other)
        {
            if (Other == null)
                return;
            if (Other.GetTeam() != GetTeam() && Other.CanBeTargeted())
            {
                for (int i = 0; i < 2; i++)
                    Other.Damage(2, 10, Speed, this, AttackType.Green);
                Destroy();
            }
            return;
        }

        public override bool CanInteract(PlayerShip p)
        {
            return false;
        }

        public override bool AllowInteract(PlayerShip p)
        {
            return false;
        }

        public override void BlowUp()
        {
            Destroy();
        }

        public override void Destroy()
        {
            SoundManager.Play3DSound("MineImpact",
                new Vector3(Position.X(), Y, Position.Y()), 0.25f, 800, 2f);

            QuadGrid grid = Parent2DScene.quadGrids.First.Value;
            float BulletExplosionDistance = 200;
            for (int i = 0; i < 2; i++)
                foreach (Basic2DObject o in grid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance)))
                    if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        float dist = Vector2.Distance(s.Position.get(), Position.get()) - o.Size.X() / 2;

                        if (dist < BulletExplosionDistance && GetTeam() != s.GetTeam())
                        {
                            float DistMult = 1;
                            if (dist > 0)
                                DistMult = (BulletExplosionDistance - dist) / BulletExplosionDistance;
                            s.Damage(DistMult * 4, DistMult, Vector2.Normalize(s.Position.get() - Position.get()) * 1 * DistMult, this, AttackType.Green);
                        }
                    }

            base.Destroy();
        }

        public override int GetIntType()
        {
            return InstanceManager.EmpireUnitIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Turret2");
        }
    }
}
