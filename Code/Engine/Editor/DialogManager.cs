#if WINDOWS && EDITOR
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class DialogManager
    {
        public static string LastFileLocation = "c:\\";
        public static bool InUse = false;
        public static bool FileOpenSave = false;
        public static bool FileOpenLoad = false;
        public static string DefaultSaveLocation = "";

        public static void Load()
        {
            FileOpenLoad = true;

            Stream MyStream;
            OpenFileDialog openFileDialog1;

            openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = LastFileLocation;
            openFileDialog1.Filter = "cardLevel files (*.lvl)|*.lvl";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            InUse = true;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Game1.self.Window.Title = Path.GetFileName(openFileDialog1.FileName) + " - " + Game1.EngineTitle;
                LastFileLocation = openFileDialog1.FileName;

                try
                {
                    if ((MyStream = File.Open(openFileDialog1.FileName, FileMode.Open)) != null)
                    {
                        EditorManager.LoadNewLevel(ReadFile(new BinaryReader(MyStream), true));
                        SetDefaultSaveLocation(openFileDialog1.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            InUse = false;
        }

        private static void SetDefaultSaveLocation(string Location)
        {
            DefaultSaveLocation = Location;

            Stream MyStream;
            try
            {
                string LastProjectFileName = DefaultLoader.LastProjectFileName;
                if (File.Exists(LastProjectFileName))
                    File.Delete(LastProjectFileName);
                if ((MyStream = File.Create(LastProjectFileName)) != null)
                {
                    using (MyStream)
                    {
                        BinaryWriter writer = new BinaryWriter(MyStream);
                        writer.Write(DefaultSaveLocation);
                        writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Creating Default File Path: " + ex.Message);
            }

        }

        public static bool Save()
        {
            if (DefaultSaveLocation != "")
                return Save(DefaultSaveLocation);
            else
                return SaveAs();
        }

        public static void ShowBox(string Message)
        {
            MessageBox.Show(Message);
        }

        public static bool Save(string Location)
        {
            return Save(Location, true);
        }

        public static bool Save(string Location, bool SafeWrite)
        {
            bool Result = false;
            Stream MyStream;
            try
            {
                Game1.self.Window.Title = Path.GetFileName(Location) + " - " + Game1.EngineTitle;

                if (File.Exists(Location))
                    File.Delete(Location);
                if ((MyStream = File.Create(Location)) != null)
                {
                    using (MyStream)
                    {
                        WriteFile(new BinaryWriter(MyStream), GameManager.GetLevel(), SafeWrite);
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            return Result;
        }

        public static bool SaveAs()
        {
            return SaveAs(true);
        }

        public static bool SaveAs(bool SafeWrite)
        {
            bool Result = false;

            FileOpenSave = true;

            SaveFileDialog openFileDialog1;

            openFileDialog1 = new SaveFileDialog();

            openFileDialog1.InitialDirectory = LastFileLocation;
            openFileDialog1.Filter = "Level files (*.lvl)|*.lvl";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.OverwritePrompt = true;

            InUse = true;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SetDefaultSaveLocation(openFileDialog1.FileName);

                LastFileLocation = openFileDialog1.FileName;

                Save(openFileDialog1.FileName, SafeWrite);
                Result = true;
            }

            InUse = false;
            return Result;
        }

        static void WriteFile(BinaryWriter Writer, Level level, bool SafeWrite)
        {
            level.Write(Writer, SafeWrite);
        }

        public static Level ReadFile(BinaryReader Reader, bool ForEditor)
        {
            Level NewLevel = new Level(ForEditor);

            NewLevel.Read(Reader);

            return NewLevel;
        }
    }
}

#endif