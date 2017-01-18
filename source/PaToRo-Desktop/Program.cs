using System;

namespace PaToRo_Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new PaToRoGame())
                game.Run();
        }
    }
}
