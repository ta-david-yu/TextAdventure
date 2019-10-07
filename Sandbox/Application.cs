using System;
using DYTA.Math;

namespace DYTA
{
    class Application
    {
        static void Main(string[] args)
        {
            var app = new Application();
            app.run();
        }

        void run()
        {
            RectInt rect = new RectInt(0, 0, 3, 3);
            RectInt rect2 = new RectInt(new Vector2Int(0, 0), new Vector2Int(3, 3));

            foreach (var pt in rect.AllPositionsWithin())
            {
                Console.WriteLine(pt);
            }

            System.Diagnostics.Debug.Assert(rect.Equals(rect2));
        }
    }
}
