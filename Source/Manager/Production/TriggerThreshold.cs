﻿using UnityEngine;
using Verse;

namespace FM
{
    public class TriggerThreshold : Trigger
    {
        public TriggerThreshold(ManagerJobProduction job)
        {
            Op = Ops.LowerThan;
            MaxUpperThreshold = job.MainProduct.MaxUpperThreshold;
            Count = MaxUpperThreshold / 5;
            ThresholdFilter = new ThingFilter();
            ThresholdFilter.SetDisallowAll();
            if (job.MainProduct.ThingDef != null) ThresholdFilter.SetAllow(job.MainProduct.ThingDef, true);
            if (job.MainProduct.CategoryDef != null) ThresholdFilter.SetAllow(job.MainProduct.CategoryDef, true);
        }

        public int MaxUpperThreshold;

        public bool IsValid
        {
            get { return ThresholdFilter.AllowedDefCount > 0; }
        }

        public int CurCount
        {
            get
            {
                return Utilities.CountProducts(ThresholdFilter);
            }
        }

        public ThingFilter ThresholdFilter;

        public enum Ops
        {
            LowerThan,
            Equals,
            HigherThan
        }

        public Ops Op;

        public virtual string OpString
        {
            get
            {
                switch (Op)
                {
                    case Ops.LowerThan:
                        return " < ";
                    case Ops.Equals:
                        return " = ";
                    case Ops.HigherThan:
                        return " > ";
                    default:
                        return " ? ";
                }
            }
        }

        public override bool State
        {
            get
            {
                switch (Op)
                {
                    case Ops.LowerThan:
                        return CurCount < Count;
                    case Ops.Equals:
                        return CurCount == Count;
                    case Ops.HigherThan:
                        return CurCount > Count;
                    default:
                        Log.Warning("Trigger_ThingThreshold was defined without a correct operator");
                        return true;
                }
            }
        }


        public override string ToString()
        {
            return "Trigger_Threshold.ToString() not implemented";
        }

        public int Count;

        public override void ExposeData()
        {
            Scribe_Values.LookValue(ref Count, "Count");
            Scribe_Values.LookValue(ref MaxUpperThreshold, "MaxUpperThreshold");
            Scribe_Values.LookValue(ref Op, "Operator");
            Scribe_Deep.LookDeep(ref ThresholdFilter, "ThresholdFilter");
        }

        public override void DrawThresholdConfig(ref Listing_Standard listing)
        {
            // target threshold
            listing.DoGap();
            listing.DoLabel("FMP.Threshold".Translate() + ":");
            listing.DoLabel("FMP.ThresholdCount".Translate(CurCount, Count));
            // TODO: implement trade screen sliders - they're so pretty! :D
            Count = Mathf.RoundToInt(listing.DoSlider(Count, 0, MaxUpperThreshold));
            listing.DoGap(6f);
            if (listing.DoTextButton("FMP.ThresholdDetails".Translate()))
            {
                Find.WindowStack.Add(DetailsWindow);
            }
        }

        public WindowTriggerThresholdDetails DetailsWindow
        {
            get
            {
                WindowTriggerThresholdDetails window = new WindowTriggerThresholdDetails
                {
                    Trigger = this,
                    closeOnClickedOutside = true,
                    draggable = true
                };
                return window;
            }
        }
    }
}