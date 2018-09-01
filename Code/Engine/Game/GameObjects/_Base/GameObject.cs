using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.EffectParameters;


namespace BadRabbit.Carrot
{
    public class GameObject : HierarchyParent
    {
#if EDITOR && WINDOWS
        public static Dictionary<GameObject, GameObject> CloneDictionary;
#endif
        public static GameObject ReferenceGameObject;

        public Level ParentLevel;
        public SceneObject ParentScene;

        public bool TagsReady = false;
        public bool Created = false;
        public LinkedList<GameObjectTag> Tags = new LinkedList<GameObjectTag>();
        public LinkedList<Value> Values = new LinkedList<Value>();
        public LinkedList<EffectValue> ConstructEffects = new LinkedList<EffectValue>();
        public bool ConstructEffectsNeeded = false;
        public LinkedList<BasicEffectParameter> ParameterQue = new LinkedList<BasicEffectParameter>();
        public bool ParameterQueNeeded = false;

        public int IdNumber = 0;
        public StringValue Name;

        public static int MasterIdTracker;
        public bool CanLoad = false;

        public LinkedList<GameObject> HierarchyChildren = new LinkedList<GameObject>();
        public LinkedList<int> IdsOfChildren = new LinkedList<int>();
        public HierarchyParent hierarchyParent;
        

#if EDITOR && WINDOWS
        public Hierarchy TopHierarchyParent;
        public Dictionary<string, ClickEvent> RightClickActions;

        public bool EditorSelected = false;
        public bool EditorChildSelected = false;
        public bool HierarchyExpanded = true;
        public bool CreatedHierarchyRectangles = false;

        public Vector2 HierarchyDrawPosition = Vector2.Zero;
        public Vector2 HierarchyTextPosition = Vector2.Zero;
        public int HierarchyLineHeight = 0;
        public Rectangle HierarchyHorizontalLine;
        public Rectangle HierarchyVerticleLine;
        public Rectangle HierarchyBox;
        public Rectangle HierarchyBoxOutline;
        public Rectangle HierarchyPlusVerticle;
        public Rectangle HierarchyPlusHorizontal;
        public Rectangle HierarchyMouseRectangle;
        public Rectangle HierarchyDragRectangle;
        public Rectangle HierarchyDropRectangle;
        public int HierarchyDropBorder = 3;
        public double DoubleClickTime = 0;

        public bool MouseHighlighted;
        public bool MouseExpandHighlighted;
#endif

        public virtual void SetParents(Level ParentLevel, SceneObject ParentScene)
        {
            this.ParentScene = ParentScene;
            this.ParentLevel = ParentLevel;
        }

        public virtual void Create()
        {
            Name = new StringValue("Name: ", GetType().Name);

            Values.Remove(Name);
            Values.AddFirst(Name);

#if EDITOR && WINDOWS
            AddRightClickEvent("Expand All", ExpandAll);
            AddRightClickEvent("Colapse All", ColapseAll);
            AddRightClickEvent("Clone", clone);
            AddRightClickEvent("Delete", Destroy);
#endif

            Created = true;
            ModifyCollection();
        }

        public virtual bool TriggerEvent(EventType Event, string[] args)
        {
            switch (Event)
            {
                case EventType.Delete:
                    Destroy();
                    return true;

                case EventType.ChangeName:
                    Name.set(args[0]);
                    return true;

                case EventType.AddTag:
                    foreach (GameObjectTag t in GameObjectTagList.AllTags)
                        if (t.ToString().ToLower().Equals(args[0]))
                        {
                            AddTag(t);
                            return true;
                        }
                    return true;

                case EventType.RemoveTag:
                    foreach (GameObjectTag t in Tags)
                        if (t.ToString().ToLower().Equals(args[0]))
                        {
                            RemoveTag(t);
                            return true;
                        }
                    return true;

                case EventType.ViewOn:
                    if (!Tags.Contains(GameObjectTag.WorldViewer))
                        AddTag(GameObjectTag.WorldViewer);
                    return true;

                case EventType.ViewOff:
                    if (Tags.Contains(GameObjectTag.WorldViewer))
                        RemoveTag(GameObjectTag.WorldViewer);
                    return true;

                case EventType.SetValue:
                    Value v = FindValue(args[0]);
                    if (v != null)
                        v.SetFromArgs(args);
                    return true;
            }
            return false;
        }

