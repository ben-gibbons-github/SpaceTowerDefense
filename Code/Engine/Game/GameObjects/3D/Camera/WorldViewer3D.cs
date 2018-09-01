using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public interface WorldViewer3D
    {
        Camera3D getCamera();
        SceneView getSceneView();
        void setSceneView(SceneView sceneView);
    }
}
