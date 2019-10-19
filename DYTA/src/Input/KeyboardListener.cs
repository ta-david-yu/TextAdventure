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

        private List<ConsoleKeyInfo> m_KeyBuffer = new List<ConsoleKeyInfo>();

        public void QueryInput()
        {
            m_KeyBuffer = new List<ConsoleKeyInfo>();
            while (Console.KeyAvailable)
            {
                m_KeyBuffer.Add(Console.ReadKey(true));
            }

            foreach (var keyInfo in m_KeyBuffer)
            {
                OnKeyPressed.Invoke(keyInfo);
            }
        }
    }
}
