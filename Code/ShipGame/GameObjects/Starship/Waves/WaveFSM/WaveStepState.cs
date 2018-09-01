using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class WaveStepState : WaveState
    {
        public static bool WeaponsFree = false;

        public static WaveStepState self;

        static WaveStepState()
        {
            self = new WaveStepState();
        }

        public int Timer = 0;
        public int ExplosionTimer = 0;
        public int MaxExplosionTimer = 0;

        public override void Enter()
        {
            Timer = 0;
            ExplosionTimer = 0;
            MaxExplosionTimer = 3000;

            NeutralManager.WaveActive = true;
            WeaponsFree = true;
        }

        public override void Exit()
        {
            NeutralManager.WaveActive = false;
            WeaponsFree = false;
            base.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;

            if (Timer > FactionManager.NeutralUnitCount * 10000 && Timer > 5000)
            {
                ExplosionTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (ExplosionTimer > MaxExplosionTimer)
                    WaveManager.SetState(WaveEndState.self);
            }

            base.Update(gameTime);
        }
    }
}
