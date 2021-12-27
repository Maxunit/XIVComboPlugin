namespace XIVComboExpandedPlugin.Combos
{
    internal static class DOL
    {
        public const byte ClassID = 0;
        public const byte JobID = 51;

        public const uint
            Cast = 289,
            ThaliaksFavor = 26804,
            AgelessWords = 215,
            SolidReason = 232,
            MinWiseToTheWorld = 26521,
            BtnWiseToTheWorld = 26522,
            PioneersGift1 = 21178,
            PioneersGift2 = 25590,
            MountaineersGift1 = 21177,
            MountaineersGift2 = 25589,
            PriceCatch = 26806;

        public static class Buffs
        {
            public const ushort
                EurekaMoment = 2765,
                GiftoftheLand = 2666,
                GiftoftheLand2 = 759,
                AnglersArt = 2778,
                PriceCatch = 2780;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                WiseToTheWorld = 90,
                PioneersGift1 = 15,
                MountaineersGift1 = 15,
                PioneersGift2 = 50,
                MountaineersGift2 = 50;
        }
    }

    internal class MinerEurekaFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.Disabled; // DolEurekaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DOL.SolidReason)
            {
                if (level >= DOL.Levels.WiseToTheWorld && HasEffect(DOL.Buffs.EurekaMoment))
                    return DOL.MinWiseToTheWorld;
            }

            if (actionID == DOL.AgelessWords)
            {
                if (level >= DOL.Levels.WiseToTheWorld && HasEffect(DOL.Buffs.EurekaMoment))
                    return DOL.BtnWiseToTheWorld;
            }

            return actionID;
        }
    }

    internal class DolGiftoftheLand : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DolGiftoftheLand;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DOL.PioneersGift1)
            {
                if (level >= DOL.Levels.PioneersGift2 && HasEffect(DOL.Buffs.GiftoftheLand))
                    return DOL.PioneersGift2;

                return DOL.PioneersGift1;
            }

            if (actionID == DOL.MountaineersGift1)
            {
                if (level >= DOL.Levels.PioneersGift2 && HasEffect(DOL.Buffs.GiftoftheLand))
                    return DOL.MountaineersGift2;

                return DOL.MountaineersGift1;
            }

            return actionID;
        }
    }

    internal class FisherPriceCatcher : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.Disabled;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            if (actionID == DOL.Cast)
            {
                // Need to work something out
            }

            return actionID;
        }
    }
}