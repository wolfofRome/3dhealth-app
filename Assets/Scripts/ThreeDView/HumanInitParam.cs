using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ThreeDView
{
    [Serializable]
    public class HumanInitParam
    {
        public HumanInitParam()
        {
            _alphaOfEachLayer = new Dictionary<HumanLayer, float>();
            foreach (HumanLayer layer in Enum.GetValues(typeof(HumanLayer)))
            {
                _alphaOfEachLayer.Add(layer, 1.0f);
            }
        }

        private Dictionary<HumanLayer, float> _alphaOfEachLayer;

        public float GetAlpha(HumanLayer layer)
        {
            return _alphaOfEachLayer[layer];
        }

        public void SetAlpha(HumanLayer layer, float alpha)
        {
            _alphaOfEachLayer[layer] = alpha;
        }
    }
}
