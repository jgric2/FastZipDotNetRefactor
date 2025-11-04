using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TreeViewMS
{
    /// <summary>
    /// Summary description for TreeViewMS.
    /// </summary>
    public class TreeViewMS : TreeView
    {
        protected ArrayList m_coll = new ArrayList();
        protected TreeNode m_lastNode, m_firstNode;

        public TreeViewMS()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            m_coll = new ArrayList();

            // Removed the event subscription to prevent recursion
            // this.ItemDrag += new ItemDragEventHandler(OnItemDrag);
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl && m_coll.Contains(e.Node))
            {
                e.Cancel = true;
                removePaintFromNodes();
                m_coll.Remove(e.Node);
                paintSelectedNodes();
                return;
            }

            m_lastNode = e.Node;
            if (!bShift) m_firstNode = e.Node;
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!m_coll.Contains(e.Node))
                {
                    m_coll.Add(e.Node);
                }
                else
                {
                    removePaintFromNodes();
                    m_coll.Remove(e.Node);
                }
                paintSelectedNodes();
            }
            else
            {
                if (bShift)
                {
                    Queue myQueue = new Queue();
                    TreeNode uppernode = m_firstNode;
                    TreeNode bottomnode = e.Node;

                    bool bParent = isParent(m_firstNode, e.Node);
                    if (!bParent)
                    {
                        bParent = isParent(bottomnode, uppernode);
                        if (bParent)
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }
                    if (bParent)
                    {
                        TreeNode n = bottomnode;
                        while (n != uppernode.Parent)
                        {
                            if (!m_coll.Contains(n))
                                myQueue.Enqueue(n);
                            n = n.Parent;
                        }
                    }
                    else
                    {
                        if ((uppernode.Parent == null && bottomnode.Parent == null) ||
                            (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode)))
                        {
                            int nIndexUpper = uppernode.Index;
                            int nIndexBottom = bottomnode.Index;
                            if (nIndexBottom < nIndexUpper)
                            {
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            TreeNode n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!m_coll.Contains(n))
                                    myQueue.Enqueue(n);
                                n = n.NextNode;
                                nIndexUpper++;
                            }
                        }
                        else
                        {
                            if (!m_coll.Contains(uppernode)) myQueue.Enqueue(uppernode);
                            if (!m_coll.Contains(bottomnode)) myQueue.Enqueue(bottomnode);
                        }
                    }

                    m_coll.AddRange(myQueue);
                    paintSelectedNodes();
                    m_firstNode = e.Node;
                }
                else
                {
                    if (m_coll != null && m_coll.Count > 0)
                    {
                        removePaintFromNodes();
                        m_coll.Clear();
                    }

                    if (m_coll == null)
                    {
                        m_coll = new ArrayList();
                    }

                    m_coll.Add(e.Node);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ArrayList SelectedNodes
        {
            get => m_coll;
            set
            {
                removePaintFromNodes();
                m_coll.Clear();
                m_coll = value;
                paintSelectedNodes();
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            // Only start a drag for left-button drags; ignore Ctrl/Shift modifiers to avoid clashing with multi-select
            if (e.Button != MouseButtons.Left)
            {
                base.OnItemDrag(e);
                return;
            }

            // Build list of nodes to drag (multi-select aware)
            var nodes = new List<TreeNode>();
            if (m_coll != null && m_coll.Count > 0 && m_coll.Contains((TreeNode)e.Item))
            {
                foreach (TreeNode n in m_coll)
                    if (n != null) nodes.Add(n);
            }
            else
            {
                nodes.Add((TreeNode)e.Item);
            }

            // Extract distinct paths (Tag holds full path)
            var paths = new List<string>(nodes.Count);
            foreach (var n in nodes)
            {
                if (n?.Tag is string p && !string.IsNullOrWhiteSpace(p))
                    paths.Add(p);
            }
            var distinct = paths.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

            // Prepare a DataObject that contains:
            // 1) Standard FileDrop (so any drop target that expects explorer-like drops will work)
            // 2) Your custom "TreeNodeArray" (back-compat with your app)
            var data = new DataObject();

            if (distinct.Length > 0)
            {
                var sc = new StringCollection();
                sc.AddRange(distinct);
                data.SetFileDropList(sc);
            }

            var arr = new ArrayList(nodes);
            data.SetData("TreeNodeArray", arr);

            DoDragDrop(data, DragDropEffects.Copy);
        }

        protected bool isParent(TreeNode parentNode, TreeNode childNode)
        {
            if (parentNode == childNode)
                return true;

            TreeNode n = childNode;
            bool bFound = false;
            while (!bFound && n != null)
            {
                n = n.Parent;
                bFound = (n == parentNode);
            }
            return bFound;
        }

        protected void paintSelectedNodes()
        {
            if (m_coll == null)
                return;

            foreach (TreeNode n in m_coll)
            {
                n.BackColor = SystemColors.Highlight;
                n.ForeColor = SystemColors.HighlightText;
            }
        }

        protected void removePaintFromNodes()
        {
            if (m_coll == null)
                return;

            if (m_coll.Count == 0)
                return;

            try
            {
                TreeNode n0 = (TreeNode)m_coll[0];
                Color back = this.BackColor;
                Color fore = this.ForeColor;

                foreach (TreeNode n in m_coll)
                {
                    n.BackColor = back;
                    n.ForeColor = fore;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if necessary.
            }
        }
    }
}
