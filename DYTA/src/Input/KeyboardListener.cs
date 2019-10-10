using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Input
{
    public class KeyboardListener
    {
        private static KeyboardListener s_Instance = null;
        public static KeyboardListener Instance
        {
            get
            {
                //lock (s_Lock)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new KeyboardListener();
                    }
                    return s_Instance;
                }
            }
        }

        public event Action<ConsoleKeyInfo> OnKeyPressed = delegate { };

        public void QueryInput()
        {
            if (Console.KeyAvailable)
            {
                OnKeyPressed.Invoke(Console.ReadKey(true));
            }
        }
    }
}
