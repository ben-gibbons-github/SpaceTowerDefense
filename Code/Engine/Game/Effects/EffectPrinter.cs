using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class EffectPrinter
    {
        private static string Pre;

        public static void Print(Effect e)
        {
            Pre = "";
            Write("Effect Printer: ");
            Pre = " ";
            Write("Effect name: " + e.Name);
            Write("Parameter Count: " + e.Parameters.Count);

            int i = 0;
            foreach (EffectParameter p in e.Parameters)
            {
                Pre = "  ";
                Write("");
                Write("Parameter " + ++i + ": " + p.Name);
                Write(p.ParameterType.ToString());
                Write("RowCount: " + p.RowCount.ToString());
                Write("Column: " + p.ColumnCount.ToString());//
                Write("Elements: " + p.Elements.Count);
                Write("Structure Memebers" + p.StructureMembers.Count);
               
            }
        }

        private static void Write(string s)
        {
            Console.WriteLine(Pre + s);
        }

    }
}
