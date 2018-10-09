using System;
using System.IO;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    public class CreateImageEffectShaderGraph : EndNameEditAction
    {
        [MenuItem("Assets/Create/Shader/Image Effect Graph", false, 208)]
        public static void CreateMaterialGraph()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateImageEffectShaderGraph>(),
                "New Image Effect Graph.ShaderGraph", null, null);
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var graph = new MaterialGraph();

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            graph.previewData.serializedMesh.mesh = quad.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(quad);
            
            var mainTextureNode = new MainTextureNode();
            var master = new ImageEffectMasterNode();

            graph.AddNode(master);
            graph.AddNode(mainTextureNode);

            graph.Connect(mainTextureNode.GetSlotReference(MainTextureNode.OutputSlotRGBAId),
                master.GetSlotReference(ImageEffectMasterNode.ColorSlotId));


            var drawState = mainTextureNode.drawState;
            var drawStatePosition = drawState.position;
            drawStatePosition.x -= 300;
            drawState.position = drawStatePosition;
            mainTextureNode.drawState = drawState;

            File.WriteAllText(pathName, EditorJsonUtility.ToJson(graph));
            AssetDatabase.Refresh();
        }
    }
}