using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public interface WorldViewer2D
    {
        Camera2D getCamera();
        SceneView getSceneView();
        void setSceneView(SceneView sceneView);
        bool Active();
    }
}