        public virtual void CreateInGame()
        {

        }

        public virtual void SetWindowSize(Vector2 Size)
        {
            
        }

#if EDITOR && WINDOWS
        private void clone(Button b)
        {
            if (ParentScene != null)
                ParentScene.ClearSelected();
            else
            {
                SceneObject s = (SceneObject)this;
                s.ClearSelected();
            }

            Clone(null, ParentScene);
        }
#endif

        public void WriteFile(BinaryWriter Writer)
        {
            Writer.Write(GetType().Name);

            Writer.Write((Int32)this.IdNumber);

            Writer.Write((Int32)HierarchyChildren.Count);
            foreach (GameObject Child in HierarchyChildren)
                Writer.Write((Int32)Child.IdNumber);

            if (!Level.SafeWrite)
                foreach (Value v in Values)
                    v.Write(Writer);
            else
            {
                Writer.Write((Int32)Values.Count);
                foreach (Value v in Values)
                {
                    Writer.Write(v.Name);
                    Writer.Write((byte)Value.ReturnByteType(v));
                    v.Write(Writer);
                }
            }
        }

        public Value FindValue(string Name)
        {
            foreach (Value v in Values)
                if (v.Name.Equals(Name))
                    return v;
            return null;
        }

        public Value FindValue(string Name, byte Type)
        {
            foreach (Value v in Values)
                if (v.Name.Equals(Name) && Value.ReturnByteType(v) == Type)
                    return v;
            return null;
        }

        public Value FindValueLoose(string Name, byte Type)
        {
            foreach (Value v in Values)
                if ((v.Name.Equals(Name) || v.Name.Contains(Name) || Name.Contains(v.Name)) && Value.ReturnByteType(v) == Type)
                    return v;
            return null;
        }

        public static GameObject ReadFile(BinaryReader Reader, Level ParentLevel, SceneObject ParentScene)
        {
            string TypeString = Reader.ReadString();
            GameObject g = CreatorBasic.ReturnObjectOfType(TypeString);
            ParentLevel.AddObject(g, ParentScene);

            return ReadObject(Reader, ParentLevel, g);
        }

        public static GameObject ReadObject(BinaryReader Reader, Level ParentLevel, GameObject g)
        {
            g.IdNumber = Reader.ReadInt32();

            Int32 ChildCount = Reader.ReadInt32();
            for (int i = 0; i < ChildCount; i++)
            {
#if EDITOR && WINDOWS
                if (ParentLevel.LevelForEditing)
                    g.IdsOfChildren.AddLast(Reader.ReadInt32());
                else
#endif
                    g.IdsOfChildren.AddFirst(Reader.ReadInt32());
            }

            if (!Level.SafeWrite)
            {
                LinkedListNode<Value> v = g.Values.First;
                while (v != null)
                {
                    v.Value.Read(Reader);
                    v = v.Next;
                }
            }
            else
            {
                int ValueCount = Reader.ReadInt32();
                for (int i = 0; i < ValueCount; i++)
                {
                    string ValueName = Reader.ReadString();
                    byte ByteType = Reader.ReadByte();
                    Value v = g.FindValue(ValueName, ByteType);

                    if (v == null)
                        g.FindValueLoose(ValueName, ByteType);

                    if (v == null)
                        Value.DummyRead(ByteType, Reader);
                    else
                        v.Read(Reader);
                }
            }

            return g;
        }

        public void FindHierarchyChildren()
        {
            foreach (int ID in IdsOfChildren)
                AddHierarchyObject(ParentLevel.FindObject(ID));
        }

        public void WriteClone(BinaryWriter Writer)
        {
            SaveHelper.MyWriter = Writer;
            foreach (Value v in Values)
                v.Write(Writer);
        }

        public void ReadClone(BinaryReader Reader)
        {
            SaveHelper.MyReader = Reader;

            foreach (Value v in Values)
                v.Read(Reader);
        }

#if EDITOR && WINDOWS
        public void FillBlankClones()
        {
            foreach (GameObject g in HierarchyChildren)
            {
                CloneDictionary.Add(g, null);
                g.FillBlankClones();
            }
        }

