using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class GameObjectTagList
    {
        public static GameObjectTag[] AllTags =
        {
            GameObjectTag.SceneDrawScene,
            GameObjectTag.Update,
            GameObjectTag._3DBackground,
            GameObjectTag._3DForward,
            GameObjectTag._3DDepthOver,
            GameObjectTag._3DDeferredGBuffer,
            GameObjectTag._3DDeferredOverLighting,
            GameObjectTag._3DDeferredWorldLighting,
            GameObjectTag._3DShadow,
            GameObjectTag._3DPreDraw,
            GameObjectTag._2DForward,
            GameObjectTag._2DPreDraw,
            GameObjectTag._2DSolid,
            GameObjectTag._3DSolid,
            GameObjectTag.WorldViewer,
            GameObjectTag.ShipGameUnitBasic,
            GameObjectTag.Form,
            GameObjectTag.Update,
        };
    }

    public enum GameObjectTag
    {
        SceneDrawScene,

        _3DBackground,
        _3DForward,
        _3DDepthOver,
        _3DDeferredGBuffer,
        _3DDeferredOverLighting,
        _3DDeferredWorldLighting,
        _3DShadow,
        _3DPreDraw,
        _3DSolid,

        _2DForward,
        _2DPreDraw,
        _2DSolid,
        _2DOverDraw,
        OverDrawViews,

        WorldViewer,
        ShipGameUnitBasic,
        Form,
        Update,
        _3DDistortion,
    }

}
