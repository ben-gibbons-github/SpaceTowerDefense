using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot.SpyGame
{
    public class SpyScene : Deferred3DScene
    {
        public override void PlayerJoinedEvent(PlayerProfile p)
        {
            SpyPlayer player = new SpyPlayer(p);
            ParentLevel.AddObject(player, this);

            foreach(GameObject o in Children)
                if (o.GetType().Equals(typeof(SpyPlayerSpawn)))
                {
                    SpyPlayerSpawn spawn = (SpyPlayerSpawn)o;
                    if (spawn.PlayerNumber.get() == p.PlayerNumber)
                        player.Position.set(spawn.Position.get());
                        return;
                }

            base.PlayerJoinedEvent(p);
        }
    }
}
