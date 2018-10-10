using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Input","Image Effects", "Main Texture")]
    public class MainTextureNode : AbstractMaterialNode, IGeneratesBodyCode, IMayRequireMeshUV
    {
        public const int OutputSlotRGBAId = 0;
        public const int OutputSlotRId = 1;
        public const int OutputSlotGId = 2;
        public const int OutputSlotBId = 3;
        public const int OutputSlotAId = 4;

        public const int UVInput = 5;
        public const int UVOutput = 6;

        const string kOutputSlotRGBAName = "RGBA";
        const string kOutputSlotRName = "R";
        const string kOutputSlotGName = "G";
        const string kOutputSlotBName = "B";
        const string kOutputSlotAName = "A";

        const string kUVInputName = "UV";
        const string kUVOutputName = "UV";

        public const int TextureInputId = 7;
        const string kTextureInputName = "_MainTex";
        const string kTexturePreviewName = "_PreviewTexture";

        public override bool hasPreview => true;

        public MainTextureNode()
        {
            name = "Main Texture";
            UpdateNodeAfterDeserialization();
        }


        public sealed override void UpdateNodeAfterDeserialization()
        {
            precision = OutputPrecision.@float;
            
            AddSlot(new Vector4MaterialSlot(OutputSlotRGBAId, kOutputSlotRGBAName, kOutputSlotRGBAName, SlotType.Output,
                Vector4.zero, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(OutputSlotRId, kOutputSlotRName, kOutputSlotRName, SlotType.Output, 0,
                ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(OutputSlotGId, kOutputSlotGName, kOutputSlotGName, SlotType.Output, 0,
                ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(OutputSlotBId, kOutputSlotBName, kOutputSlotBName, SlotType.Output, 0,
                ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(OutputSlotAId, kOutputSlotAName, kOutputSlotAName, SlotType.Output, 0,
                ShaderStageCapability.Fragment));

            var uvOutSlot = new UVMaterialSlot(UVOutput, kUVOutputName, kUVOutputName, UVChannel.UV0);
            typeof(MaterialSlot).GetField("m_SlotType", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(uvOutSlot, SlotType.Output);
            AddSlot(uvOutSlot);
            
            AddSlot(new Texture2DMaterialSlot(TextureInputId, kTextureInputName, kTextureInputName, SlotType.Input, ShaderStageCapability.All, true));
            
            AddSlot(new UVMaterialSlot(UVInput, kUVInputName, kUVInputName, UVChannel.UV0));

            RemoveSlotsNameNotMatching(new[]
                {OutputSlotRGBAId, OutputSlotRId, OutputSlotGId, OutputSlotBId, OutputSlotAId, UVOutput, UVInput, TextureInputId});
        }
        
        public override string GetVariableNameForSlot(int slotId)
        {
            if (slotId == TextureInputId)
                return kTextureInputName;
            
            return base.GetVariableNameForSlot(slotId);
        }

        // Node generations
        public virtual void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext,
            GenerationMode generationMode)
        {

            var textureInput = generationMode.IsPreview() ? kTexturePreviewName : kTextureInputName;
            
            var uvName = GetSlotValue(UVInput, generationMode);
            
            var uvSet = string.Format("{0}2 {1} = {2};"
                , precision
                , GetVariableNameForSlot(UVInput)
                , uvName);
            visitor.AddShaderChunk(uvSet, true);

            var result = string.Format("{0}4 {1} = SAMPLE_TEXTURE2D({2}, {3}, {4});"
                , precision
                , GetVariableNameForSlot(OutputSlotRGBAId)
                , textureInput
                , "sampler" + textureInput
                , GetVariableNameForSlot(UVInput));

            visitor.AddShaderChunk(result, true);

            visitor.AddShaderChunk(
                string.Format("{0} {1} = {2}.r;", precision, GetVariableNameForSlot(OutputSlotRId),
                    GetVariableNameForSlot(OutputSlotRGBAId)), true);
            visitor.AddShaderChunk(
                string.Format("{0} {1} = {2}.g;", precision, GetVariableNameForSlot(OutputSlotGId),
                    GetVariableNameForSlot(OutputSlotRGBAId)), true);
            visitor.AddShaderChunk(
                string.Format("{0} {1} = {2}.b;", precision, GetVariableNameForSlot(OutputSlotBId),
                    GetVariableNameForSlot(OutputSlotRGBAId)), true);
            visitor.AddShaderChunk(
                string.Format("{0} {1} = {2}.a;", precision, GetVariableNameForSlot(OutputSlotAId),
                    GetVariableNameForSlot(OutputSlotRGBAId)), true);
        }

        public bool RequiresMeshUV(UVChannel channel, ShaderStageCapability stageCapability)
        {
            s_TempSlots.Clear();
            GetInputSlots(s_TempSlots);
            foreach (var slot in s_TempSlots)
            {
                if (slot.RequiresMeshUV(channel))
                    return true;
            }

            return false;
        }

        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            base.CollectShaderProperties(properties, generationMode);
            properties.AddShaderProperty(new TextureShaderProperty()
            {
                overrideReferenceName = "_PreviewTexture",
                generatePropertyBlock = false
            });
        }
    }
}