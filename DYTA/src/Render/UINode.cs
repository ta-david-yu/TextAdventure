﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using DYTA.Math;

namespace DYTA.Render
{
    public class UINode
    {
        #region Engine
        public class Engine
        {
            class RenderUnit
            {
                public Vector2Int ParentAnchor { get; set; }
                public UINode UINode { get; set; }

                public RenderUnit(Vector2Int anchor, UINode node)
                {
                    ParentAnchor = anchor;
                    UINode = node;
                }
            }

            private static readonly object s_Lock = new object();

            private static Engine s_Instance = null;
            public static Engine Instance
            {
                get
                {
                    //lock (s_Lock)
                    {
                        if (s_Instance == null)
                        {
                            Console.WriteLine("Error - need an instance as Singleton");
                        }
                        return s_Instance;
                    }
                }
            }

            public static void CreateSingleton(Math.RectInt bounds, PixelColor color)
            {
                s_Instance = new Engine(bounds, color);

                RectInt bound = new RectInt(bounds.Max, new Vector2Int(120, 15));
                FrameLogger.CreateSingleton(bound);
            }

            public bool IsDelayedCleanup { get; private set; } = false;
            public int NodeIdCounter { get; private set; } = 0;

            public UINode RootNode { get; private set; }

            public SingleColorCanvas RootCanvas { get; private set; }

            private Engine(Math.RectInt mainBounds, PixelColor color)
            {
                init(mainBounds, color);
            }

            private void init(Math.RectInt mainBounds, PixelColor color)
            {
                NodeIdCounter = 0;
                RootNode = new UINode(NodeIdCounter, mainBounds, "Root");
                NodeIdCounter++;

                RootCanvas = RootNode.AddUIComponent<SingleColorCanvas>();
                RootCanvas.CanvasPixelColor = color;

                RootNode.ParentCanvas = RootCanvas;
            }

            public UINode CreateNode(Math.RectInt nodeBounds, UINode parent = null, string name = "")
            {
                var node = new UINode(NodeIdCounter, nodeBounds, name);
                NodeIdCounter += 1;

                node.setParent(parent);
                return node;
            }

            public UINodeEnumerator NodeTreeTraverse
            {
                get
                {
                    return new UINodeEnumerator(RootNode);
                }
            }

            // Nodes with Dirty ParentCanvas are pre-rendered again
            public void PreRenderNodes()
            {
                int index = 0;
                foreach (var node in UINode.Engine.Instance.NodeTreeTraverse)
                {
                    if (node.ParentCanvas.IsDirty || (node.Canvas != null && node.Canvas.IsDirty))
                    {
                        node.PreRenderUIs();

                        /*
                        var info = string.Empty;
                        info += string.Format("{0,2} : {1,-15} ", node.InstanceId, node.Name);
                        info += string.Format("p {0,2} {1,-15} PCD:{2}", node.ParentCanvas.Node.InstanceId, node.ParentCanvas.Node.Name, node.ParentCanvas.IsDirty);
                        info += " - PreRenderer";
                        FrameLogger.Log(info);
                        */
                    }
                    index++;
                }
            }

            // Nodes with Dirty ParentCanvas are rendered again
            public void RenderNodes()
            {
                //FrameLogger.Log("");
                HashSet<Canvas> dirtyCanvases = new HashSet<Canvas>();

                int index = 0;
                foreach (var node in UINode.Engine.Instance.NodeTreeTraverse)
                {
                    if (node.ParentCanvas.IsDirty || (node.Canvas != null && node.Canvas.IsDirty))
                    {
                        node.RenderUIs();
                        dirtyCanvases.Add(node.ParentCanvas);
                        /*
                        var info = string.Empty;
                        info += string.Format("{0,2} : {1,-15} ", node.InstanceId, node.Name);
                        info += string.Format("p {0,2} {1,-15} PCD:{2}", node.ParentCanvas.Node.InstanceId, node.ParentCanvas.Node.Name, node.ParentCanvas.IsDirty);
                        info += " - Render";
                        FrameLogger.Log(info);
                        */
                    }
                    index++;
                }

                foreach (var canvas in dirtyCanvases)
                {
                    canvas.IsDirty = false;
                }
            }

