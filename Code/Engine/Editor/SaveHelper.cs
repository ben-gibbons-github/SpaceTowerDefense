using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SaveHelper
    {
        public static BinaryWriter MyWriter;
        public static BinaryReader MyReader;
        public static StringBuilder stringBuilder = new StringBuilder();

        public static Vector2 ReadVector2()
        {
            return new Vector2(MyReader.ReadSingle(), MyReader.ReadSingle());
        }

        public static Vector3 ReadVector3()
        {
            return new Vector3(MyReader.ReadSingle(), MyReader.ReadSingle(), MyReader.ReadSingle());
        }

        public static Vector4 ReadVector4()
        {
            return new Vector4(MyReader.ReadSingle(), MyReader.ReadSingle(), MyReader.ReadSingle(), MyReader.ReadSingle());
        }

        public static void Write(Vector2 Vector)
        {
            MyWriter.Write((Single)Vector.X);
            MyWriter.Write((Single)Vector.Y);
        }

        public static void Write(Vector3 Vector)
        {
            MyWriter.Write((Single)Vector.X);
            MyWriter.Write((Single)Vector.Y);
            MyWriter.Write((Single)Vector.Z);
        }

        public static void Write(Vector4 Vector)
        {
            MyWriter.Write((Single)Vector.X);
            MyWriter.Write((Single)Vector.Y);
            MyWriter.Write((Single)Vector.Z);
            MyWriter.Write((Single)Vector.W);
        }




    }
}
