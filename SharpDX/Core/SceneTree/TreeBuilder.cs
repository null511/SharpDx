using SharpDX.Core.Filters;
using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using System.Threading;

namespace SharpDX.Core.SceneTree
{
    class TreeBuilder
    {
        private TreeDescription _description;
        private int _entityCount;

        public IShader Shader;
        public Mesh Mesh;

        public int EntityCount => _entityCount;


        public TreeBuilder() {
            _description = new TreeDescription();
        }

        public TreeBuilder(TreeDescription description) {
            _description = description;
        }

        public Tree Build() {
            CalculateBounds();
            var tree = new Tree(_description);
            tree.Create();
            return tree;
        }

        public Tree Build(IFilter filter, int threadCount) {
            CalculateBounds();
            var tree = new Tree(_description);
            tree.Create();

            var runner = new FilterRunner(tree, filter, threadCount);
            runner.Shader = Shader;
            runner.Mesh = Mesh;
            runner.Start();

            while (!runner.TryComplete()) {
                Thread.Sleep(10);
            }

            _entityCount = runner.EntityCount;
            return tree;
        }

        private void CalculateBounds() {
            Vector3 sceneSize;
            _description.CalculateSize(out sceneSize);
            _description.Bounds.Minimum = -sceneSize * 0.5f;
            _description.Bounds.Maximum = sceneSize * 0.5f;
        }
    }
}
