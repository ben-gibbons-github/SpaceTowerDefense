using System;

namespace BadRabbit.Carrot
{
#if WINDOWS || XBOX
    static class Program
    {
        #if WINDOWS && EDITOR
            [STAThread]
        #endif

        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