        public GameObject Clone(GameObject Parent, SceneObject Scene)
        {
            bool DictionaryCreator = CloneDictionary == null;
            if (DictionaryCreator)
                CloneDictionary = new Dictionary<GameObject, GameObject>();

            GameObject g = CreatorBasic.ReturnObjectOfType(GetType());

            if (!CloneDictionary.ContainsKey(this))
                CloneDictionary.Add(this, g);
            else
                CloneDictionary[this] = g;

            FillBlankClones();
            
            ReferenceGameObject = g;
            ParentLevel.AddObject(g, Scene);

            Stream s = new MemoryStream();
            WriteClone(new BinaryWriter(s));
            Console.Write(s.Length);
            s.Position = 0;
            g.ReadClone(new BinaryReader(s));
            s.Close();

            if (Scene != null)
                Scene.AddSelected(g);


            SceneObject SceneParent = Scene;

            if (!g.GetType().IsSubclassOf(typeof(SceneObject)))
            {
                if (Parent == null)
                    hierarchyParent.AddHierarchyObject(g);
                else
                    Parent.AddHierarchyObject(g);
            }
            else
                SceneParent = (SceneObject)g;

            foreach (GameObject child in HierarchyChildren)
                if (child != this)
                    child.Clone(g,SceneParent);

            g.PostRead();

            if (ObjectListValue.LinkedObjects.ContainsKey(this))
                foreach (ObjectListValue l in ObjectListValue.LinkedObjects[this])
                    if (!CloneDictionary.ContainsValue(l.Parent) && !CloneDictionary.ContainsKey(l.Parent))
                        l.add(g);

            if (DictionaryCreator)
                CloneDictionary = null;

            return g;
        }
#endif
        public void PostRead()
        {
            foreach (Value v in Values)
                v.PostRead();
        }

        private void Destroy(Button b)
        {
            Destroy();
        }

#if EDITOR && WINDOWS
        private void ColapseAll(Button b)
        {
            HierarchyCollapse();
        }

        private void HierarchyCollapse()
        {
            HierarchyExpanded = false;

            foreach (GameObject g in HierarchyChildren)
                g.HierarchyCollapse();

            ModifyCollection();
        }

        private void ExpandAll(Button b)
        {
            HierarchyExpand();
        }

        private void HierarchyExpand()
        {
            HierarchyExpanded = true;

            foreach (GameObject g in HierarchyChildren)
                g.HierarchyExpand();

            ModifyCollection();
        }

        public void CreateHierarchyRectangles()
        {
            if (!CreatedHierarchyRectangles)
            {
                CreatedHierarchyRectangles = true;
                HierarchyHorizontalLine = new Rectangle(0, 0, 1, 1);
                HierarchyVerticleLine = new Rectangle(0, 0, 1, 1);
                HierarchyBoxOutline = new Rectangle();
                HierarchyBox = new Rectangle();

                HierarchyPlusHorizontal = new Rectangle(0, 0, 1, 1);
                HierarchyPlusVerticle = new Rectangle(0, 0, 1, 1);
                HierarchyMouseRectangle = new Rectangle(0, 0, 0, 0);
                HierarchyDragRectangle = new Rectangle(0, 0, 0, 0);
                HierarchyDropRectangle = new Rectangle(0, 0, 0, 0);
            }
        }
#endif
        public virtual void UpdateEditor(GameTime gameTime)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Update2(GameTime gameTime)
        {

        }

        public virtual void PreDraw()
        {

        }

        public virtual void Draw2D(GameObjectTag DrawTag)
        {
            DrawEffects();
        }

        public virtual void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            DrawEffects();
        }

        private void DrawEffects()
        {
            if (ConstructEffectsNeeded)
            {
                foreach (EffectValue v in ConstructEffects)
                    v.ConstructParameters();
                ConstructEffects.Clear();
                ConstructEffectsNeeded = false;
            } 
            if (ParameterQueNeeded)
            {
                foreach (BasicEffectParameter p in ParameterQue)
                    p.UpdateParameter();
                ParameterQue.Clear();
                ParameterQueNeeded = false;
            }
        }

        public virtual void Load()
        {
            CanLoad = true;
            foreach (Value v in Values)
                v.Load();
        }

