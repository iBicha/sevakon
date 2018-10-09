using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph.Drawing
{
    public class ImageEffectsView : VisualElement
    {
        ImageEffectMasterNode m_Node;

        public ImageEffectsView(ImageEffectMasterNode node)
        {
            m_Node = node;

            PropertySheet ps = new PropertySheet();
            Add(ps);
        }
    }
}
