using SharpDX.Core;
using SharpDX.Core.Entities;
using SharpDX.Core.Filters;
using SharpDX.Core.SceneTree;
using SharpDX.Direct3D11;
using SharpDX.Test;
using SharpDX.Verticies;
using System;
using System.Collections.Generic;

namespace SharpDX.Scenes
{
    class SceneGraph : IDisposable
    {
        private const int BuilderThreadCount = 8;
        private static readonly bool _disableTree = false;
        private static readonly bool _disableInstancing = false;

        private readonly EntityCollection entities;
        private readonly EntityCollection visible;
        private readonly List<EntityCollection> visibleInstanced;
        private readonly TestOptions _options;
        private SceneTreeCubeShader _debugShader;
        private GeoCubeShader _shader;
        private GeoCubeShaderInstanced _shaderInstanced;
        private TestCubeMesh _cubeMesh;
        private DebugCubeMesh _debugCubeMesh;
        private TreeBuilder _treeBuilder;

        public Tree Tree;
        public int EntityCount, RenderCount;
        private Vector3 sunDir;


        public SceneGraph(TreeDescription description) {
            entities = new EntityCollection();
            visible = new EntityCollection();
            visibleInstanced = new List<EntityCollection>();

            _treeBuilder = new TreeBuilder(description);

            _options = new TestOptions {
                EnableDebugCubeRendering = false,
                DebugCubeRenderLevel = 1,
                debugCubeList = new List<RenderCube>(),
            };

            sunDir = new Vector3(0.2f, 1f, 0.6f);
            sunDir.Normalize();
        }

        public void Dispose() {
            Utilities.Dispose(ref _cubeMesh);
            Utilities.Dispose(ref _debugCubeMesh);
            Utilities.Dispose(ref _shader);
            Utilities.Dispose(ref _debugShader);
        }

        public void Create(DeviceContext context, View view, Vector3 cubeSize, IFilter[] filters) {
            // Cube Mesh
            _cubeMesh = new TestCubeMesh(context, cubeSize);

            // Debug-Cube Mesh
            _debugCubeMesh = new DebugCubeMesh(context);

            // Cube Shader
            if (_disableInstancing) {
                _shader = new GeoCubeShader();
                _shader.Load(context, VertexTestCube.Info);
                _shader.SetSunDir(ref sunDir);

                _shader.ActionRegistry.Add<TestCube>(e => {
                    _shader.SetWVP(ref e.World, view);
                    _shader.SetColor(ref e.Color);
                });
            } else {
                _shaderInstanced = new GeoCubeShaderInstanced();
                _shaderInstanced.Load(context, VertexTestCube.InstanceInfo);
                _shaderInstanced.SetSunDir(ref sunDir);
            }

            // Debug-Cube Shader
            _debugShader = new SceneTreeCubeShader();
            _debugShader.Load(context);

            _debugShader.ActionRegistry.Add<RenderCube>(e => {
                _debugShader.SetWVP(ref e.World, view);
                _debugShader.SetColor(ref e.Color);
            });

            // Build Tree
            if (_disableInstancing) _treeBuilder.Shader = _shader;
            else _treeBuilder.Shader = _shaderInstanced;
            _treeBuilder.Mesh = _cubeMesh;

            Tree = _treeBuilder.Build(filters, BuilderThreadCount);
            EntityCount = _treeBuilder.EntityCount;
        }

        public void Update() {
            int count = entities.AllEntities.Count;
            for (int i = 0; i < count; i++) {
                entities.AllEntities[i].Update();
            }
        }

        public void Render(DeviceContext context, View view) {
            if (_disableTree) {
                RenderCount = entities.Render(context);
                return;
            }

            if (_options.EnableDebugCubeRendering)
                _options.debugCubeList.Clear();

            _options.Frustum = view.Frustum;
            
            if (_disableInstancing) {
                Tree.Test(visible, _options);
                RenderCount = visible.Render(context);
            } else {
                Tree.TestByRegion(visibleInstanced, _options);

                var instances = new InstanceCollection();

                int count = visibleInstanced.Count;
                for (int i = 0; i < count; i++) {
                    visibleInstanced[i].CreateInstances(context, instances);
                }

                _shaderInstanced.SetVP(view);
                _shaderInstanced.Update(context);

                RenderCount = instances.Render(context);
            }

            if (_options.EnableDebugCubeRendering)
                RenderDebugCubes(context);
        }

        private void RenderDebugCubes(DeviceContext context) {
            _debugShader.Apply(context);
            _debugCubeMesh.Apply(context);

            int count = _options.debugCubeList.Count;
            for (int i = 0; i < count; i++) {
                var cube = _options.debugCubeList[i];

                cube.Update();

                _debugShader.ActionRegistry.Apply(cube);
                _debugShader.Update(context);

                _debugCubeMesh.Render(context);
            }
        }

        public void Increase() {
            if (!_options.EnableDebugCubeRendering) {
                _options.EnableDebugCubeRendering = true;
                _options.DebugCubeRenderLevel = 1;
            }
            else if (_options.DebugCubeRenderLevel < Tree.Description.MaxLevels) {
                _options.DebugCubeRenderLevel++;
            }
        }

        public void Decrease() {
            if (_options.EnableDebugCubeRendering) {
                _options.DebugCubeRenderLevel--;
                if (_options.DebugCubeRenderLevel == 0)
                    _options.EnableDebugCubeRendering = false;
            }
        }
    }
}
