
using Microsoft.Xna.Framework;

namespace Game2DFramework.Cameras
{
    public interface ICameraBehavior
    {
        Vector2 Position { get; }
        void Update(float elapsed);
    }
}
