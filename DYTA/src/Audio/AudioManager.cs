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

        private System.Threading.Thread m_MusicThread;
        private System.Threading.Thread m_SfxThread;

        private Queue<BeepUnit> m_MusicQueue = new Queue<BeepUnit>();
        private Queue<BeepUnit> m_SfxQueue = new Queue<BeepUnit>();

        private bool m_IsPlaying = false;

        public event Action OnMusicQueueEmptied = delegate { };
        public event Action OnSoundQueueEmptied = delegate { };

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
                                lock (s_Lock)
                                {
                                    unit = m_MusicQueue.Dequeue();
                                }

                                if (!unit.IsMute)
                                    Console.Beep(unit.Frequency, unit.Duration);
                                else
                                    System.Threading.Thread.Sleep(unit.Duration);

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

            if (m_SfxThread == null)
            {
                m_SfxThread = new System.Threading.Thread(() =>
                {
                    while (true)
                    {
                        if (m_IsPlaying)
                        {
                            if (m_SfxQueue.Count > 0)
                            {
                                BeepUnit unit;
                                lock (s_Lock)
                                {
                                    unit = m_SfxQueue.Dequeue();
                                }

                                if (!unit.IsMute)
                                    Console.Beep(unit.Frequency, unit.Duration);
                                else
                                    System.Threading.Thread.Sleep(unit.Duration);

                                if (m_SfxQueue.Count == 0)
                                {
                                    OnSoundQueueEmptied.Invoke();
                                }
                            }
                        }
                    }
                });
                m_SfxThread.IsBackground = true;
                m_SfxThread.Start();
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
            lock (s_Lock)
            {
                m_MusicQueue.Enqueue(unit);
            }
        }

        public void DelayMusic(int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.IsMute = true;
            unit.Duration = (int)(dur);
            lock (s_Lock)
            {
                m_MusicQueue.Enqueue(unit);
            }
        }

        public void BeepSfx(int fre, int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.Frequency = fre; unit.Duration = (int)(dur);
            lock (s_Lock)
            {
                m_SfxQueue.Enqueue(unit);
            }
        }

        public void DelaySfx(int dur)
        {
            BeepUnit unit = new BeepUnit();
            unit.IsMute = true;
            unit.Duration = (int)(dur);
            lock (s_Lock)
            {
                m_SfxQueue.Enqueue(unit);
            }
        }

        public void StopAllAudio()
        {
            lock (s_Lock)
            {
                m_MusicQueue.Clear();
                m_SfxQueue.Clear();
            }
        }
    }
}
