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

        private static readonly object s_QueueLock = new object();
        private static readonly object s_IsMuteLock = new object();

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

        private System.Threading.Thread m_MusicThread;

        private Queue<BeepUnit> m_MusicQueue = new Queue<BeepUnit>();

        private bool m_IsPlaying = false;

        public event Action OnMusicQueueEmptied = delegate { };
        public event Action OnSoundQueueEmptied = delegate { };

        private bool m_IsMute = false;
        public bool IsMute
        {
            get
            {
                return m_IsMute;
            }
            set
            {
                lock (s_IsMuteLock)
                {
                    m_IsMute = value;
                }
            }
        }

        AudioManager()
        {

        }

        public void Begin()
        {
            if (m_MusicThread == null)
            {
                m_MusicThread = new System.Threading.Thread(() =>
                {
                    while (true)
                    {
                        if (m_IsPlaying)
                        {
                            if (m_MusicQueue.Count > 0)
                            {
                                BeepUnit unit;
                                lock (s_QueueLock)
                                {
                                    unit = m_MusicQueue.Dequeue();
                                }

                                if (!IsMute)
                                {
                                    if (!unit.IsMute)
                                        Console.Beep(unit.Frequency, unit.Duration);
                                    else
                                        System.Threading.Thread.Sleep(unit.Duration);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(unit.Duration);
                                }

                                if (m_MusicQueue.Count == 0)
                                {
                                    OnMusicQueueEmptied.Invoke();
                                }
                            }
                        }
                    }
                });
                m_MusicThread.IsBackground = true;
                m_MusicThread.Start();
            }

            m_IsPlaying = true;
        }

        public void End()
        {
            m_IsPlaying = false;
            StopAllAudio();
        }

        public void BeepMusic(int fre, int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.Frequency = fre; unit.Duration = (int)(dur);
            lock (s_QueueLock)
            {
                m_MusicQueue.Enqueue(unit);
            }
        }

        public void DelayMusic(int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.IsMute = true;
            unit.Duration = (int)(dur);
            lock (s_QueueLock)
            {
                m_MusicQueue.Enqueue(unit);
            }
        }

        public void StopAllAudio()
        {
            lock (s_QueueLock)
            {
                m_MusicQueue.Clear();
            }
        }
    }
}
