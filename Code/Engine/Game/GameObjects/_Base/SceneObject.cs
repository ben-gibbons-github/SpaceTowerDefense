using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class SceneObject : GameObject, IEnumerable
    {
#if EDITOR && WINDOWS
        public FormHolder TopbarTools;
        public FormHolder WindowTools;
        public FormHolder HierarchyHolder;
        public FormHolder ObjectPropertiesHolder;
        
        public LinkedList<GameObject> SelectedGameObjects = new LinkedList<GameObject>();
        
        public Vector2 ObjectProperitesBuffer = new Vector2(10);
        public Hierarchy ChildHierarchy;
        public int ThumbnailCounter = 60;
        public int MaxThumbnailCounter = 30;
        public RenderTarget2D SceneThumbnail;
        public Rectangle ThumbRectangle;
        public Rectangle ThumbMouseRectangle;
        public Vector2 ThumbNamePosition;
        public SceneHierarchy ParentSceneHierarchy;
        
#endif
        private bool NeedsToClear = false;

        public LinkedList<GameObject> WorldViewerChildren;
        public Dictionary<GameObjectTag, LinkedList<GameObject>> SortedChildren = new Dictionary<GameObjectTag, LinkedList<GameObject>>();
        public LinkedList<GameObject> Children = new LinkedList<GameObject>();
        public LinkedListNode<GameObject> ListNode;
        public SceneView[] Views;

        public LinkedList<GameObject> ObjectsToReadyTags = new LinkedList<GameObject>();
        public bool NeedsToReadyTags = false;

        private LinkedList<GameObject> ObjectsToClear = new LinkedList<GameObject>();

        public Color ClearColor = Color.Black;

        public Vector2 WindowSize = Vector2.Zero;

        public StringValue EffectDirectory;
        public StringValue ModelDirectory;
        public StringValue TextureDirectory;
        public bool NeedsPlayers = true;

        public FadeManager FadeManager;

        public LinkedList<StopwatchWrapper> Watches = new LinkedList<StopwatchWrapper>();
        private LinkedList<GameObject> SidelinedViewers = new LinkedList<GameObject>();

#if EDITOR
        public StopwatchWrapper UpdateTime;
        public StopwatchWrapper DrawTime;
#endif
        Type EnumerationType;

        public override void Create()
        {
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                HierarchyHolder = new FormHolder();
                HierarchyHolder.AddForm(ChildHierarchy = new Hierarchy(new Vector2(0), this));
                ChildHierarchy.AddHierarchyObject(this);
            }
#endif
#if EDITOR
            UpdateTime = new StopwatchWrapper("UpdateTime");
            DrawTime = new StopwatchWrapper("DrawTime");
#endif

            WorldViewerChildren = AddTag(GameObjectTag.WorldViewer);

            EffectDirectory = new StringValue("Effect Directory:", "Effects/");
            ModelDirectory = new StringValue("Model Directory:", "Models/");
            TextureDirectory = new StringValue("Texture Directory", "Textures/");
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
                AddRightClickEvent("Set as Starting Scene", SetAsStart);
#endif
            base.Create();
        }

        public virtual void PlayerJoinedEvent(PlayerProfile p)
        {

        }

        public virtual void PlayerQuitEvent(PlayerProfile p)
        {

        }

        public override void SetWindowSize(Vector2 WindowSize)
        {
            this.WindowSize = WindowSize;

            int c = WorldViewerChildren.Count();
            UpdateViews();

            foreach (GameObject g in Children)
                g.SetWindowSize(WindowSize);

            base.SetWindowSize(WindowSize);
        }
