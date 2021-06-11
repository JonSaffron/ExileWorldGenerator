namespace ExileWorldGenerator
    {
    internal class BackgroundObjectData
        {
        /// <summary>
        /// The resulting background to use, and when in the range 0 to f also which background handler applies
        /// </summary>
        /// <remarks>Range is 0 to 3f corresponding to an entry in background_sprite_lookup</remarks>
        public byte Result;

        /// <summary>
        /// When set, this indicates that a default piece of scenery has been used
        /// </summary>
        /// <remarks>When set, Number will not have a value</remarks>
        public bool IsHashDefault;

        /// <summary>
        /// The unique number of the background object.
        /// </summary>
        /// <remarks>Range is 0 to fd. If set, then IsHashDefault will be false.</remarks>
        public int? Number;

        /// <summary>
        /// The id of the background object as determined by the index of its data byte
        /// </summary>
        /// <remarks>This value is used by switches to change the object's state. Range is 1 to ea</remarks>
        public int? Id;

        /// <summary>
        /// Object specific data for the background object
        /// </summary>
        public byte? Data;

        /// <summary>
        /// Object type for the background object
        /// </summary>
        public byte? Type;

        public bool IsBackgroundEvent
            {
            get 
                {
                var background = this.Result & 0x3f;
                if (this.Number.HasValue & background <= 0xc)
                    return true;
                return background <= 0xf;
                }
            }
        }
    }
