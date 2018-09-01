using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class InstanceManager
    {
        public const int WorldIndex = 3;

        public const int HumanBasicIndex = WorldIndex + 3;
        public const int HumanUnitIndex = HumanBasicIndex + 8;
        public const int HumanTurretIndex = HumanUnitIndex + 7;

        public const int EmpireBasicIndex = HumanTurretIndex + 3;
        public const int EmpireUnitIndex = EmpireBasicIndex + 5;
        public const int EmpireTurretIndex = EmpireUnitIndex + 5;

        public const int MonsterBasicIndex = EmpireTurretIndex + 3;
        public const int MonsterUnitIndex = MonsterBasicIndex + 5;
        public const int MonsterTurretIndex = MonsterUnitIndex + 5;

        public const int AlienBasicIndex = MonsterTurretIndex + 3;
        public const int AlienUnitIndex = AlienBasicIndex + 6;
        public const int AlienTurretIndex = AlienUnitIndex + 5;

        public const int ExtraTurretIndex = AlienTurretIndex + 5;

        private static List<DrawItem> DrawItems = new List<DrawItem>();
        private static LinkedList<int> DrawIdList = new LinkedList<int>();
        private static List<LinkedList<BasicShipGameObject>> SortedChildren = new List<LinkedList<BasicShipGameObject>>();
        static BulletInstancer bulletInstancer;
        static ShieldInstancer shieldInstancer;

        private static Dictionary<int, LinkedList<BasicShipGameObject>> DisplacementSortedChildren = new Dictionary<int, LinkedList<BasicShipGameObject>>();
        
        static InstanceManager()
        {
            bulletInstancer = new BulletInstancer();
            shieldInstancer = new ShieldInstancer();
        }

        public static void AddBasicChild(BasicShipGameObject Object)
        {
            if (DrawItems.Count > Object.GetIntType())
            {
                if (DrawItems[Object.GetIntType()] != null)
                {
                    SortedChildren[Object.GetIntType()].AddLast(Object);
                    if (!DrawIdList.Contains(Object.GetIntType()))
                        DrawIdList.AddLast(Object.GetIntType());
                }
                else
                {
                    DrawItems[Object.GetIntType()] = Object.getDrawItem();
                    if (DrawItems[Object.GetIntType()] == null)
                        DrawItems[Object.GetIntType()] = new DrawShip(InstanceModelList.GetList()[Object.GetIntType()]);

                    SortedChildren[Object.GetIntType()] = new LinkedList<BasicShipGameObject>();
                    SortedChildren[Object.GetIntType()].AddLast(Object);
                    DrawIdList.AddLast(Object.GetIntType());
                    if (!DrawIdList.Contains(Object.GetIntType()))
                        DrawIdList.AddLast(Object.GetIntType());
                }
            }
            else
            {
                int c = DrawItems.Count;
                int numb = Object.GetIntType() - c + 1;

                for (int i = 0; i < numb; i++)
                {
                    if (i + c != Object.GetIntType())
                    {
                        DrawItems.Add(null);
                        SortedChildren.Add(null);
                    }
                    else
                    {
                        DrawItems.Add(Object.getDrawItem());
                        if (DrawItems[Object.GetIntType()] == null)
                            DrawItems.Add(new DrawShip(InstanceModelList.GetList()[Object.GetIntType()]));

                        SortedChildren.Add(new LinkedList<BasicShipGameObject>());
                        SortedChildren[Object.GetIntType()].AddLast(Object);
                        if (!DrawIdList.Contains(Object.GetIntType()))
                            DrawIdList.AddLast(Object.GetIntType());
                    }
                }
            }
        }

        public static void AddDisplacementChild(BasicShipGameObject UnitShip)
        {
            if (!DisplacementSortedChildren.ContainsKey(UnitShip.GetIntType()))
            {
                DisplacementSortedChildren.Add(UnitShip.GetIntType(), new LinkedList<BasicShipGameObject>());
                DisplacementSortedChildren[UnitShip.GetIntType()].AddLast(UnitShip);
            }
            else
            {
                DisplacementSortedChildren[UnitShip.GetIntType()].AddLast(UnitShip);
            }
        }

        public static void RemoveDisplacementChild(BasicShipGameObject UnitShip)
        {
            DisplacementSortedChildren[UnitShip.GetIntType()].Remove(UnitShip);
        }

        public static float AddChild(BasicShipGameObject UnitShip)
        {
            if (DrawItems.Count > UnitShip.GetIntType())
            {
                if (DrawItems[UnitShip.GetIntType()] != null)
                {
                    SortedChildren[UnitShip.GetIntType()].AddLast(UnitShip);
                    if (!DrawIdList.Contains(UnitShip.GetIntType()))
                        DrawIdList.AddLast(UnitShip.GetIntType());
                }
                else
                {
                    DrawItems[UnitShip.GetIntType()] = UnitShip.getDrawItem();
                    if (DrawItems[UnitShip.GetIntType()] == null)
                        DrawItems.Add(new DrawShip(InstanceModelList.GetList()[UnitShip.GetIntType()]));

                    SortedChildren[UnitShip.GetIntType()] = new LinkedList<BasicShipGameObject>();
                    SortedChildren[UnitShip.GetIntType()].AddLast(UnitShip);
                    DrawIdList.AddLast(UnitShip.GetIntType());
                    if (!DrawIdList.Contains(UnitShip.GetIntType()))
                        DrawIdList.AddLast(UnitShip.GetIntType());
                }
            }
            else
            {
                int c = DrawItems.Count;
                int numb = UnitShip.GetIntType() - c + 1;

                for (int i = 0; i < numb; i++)
                {
                    if (i + c != UnitShip.GetIntType())
                    {
                        DrawItems.Add(null);
                        SortedChildren.Add(null);
                    }
                    else
                    {
                        DrawItems.Add(UnitShip.getDrawItem());
                        if (DrawItems[UnitShip.GetIntType()] == null)
                            DrawItems.Add(new DrawShip(InstanceModelList.GetList()[UnitShip.GetIntType()]));

                        SortedChildren.Add(new LinkedList<BasicShipGameObject>());
                        SortedChildren[UnitShip.GetIntType()].AddLast(UnitShip);
                        if (!DrawIdList.Contains(UnitShip.GetIntType()))
                            DrawIdList.AddLast(UnitShip.GetIntType());
                    }
                }
            }

            DrawShip r = (DrawShip)DrawItems[UnitShip.GetIntType()];
            return r.ShipScale / 1.2f;
        }

        public static void Clear()
        {
            WallInstancer.Clear();
            DrawIdList.Clear();
            foreach (LinkedList<BasicShipGameObject> list in SortedChildren)
                if (list != null)
                    list.Clear();
        }

        public static void RemoveChild(BasicShipGameObject UnitShip)
        {
            SortedChildren[UnitShip.GetIntType()].Remove(UnitShip);
            if (SortedChildren[UnitShip.GetIntType()].Count < 1 && DrawIdList.Contains(UnitShip.GetIntType()))
                DrawIdList.Remove(UnitShip.GetIntType());
        }

        public static Vector3 GetWeaponPosition(int ShipIndex, Vector3 Position, ref Matrix RotationMatrix, int ID, float Size)
        {
            if (ShipIndex != -1)
                return DrawItems[ShipIndex].GetWeaponPosition(Position, ref RotationMatrix, ID, Size);
            else
                return Position;
        }

        public static void EmitParticle(int ShipIndex, Vector3 Position, ref Matrix RotationMatrix, int Layer, float Scale, float ColorMult)
        {
            if (ShipIndex != -1)
                DrawItems[ShipIndex].EmitParticle(Layer, ref Position, ref RotationMatrix, Scale, ColorMult);
        }

        public static void Update(GameTime gameTime)
        {
            bulletInstancer.Update(gameTime);
            shieldInstancer.Update(gameTime);
            
            foreach (int i in DrawIdList)
                DrawItems[i].Update(gameTime);
        }

        public static void Draw(Camera3D DrawCamera)
        {
            foreach (int i in DrawIdList)
                DrawItems[i].DrawInstanced(SortedChildren[i], DrawCamera);

            WallInstancer.Draw(DrawCamera);

            bulletInstancer.DrawInstanced(DrawCamera);
        }

        public static void DrawShield(Camera3D DrawCamera)
        {
            shieldInstancer.DrawInstanced(DrawCamera);
        }

        public static void DrawDistortion(Camera3D DrawCamera)
        {
            foreach (int i in DisplacementSortedChildren.Keys)
                DrawItems[i].DrawDistortion(DisplacementSortedChildren[i], DrawCamera);
        }

        public static void DrawSingle(int Index, Vector3 Position, float Size, Vector4 Color, Camera3D DrawCamera)
        {
            if (DrawItems.Count <= Index || DrawItems[Index] != null)
            {
                if (DrawItems.Count > Index)
                    DrawItems[Index] = new DrawShip(InstanceModelList.GetList()[Index]);
                else
                {
                    int c = DrawItems.Count;
                    int numb = Index - c + 1;

                    for (int i = 0; i < numb; i++)
                    {
                        if (i + c != Index)
                        {
                            DrawItems.Add(null);
                            SortedChildren.Add(null);
                        }
                        else
                        {
                            DrawItems.Add(new DrawShip(InstanceModelList.GetList()[Index]));
                            SortedChildren.Add(new LinkedList<BasicShipGameObject>());
                        }
                    }
                }
            }
            DrawItems[Index].DrawSingle(Position, Size, Color, DrawCamera);
        }
    }
}