#if EDITOR && WINDOWS
        private void SetAsStart(Button b)
        {
            ParentLevel.SetStartingScene(this);
        }

        public void NewSelected(GameObject g)
        {
            ClearSelected();
            AddSelected(g);

            UpdateSelected();
        }

        public void AddSelected(LinkedList<GameObject> l)
        {
            foreach (GameObject g in l)
                AddSelected(g);
        }

        public void AddSelected(GameObject g)
        {
            if ((Children.Contains(g) || g == this) && !SelectedGameObjects.Contains(g))
            {
                SelectedGameObjects.AddFirst(g);
                g.EditorSelected = true;

                UpdateSelected();
            }
        }

        public void RemoveSelected(GameObject g)
        {
            if ((Children.Contains(g) || g == this) && SelectedGameObjects.Contains(g))
            {
                SelectedGameObjects.Remove(g);
                g.EditorSelected = false;

                UpdateSelected();
            }
        }

        public void ClearSelected()
        {
            foreach (GameObject g in SelectedGameObjects)
                g.EditorSelected = false;
            SelectedGameObjects.Clear();

            UpdateSelected();
        }

        public override void UpdateEditor(GameTime gameTime)
        {
            base.UpdateEditor(gameTime);

            foreach (GameObject g in Children)
                g.UpdateEditor(gameTime);

            if (KeyboardManager.keyboardState.IsKeyDown(Keys.Delete))
            {
                LinkedList<GameObject> ToDestroy = new LinkedList<GameObject>();

                foreach (GameObject o in Children)
                    if (o.EditorSelected)
                        ToDestroy.AddLast(o);

                foreach (GameObject o in ToDestroy)
                    o.Destroy();
            }
        }

        public void UpdateSelected()
        {
            if (ObjectPropertiesHolder == null)
                ObjectPropertiesHolder = new FormHolder();

            ObjectPropertiesHolder.FormChildren.Clear();
            foreach (Form f in GameObject.CreateValueEditors(SelectedGameObjects))
                ObjectPropertiesHolder.AddForm(f);

            ArrayForms();

        }
