using System;

namespace Sweep.Core {

   [Serializable]
    public enum KeyNotation
    {
        CamelotWithoutTone,
        Is_M,
        IsMollDur,
        IsMinMaj,
        IsMinorMajor,
        Sharp_M,
        SharpMollDur,
        SharpMinMaj,
        SharpMajorMinor,
        IsStrip_M,
        IsStripMollDur,
        IsStripMinMaj,
        IsStripMinorMajor,
        Sharp_StripM,
        SharpStripMollDur,
        SharpStripMinMaj,
        SharpStripMajorMinor,
        Default = IsMollDur
    }
}
