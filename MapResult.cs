using System;

namespace ExileMappedBackground
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
                return _positionInMappedData.Value;
                }

            set => _positionInMappedData = value;
            }
        }
    }
