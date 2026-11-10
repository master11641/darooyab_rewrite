using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class SlidSettingsTopIrc
    {
        public decimal SlideImageId { get; set; }
        public int SlidePageId { get; set; }
        public string SlideTitle { get; set; }
        public int? SlideWidth { get; set; }
        public int? SlideHeight { get; set; }
        public int? SlideAutoPlayInterval { get; set; }
        public int? SlideDragOrientation { get; set; }
        public int? SlideChanceToShow { get; set; }
        public int? SlideActionMode { get; set; }
        public int? SlideAutoCenter { get; set; }
        public int? SlideSteps { get; set; }
        public int? SlideLanes { get; set; }
        public int? SlideSpeed { get; set; }
        public int? SlideSpacingX { get; set; }
        public int? SlideSpacingY { get; set; }
        public int? SlideOrientation { get; set; }
        public bool? SlideIsActive { get; set; }
    }
}
