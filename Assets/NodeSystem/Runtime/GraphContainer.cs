using System;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class GraphContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<GraphNodeData> NodeData = new List<GraphNodeData>();
    }
}