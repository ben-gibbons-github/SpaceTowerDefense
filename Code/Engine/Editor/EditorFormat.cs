#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    class EditorFormat
    {

        public static void InitWindows(Editor editor)
        {

            Window window = null;

            //Left Window
            {
                window = editor.AddWindow(new HierarchyViewer());
                //window = editor.AddWindow(new Window());
                window.SetDefaultSize(new Vector2(200, 200));
                window.AddAnchors(Direction.UP, Direction.DOWN, Direction.LEFT);
                window.SetMargins(new Vector2(0, 100), new Vector2(0, 00));
                //window.BorderSize = 4;
            }
            // Right
            {
                window = editor.AddWindow(new ObjectProperties());
                //window = editor.AddWindow(new Window());
                window.SetDefaultSize(new Vector2(300, 200));
                window.AddAnchors(Direction.UP, Direction.DOWN, Direction.RIGHT);
                window.SetMargins(new Vector2(0, 0), new Vector2(0, 0));
            }
            // Top
            {
                window = editor.AddWindow(new TopBar());
               // window = editor.AddWindow(new Window());
                window.SetDefaultSize(new Vector2(300, 100));
                window.AddAnchors(Direction.RIGHT, Direction.UP, Direction.LEFT);
                window.SetMargins(new Vector2(0, 00), new Vector2(300, 0));
                //window.BorderSize = 4;
            }
            // WorldViewer 
            {
                window = editor.AddWindow(new WorldViewer());
                //window = editor.AddWindow(new Window());
                window.AddAnchors(Direction.RIGHT, Direction.UP, Direction.LEFT, Direction.DOWN);
                window.SetMargins(new Vector2(200, 100), new Vector2(300, 100));
            }
            // Bottom
            {
                window = editor.AddWindow(new SceneSelect());
                // window = editor.AddWindow(new Window());
                window.SetDefaultSize(new Vector2(0, 100));
                window.AddAnchors(Direction.RIGHT, Direction.DOWN, Direction.LEFT);
                window.SetMargins(new Vector2(200, 00), new Vector2(300, 0));
                //window.BorderSize = 4;
            }
        }
    }
}
#endif