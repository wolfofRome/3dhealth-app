using Assets.Scripts.ThreeDView;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Common.Data
{
    public class FatMaterialConfig : ScriptableObject
    {
        public List<FatPartMaterialPair> _fatMaterial = new List<FatPartMaterialPair>();

        [Serializable]
        public class FatPartMaterialPair
        {
            public FatPart part;
            public Material material;
        }

        public Material GetMaterial(FatPart part)
        {
            Assert.IsTrue(_fatMaterial != null && _fatMaterial.Count > 0);
            Material mat = null;
            foreach (FatPartMaterialPair fmp in _fatMaterial)
            {
                if (fmp.part == part)
                {
                    mat = fmp.material;
                    break;
                }
            }
            return mat;
        }
    }
}