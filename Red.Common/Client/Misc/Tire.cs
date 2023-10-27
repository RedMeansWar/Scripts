using CitizenFX.Core;

namespace Red.Common.Client.Misc
{
    public class Tire
    {
        public float Distance { get; set; }
        public Vector3 BonePosition { get; set; }
        public int TireIndex { get; set; }
    }

    public enum TireIndex
    {
        FrontLeft = 0,
        FrontRight = 1,
        LeftMiddle1 = 2,
        RightMiddle1 = 3,
        LeftRear = 4,
        RightRear = 5,
        LeftMiddle2 = 45,
        RightMiddle2 = 47,
        LeftMiddle3 = 46,
        RightMiddle3 = 48,
    }

    public class WheelIndex
    {
        public static string FrontLeft = "wheel_lf";
        public static string FrontRight = "wheel_rf";
        public static string LeftMiddle1 = "wheel_lm1";
        public static string RightMiddle1 = "wheel_rm1";
        public static string LeftRear = "wheel_lm2";
        public static string RightRear = "wheel_rm2";
        public static string LeftMiddle2 = "wheel_lm3";
        public static string RightMiddle2 = "wheel_rm3";
        public static string LeftMiddle3 = "wheel_lr";
        public static string RightMiddle3 = "wheel_rr";
    }
}
