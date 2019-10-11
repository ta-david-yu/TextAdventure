﻿using DYTA.Render;
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

        private Action m_NextSceneSetupCall = delegate { };

        public ApplicationBase(Math.RectInt bounds, PixelColor color)
        {
            UINode.Engine.CreateSingleton(bounds, color);
        }

        public void Run()
        {
            registerGlobalEvent();

            loadInitialScene();

            IsRunning = true;

            gameLoop();
        }

        private void gameLoop()
        {
            long minimumStepPerFrame = 20; // TODO: 40

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            long timeStep = minimumStepPerFrame;

            long frameCounter = 0;

            while (true)
            {
                // debug
                string frameInfo = string.Format("FRAME: {0, -5}- TIMESTEP: {1, -5}- NODE CT: {2, -5}", ++frameCounter, timeStep, UINode.Engine.Instance.NodeIdCounter);
                FrameLogger.Log(frameInfo);

                // input
                Input.KeyboardListener.Instance.QueryInput();

                // logic update
                logicUpdate(timeStep);

                // rendering
                UINode.Engine.Instance.PreRenderNodes();
                UINode.Engine.Instance.RenderNodes();

                // rendering destruction
                if (UINode.Engine.Instance.IsDelayedCleanup)
                {
                    UINode.Engine.Instance.Destruction();
                }

                // logging
                FrameLogger.Update();

                // scene loading
                if (WillChangeScene)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("LOADING......");

                    WillChangeScene = false;

                    Audio.AudioManager.Instance.End();
                    UINode.Engine.Instance.Destruction();

                    m_NextSceneSetupCall.Invoke();

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

        protected void loadScene(Action sceneSetupCall)
        {
            WillChangeScene = true;
            m_NextSceneSetupCall = sceneSetupCall;
        }

        protected virtual void registerGlobalEvent()
        {
            Input.KeyboardListener.Instance.OnKeyPressed += handleOnKeyPressed;
        }

        protected abstract void loadInitialScene();

        protected abstract void logicUpdate(long timeStep);

        protected abstract void handleOnKeyPressed(ConsoleKeyInfo keyInfo);
    }
}
