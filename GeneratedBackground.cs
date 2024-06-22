using System;
using System.Diagnostics;

namespace ExileWorldGenerator
    {
    internal class GeneratedBackground
        {
        /// <summary>
        /// The background - could be scenery or a background object
        /// </summary>
        public byte Result;

        /// <summary>
        /// Returns the background type, and when in the range 0 to f also which background handler applies
        /// </summary>
        /// <remarks>Range is 0 to 3f corresponding to an entry in background_sprite_lookup</remarks>
        public byte Background => (byte) (this.Result & 0x3f);

        /// <summary>
        /// Returns the orientation of the background, whether it is shown left-to-right or right-to-left, top-to-bottom or bottom-to-top
        /// </summary>
        /// <remarks>One bit will describe the horizontal orientation, and the other describes the vertical orientation</remarks>
        public byte Orientation => (byte) (this.Result & 0xc0);

        /// <summary>
        /// When set, it indicates that the background is one of the 1024 squares of explicitly defined content.
        /// Otherwise, the result has been procedurally generated.
        /// </summary>
        public bool IsMappedData;

        private int? _positionInMappedData;

        /// <summary>
        /// When IsMappedData is set, this indicates which of the 1024 mapped data points is used to determine the content
        /// </summary>
        public int PositionInMappedData
            {
            get
                {
                if (!this.IsMappedData)
                    throw new InvalidOperationException("Not mapped data");
                Debug.Assert(_positionInMappedData != null, nameof(_positionInMappedData) + " != null");
                return _positionInMappedData!.Value;
                }

            set => _positionInMappedData = value;
            }
        }
    }
