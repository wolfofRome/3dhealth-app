using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Common.Config
{
    public enum BlockmanPart : byte
    {
        NeckDown,
        NeckUp,
        Face,
        MuneL,
        MuneR,
        HaraUp,
        HaraDown,
        UpLegL,
        UpLegR,
        Senkotsu,
        KataL,
        JowanL,
        HandL,
        ZenwanL,
        KataR,
        JowanR,
        HandR,
        ZenwanR,
        Leg1L,
        Leg2L,
        FootL,
        Leg1R,
        Leg2R,
        FootR
    }

    public class DistortionConfig : ScriptableObject
    {
        [FormerlySerializedAs("_blockmanTextureList")] [SerializeField]
        private List<BlockmanTextureInfo> blockmanTextureList = new List<BlockmanTextureInfo>
        {
            new BlockmanTextureInfo(BlockmanPart.Face     ),
            new BlockmanTextureInfo(BlockmanPart.NeckUp  ),
            new BlockmanTextureInfo(BlockmanPart.NeckDown),
            new BlockmanTextureInfo(BlockmanPart.MuneL   ),
            new BlockmanTextureInfo(BlockmanPart.MuneR   ),
            new BlockmanTextureInfo(BlockmanPart.HaraUp  ),
            new BlockmanTextureInfo(BlockmanPart.HaraDown),
            new BlockmanTextureInfo(BlockmanPart.Senkotsu ),
            new BlockmanTextureInfo(BlockmanPart.UpLegL  ),
            new BlockmanTextureInfo(BlockmanPart.UpLegR  ),
            new BlockmanTextureInfo(BlockmanPart.KataL   ),
            new BlockmanTextureInfo(BlockmanPart.KataR   ),
            new BlockmanTextureInfo(BlockmanPart.JowanL  ),
            new BlockmanTextureInfo(BlockmanPart.JowanR  ),
            new BlockmanTextureInfo(BlockmanPart.ZenwanL ),
            new BlockmanTextureInfo(BlockmanPart.ZenwanR ),
            new BlockmanTextureInfo(BlockmanPart.HandL   ),
            new BlockmanTextureInfo(BlockmanPart.HandR   ),
            new BlockmanTextureInfo(BlockmanPart.Leg1L   ),
            new BlockmanTextureInfo(BlockmanPart.Leg1R   ),
            new BlockmanTextureInfo(BlockmanPart.Leg2L   ),
            new BlockmanTextureInfo(BlockmanPart.Leg2R   ),
            new BlockmanTextureInfo(BlockmanPart.FootL   ),
            new BlockmanTextureInfo(BlockmanPart.FootR   )
        };

        public IEnumerable<BlockmanTextureInfo> BlockmanTextureList => blockmanTextureList;

        [Serializable]
        public class BlockmanTextureInfo
        {
            [FormerlySerializedAs("_part")] [SerializeField]
            private BlockmanPart part;
            public BlockmanPart Part => part;

            [FormerlySerializedAs("_goodTexture")] [SerializeField]
            private Texture goodTexture;
            public Texture GoodTexture => goodTexture;

            [FormerlySerializedAs("_badTexture")] [SerializeField]
            private Texture badTexture;
            public Texture BadTexture => badTexture;

            public BlockmanTextureInfo(BlockmanPart part)
            {
                this.part = part;
            }
        }
    }
}
