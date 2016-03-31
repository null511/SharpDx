using SharpDX.Direct3D11;

namespace SharpDX.Core.Geometry
{
    interface IVertexDescription
    {
        int Size {get;}
        InputElement[] Elements {get;}
    }
}
