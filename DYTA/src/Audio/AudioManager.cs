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

        private static readonly object s_Lock = new object();

        private static AudioManager s_Instance = null;
        public static AudioManager Instance
        {
            get
            {
                //lock (s_Lock)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new AudioManager();
                    }
                    return s_Instance;
                }
            }
        }

        private System.Threading.Thread m_Thread;

        private Queue<BeepUnit> m_UnitQueue = new Queue<BeepUnit>();

        private bool m_IsPlaying = false;

        public event Action OnQueueEmptied = delegate { };

        public AudioManager()
        {

        }

        public void Begin()
        {
            {
                m_Thread = new System.Threading.Thread(() =>
                {
                    while (m_IsPlaying)
                    {
                        lock (s_Lock)
                        {
                            if (m_UnitQueue.Count > 0)
                            {
                                var unit = m_UnitQueue.Dequeue();

                                if (!unit.IsMute)
                                    Console.Beep(unit.Frequency, unit.Duration);
                                else
                                    System.Threading.Thread.Sleep(unit.Duration);

                                if (m_UnitQueue.Count == 0)
                                {
                                    OnQueueEmptied.Invoke();
                                }
                            }
                        }
                    }
                });
                m_Thread.IsBackground = true;
                m_Thread.Start();
            }
            m_IsPlaying = true;
        }

        public void End()
        {
            m_IsPlaying = false;
            StopAllAudio();
        }

        public void Beep(int fre, int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.Frequency = fre; unit.Duration = (int)(dur);
            lock (s_Lock)
            {
                m_UnitQueue.Enqueue(unit);
            }
        }

        public void Delay(int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.IsMute = true;
            unit.Duration = (int)(dur);
            lock (s_Lock)
            {
                m_UnitQueue.Enqueue(unit);
            }
        }

        public void StopAllAudio()
        {
            lock (s_Lock)
            {
                m_UnitQueue.Clear();
            }
        }
    }
}
