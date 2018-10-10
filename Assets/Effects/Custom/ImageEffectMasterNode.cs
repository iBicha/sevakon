using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    [Title("Master", "Image Effect")]
    public class ImageEffectMasterNode : MasterNode<IImageEffectSubShader>
    {
        public const string ColorSlotName = "Color";

        public const int ColorSlotId = 0;

        [SerializeField] SurfaceType m_SurfaceType;

        public SurfaceType surfaceType
        {
            get { return m_SurfaceType; }
            set
            {
                if (m_SurfaceType == value)
                    return;

                m_SurfaceType = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField] AlphaMode m_AlphaMode;

        public AlphaMode alphaMode
        {
            get { return m_AlphaMode; }
            set
            {
                if (m_AlphaMode == value)
                    return;

                m_AlphaMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public ImageEffectMasterNode()
        {
            UpdateNodeAfterDeserialization();
        }

//        public override string documentationURL
//        {
//            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Unlit-Master-Node"; }
//        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            name = "Image Effect Master";
            AddSlot(new ColorRGBMaterialSlot(ColorSlotId, ColorSlotName, ColorSlotName, SlotType.Input, Color.grey,
                ColorMode.Default, ShaderStageCapability.Fragment));

            // clear out slot names that do not match the slots
            // we support
            RemoveSlotsNameNotMatching(
                new[]
                {
                    ColorSlotId
                });
        }

        protected override VisualElement CreateCommonSettingsElement()
        {
            return new ImageEffectsView(this);
        }
    }
}