        public virtual void UnLoad()
        {
            CanLoad = false;
        }

#if EDITOR && WINDOWS
        public void DrawAsHierarchy()
        {
            Game1.spriteBatch.DrawString(TopHierarchyParent.Font, Name.get(), HierarchyTextPosition, EditorSelected ? TopHierarchyParent.SelectedTextColor : TopHierarchyParent.HighlightedObject == this ? TopHierarchyParent.HighlightedTextColor : TopHierarchyParent.TextColor);
            Game1.spriteBatch.Draw(TopHierarchyParent.LineTexture, HierarchyHorizontalLine, TopHierarchyParent.LineColor);

            if(HierarchyDragRectangle.Contains(TopHierarchyParent.RelativeMouse) && MouseManager.DraggedObject == this)
                MouseManager.DraggedMouseAlpha = 1;

            if ((HierarchyDragRectangle.Contains(TopHierarchyParent.RelativeMouse) && MouseManager.DraggedObject == null))
                Render.DrawOutlineRect(HierarchyDragRectangle, 1, TopHierarchyParent.LineColor);
            else if (MouseManager.DraggedObject == this)
                Render.DrawOutlineRect(HierarchyDragRectangle, 1, TopHierarchyParent.LineColor * Math.Max(0.1f,MouseManager.DraggedMouseAlpha));
            else
            {
                Render.DrawOutlineRect(HierarchyDragRectangle, 1, TopHierarchyParent.LineColor * 0.1f);
                if (TopHierarchyParent.HighlightedObject == this && MouseManager.DraggedObject != null && MouseManager.DraggedObject.GetType().IsSubclassOf(typeof(GameObject)))
                    Render.DrawSolidRect(HierarchyDropRectangle, TopHierarchyParent.LineColor);
            }
        
            if (HierarchyChildren.Count() > 0)
            {
                Game1.spriteBatch.Draw(TopHierarchyParent.LineTexture, HierarchyBoxOutline, TopHierarchyParent.LineColor);
                Game1.spriteBatch.Draw(TopHierarchyParent.LineTexture, HierarchyBox, TopHierarchyParent.BoxColor);
                Game1.spriteBatch.Draw(TopHierarchyParent.LineTexture, HierarchyPlusHorizontal, TopHierarchyParent.LineColor);
                if (!HierarchyExpanded)
                    Game1.spriteBatch.Draw(TopHierarchyParent.LineTexture, HierarchyPlusVerticle, TopHierarchyParent.LineColor);
                else
                    Game1.spriteBatch.Draw(TopHierarchyParent.LineTexture, HierarchyVerticleLine, TopHierarchyParent.LineColor);
            }

            if (HierarchyExpanded)
                foreach (GameObject Child in HierarchyChildren)
                    Child.DrawAsHierarchy();

        }
#endif


        public void readyTags()
        {
            TagsReady = true;
            foreach (GameObjectTag Tag in Tags)
                ReadyTag(Tag);
            TagsReady = false;
        }

        public virtual void UpdateViewsEvent(int ViewCount)
        {

        }

        private void ReadyTag(GameObjectTag Tag)
        {
            if (TagsReady)
            {
                if (Tags.Contains(Tag))
                {
                    if (ParentScene != null && ParentScene.Tags.Contains(Tag))
                    {
                        LinkedList<GameObject> l = ParentScene.GetList(Tag);
                        if (l != null && !l.Contains(this))
                        {
                            l.AddFirst(this);
                            if (Tag == GameObjectTag.WorldViewer)
                                ParentScene.UpdateViews();
                        }
                    }
                }
                else
                {
                    if (ParentScene != null && ParentScene.Tags.Contains(Tag))
                    {
                        LinkedList<GameObject> l = ParentScene.GetList(Tag);
                        if (l != null && l.Contains(this))
                        {
                            l.Remove(this);
                            if (Tag == GameObjectTag.WorldViewer)
                                ParentScene.UpdateViews();
                        }
                    }
                }
            }
            else
            {
                ParentScene.NeedsToReadyTags = true;
                if (!ParentScene.ObjectsToReadyTags.Contains(this))
                    ParentScene.ObjectsToReadyTags.AddLast(this);
            }
        }

