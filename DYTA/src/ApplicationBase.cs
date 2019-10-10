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

        public void Run()
        {
            registerGlobalEvent();

            initialSetup();

            IsRunning = true;

            gameLoop();
        }

        protected virtual void registerGlobalEvent()
        {
            Input.KeyboardListener.Instance.OnKeyPressed += handleOnKeyPressed;
        }

        protected abstract void initialSetup();

        protected virtual void gameLoop()
        {
            long minimumStepPerFrame = 20; // TODO: 40

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            long timeStep = minimumStepPerFrame;

            long frameCounter = 0;

            while (true)
            {
                // debug
                string frameInfo = string.Format("FRAME: {0, -5}- TIMESTEP: {1, -5}", ++frameCounter, timeStep);
                FrameLogger.Log(frameInfo);

                // input
                Input.KeyboardListener.Instance.QueryInput();

                // logic update
                logicUpdate(timeStep);

                // rendering
                UINode.Engine.Instance.PreRenderNodes();
                UINode.Engine.Instance.RenderNodes();

                // logging
                FrameLogger.Update();

                // time calculation
                timeStep = stopwatch.ElapsedMilliseconds;

                if (timeStep < minimumStepPerFrame)
                {
                    int sleep = (int)(minimumStepPerFrame - timeStep);
                    System.Threading.Thread.Sleep(sleep);
                    timeStep = minimumStepPerFrame;
                }

                stopwatch.Restart();

                if (!IsRunning)
                {
                    break;
                }
            }

            stopwatch.Stop();
        }

        protected abstract void logicUpdate(long timeStep);

        protected abstract void handleOnKeyPressed(ConsoleKeyInfo keyInfo);

    }
}
