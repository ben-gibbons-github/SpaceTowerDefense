#if EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
#endif

namespace BadRabbit.Carrot
{
    public class StopwatchWrapper
    {
#if EDITOR
        private string Name;
        private Stopwatch Watch = new Stopwatch();
#endif
        public StopwatchWrapper(string Name, bool NoParent)
        {
#if EDITOR
            this.Name = Name;
#endif
        }

        public StopwatchWrapper(string Name)
        {
#if EDITOR
            this.Name = Name;
            SceneObject s = (SceneObject)Level.ReferenceObject;
            s.Add(this);
#endif
        }

        public void Start()
        {
#if EDITOR
            Watch.Reset();
            Watch.Start();
#endif
        }

        public void Continue()
        {
#if EDITOR
            Watch.Start();
#endif
        }

        public void Stop()
        {
#if EDITOR
            Watch.Stop();
#endif
        }

        public void Reset()
        {
#if EDITOR
            Watch.Reset();
#endif
        }

        public string get()
        {
#if EDITOR
            return Name + ": " + Watch.Elapsed.ToString();
#endif
#if !EDITOR
            return null;
#endif
        }
    }
}