        public virtual LinkedList<GameObject> AddTag(GameObjectTag Tag)
        {
            Tags.AddFirst(Tag);

            if (ParentScene == null)
                ParentScene = Level.ReferenceScene;

            ReadyTag(Tag);
            return null;
        }

        public virtual void RemoveTag(GameObjectTag Tag)
        {
            if (Tags.Contains(Tag))
            {
                TagsReady = false;
                Tags.Remove(Tag);
                ReadyTag(Tag);
            }
        }

        public void MoveValuetoFront(params Value[] vals)
        {
#if EDITOR && WINDOWS
            foreach (Value v in vals)
                if (Values.Contains(v))
                    Values.Remove(v);

            for (int i = vals.Count() - 1; i >= 0; i--)
                Values.AddFirst(vals[i]);
#endif
        }

        public void AddValue(Value val)
        {
#if EDITOR
            if (ParentLevel.LevelForEditing)
                Values.AddLast(val);
            else
#endif
                Values.AddLast(val);
        }

        public virtual void Destroy()
        {
            if (HierarchyChildren.Count > 0)
            {
                GameObject[] hc = HierarchyChildren.ToArray();
                foreach (GameObject g in hc)
                    g.Destroy();
            }

            ParentScene.Remove(this);
        }

        public virtual void OnDestroy()
        {
            ObjectValue.ClearObject(this);
            ObjectListValue.ClearObject(this);

            if (hierarchyParent != null)
                hierarchyParent.RemoveHierarchyObject(this);

            foreach (Value v in Values)
                v.Destroy();
        }

        public virtual void AddConstructEffect(EffectValue v)
        {
            if (!ConstructEffects.Contains(v))
            {
                ConstructEffectsNeeded = true;
                ConstructEffects.AddFirst(v);
            }
        }

        public void Add(BasicEffectParameter p)
        {
            if (!ParameterQue.Contains(p))
            {
                ParameterQueNeeded = true;
                ParameterQue.AddLast(p);
            }
        }

        public virtual GameObject Add(GameObject o)
        {
            AddHierarchyObject(ParentLevel.AddObject(o));
            return o;
        }

        public void AddHierarchyObject(GameObject NewObject)
        {
            if (NewObject != null)
            {
                NewObject.AddToHierarchy();
#if EDITOR && WINDOWS
                if (ParentLevel.LevelForEditing)
                {
                    NewObject.TopHierarchyParent = TopHierarchyParent;
                    HierarchyChildren.AddLast(NewObject);
                }
                else
#endif
                    HierarchyChildren.AddFirst(NewObject);

                NewObject.hierarchyParent = this;
                ModifyCollection();
            }
        }

        public void RemoveValue(Value v)
        {
            if (Values.Contains(v))
                Values.Remove(v);
        }

        public void RemoveHierarchyObject(GameObject Remove)
        {
            HierarchyChildren.Remove(Remove);
            ModifyCollection();
        }

        public void AddToHierarchy()
        {
            if (hierarchyParent != null && hierarchyParent.GetChildren().Contains(this))
                hierarchyParent.GetChildren().Remove(this);
        }

        public bool HierarchyObjectIschild(GameObject Object)
        {
            if (HierarchyChildren.Contains(Object))
                return true;
            else
                foreach (GameObject Child in HierarchyChildren)
                    if (Child.HierarchyObjectIschild(Object))
                        return true;
            return false;

        }
#if EDITOR && WINDOWS
        public virtual Vector2 UpdateHierarchy(Vector2 DrawPosition)
        {
            CreateHierarchyRectangles();

            Vector2 ReturnPosition = DrawPosition;

            this.HierarchyDrawPosition = DrawPosition;

            if (HierarchyExpanded)
                foreach (GameObject Child in HierarchyChildren)
                {
                    HierarchyLineHeight = (int)(DrawPosition.Y - HierarchyDrawPosition.Y + TopHierarchyParent.ItemHeight * 0.5);
                    DrawPosition = Child.UpdateHierarchy(ReturnPosition + TopHierarchyParent.ItemPush + new Vector2(0, TopHierarchyParent.ItemHeight));

                    ReturnPosition.Y = DrawPosition.Y;
                }


            UpdateHierarchyRectangles();

            return ReturnPosition + new Vector2(Math.Max(DrawPosition.X, HierarchyMouseRectangle.Width + HierarchyMouseRectangle.X), 0);

        }

