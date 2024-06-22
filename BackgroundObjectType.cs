using System.ComponentModel;

namespace ExileWorldGenerator
    {
    public enum BackgroundObjectType : byte
        {
        [Description("Invisible switch")]
        InvisibleSwitch = 0,

        [Description("Teleport")]
        Teleport = 1,

        [Description("Object from data")]
        ObjectFromData = 2,

        [Description("Door")]
        Door = 3,

        [Description("Stone door")]
        StoneDoor = 4,

        [Description("Object from type with wall")]
        ObjectFromTypeWithWall = 5,

        [Description("Object from type")]
        ObjectFromType = 6,

        [Description("Object from type with foliage")]
        ObjectFromTypeWithFoliage = 7,

        [Description("Switch")]
        Switch = 8,

        [Description("Object emerging from bush")]
        ObjectEmergingFromBush = 9,

        [Description("Object emerging from pipe")]
        ObjectEmergingFromPipe = 0xa,

        [Description("Fixed wind")]
        FixedWind = 0xb,

        [Description("Engine thruster")]
        EngineThruster = 0xc,

        [Description("Water")]
        Water = 0xd,

        [Description("Random wind")]
        RandomWind = 0xe,

        [Description("Mushrooms")]
        Mushrooms = 0xf
        }
    }
