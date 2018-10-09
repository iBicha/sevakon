using System.Collections.Generic;
using System.Reflection;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Texture", "Source Texture")]
    public class SourceTextureNode : AbstractMaterialNode, IGeneratesBodyCode
    {
        public const int k_TextureOutputId = 0;
        public const int k_UVOutputId = 1;

        const string k_TextureOutputName = "MainTexture";
        const string k_UVOutputName = "UV";

        public SourceTextureNode()
        {
            name = "Source Texture";
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Texture2DMaterialSlot(k_TextureOutputId, k_TextureOutputName, k_TextureOutputName,
                SlotType.Output));

            var uvSlot = new UVMaterialSlot(k_UVOutputId, k_UVOutputName, k_UVOutputName, UVChannel.UV0);
            typeof(MaterialSlot).GetField("m_SlotType", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(uvSlot, SlotType.Output);
            AddSlot(uvSlot);
            RemoveSlotsNameNotMatching(new[] {k_TextureOutputId, k_UVOutputId});
        }

        public override string GetVariableNameForSlot(int slotId)
        {
            switch (slotId)
            {
                case k_TextureOutputId:
                    return "_MainText";
                case k_UVOutputId:
                    return "_UV";
            }
            return null;
//            return base.GetVariableNameForSlot(slotId);
        }

//        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
//        {
//            properties.AddShaderProperty(new ColorShaderProperty()
//            {
//                overrideReferenceName = GetVariableNameForSlot(OutputSlotId),
//                generatePropertyBlock = true,
////                value = m_Texture,
//            });
//        }
//
//        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
//        {
//            properties.Add(new PreviewProperty(PropertyType.Color)
//            {
//                name = GetVariableNameForSlot(OutputSlotId)
////                textureValue = texture
//            });
//        }
        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            var uvName = GetSlotValue(k_UVOutputId, generationMode);

            //Sampler input slot

            var id = GetSlotValue(k_TextureOutputId, generationMode);
            var result = string.Format("{0}4 {1} = SAMPLE_TEXTURE2D({2}, {3}, {4});"
                    , precision
                    , GetVariableNameForSlot(k_TextureOutputId)
                    , id
                    ,  "sampler" + id
                    , uvName);

            visitor.AddShaderChunk(result, true);

//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.r;", precision, GetVariableNameForSlot(k_TextureOutputId), GetVariableNameForSlot(OutputSlotRGBAId)), true);
//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.g;", precision, GetVariableNameForSlot(k_TextureOutputId), GetVariableNameForSlot(OutputSlotRGBAId)), true);
//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.b;", precision, GetVariableNameForSlot(k_TextureOutputId), GetVariableNameForSlot(OutputSlotRGBAId)), true);
//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.a;", precision, GetVariableNameForSlot(k_TextureOutputId), GetVariableNameForSlot(OutputSlotRGBAId)), true);
        }
    }
}