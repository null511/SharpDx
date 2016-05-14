using System.IO;

namespace SharpDX.Core.Documents
{
    abstract class BaseDocument
    {
        public void Load(string filename) {
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream)) {
                OnLoad(reader);
            }
        }

        public void Save(string filename) {
            using (var stream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream)) {
                OnSave(writer);
            }
        }

        protected abstract void OnLoad(BinaryReader reader);

        protected abstract void OnSave(BinaryWriter writer);
    }
}
