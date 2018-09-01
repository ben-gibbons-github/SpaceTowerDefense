using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

#if WINDOWS && EDITOR
using System.Windows.Forms;
#endif

namespace BadRabbit.Carrot
{
    public class Level //cardLevel forms hold the scene form
    {
        enum PostLoadingStage
        {
            HierarchyChildren,
            PostRead,
            CreateInGame,
            Ready
        }

        public static LoadingScreen loadingScreen = new StarshipLoadingScreen();
        private static LinkedList<Level> LoadingLevels = new LinkedList<Level>();
        private static LinkedList<Level> RemovingLevels = new LinkedList<Level>();

        public static bool LoadLevels()
        {
            bool Loaded = false;

            foreach (Level l in LoadingLevels)
            {
#if !DEBUG
                try
                {
                    l.Read(null);
                }
                catch (Exception ex)
                {
#if EDITOR && WINDOWS
                    if (l.LevelForEditing)
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
#endif
                    l.LoadingReader.BaseStream.Close();
                }
#endif
#if DEBUG
                l.Read(null);
#endif
                if (!l.Loading)
                    RemovingLevels.AddLast(l);

                Loaded = true;
            }

            foreach (Level l in RemovingLevels)
                LoadingLevels.Remove(l);

            RemovingLevels.Clear();

            return Loaded;
        }


        public static GameObject ReferenceObject;
        public static SceneObject ReferenceScene;
        public static Level ReferenceLevel;
        public static bool SafeWrite;
        
        public LinkedList<SceneObject> Scenes = new LinkedList<SceneObject>();
        public SceneObject MyScene;
        public SceneObject StartingScene;

        public bool AllowJoining = true;
        public bool Paused = false;
        public int IdCounter;
        public static float Time;

        public bool LevelForEditing;

        public bool Loading = false;
        private BinaryReader LoadingReader;
        private int LoadingScenesLoaded = 0;
        private int LoadingObjectsLoaded = 0;

        private int LoadingProgressBar = 0;
        private int LoadingProgressBarMax = 0;
        private float LoadingProgressBarAlpha = 0;

        private PostLoadingStage postLoadingStage = PostLoadingStage.HierarchyChildren;
        private LinkedListNode<SceneObject> PostLoadingCurrentScene;
        private LinkedListNode<GameObject> PostLoadingCurrentObject;

        private GameTime LastLoadTime = Game1.gameTime;

        byte LoadingTagsCount;
        byte LoadingSaveType;
        int LoadingSceneCount;
        int LoadingObjectCount;
        SceneObject LoadingScene;

#if EDITOR && WINDOWS
        public FormHolder SceneHierarchyHolder;
        public SceneHierarchy MySceneHieararchy;
#endif
        public Level(bool LevelForEditing)
        {
#if EDITOR
            Render.EffectUpdateCalls = 0;
#endif
            this.LevelForEditing = LevelForEditing;
#if EDITOR && WINDOWS
            SceneHierarchyHolder = new FormHolder();
            if (LevelForEditing)
                SceneHierarchyHolder.AddForm(MySceneHieararchy = new SceneHierarchy(new Vector2(32,4), this));
#endif
        }

        public void PlayerJoinedEvent(PlayerProfile profile)
        {
            if (LoadingLevels.Count > 0)
                return;

            if (MyScene != null)
                MyScene.PlayerJoinedEvent(profile);
#if WINDOWS && EDITOR
            else
                Console.WriteLine("Error with player joining: no scene exists");
            Console.WriteLine(profile.PlayerName + " just joined!");
#endif
        }

        public void PlayerQuitEvent(PlayerProfile profile)
        {
            if (MyScene != null)
                MyScene.PlayerQuitEvent(profile);
#if WINDOWS && EDITOR
            else
                Console.WriteLine("Error with player quitting: no scene exists");
            Console.WriteLine(profile.PlayerName + " just quit!");
#endif
        }

        public void SetSize(Vector2 Size)
        {
            if (MyScene != null)
                MyScene.SetWindowSize(Size);
        }

        public void AddEditingForms()
        {

        }

