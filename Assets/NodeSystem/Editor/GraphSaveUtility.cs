using Subtegral.DialogueSystem.DataContainers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Subtegral.DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<GraphNode> Nodes => _graphView.nodes.ToList().Cast<GraphNode>().ToList();

        private GraphContainer _GraphContainer;
        private Graph _graphView;

        public static GraphSaveUtility GetInstance(Graph graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var GraphContainerObject = ScriptableObject.CreateInstance<GraphContainer>();
            if (!SaveNodes(fileName, GraphContainerObject))
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(GraphContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(GraphContainerObject, $"Assets/Resources/{fileName}.asset");
            }
            else
            {
                GraphContainer container = loadedAsset as GraphContainer;
                container.NodeLinks = GraphContainerObject.NodeLinks;
                container.NodeData = GraphContainerObject.NodeData;                
                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, GraphContainer GraphContainerObject)
        {
            if (!Edges.Any())
            {
                return false;
            }

            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as GraphNode);
                var inputNode = (connectedSockets[i].input.node as GraphNode);
                GraphContainerObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = connectedSockets[i].output.portName,
                    TargetNodeGUID = inputNode.GUID
                });
            }

            foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {
                GraphContainerObject.NodeData.Add(new GraphNodeData
                {
                    NodeGUID = node.GUID,
                    Text = node.Text,
                    Position = node.GetPosition().position
                });
            }

            return true;
        }

        public void LoadGraph(string fileName)
        {
            _GraphContainer = Resources.Load<GraphContainer>(fileName);
            if (_GraphContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Graph Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            GenerateGraphNodes();
            ConnectGraphNodes();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.EntyPoint).GUID = _GraphContainer.NodeLinks[0].BaseNodeGUID;
            foreach (var perNode in Nodes)
            {
                if (perNode.EntyPoint) continue;
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateGraphNodes()
        {
            foreach (var perNode in _GraphContainer.NodeData)
            {
                var tempNode = _graphView.CreateNode(perNode.Text, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _GraphContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ConnectGraphNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _GraphContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        _GraphContainer.NodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }
    }
}