#endif

        public void MakeFadeManager()
        {
            if (FadeManager == null)
                ParentLevel.AddObject(FadeManager = new FadeManager());
            FadeManager.self = FadeManager;
        }

        public override bool TriggerEvent(EventType Event, string[] args)
        {
            switch (Event)
            {
                case EventType.GotoLevel:
                    MakeFadeManager();
                    FadeManager.SetFadingTarget(args[0]);
                    return true;

                case EventType.GotoScene:
                    MakeFadeManager();
                    FadeManager.SetFadingTarget(ParentLevel.FindScene(args[0]));
                    return true;

                case EventType.SwapViews:
                    LinkedList<GameObject> tempSidelined = new LinkedList<GameObject>(SidelinedViewers);
                    SidelinedViewers.Clear();

                    LinkedList<GameObject> temp = new LinkedList<GameObject>(WorldViewerChildren);
                    foreach (GameObject o in temp)
                    {
                        o.RemoveTag(GameObjectTag.WorldViewer);
                        SidelinedViewers.AddLast(o);
                    }

                    foreach (GameObject o in tempSidelined)
                        o.AddTag(GameObjectTag.WorldViewer);

                    return true;
            }


            return base.TriggerEvent(Event, args);
        }

        public bool TriggerEvent(string Object, EventType Event, string[] args)
        {
            if (Name.get().Equals(Object))
            {
                TriggerEvent(Event, args);
                return true;
            }
            else
            {
                GameObject g = FindObject(Object);
                if (g != null)
                    return g.TriggerEvent(Event, args);
                else
                    return false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (NeedsToReadyTags)
            {
                foreach (GameObject o in ObjectsToReadyTags)
                    o.readyTags();
                ObjectsToReadyTags.Clear();
                NeedsToReadyTags = false;
            }
            if (NeedsToClear)
            {
                foreach (GameObject o in ObjectsToClear)
                    Clear(o);
                ObjectsToClear.Clear();
                NeedsToClear = false;
            }
        }

        public void UpdateViews()
        {
            int c = WorldViewerChildren.Count;
            Views = SceneView.GetViews(c, WindowSize);
            int i = 0;
            foreach (GameObject g in WorldViewerChildren)
            {
                if (g.GetType().GetInterfaces().Contains(typeof(WorldViewer2D)))
                {
                    WorldViewer2D worldViewer = (WorldViewer2D)g;
                    worldViewer.setSceneView(Views[i]);
                    Views[i].SetCamera(worldViewer.getCamera());
                }

                if (g.GetType().GetInterfaces().Contains(typeof(WorldViewer3D)))
                {
                    WorldViewer3D worldViewer = (WorldViewer3D)g;
                    worldViewer.setSceneView(Views[i]);
                    Views[i].SetCamera(worldViewer.getCamera(), WindowSize);
                }

                i++;
            }

            foreach (GameObject g in Children)
            {
                g.UpdateViewsEvent(Math.Min(WorldViewerChildren.Count, 4));
            }
        }

        public GameObject FindObject(int ID)
        {
            if (ID == IdNumber)
                return this;

            foreach (GameObject g in Children)
                if (g.IdNumber == ID)
                    return g;

            return null;
        }

        public GameObject FindObject(string Name)
        {
            if (this.Name.get().Equals(Name))
                return this;

            foreach (GameObject g in Children)
                if (g.Name.get().Equals(Name))
                    return g;

            return null;
        }

        public GameObject FindObject(string Name , Type T)
        {
            if (this.Name.get().Equals(Name) && GetType().Equals(T))
                return this;

            foreach (GameObject g in Children)
                if (g.Name.get().Equals(Name) && g.GetType().Equals(T))
                    return g;

            return null;
        }

        public GameObject FindObject(Type T)
        {
            if (GetType().Equals(T))
                return this;

            foreach (GameObject g in Children)
                if (g.GetType().Equals(T))
                    return g;

            return null;
        }

        public TextureCubeReference getTextureCube(string Name)
        {
            foreach(GameObject o in Children)
                if (o.GetType().IsSubclassOf(typeof(ReturnsTextureCube)) && Name.Equals(o.Name.get()))
                {
                    ReturnsTextureCube tc = (ReturnsTextureCube)o;
                    return tc.returnTextureCube();
                }
            return null;
        }

#if EDITOR && WINDOWS
        public void ArrayForms()
        {
            Vector2 FormPosition = new Vector2(20);
            foreach (Form f in ObjectPropertiesHolder.FormChildren)
            {

                f.SetPosition(FormPosition);
                
               // if (f != ObjectPropertiesHolder.FormChildren[0])
                {
                    if (!f.StackRight || f.Size.X + FormPosition.X > ObjectProperties.self.MyRectangle.Width)
                        FormPosition.Y += f.Size.Y + ObjectProperitesBuffer.Y;
                    else
                        FormPosition.X += f.Size.X + ObjectProperitesBuffer.X;

                }
            }
        }
#endif
        public void AddWindowForm(Form f)
        {
#if EDITOR && WINDOWS
            if (ParentLevel == null)
                ParentLevel = Level.ReferenceLevel;
            if (ParentLevel.LevelForEditing)
            {
                if (WindowTools == null)
                    WindowTools = new FormHolder();
                WindowTools.AddForm(f);
            }
#endif
        }

        public void AddToolbarForm(Form f)
        {
#if EDITOR && WINDOWS
            if (ParentLevel == null)
                ParentLevel = Level.ReferenceLevel;
            if (ParentLevel.LevelForEditing)
            {
                if (TopbarTools == null)
                    TopbarTools = new FormHolder();
                WindowTools.AddForm(f);
            }
#endif
        }

        public StopwatchWrapper Add(StopwatchWrapper s)
        {
            Watches.AddLast(s);
            return s;
        }

        public override GameObject Add(GameObject o)
        {
            if (!o.Created)
            {
                o.TagsReady = ParentLevel.LevelForEditing;
                Level.ObjectCreate(o, this, ParentLevel);
#if EDITOR && WINDOWS
                if (ParentLevel.LevelForEditing)
                    o.IdNumber = ParentLevel.FindUniqueID();
#endif
                o.SetParents(ParentLevel, this);
                o.Create();
                if (!o.TagsReady)
                    ObjectsToReadyTags.AddFirst(o);
            }

            AddHierarchyObject(o);

            Children.AddFirst(o);

            ReferenceGameObject = null;

            if (CanLoad)
                o.Load();

            return o;
        }

        private void Clear(GameObject o)
        {
            Children.Remove(o);

            foreach (GameObjectTag Tag in o.Tags)
            {
                LinkedList<GameObject> list = GetList(Tag);
                if (list != null && list.Contains(o))
                    list.Remove(o);
            }

            RemoveHierarchyObject(o);
#if EDITOR && WINDOWS
            if (SelectedGameObjects.Contains(o))
                SelectedGameObjects.Remove(o);
#endif

            o.OnDestroy();
        }

        public void Remove(GameObject o)
        {
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
                Clear(o);
            else
#endif
            {
                NeedsToClear = true;
                ObjectsToClear.AddFirst(o);
            }
        }

        public override LinkedList<GameObject> AddTag(GameObjectTag Tag)
        {
            LinkedList<GameObject> r;
            Tags.AddFirst(Tag);
            SortedChildren.Add(Tag,r = new LinkedList<GameObject>());

            foreach (GameObject o in Children)
                if (o.Tags.Contains(Tag))
                    SortedChildren[Tag].AddFirst(o);
            return r;
        }

        public override void RemoveTag(GameObjectTag Tag)
        {
            SortedChildren.Remove(Tag);
            base.RemoveTag(Tag);
        }

        public LinkedList<GameObject> GetList(GameObjectTag Tag)
        {
            return Tags.Contains(Tag) ? SortedChildren[Tag] : null;
        }
#if EDITOR && WINDOWS
        public Vector2 UpdateThumbnail(Vector2 DrawPosition)
        {
            if (ThumbRectangle == null)
            {
                ThumbRectangle = new Rectangle((int)DrawPosition.X, (int)DrawPosition.Y, ParentSceneHierarchy.ItemSize, ParentSceneHierarchy.ItemSize);
                ThumbMouseRectangle = new Rectangle();
            }

            ThumbRectangle.X = (int)DrawPosition.X;
            ThumbRectangle.Y = (int)DrawPosition.Y;
            ThumbRectangle.Width = ParentSceneHierarchy.ItemSize;
            ThumbRectangle.Height = ParentSceneHierarchy.ItemSize;

            ThumbMouseRectangle.X = (int)DrawPosition.X - ParentSceneHierarchy.ItemSeperation/2;
            ThumbMouseRectangle.Y = (int)DrawPosition.Y;
            ThumbMouseRectangle.Width = ParentSceneHierarchy.ItemSize + ParentSceneHierarchy.ItemSeperation;
            ThumbMouseRectangle.Height = ParentSceneHierarchy.ItemSize + 20;

            ThumbNamePosition = DrawPosition + new Vector2(ParentSceneHierarchy.ItemSize / 2 - FormFormat.NormalFont.MeasureString(Name.get()).X / 2, ParentSceneHierarchy.ItemSize + 4);

            return DrawPosition + new Vector2(ParentSceneHierarchy.ItemSeperation + ParentSceneHierarchy.ItemSize, 0);
        }

        public SceneObject ReturnMouseOverThumbnail(Window Updater)
        {
            return ThumbMouseRectangle.Contains(Updater.RelativeMousePoint) ? this : null;
        }

        public void DrawToThumbnail()
        {
            ThumbnailCounter++;
            if (ThumbnailCounter > MaxThumbnailCounter)
            {
                ThumbnailCounter = 0;
                if (SceneThumbnail == null)
                    SceneThumbnail = new RenderTarget2D(Game1.graphicsDevice, 64, 64);

                Game1.graphicsDevice.SetRenderTarget(SceneThumbnail);
                Game1.graphicsDevice.Clear(Color.Black);
                Draw2D(GameObjectTag.SceneDrawScene);
                Game1.graphicsDevice.SetRenderTarget(null);
                Game1.graphicsDevice.Clear(Color.Black);
            }
        }

        public void DrawThumbnail()
        {
            if (ThumbRectangle != null)
            {
                if (SceneThumbnail != null)
                    Game1.spriteBatch.Draw(SceneThumbnail, ThumbRectangle, Color.White);
                Render.DrawSolidRect(ThumbRectangle, ParentLevel.MyScene == this ? Color.Red : ParentLevel.StartingScene == this ? Color.LightGreen : Color.White);
                Game1.spriteBatch.DrawString(FormFormat.NormalFont, Name.get(), ThumbNamePosition, ParentLevel.MyScene == this ? FormFormat.SelectedTextColor : FormFormat.TextColor);

                if (ParentLevel.StartingScene == this)
                {
                    Render.DrawLine(new Vector2(ThumbRectangle.X + 16, ThumbRectangle.Y + 16), new Vector2(ThumbRectangle.X + 16, ThumbRectangle.Y + 48), Color.LightGreen);
                    Render.DrawLine(new Vector2(ThumbRectangle.X + 16, ThumbRectangle.Y + 16), new Vector2(ThumbRectangle.X + 48, ThumbRectangle.Y + 32), Color.LightGreen);
                    Render.DrawLine(new Vector2(ThumbRectangle.X + 16, ThumbRectangle.Y + 48), new Vector2(ThumbRectangle.X + 48, ThumbRectangle.Y + 32), Color.LightGreen);
                }

                SceneSelect.self.NeedsToRedraw = true;
            }
        }

        public override void LeftClick(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds < DoubleClickTime + 300)
                ParentLevel.SetScene(this);

            base.LeftClick(gameTime);
        }


        public void DisposeRenderTarget()
        {
            if (SceneThumbnail != null)
            {
                SceneThumbnail.Dispose();
                SceneThumbnail = null;
            }
        }