        public void TestContained(float MinY, float MaxY, LinkedList<GameObject> l)
        {
            if (HierarchyDrawPosition.Y >= MinY && HierarchyDrawPosition.Y <= MaxY)
                l.AddLast(this);
            if (HierarchyExpanded)
                foreach (GameObject o in HierarchyChildren)
                    o.TestContained(MinY, MaxY, l);
        }

        public void TestPosition(ref float MinY, ref float MaxY)
        {
            if (HierarchyDrawPosition.Y < MinY)
                MinY = HierarchyDrawPosition.Y;
            if (HierarchyDrawPosition.Y > MaxY)
                MaxY = HierarchyDrawPosition.Y;

            if (HierarchyExpanded)
                foreach (GameObject o in HierarchyChildren)
                    o.TestPosition(ref MinY, ref MaxY);
        }

        public virtual void UpdateHierarchyRectangles()
        {
            Vector2 MiddlePosition = HierarchyDrawPosition + new Vector2(0, TopHierarchyParent.ItemHeight / 2);

            HierarchyTextPosition = MiddlePosition - new Vector2(0, TopHierarchyParent.Font.MeasureString(Name!= null? Name.get() : GetType().Name).Y / 2);

            HierarchyHorizontalLine.X = (int)(HierarchyDrawPosition.X - TopHierarchyParent.ItemPush.X);
            HierarchyHorizontalLine.Width = (int)TopHierarchyParent.ItemPush.X - 3;
            HierarchyHorizontalLine.Y = (int)MiddlePosition.Y - 1;

            HierarchyVerticleLine.X = (int)HierarchyDrawPosition.X;
            HierarchyVerticleLine.Y = (int)HierarchyDrawPosition.Y + TopHierarchyParent.ItemHeight;
            HierarchyVerticleLine.Height = HierarchyLineHeight;

            HierarchyBox.X = (int)(MiddlePosition.X - TopHierarchyParent.ItemPush.X - TopHierarchyParent.BoxSize / 2);
            HierarchyBox.Y = (int)MiddlePosition.Y - TopHierarchyParent.BoxSize / 2;
            HierarchyBox.Width = TopHierarchyParent.BoxSize;
            HierarchyBox.Height = TopHierarchyParent.BoxSize;

            HierarchyBoxOutline.X = HierarchyBox.X - 1;
            HierarchyBoxOutline.Y = HierarchyBox.Y - 1;
            HierarchyBoxOutline.Width = HierarchyBox.Width + 2;
            HierarchyBoxOutline.Height = HierarchyBox.Height + 2;

            HierarchyPlusHorizontal.X = (int)(MiddlePosition.X - TopHierarchyParent.ItemPush.X - TopHierarchyParent.PlusSize / 2);
            HierarchyPlusHorizontal.Y = (int)(MiddlePosition.Y);
            HierarchyPlusHorizontal.Width = TopHierarchyParent.PlusSize;

            HierarchyPlusVerticle.X = (int)(MiddlePosition.X - TopHierarchyParent.ItemPush.X);
            HierarchyPlusVerticle.Y = (int)(MiddlePosition.Y - TopHierarchyParent.PlusSize / 2);
            HierarchyPlusVerticle.Height = TopHierarchyParent.PlusSize;

            HierarchyMouseRectangle.X = (int)(HierarchyDrawPosition.X - TopHierarchyParent.ItemPush.X - TopHierarchyParent.BoxSize / 2 - 2);
            HierarchyMouseRectangle.Y = (int)HierarchyDrawPosition.Y;
            HierarchyMouseRectangle.Width = (int)(TopHierarchyParent.Font.MeasureString(Name != null ? Name.get() : GetType().Name).X + TopHierarchyParent.ItemPush.X + TopHierarchyParent.BoxSize / 2 + 4);
            HierarchyMouseRectangle.Height = (int)TopHierarchyParent.ItemHeight;

            HierarchyDragRectangle.X = HierarchyMouseRectangle.X + HierarchyMouseRectangle.Width + 2;
            HierarchyDragRectangle.Y = HierarchyMouseRectangle.Y + 2;
            HierarchyDragRectangle.Width = HierarchyMouseRectangle.Height - 4;
            HierarchyDragRectangle.Height = HierarchyMouseRectangle.Height - 4;

            HierarchyDropRectangle.X = HierarchyDragRectangle.X + HierarchyDropBorder;
            HierarchyDropRectangle.Y = HierarchyDragRectangle.Y + HierarchyDropBorder;
            HierarchyDropRectangle.Width = HierarchyDragRectangle.Width - HierarchyDropBorder * 2;
            HierarchyDropRectangle.Height = HierarchyDragRectangle.Height - HierarchyDropBorder * 2;

            //very last
            HierarchyMouseRectangle.Width += 200;
            HierarchyMouseRectangle.X -= 100;
        }

        
        public virtual GameObject ReturnMouseOver(Window Updater)
        {
            if (HierarchyMouseRectangle.Contains(Updater.RelativeMousePoint) || (HierarchyDragRectangle.Contains(Updater.RelativeMousePoint)))
                return this;
            else if (HierarchyExpanded)
                foreach (GameObject Child in HierarchyChildren)
                {
                    GameObject g = Child.ReturnMouseOver(Updater);
                    if (g != null)
                        return g; 
                }
            return null;
        }
#endif
        public virtual void ModifyCollection()
        {
            if (hierarchyParent != null && ParentLevel.LevelForEditing)
                hierarchyParent.ModifyCollection();
        }

        
        public SceneObject GetParent()
        {
            if (GetType().IsSubclassOf(typeof(SceneObject)))
                return (SceneObject)this;
            else
                return ParentScene;
        }