        public void TriggerEvent(string Object, EventType Event, string[] args)
        {
            if (MyScene != null)
            {
                bool Sucess = MyScene.TriggerEvent(Object, Event, args);
#if EDITOR && WINDOWS
                if (!Sucess)
                {
                    Console.Write(Object + '.' + Event.ToString() + '(');
                    foreach (string a in args)
                        Console.Write(a);
                    Console.WriteLine(')');
                    Console.WriteLine("FAILED!");
                }
#endif
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateTime(gameTime);

            if (Loading)
            {
                LoadingProgressBarAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f;
                if (LoadingProgressBarAlpha > 1)
                    LoadingProgressBarAlpha = 1;
                return;
            }
            if (LoadingProgressBarAlpha > 0)
            {
                LoadingProgressBarAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f;
                if (LoadingProgressBarAlpha < 0)
                    LoadingProgressBarAlpha = 0;
            }

            if (MyScene != null)
            {    
#if EDITOR && WINDOWS
                if (LevelForEditing)
                    MyScene.UpdateEditor(gameTime);
                else if (true)
#endif
                    MyScene.Update(gameTime);

                if (MyScene.NeedsPlayers)
                {
                    PlayerProfile.CallLevelEvents();
                    MyScene.NeedsPlayers = false;
                }
            }

            PlayerProfile.Update(gameTime, AllowJoining);
        }

        private void UpdateTime(GameTime gameTime)
        {
            Time += (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            if (Time > 100000) 
                Time = 0;
        }

#if EDITOR && WINDOWS
        public void ModifyWindows()
        {
            if (LevelForEditing)
            {
                SceneSelect.self.NeedsToRedraw = true;
                HierarchyViewer.self.NeedsToRedraw = true;
                WorldViewer.self.NeedsToRedraw = true;
                ObjectProperties.self.NeedsToRedraw = true;
            }
        }
#endif

        public void Destroy()
        {
            SceneObject[] s = Scenes.ToArray();
            foreach (SceneObject scene in s)
                scene.Destroy();
        }

        public SceneObject getCurrentScene()
        {
            return MyScene != null ? MyScene : Scenes.First.Value;
        }

        public void SetScene(SceneObject scene)
        {
            if (Scenes.Contains(scene))
            {
                MyScene = scene;
#if EDITOR && WINDOWS
                MyScene.UpdateSelected();
                ModifyWindows();

                if (!LevelForEditing)
#endif
                    if (MyScene.NeedsPlayers)
                    {
                        PlayerProfile.CallLevelEvents();
                        MyScene.NeedsPlayers = false;
                    }
            }
        }

        public void SetStartingScene(SceneObject scene)
        {
            if (Scenes.Contains(scene))
            {
                StartingScene = scene;
#if EDITOR && WINDOWS
                ModifyWindows();
#endif
            }
        }

        private SceneObject AddScene(SceneObject scene)
        {
            if (Scenes.Count == 0)
                StartingScene = scene;

            if (!Scenes.Contains(scene))
                Scenes.AddFirst(scene);

            Level.ObjectCreate(scene,null,this);

            if (!scene.Created)
            {
                scene.SetParents(this, null);
                scene.Create();
            }
            MyScene = scene;

#if EDITOR && WINDOWS
            if (MySceneHieararchy != null)
                MySceneHieararchy.Add(scene);

            ModifyWindows();

            if (LevelForEditing)
                scene.Load();
#endif
            return scene;
        }

        public GameObject AddObject(GameObject gobject)
        {
            return AddObject(gobject, MyScene);
        }

        public GameObject AddObject(GameObject gobject, SceneObject scene)
        {
            if (!gobject.GetType().IsSubclassOf(typeof(SceneObject)))
            {
                if (Scenes.Contains(scene))
                {
                    scene.Add(gobject);
#if EDITOR && WINDOWS
                    if (LevelForEditing)
                        ModifyWindows();
#endif
                }
            }
            else
                AddScene((SceneObject)gobject);

            return gobject;
        }

        public void RemoveScene(SceneObject scene)
        {
            if (Scenes.Contains(scene))
                Scenes.Remove(scene);
#if EDITOR && WINDOWS
            if (MySceneHieararchy != null)
                MySceneHieararchy.Remove(scene);
#endif
            if (MyScene == scene)
                MyScene = Scenes.Count == 0 ? null : Scenes.First.Value;
            if (StartingScene == scene)
                StartingScene = Scenes.Count == 0 ? null : Scenes.First.Value;
#if EDITOR && WINDOWS
            ModifyWindows(); 
#endif
            scene.OnDestroy();
        }
#if EDITOR && WINDOWS
        public int FindUniqueID()
        {
            int ID = -1;
            bool Taken = true;

            while (Taken)
            {
                ID++;
                Taken = false;

                foreach (SceneObject scene in Scenes)
                    if (scene.IdNumber == ID)
                        Taken = true;
                    else
                        foreach(GameObject g in scene.Children)
                            if (g.IdNumber == ID)
                            {
                                Taken = true;
                                break;
                            }
            }
            

            return ID;
        }
#endif

        public GameObject FindObject(int ID)
        {
            foreach (SceneObject s in Scenes)
            {
                GameObject g = s.FindObject(ID);
                if (g != null)
                    return g;
            }
            return null;
        }

        public GameObject FindObject(string Name)
        {
            foreach (SceneObject s in Scenes)
            {
                GameObject g = s.FindObject(Name);
                if (g != null)
                    return g;
            }
            return null;
        }

        public SceneObject FindScene(string Name)
        {
            foreach (SceneObject s in Scenes)
                if (s.Name.get().Equals(Name))
                    return s;
            return null;
        }

        public GameObject FindObject(string Name, Type T)
        {
            foreach (SceneObject s in Scenes)
            {
                GameObject g = s.FindObject(Name, T);
                if (g != null)
                    return g;
            }
            return null;
        }

        public GameObject FindObject(Type T)
        {
            foreach (SceneObject s in Scenes)
            {
                GameObject g = s.FindObject(T);
                if (g != null)
                    return g;
            }
            return null;
        }

        public TextureCubeReference getTextureCube(string Name)
        {
            foreach (SceneObject scene in Scenes)
            {
                TextureCubeReference tc = scene.getTextureCube(Name);
                if (tc != null)
                    return tc;
            }
            return null;
        }

        public void Write(BinaryWriter Writer, bool SafeWrite)
        {
            try
            {
                SaveHelper.MyWriter = Writer;
                Level.SafeWrite = SafeWrite;

                Writer.Write((byte)0);

                Writer.Write((byte)(SafeWrite ? 1 : 0));

                Writer.Write((Int32)Scenes.Count());

                foreach (SceneObject Child in Scenes)
                {
                    Child.WriteFile(Writer);

                    Writer.Write((Int32)Child.Children.Count);

                    foreach (GameObject g in Child.Children)
                        g.WriteFile(Writer);
                }

                Console.Write("cardLevel Writer Position: " + Writer.BaseStream.Position + "\n");
            }
            catch (Exception e)
            {
                MasterManager.e = e;
#if EDITOR && WINDOWS
                Console.WriteLine(e.Message);
                DialogManager.ShowBox(e.Message);
                throw e;
#endif
            }
        }

        public void Read(BinaryReader Reader)
        {
            for (int i = 0; i < 4; i++)
            {
                if (LoadingReader == null)
                {
                    LoadingReader = Reader;
                    Loading = true;
                    LoadingObjectsLoaded = 0;
                    LoadingScenesLoaded = 0;

                    LoadingProgressBar = 0;
                    LoadingProgressBarMax = 0;

                    postLoadingStage = PostLoadingStage.HierarchyChildren;
                    PostLoadingCurrentScene = null;
                    PostLoadingCurrentObject = null;

                    LastLoadTime = Game1.gameTime;

                    LoadingTagsCount = Reader.ReadByte();
                    LoadingSaveType = Reader.ReadByte();
                    Level.SafeWrite = LoadingSaveType == 1;
                    LoadingSceneCount = Reader.ReadInt32();
                    LoadingScene = null;

                    LoadingLevels.AddLast(this);
                }
                else
                    Reader = LoadingReader;

                SaveHelper.MyReader = Reader;

                if (LoadingSceneCount > 0)
                {
                    if (LoadingScene == null)
                    {
                        LoadingScene = (SceneObject)GameObject.ReadFile(LoadingReader, this, null);
                        LoadingObjectCount = Reader.ReadInt32();
                        LoadingScenesLoaded++;
                        LoadingObjectsLoaded = 0;

                        LoadingProgressBarMax += LoadingObjectCount * 4;
                    }
                    else
                    {
                        if (LoadingObjectsLoaded < LoadingObjectCount)
                        {
                            GameObject.ReadFile(Reader, this, LoadingScene);
                            LoadingObjectsLoaded++;
                            LoadingProgressBar++;
                        }
                        else if (LoadingScenesLoaded < LoadingSceneCount)
                        {
                            LoadingScene = (SceneObject)GameObject.ReadFile(Reader, this, null);
                            LoadingObjectCount = Reader.ReadInt32();
                            LoadingScenesLoaded++;
                            LoadingProgressBar++;
                        }
                    }
                }

                if (LoadingScenesLoaded == LoadingSceneCount && LoadingObjectCount == LoadingObjectsLoaded)
                {
                    LoadingProgressBar++;

                    switch (postLoadingStage)
                    {
                        case PostLoadingStage.HierarchyChildren:

                            if (PostLoadingCurrentScene == null)
                            {
                                PostLoadingCurrentScene = Scenes.First;
                                PostLoadingCurrentObject = PostLoadingCurrentScene.Value.Children.First;
                                PostLoadingCurrentScene.Value.FindHierarchyChildren();
                                LoadingProgressBar++;
                            }
                            PostLoadingCurrentObject.Value.FindHierarchyChildren();

                            if (PostLoadingCurrentObject.Next != null)
                                PostLoadingCurrentObject = PostLoadingCurrentObject.Next;
                            else
                            {
                                if (PostLoadingCurrentScene.Next != null)
                                {
                                    PostLoadingCurrentScene = PostLoadingCurrentScene.Next;
                                    PostLoadingCurrentScene.Value.FindHierarchyChildren();
                                    LoadingProgressBar++;
                                }
                                else
                                {
                                    PostLoadingCurrentScene = null;
                                    postLoadingStage = PostLoadingStage.PostRead;
                                }
                            }

                            break;

                        case PostLoadingStage.PostRead:

                            if (PostLoadingCurrentScene == null)
                            {
                                PostLoadingCurrentScene = Scenes.First;
                                PostLoadingCurrentObject = PostLoadingCurrentScene.Value.Children.First;
                                PostLoadingCurrentScene.Value.PostRead();
                                LoadingProgressBar++;
                            }
                            PostLoadingCurrentObject.Value.PostRead();

                            if (PostLoadingCurrentObject.Next != null)
                                PostLoadingCurrentObject = PostLoadingCurrentObject.Next;
                            else
                            {
                                if (PostLoadingCurrentScene.Next != null)
                                {
                                    PostLoadingCurrentScene = PostLoadingCurrentScene.Next;
                                    PostLoadingCurrentScene.Value.PostRead();
                                    LoadingProgressBar++;
                                }
                                else
                                {
                                    PostLoadingCurrentScene = null;
                                    if (!LevelForEditing)
                                        postLoadingStage = PostLoadingStage.CreateInGame;
                                    else
                                        postLoadingStage = PostLoadingStage.Ready;
                                }
                            }

                            break;

                        case PostLoadingStage.CreateInGame:

                            if (PostLoadingCurrentScene == null)
                            {
                                PostLoadingCurrentScene = Scenes.First;
                                PostLoadingCurrentObject = PostLoadingCurrentScene.Value.Children.First;
                                PostLoadingCurrentScene.Value.CreateInGame();
                                LoadingProgressBar++;
                            }
                            PostLoadingCurrentObject.Value.CreateInGame();

                            if (PostLoadingCurrentObject.Next != null)
                                PostLoadingCurrentObject = PostLoadingCurrentObject.Next;
                            else
                            {
                                if (PostLoadingCurrentScene.Next != null)
                                {
                                    PostLoadingCurrentScene = PostLoadingCurrentScene.Next;
                                    PostLoadingCurrentScene.Value.CreateInGame();
                                    LoadingProgressBar++;
                                }
                                else
                                {
                                    PostLoadingCurrentScene = null;
                                    postLoadingStage = PostLoadingStage.Ready;
                                }
                            }

                            break;

                        case PostLoadingStage.Ready:

                            LoadingReader = null;
                            Loading = false;
                            Console.Write("cardLevel Read Position: " + Reader.BaseStream.Position + "\n");
                            Reader.BaseStream.Close();
                            return;
                    }
                }
            }
        }

        public void Draw()
        {
            Game1.graphicsDevice.SetRenderTarget(null);
            Game1.graphicsDevice.Clear(Color.Black);

#if !EDITOR
            //try
#endif
            {
                if (!Loading && MyScene != null)
                {
#if EDITOR && WINDOWS
                    Render.RenderTime.Reset();
#endif

                    if (!MyScene.CanLoad)
                        MyScene.Load();

#if !EDITOR
                if (MyScene.WindowSize.X != Game1.ResolutionX || MyScene.WindowSize.Y != Game1.ResolutionY)
                    MyScene.SetWindowSize(new Vector2(Game1.ResolutionX, Game1.ResolutionY));
#endif
#if EDITOR

                    if (MyScene.WindowSize.X != Game1.self.Window.ClientBounds.Width || MyScene.WindowSize.Y != Game1.self.Window.ClientBounds.Height)
                        MyScene.SetWindowSize(new Vector2(Game1.self.Window.ClientBounds.Width, Game1.self.Window.ClientBounds.Height));
#endif

#if WINDOWS && EDITOR
                    if (!LevelForEditing || Game1.self.IsActive)
#endif
                        MyScene.PreDraw();

                    MyScene.Draw2D(GameObjectTag.SceneDrawScene);
                    Game1.graphicsDevice.SetRenderTarget(null);
                    MasterManager.SetViewportToFullscreen();
                }
            }
#if !EDITOR
            //catch (Exception e)
            //{ MasterManager.e = e; }
#endif
            loadingScreen.Draw(Math.Min(LoadingProgressBar, LoadingProgressBarMax), LoadingProgressBarMax, LoadingProgressBarAlpha);
            PlayerProfile.Draw();
        }

        public static void ObjectCreate(GameObject o, SceneObject ParentScene, Level ParentLevel)
        {
            ReferenceLevel = ParentLevel;
            ReferenceScene = ParentScene;
            ReferenceObject = o;
        }

    }
}