            public void Destruction()
            {
                var color = new PixelColor(RootCanvas.CanvasPixelColor.BackgroundColor, RootCanvas.CanvasPixelColor.ForegroundColor);
                var bounds = RootNode.Bounds;
                init(bounds, color);
                IsDelayedCleanup = false;
            }

            // Clear all nodes
            public void CleanUp()
            {
                IsDelayedCleanup = true;
            }
        }
        #endregion

        public int InstanceId { get; private set; } = -1;

        public string Name { get; private set; } = string.Empty;

        private UINode m_Parent;
        public UINode Parent
        {
            get
            {
                if (m_Parent == null)
                {
                    if (this == Engine.Instance.RootNode)
                    {
                        return null;
                    }
                    else
                    {
                        return Engine.Instance.RootNode;
                    }
                }
                else
                {
                    return m_Parent;
                }
            }

            private set
            {
                m_Parent = value;
            }
        }

        public Canvas Canvas { get; private set; }

        private Canvas m_ParentCanvas;
        public Canvas ParentCanvas
        {
            get
            {
                return m_ParentCanvas;
            }

            private set
            {
                m_ParentCanvas = value;
            }
        }

        public Vector2Int WorldAnchor
        {
            get
            {
                Vector2Int pos = Vector2Int.Zero;
                var node = Parent;
                while (node != null)
                {
                    pos += node.Bounds.Position;
                    node = node.Parent;
                }
                return pos;
            }
        }

        public RectInt Bounds { get; private set; }

        private bool m_IsActive = true;
        public bool IsActive
        {
            get { return m_IsActive; }
            set
            {
                m_IsActive = value;
                SetDirty();
            }
        }

        private List<UINode> m_Children = new List<UINode>();
        public ReadOnlyCollection<UINode> Children { get { return m_Children.AsReadOnly(); } }

        private List<UIComponent> m_UIComponents = new List<UIComponent>();
        public ReadOnlyCollection<UIComponent> UIComponents => m_UIComponents.AsReadOnly();

        private UINode(int id, RectInt bound, string name)
        {
            InstanceId = id;
            Bounds = bound;
            Name = name;
        }

        public void SetPosition(Vector2Int pos)
        {
            Bounds = new RectInt(pos, Bounds.Size);
            SetDirty();
        }

        public void Translate(Vector2Int offset)
        {
            SetPosition(Bounds.Position + offset);
        }

        public T AddUIComponent<T>() where T : UIComponent, new()
        {
            T instance = new T();
            instance.OnInitializedByNode(this);
            m_UIComponents.Add(instance);

            var canvas = instance as Canvas;
            if (canvas != null)
            {
                Canvas = canvas as Canvas;
            }

            return instance;
        }

        public T GetUIComponent<T>() where T : UIComponent
        {
            foreach (var comp in UIComponents)
            {
                if (comp is T)
                {
                    return comp as T;
                }
            }
            return null;
        }

        public void PreRenderUIs()
        {
            for (int i = 0; i < UIComponents.Count; i++)
            {
                var ui = UIComponents[i];
                ui.PreRender();
            }

        }

        public void RenderUIs()
        {
            for (int i = 0; i < UIComponents.Count; i++)
            {
                var ui = UIComponents[i];
                ui.Render();
            }
        }

        // set position, set size
        public void SetDirty()
        {
            // ParentCanvas has to be set Dirty
            ParentCanvas.IsDirty = true;

            /*
            bool isDirtyForAll = false;
            foreach (var node in ParentCanvas.Node.Parent.Children)
            {
                if (node.Canvas != null)
                {
                    if (node.Canvas == ParentCanvas)
                    {
                        isDirtyForAll = true;
                    }
                    else if (isDirtyForAll)
                    {
                        node.Canvas.IsDirty = true;
                    }
                }
            }
            */
        }

        private void setParent(UINode node)
        {
            if (node == null)
            {
                node = Engine.Instance.RootNode;
            }

            Parent = node;
            Parent.m_Children.Add(this);

            // setParentCanvas
            if (Parent.Canvas != null)
            {
                ParentCanvas = Parent.Canvas;
            }
            else
            {
                ParentCanvas = Parent.ParentCanvas;
            }
        }
    }
}
