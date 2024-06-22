
using System;

namespace ExileWorldGenerator
    {
    internal class BackgroundOverride
        {
        internal BackgroundOverride(byte listId, byte content)
            {
            this.ListId = listId;
            this.OverrideValue = content;
            }

        /// <summary>
        /// Returns which of the 9 lookup lists was used to determine the background
        /// </summary>
        public readonly byte ListId;

        /// <summary>
        /// The revised background
        /// </summary>
        /// <remarks>Consists of 2 bits describing the orientation of the item, and 6 bits that describe the item type</remarks>
        public readonly byte OverrideValue;

        /// <summary>
        /// Returns the background type, and when in the range 0 to f also which background handler applies
        /// </summary>
        /// <remarks>Range is 0 to 3f corresponding to an entry in background_sprite_lookup</remarks>
        public byte Background => (byte) (this.OverrideValue & 0x3f);

        /// <summary>
        /// Returns the orientation of the background, whether it is shown left-to-right or right-to-left, top-to-bottom or bottom-to-top
        /// </summary>
        /// <remarks>One bit will describe the horizontal orientation, and the other describes the vertical orientation</remarks>
        public byte Orientation => (byte) (this.OverrideValue & 0xc0);

        /// <summary>
        /// When set, this indicates that the default scenery for the list has been used
        /// </summary>
        public bool IsHashDefault => this.GetType() == typeof(BackgroundOverride);
        }

    internal class BackgroundObject : BackgroundOverride
        {
        internal BackgroundObject(byte listId, byte index, byte content) : base(listId, content)
            {
            if (index > 0xfd) throw new ArgumentOutOfRangeException(nameof(index));
            this.Index = index;
            }

        /// <summary>
        /// The index of the background object, based on its location in background_objects_x_lookup
        /// </summary>
        /// <remarks>Range is 0 to fd.</remarks>
        public readonly int Index;
        }

    internal class BackgroundEvent : BackgroundObject
        {
        internal BackgroundEvent(byte listId, byte index, byte content, byte dataIndex, byte data, byte? objectType) : base(listId, index, content)
            {
            this.DataIndex = dataIndex;
            this.ObjectData = data;
            this.ObjectType = objectType;
            }

        /// <summary>
        /// The id of the background object as determined by the index of its data byte
        /// </summary>
        /// <remarks>This value is used (for example) by switches to change another object's state. Range is 1 to ea</remarks>
        public readonly byte DataIndex;

        /// <summary>
        /// Object specific data for the background object
        /// </summary>
        /// <remarks>If the object needs to maintain some information about its state, this is where it is stored. Each object type will store different information and in different ways.</remarks>
        public byte ObjectData;

        /// <summary>
        /// Object type for the background object
        /// </summary>
        /// <remarks>If the background is an object_emerging_from or object_from_type then this identifies what type of object is created,
        /// for an invisible_switch this identifies the type of object that triggers it</remarks>
        public readonly byte? ObjectType;
        }
    }
