using UnityEngine;
using Assets.Scripts.Common;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ThreeDView
{
    public enum HumanLayer : byte
    {
        Fat,
        Bone,
        Muscle
    }

    public enum FatPart : byte
    {
        Trunk,
        Breath,
        ArmL,
        ArmR,
        LegL,
        LegR
    }

    public enum BonePart : byte
    {
        head_b,
        nose_nan,
        ear_nan,
        neck_b,
        neck_nan,
        sakotsu_l_b,
        sakotsu_r_b,
        kenko_l_b,
        kenko_r_b,
        rokkotsu_b,
        rokkotsu_nan,
        jowan_l_b,
        jowan_l_nan1,
        jowan_l_nan2,
        jowan_r_b,
        jowan_r_nan1,
        jowan_r_nan2,
        zenwan_l_b,
        zenwan_l_nan1,
        zenwan_l_nan2,
        zenwan_r_b,
        zenwan_r_nan1,
        zenwan_r_nan2,
        hand_l_b,
        hand_l_nan,
        hand_r_b,
        hand_r_nan,
        kyotsui_b,
        kyotsui_nan,
        yotsui_b,
        yotsui_nan,
        senkotsu_b,
        kankotsu_b,
        daitai_l_b,
        daitai_l_nan1,
        daitai_l_nan2,
        daitai_r_b,
        daitai_r_nan1,
        daitai_r_nan2,
        sune_l_b,
        sune_l_nan,
        sune_r_b,
        sune_r_nan,
        foot_l_b,
        foot_l_nan,
        foot_r_b,
        foot_r_nan
    }

    public static class BonePartExtension
    {
        private static readonly BonePart[] _cartilageList = new BonePart[]
        {
            BonePart.nose_nan,
            BonePart.ear_nan,
            BonePart.neck_nan,
            BonePart.rokkotsu_nan,
            BonePart.jowan_l_nan1,
            BonePart.jowan_l_nan2,
            BonePart.jowan_r_nan1,
            BonePart.jowan_r_nan2,
            BonePart.zenwan_l_nan1,
            BonePart.zenwan_l_nan2,
            BonePart.zenwan_r_nan1,
            BonePart.zenwan_r_nan2,
            BonePart.hand_l_nan,
            BonePart.hand_r_nan,
            BonePart.kyotsui_nan,
            BonePart.yotsui_nan,
            BonePart.daitai_l_nan1,
            BonePart.daitai_l_nan2,
            BonePart.daitai_r_nan1,
            BonePart.daitai_r_nan2,
            BonePart.sune_l_nan,
            BonePart.sune_r_nan,
            BonePart.foot_l_nan,
            BonePart.foot_r_nan,
        };

        public static bool isCartilage(this BonePart part)
        {
            return _cartilageList.Contains(part);
        }
    }

    public abstract class BaseHuman : MonoBehaviour
    {
        public abstract Gender Gender {
            get;
        }

        public abstract BodyType BodyType {
            get;
        }

        public abstract void SetActive(HumanLayer layer, bool active);

        public abstract void SetBlendShape(string name, int value);

        public abstract void SetMaterialColor(HumanLayer layer, string name, Color albedoColor, Color specularColor);

        public abstract void MulFatAlpha(FatPart part, float ratio);

        public abstract AvatarController AvatarController { get; }

        public abstract HumanInitParam HumanInitParam { get; }

        public abstract void SetHumanInitParam(HumanInitParam param);
    }
}