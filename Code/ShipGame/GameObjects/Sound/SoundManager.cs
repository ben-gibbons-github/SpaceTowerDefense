using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace BadRabbit.Carrot
{
    public class SoundManager
    {
        public static Vector3 CurrentGameForward = new Vector3(0, -1, 0);
        public static int MaxSoundDistance = 1000;

        const int MaxActiveSounds = 60;

        public static ActiveSound[] ActiveSounds = new ActiveSound[MaxActiveSounds];
        public static int ActiveSoundCount = 0;


        static float SoundEffectVolume = 1;
        static SoundEffectInstance DeafToneInstance;
        static float DeafToneVolume = 0;
        static float DeafToneIncrease = 0;
        static float SoundEffectIncrease = 0;
        const float DeafToneMult = 0.1f;

        static AudioEmitter emitter = new AudioEmitter();

        public static void DeafTone()
        {
            DeafToneIncrease = 0.01f;
            SoundEffectIncrease = 0.0025f;
            DeafToneVolume = 0;
            SoundEffectVolume = 0;
            if (DeafToneInstance == null)
            {
                DeafToneInstance = SoundLibrary.soundEffects["DeafTone"].CreateInstance();
                DeafToneInstance.Volume = 0;
                DeafToneInstance.IsLooped = true;
                DeafToneInstance.Play();
            }
        }

        public static void LevelEnd()
        {
            if (DeafToneInstance != null && !DeafToneInstance.IsDisposed)
                DeafToneInstance.Dispose();
        }

        static SoundManager()
        {
            for (int i = 0; i < MaxActiveSounds; i++)
                ActiveSounds[i] = new ActiveSound();
        }

        public static void Update(GameTime gameTime)
        {
            SoundEffectVolume += SoundEffectIncrease * gameTime.ElapsedGameTime.Milliseconds / 25f;
            DeafToneVolume += DeafToneIncrease * gameTime.ElapsedGameTime.Milliseconds / 25f;

            if (SoundEffectVolume > 1)
            {
                SoundEffectVolume = 1;
                SoundEffectIncrease = 0;
            }
            if (DeafToneVolume > 1)
            {
                DeafToneVolume = 1;
                DeafToneIncrease = -Math.Abs(DeafToneIncrease);
            }
            else if (DeafToneVolume < 0)
            {
                DeafToneVolume = 0;
                DeafToneIncrease = 0;

                if (DeafToneInstance != null && !DeafToneInstance.IsDisposed)
                    DeafToneInstance.Stop();
                DeafToneInstance = null;
            }

            if (DeafToneInstance != null && !DeafToneInstance.IsDisposed)
                DeafToneInstance.Volume = DeafToneVolume * DeafToneMult * 0.1f;
        }

        public static void PlaySound(string SoundEffect, float Volume, float Pitch, float Pan)
        {
//#if EDITOR && WINDOWS
            if (SoundLibrary.soundEffects.ContainsKey(SoundEffect))
//#endif
            {
                SoundLibrary.soundEffects[SoundEffect].Play(Volume * SoundEffectVolume, Pitch, Pan);
            }
#if EDITOR && WINDOWS
            else
                SoundEffect.Clone();
#endif
        }

        public static void Play3DSound(string SoundEffect, Vector3 Position, float Volume, float MaxDistance, float Exponent)
        {
            if (SoundEffect == null)
                return;
//#if EDITOR && WINDOWS
            if (SoundLibrary.soundEffects.ContainsKey(SoundEffect))
//#endif
            {
                float AvgPan = 0;
                float SumOfVolume = 0;

                foreach (WorldViewer3D w in GameManager.GetLevel().getCurrentScene().WorldViewerChildren)
                {
                    float D = (MaxDistance -  Vector3.Distance(Position, w.getCamera().LookAt)) / MaxDistance;
                    if (D > 0)
                    {
                        float Weight = (float)Math.Pow(D, Exponent);
                        if (Weight > 0)
                        {
                            SumOfVolume += Weight;
                            AvgPan += (Position.X - w.getCamera().LookAt.X) / 
                                Vector3.Distance(w.getCamera().Position, w.getCamera().LookAt) * Weight;
                        }
                    }
                }

                if (SumOfVolume > 0)
                {
                    SoundLibrary.soundEffects[SoundEffect].Play(Math.Min(SumOfVolume * Volume * SoundEffectVolume, Math.Min(1, Volume)), 0,
                        MathHelper.Clamp(AvgPan / SumOfVolume, -1, 1));
                }
            }
#if EDITOR && WINDOWS
            else
                SoundEffect.Clone();
#endif
        }

        public static SoundEffectInstance PlayLoopingSound(SoundEffectInstance CurrentInstance, string SoundEffect, Vector3 Position,
            float Volume, float MaxDistance, float Exponent)
        {
            if (CurrentInstance != null && !CurrentInstance.IsDisposed)
            {
                if (CurrentInstance.State != SoundState.Playing)
                {
                    CurrentInstance.Volume = 0;
                    CurrentInstance.Play();
                    CurrentInstance.IsLooped = true;
                }

                float AvgPan = 0;
                float SumOfVolume = 0;

                foreach (WorldViewer3D w in GameManager.GetLevel().getCurrentScene().WorldViewerChildren)
                {
                    float D = (MaxDistance - Vector3.Distance(Position, w.getCamera().LookAt)) / MaxDistance;
                    if (D > 0)
                    {
                        float Weight = (float)Math.Pow(D, Exponent);
                        if (Weight > 0)
                        {
                            SumOfVolume += Weight;
                            AvgPan += (Position.X - w.getCamera().LookAt.X) /
                                Vector3.Distance(w.getCamera().Position, w.getCamera().LookAt) * Weight;
                        }
                    }
                }

                if (SumOfVolume > 0)
                {
                    CurrentInstance.Volume = Math.Min(SumOfVolume * Volume * SoundEffectVolume, 1);
                    CurrentInstance.Pan = MathHelper.Clamp(AvgPan / SumOfVolume, -1, 1);
                }

                return CurrentInstance;
            }
            else
            {
//#if EDITOR && WINDOWS
                if (SoundLibrary.soundEffects.ContainsKey(SoundEffect))
//#endif
                {
                    SoundEffectInstance i = SoundLibrary.soundEffects[SoundEffect].CreateInstance();
                    i.IsLooped = true;
                    i.Volume = 0;
                    i.Play();
                    return i;
                }
//#if EDITOR && WINDOWS
                else
                {
                    //SoundEffect.Clone();
                    return null;
                }
//#endif
            }
        }

        static void Apply3D(ActiveSound activeSound)
        {
            emitter.Position = activeSound.Position;
            emitter.Forward = activeSound.Forward;
            emitter.Up = activeSound.Up;
            emitter.Velocity = activeSound.Velocity;

            activeSound.Instance.Apply3D(activeSound.Listener, emitter);
            if (activeSound.VolumeMod < 1)
                activeSound.Instance.Volume = activeSound.VolumeMod;
        }

        public class ActiveSound
        {
            public AudioListener Listener;
            public float VolumeMod;
            public SoundEffectInstance Instance;
            public Vector3 Position;
            public Vector3 Forward = Vector3.Forward;
            public Vector3 Up = Vector3.Up;
            public Vector3 Velocity = Vector3.Zero;
        }
    }

}
