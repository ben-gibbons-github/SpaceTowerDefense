using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BadRabbit.Carrot.SpyGame;
using BadRabbit.Carrot.Engine;

namespace BadRabbit.Carrot
{
    public class CreatorBasic
    {
        public static CreatorBasic[] AllCreators = 
        {
            new Forward2DSceneCreator(),
            new NullCreator(),
            new Forward3DSceneCreator(),
            new Camera3DObjectCreator(),
            new BasicModelCreator(),
            new Deferred3DSceneCreator(),
            new DirectionalLightCreator(),
            new PointLightCreator(),
            new DecalCreator(),
            new ParticleSystemCreator(),
            new ParticleEmitterCreator(),
            new AmbientLightCreator(),
            new SkyTextureCreator(),
            new SkyBoxCreator(),
            new Camera2DObjectCreator(),
            new BasicSpriteCreator(),
            new PlayerSpawnCreator(),
            new StarshipSceneCreator(),
            new MineralRockCreator(),
            new TimeLineCreator(),
            new Path3DCreator(),
            new CollisionBoxCreator(),
            new TriggerBoxCreator(),
            new Path3DNodeCreator(),
            new SpyPlayerSpawnCreator(),
            new SpySceneCreator(),
            new ShipViewerCreator(),
            new ShipViewerSceneCreator(),
            new ShipScaleRingCreator(),
            new ShipWeaponPointCreator(),
            new ShipParticlePointCreator(),
            new ShipLightingCreator(),
            new NeutralManagerCreator(),
            new NeutralSpawnCreator(),
            new WaveManagerCreator(),
            new TurretSpawnCreator(),
            new SettingsCreator(),
            new CameraFlybyCenterCreator(),
            new CameraFlybyNodeCreator(),
            new DummyRockCreator(),
            new WorldModelCreator(),
            new WallChainCreator(),
            new WallNodeCreator(),
            new FormTextCreator(),
            new FormSceneCreator(),
            new FormSliderCreator(),
            new FormButtonCreator(),
            new StarfieldCreator(),
            new FormSectionFlagCreator(),
            new StarshipMenuSceneCreator(),
            new LineModelCreator(),
            new WorldFlareSpawnerCreator(),
            new PolorGrapherCreator(),
        };

        public static CreatorBasic LastCreator;
        public static List<string> Catagories = new List<string>();

        public Type MyType;
        public string Catagory;
        public bool Createable = true;

        public static void Load()
        {
            foreach (CreatorBasic Creator in AllCreators)
            {
                Creator.Create();

                if (Creator.Catagory != null && !Creator.Catagory.Equals(""))
                {
                    bool CatagoryTaken = false;
                    foreach (string Cat in Catagories)
                        if (Cat.Equals(Creator.Catagory))
                            CatagoryTaken = true;

                    if (!CatagoryTaken)
                        Catagories.Add(Creator.Catagory);
                }
            }
        }

        public static GameObject ReturnObjectOfType(Type type)
        {
            foreach (CreatorBasic Creator in AllCreators)
                if (Creator.MyType == type)
                    return Creator.ReturnObject();

            return null;
        }

        public static GameObject ReturnObjectOfType(string type)
        {
            foreach (CreatorBasic Creator in AllCreators)
                if (Creator.MyType.Name.Equals(type))
                    return Creator.ReturnObject();
            foreach (CreatorBasic Creator in AllCreators)
                if (Creator.MyType.Name.Contains(type)||type.Contains(Creator.MyType.Name))
                    return Creator.ReturnObject();

            int BestCount = 0;
            CreatorBasic BestMatch = null;
            foreach (CreatorBasic Creator in AllCreators)
            {
                int Count = 0;
                for (int i = 0; i < Math.Min(Creator.MyType.Name.Length, type.Length); i++)
                    if (Creator.MyType.Name[i].Equals(type[i]))
                        Count++;
                if (Count > BestCount)
                {
                    BestCount = Count;
                    BestMatch = Creator;
                }
            }

            return BestMatch != null? BestMatch.ReturnObject() : null;
        }

        public static Type FindType(string TypeName)
        {
            foreach (CreatorBasic Creator in AllCreators)
                if (Creator.MyType.Name.Equals(TypeName))
                    return Creator.MyType;
            return null;
        }


        public virtual void Create()
        {

        }

        public virtual GameObject ReturnObject()
        {
            #if WINDOWS
                return (GameObject)Activator.CreateInstance(MyType);
            #endif
            #if XBOX
                return null;
            #endif
        }
    }
}
