#if EDITOR || AUTO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BadRabbit.Carrot
{
    public class DefaultLoader
    {
        public static string LastProjectFileName = "LastProject";

#if !EDITOR
        public static void AutoLoad()
        {
            Stream MyStream;
            try
            {
                if ((MyStream = File.Open(GetPath(), FileMode.Open)) != null)
                {
                    using (MyStream)
                    {
                        ReadFile(new BinaryReader(MyStream));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private static void ReadFile(BinaryReader Reader)
        {
            Level NewLevel = new Level(false);

            GameManager.SetLevel(NewLevel);

            NewLevel.Read(Reader);
        }
#endif

        public static string GetPath()
        {
            Stream MyStream;

            try
            {
                if ((MyStream = File.Open(LastProjectFileName, FileMode.Open)) != null)
                {
                    using (MyStream)
                    {
                        BinaryReader reader = new BinaryReader(MyStream);
                        return reader.ReadString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
    }
}
#endif