using System;
using System.Diagnostics;

namespace ExileWorldGenerator
    {
    class MapResult
        {
        public byte Result;

        public bool IsMappedData;
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