#endif
        public override void Destroy()
        {
#if EDITOR && WINDOWS
            DisposeRenderTarget();
#endif
            GameObject[] sc = Children.ToArray();
            foreach (GameObject g in sc)
                g.Destroy();
            ParentLevel.RemoveScene(this);
        }

        public override void Load()
        {
            foreach (GameObject child in Children)
                child.Load();

            base.Load();
        }

        public override void UnLoad()
        {
            foreach (GameObject child in Children)
                child.UnLoad();

            base.UnLoad();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public SceneObject Enumerate(Type T)
        {
            EnumerationType = T;
            return this;
        }

        public SceneEnum GetEnumerator()
        {
            return new SceneEnum(EnumerationType, Children);
        }
    }


    public class SceneEnum : IEnumerator
    {
        Type T;
        LinkedList<GameObject> Children;
        LinkedListNode<GameObject> CurrentNode;
        bool Started = false;

        public SceneEnum(Type T, LinkedList<GameObject> Children)
        {
            this.T = T;
            this.Children = Children;
        }

        public virtual bool MoveNext()
        {
            if (Started)
                CurrentNode = CurrentNode.Next;
            else
            {
                CurrentNode = Children.First;
                Started = true;
            }

            while (CurrentNode != null && CurrentNode.Value != null && (!CurrentNode.Value.GetType().IsSubclassOf(T) && !CurrentNode.Value.GetType().Equals(T)))
                CurrentNode = CurrentNode.Next;

            return CurrentNode != null && CurrentNode.Value != null;
        }

        public void Reset()
        {
            Started = false;
        }

        object IEnumerator.Current
        {
            get
            {
                return CurrentNode.Value;
            }
        }

        public GameObject Current
        {
            get
            {
                try
                {
                    return CurrentNode.Value;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
