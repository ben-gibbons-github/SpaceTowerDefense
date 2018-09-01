using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BombManager
    {
        static int MaxBigBombTime = 1000;
        static int MaxSmallBombDelay = 1000;
            
        public SmallBombLauncher BombLauncher;

        int BigBombTime = 0;
        public LinkedList<BigBomb> Bombs = new LinkedList<BigBomb>();
        PlayerShip ParentShip;

        int SmallBombDelay = 0;

        public BombManager(PlayerShip ParentShip)
        {
            BombLauncher = new SmallBombLauncher();
            BombLauncher.SetParent(ParentShip);
            Bombs.AddFirst(new BigBomb());
            this.ParentShip = ParentShip;
        }

        public void AddSmallBombs()
        {
            int MaxBombs = 2 + 2 * FactionManager.TeamCount;
            int GetBombs = FactionManager.TeamCount;
            BombLauncher.Ammo = (int)MathHelper.Clamp(BombLauncher.Ammo + GetBombs, GetBombs, MaxBombs);
        }

        public void Update(GameTime gameTime, BasicController MyController)
        {
            if (Bombs.Count > 0 && FactionManager.TeamCount > 1)
                Bombs.Clear();

            SmallBombDelay += gameTime.ElapsedGameTime.Milliseconds;

            if (Bombs.Count > 0)
            {
                if (MyController.LeftTrigger())
                {
                    BigBombTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (BigBombTime > MaxBigBombTime)
                    {
                        Bombs.First.Value.Trigger(ParentShip);
                        Bombs.Remove(Bombs.First);
                    }
                }
                else
                    BigBombTime = 0;
            }

            if (ParentShip.viewMode == ViewMode.Ship && !ParentShip.IsTeleporting &&
                ((!MyController.IsKeyboardController() && MyController.RightTrigger() && !MyController.RightTriggerPrevious())
                || (MyController.IsKeyboardController() && MyController.LeftTrigger() && !MyController.LeftTriggerPrevious())) &&
                ParentShip.Guns != null && ParentShip.Guns[0] != null && BombLauncher.Ammo > 0 && SmallBombDelay > MaxSmallBombDelay)
            {
                SmallBombDelay = 0;
                FactionManager.Factions[ParentShip.FactionNumber].roundReport.SmallBombsUsed++;
                BombLauncher.Fire(ParentShip.Guns[0].Rotation);
            }
        }
    }
}
