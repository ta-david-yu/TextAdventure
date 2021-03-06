﻿using DYTA.Math;
using DYTA.Render;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DYTA
{
    public abstract class ApplicationBase
    {
        public bool IsRunning { get; protected set; } = false;

        public bool WillChangeScene { get; protected set; } = false;

        private Action m_NextSceneInitCall = delegate { };
        private Action m_ExitSceneCall = delegate { };

        public ApplicationBase(Vector2Int windowSize, PixelColor color)
        {
            UINode.Engine.CreateSingleton(windowSize, color);
        }

        public void Run()
        {
            registerGlobalEvent();

            loadInitialScene();

            IsRunning = true;

            Audio.AudioManager.Instance.Begin();

            gameLoop();
        }

        private void gameLoop()
        {
            long minimumStepPerFrame = 20;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            long timeStep = minimumStepPerFrame;

            while (true)
            {
                // debug
                //string frameInfo = string.Format("FRAME: {0, -5}- TIMESTEP: {1, -5}- NODE CT: {2, -5}", ++frameCounter, timeStep, UINode.Engine.Instance.NodeIdCounter);
                //FrameLogger.Log(frameInfo);

                // input
                Input.KeyboardListener.Instance.QueryInput();

                // logic update
                update(timeStep);

                // rendering
                UINode.Engine.Instance.PreRenderNodes();
                UINode.Engine.Instance.RenderNodesToBuffer();
                UINode.Engine.Instance.DrawToConsole();

                // late logic update, after render
                postRenderUpdate(timeStep);

                // logging
                FrameLogger.DrawFrameLog();

                // scene loading
                if (WillChangeScene)
                {
                    Console.SetCursorPosition(0, UINode.Engine.Instance.RootNode.Bounds.Min.Y + 0);
                    Console.WriteLine("LOADING......");

                    WillChangeScene = false;

                    Audio.AudioManager.Instance.End();
                    UINode.Engine.Instance.Destruction();

                    m_ExitSceneCall.Invoke();
                    m_NextSceneInitCall.Invoke();

                    Audio.AudioManager.Instance.Begin();
                }
                else
                {
                    // time calculation
                    timeStep = stopwatch.ElapsedMilliseconds;

                    if (timeStep < minimumStepPerFrame)
                    {
                        int sleep = (int)(minimumStepPerFrame - timeStep);
                        System.Threading.Thread.Sleep(sleep);
                        timeStep = minimumStepPerFrame;
                    }

                    stopwatch.Restart();
                }

                if (!IsRunning)
                {
                    break;
                }
            }

            stopwatch.Stop();
        }

        protected void loadScene(Action onInitScene, Action onExitScene)
        {
            WillChangeScene = true;
            m_NextSceneInitCall = onInitScene;
            m_ExitSceneCall = onExitScene;
        }

        protected virtual void registerGlobalEvent()
        {
            Input.KeyboardListener.Instance.OnKeyPressed += handleOnKeyPressed;
        }

        protected abstract void loadInitialScene();

        protected abstract void update(long timeStep);

        protected virtual void postRenderUpdate(long timeStep) { }

        protected abstract void handleOnKeyPressed(ConsoleKeyInfo keyInfo);
    }
}
