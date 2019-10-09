using System;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Audio
{
    public class AudioManager
    {
        class BeepUnit
        {
            public bool IsMute = false;
            public int Frequency;
            public int Duration;
        }

        private static AudioManager s_Instance = null;
        public static AudioManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new AudioManager();
                }
                return s_Instance;
            }
        }

        private System.Threading.Thread m_Thread;

        private Queue<BeepUnit> m_UnitQueue = new Queue<BeepUnit>();

        private bool m_IsPlaying = false;

        public AudioManager()
        {

        }

        public void Begin()
        {
            m_Thread = new System.Threading.Thread(() =>
            {
                // DO SOMETHING
                while (m_IsPlaying)
                {
                    if (m_UnitQueue.Count > 0)
                    {
                        var unit = m_UnitQueue.Dequeue();
                        if (!unit.IsMute)
                            Console.Beep(unit.Frequency, unit.Duration);
                        else
                            System.Threading.Thread.Sleep(unit.Duration);
                    }
                }
            });
            m_Thread.IsBackground = true;
            m_IsPlaying = true;
            m_Thread.Start();
        }

        public void End()
        {
            m_IsPlaying = false;
        }

        public void Beep(int fre, int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.Frequency = fre; unit.Duration = dur;
            m_UnitQueue.Enqueue(unit);
        }

        public void Sleep(int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.IsMute = true;
            unit.Duration = dur;
            m_UnitQueue.Enqueue(unit);
        }
    }
}
