using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX.Core.Documents
{
    class MapDocument : BaseDocument
    {
        private RegionDocument[] RegionDocuments;

        public byte MaxLevels;
        public short RegionCountX;
        public short RegionCountY;
        public short RegionCountZ;
        public Vector3 CubeSize;


        public MapDocument() {}

        protected override void OnLoad(BinaryReader reader) {
            MaxLevels = reader.ReadByte();
            RegionCountX = reader.ReadInt16();
            RegionCountY = reader.ReadInt16();
            RegionCountZ = reader.ReadInt16();
            CubeSize.X = reader.ReadSingle();
            CubeSize.Y = reader.ReadSingle();
            CubeSize.Z = reader.ReadSingle();

            var regionSize = RegionCountX * RegionCountY * RegionCountZ;
            RegionDocuments = new RegionDocument[regionSize];

            for (var i = 0; i < regionSize; i++) {
                RegionDocuments[i] = new RegionDocument();
                //RegionDocuments[i].Load(...);
            }
        }

        protected override void OnSave(BinaryWriter writer) {
            var regionSize = RegionCountX * RegionCountY * RegionCountZ;
            RegionDocuments = new RegionDocument[regionSize];

            RegionDocument region;
            for (var i = 0; i < regionSize; i++) {
                region = RegionDocuments[i];

                //region.Save(...);
            }
        }
    }
}
