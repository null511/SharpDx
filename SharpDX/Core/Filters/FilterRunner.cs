using SharpDX.Core.Entities;
using SharpDX.Core.Geometry;
using SharpDX.Core.SceneTree;
using SharpDX.Core.Shaders;
using SharpDX.Test;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace SharpDX.Core.Filters
{
    class FilterRunner
    {
        private Tree _tree;
        private int _threadCount, _cubeCount;
        private IFilter _filter;
        private Thread[] _threads;
        private ConcurrentQueue<RegionData> _queue;
        private ConcurrentBag<RenderEntity> _cubes;
        private BoundingBox _cubeBounds;
        private int[] _entityCount;

        public IShader Shader;
        public Mesh Mesh;

        public int EntityCount => _entityCount.Sum();


        public FilterRunner(Tree tree, IFilter filter, int threadCount) {
            this._tree = tree;
            this._filter = filter;
            this._threadCount = threadCount;

            _queue = new ConcurrentQueue<RegionData>();
            _cubes = new ConcurrentBag<RenderEntity>();

            _entityCount = new int[threadCount];
            _cubeCount = DepthUtils.GetFaceCount(tree.Description.MaxLevels);

            _threads = new Thread[_threadCount];
            for (int i = 0; i < _threadCount; i++) {
                _threads[i] = new Thread(new ParameterizedThreadStart(ThreadProcess));
            }
        }

        public void Start() {
            _cubeBounds.Maximum = _tree.Description.CubeSize;

            var desc = _tree.Description;
            for (int z = 0; z < desc.RegionCountZ; z++) {
                for (int y = 0; y < desc.RegionCountY; y++) {
                    for (int x = 0; x < desc.RegionCountX; x++) {
                        var region = _tree.GetRegion(x, y, z);
                        _queue.Enqueue(new RegionData {
                            Data = region,
                            X = x,
                            Y = y,
                            Z = z,
                        });
                    }
                }
            }

            for (int i = 0; i < _threadCount; i++) {
                _threads[i].Start(i);
            }
        }

        public bool TryComplete() {
            if (!_queue.IsEmpty) return false;

            for (int i = 0; i < _threadCount; i++) {
                _threads[i].Join();
            }

            RenderEntity x;
            while (!_cubes.IsEmpty) {
                if (_cubes.TryTake(out x)) {
                    _tree.Insert(x);
                }
            }

            return true;
        }

        private void ThreadProcess(object args) {
            var workerIndex = (int)args;

            RegionData region;
            while (!_queue.IsEmpty) {
                if (_queue.TryDequeue(out region)) {
                    BuildTestRegion(region, workerIndex);
                }
            }
        }

        private void BuildTestRegion(RegionData region, int workerIndex) {
            var e = new TreeBuilderTestEventArgs();
            e.TreeDescription = _tree.Description;

            var cubeSize = _tree.Description.CubeSize;
            for (int z = 0; z < _cubeCount; z++) {
                for (int y = 0; y < _cubeCount; y++) {
                    for (int x = 0; x < _cubeCount; x++) {
                        var min = region.Data.Bounds.Minimum;
                        e.Position.X = min.X + x * cubeSize.X;
                        e.Position.Y = min.Y + y * cubeSize.Y;
                        e.Position.Z = min.Z + z * cubeSize.Z;

                        e.RegionX = region.X;
                        e.RegionY = region.Y;
                        e.RegionZ = region.Z;
                        e.X = x;
                        e.Y = y;
                        e.Z = z;

                        if (_filter.Test(e)) {
                            _entityCount[workerIndex]++;

                            var cube = new TestCube {
                                Position = e.Position,
                                Bounds = _cubeBounds,
                                Color = e.Color,
                                Shader = Shader,
                                Mesh = Mesh,
                            };

                            cube.Update();
                            _cubes.Add(cube);
                        }
                    }
                }
            }
        }
    }

    class RegionData
    {
        public Region Data;
        public int X, Y, Z;
    }
}