        public LinkedList<GameObject> GetChildren()
        {
            return HierarchyChildren;
        }

        public virtual int GetIntType()
        {
            return -1;
        }

        public virtual bool RayCast(GameTime gameTime)
        {
            return false;
        }

        public virtual void DoubleClick(GameTime gameTime)
        {
#if EDITOR && WINDOWS
            Select(true);
#endif
        }

#if EDITOR && WINDOWS
        public void Select(bool SelectChildren)
        {
            if (GetType().IsSubclassOf(typeof(SceneObject)))
            {
                SceneObject s = (SceneObject)this;
                s.AddSelected(this);
            }
            else
                ParentScene.AddSelected(this);

            if (SelectChildren)
                foreach (GameObject g in HierarchyChildren)
                    g.Select(true);
        }
#endif

        public virtual void LeftClick(GameTime gameTime)
        {
#if EDITOR && WINDOWS
            if (gameTime.TotalGameTime.TotalMilliseconds < DoubleClickTime + 300)
                DoubleClick(gameTime);
            else
                DoubleClickTime = gameTime.TotalGameTime.TotalMilliseconds;
#endif
        }

        public virtual void RightClick(GameTime gameTime)
        {
#if EDITOR && WINDOWS
            EditorManager.MyEditor.AddDropDown(new BasicObjectDrop(this));
#endif
        }
#if EDITOR && WINDOWS
        public static LinkedList<Form> CreateValueEditors(LinkedList<GameObject> Selected)
        {
            LinkedList<Form> Forms = new LinkedList<Form>();
            Dictionary<string, LinkedList<Value>> Values = new Dictionary<string, LinkedList<Value>>();

            foreach (GameObject o in Selected)
                foreach (Value v in o.Values)
                    if (v.Editable)
                    {
                        LinkedList<Value> Vlist = null;
                        if (!Values.Keys.Contains(v.Name))
                            Values.Add(v.Name, Vlist = new LinkedList<Value>());
                        else
                            Vlist = Values[v.Name];

                        Vlist.AddLast(v);
                    }

            foreach (string s in Values.Keys)
                if (Values[s].Count >= Selected.Count)
                {
                    Form f = Values[s].First.Value.GetForm(Values[s]);
                    if (f != null)
                        Forms.AddLast(f);
                }
            return Forms;
        }
#endif

        public void AddRightClickEvent(string Name, ClickEvent Event)
        {
#if EDITOR && WINDOWS
            if (Level.ReferenceLevel.LevelForEditing)
            {
                if (RightClickActions == null)
                    RightClickActions = new Dictionary<string, ClickEvent>();

                RightClickActions.Add(Name, Event);
            }
#endif
        }

        
    }
}
