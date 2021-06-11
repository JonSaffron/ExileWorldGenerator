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
        /// When set, it indicates that the background is one of the 1024 squares of explicitly defined content.
        /// Otherwise, the result has been procedurally generated.
        /// </summary>
        public bool IsMappedData;

        /// <summary>
        /// When IsMappedData is set, this indicates which of the 1024 mapped data points is used to determine the content
        /// </summary>
        private int? _positionInMappedData;

        public int PositionInMappedData
            {
            get
                {
                if (!this.IsMappedData)
                    throw new InvalidOperationException("Not mapped data");
                Debug.Assert(_positionInMappedData != null, nameof(_positionInMappedData) + " != null");
                return _positionInMappedData.Value;
                }

            set => _positionInMappedData = value;
            }
        }
    }
