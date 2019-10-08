using System;
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

            private static Engine s_Instance = null;
            public static Engine Instance
            {
                get
                {
                    if (s_Instance == null)
                    {
                        Console.WriteLine("Error - need an instance as Singleton");
                    }
                    return s_Instance;
                }
            }

            public static void CreateSingleton(Math.RectInt bounds, PixelColor color)
            {
                s_Instance = new Engine(bounds, color);
            }

            private int m_NodeIdCounter = 0;

            public UINode RootNode { get; private set; }

            public SingleColorCanvas RootCanvas { get; private set; }

            private Engine(Math.RectInt mainBounds, PixelColor color)
            {
                RootNode = new UINode(m_NodeIdCounter, mainBounds);
                m_NodeIdCounter += 1;

                RootCanvas = RootNode.AddUIComponent<SingleColorCanvas>();
                RootCanvas.CanvasPixelColor = color;
                RootCanvas.ResetBuffer();
            }

            public UINode CreateNode(Math.RectInt nodeBounds, UINode parent = null)
            {
                var node = new UINode(m_NodeIdCounter, nodeBounds);
                m_NodeIdCounter += 1;

                node.SetParent(parent);
                return node;
            }

            public void PreRenderNodes()
            {
                Stack<UINode> nodeStack = new Stack<UINode>();
                nodeStack.Push(RootNode);

                while (nodeStack.Count > 0)
                {
                    var currNode = nodeStack.Pop();

                    if (currNode.IsActive)
                    {
                        currNode.PreRenderUIs();

                        for (int i = currNode.Children.Count - 1; i >= 0; i--)
                        {
                            var childNode = currNode.Children[i];
                            nodeStack.Push(childNode);
                        }
                    }
                }
            }

            public void RenderNodes()
            {
                Stack<UINode> nodeStack = new Stack<UINode>();
                nodeStack.Push(RootNode);

                while (nodeStack.Count > 0)
                {
                    var currNode = nodeStack.Pop();

                    if (currNode.IsActive)
                    {
                        currNode.RenderUIs();

                        for (int i = currNode.Children.Count - 1; i >= 0; i--)
                        {
                            var childNode = currNode.Children[i];
                            nodeStack.Push(childNode);
                        }
                    }
                }
            }
        }
        #endregion

        public int InstanceId { get; private set; } = -1;


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

        public bool IsActive { get; set; } = true;

        private List<UINode> m_Children = new List<UINode>();
        public ReadOnlyCollection<UINode> Children { get { return m_Children.AsReadOnly(); } }

        private List<UIComponent> m_UIComponents = new List<UIComponent>();
        public ReadOnlyCollection<UIComponent> UIComponents => m_UIComponents.AsReadOnly();

        private UINode(int id, RectInt bound)
        {
            InstanceId = id;
            Bounds = bound;
        }

        public void SetBounds(RectInt rect)
        {
            Bounds = rect;
        }

        public void SetPosition(Vector2Int pos)
        {
            Bounds = new RectInt(pos, Bounds.Size);
        }

        public void SetSize(Vector2Int size)
        {
            Bounds = new RectInt(Bounds.Position, size);
        }

        public void SetParent(UINode node)
        {
            if (node == null)
            {
                node = Engine.Instance.RootNode;
            }

            m_Parent = node;
            m_Parent.m_Children.Add(this);
        }

        public T AddUIComponent<T>() where T : UIComponent, new()
        {
            T instance = new T();
            instance.OnAddedToNode(this);
            m_UIComponents.Add(instance);
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

            /*
            Console.SetCursorPosition(0, 25);
            Console.WriteLine(InstanceId);
            */
        }
    }
}
