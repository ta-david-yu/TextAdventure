using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DYTA.Render
{
    public struct UINodeEnumerator : IEnumerable<UINode>
    {
        private UINode m_Root;
        private bool m_IgnoreInactiveNodeAndItsChildren;

        public UINodeEnumerator(UINode root, bool ignoreInactiveNodeAndItsChildren = true)
        {
            m_Root = root;
            m_IgnoreInactiveNodeAndItsChildren = ignoreInactiveNodeAndItsChildren;
        }

        public IEnumerator<UINode> GetEnumerator()
        {
            Stack<UINode> nodeStack = new Stack<UINode>();
            nodeStack.Push(m_Root);

            while (nodeStack.Count > 0)
            {
                var currNode = nodeStack.Pop();

                if (!m_IgnoreInactiveNodeAndItsChildren || currNode.IsActive)
                {
                    yield return currNode;

                    for (int i = currNode.Children.Count - 1; i >= 0; i--)
                    {
                        var childNode = currNode.Children[i];
                        nodeStack.Push(childNode);
                    }
                }
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
