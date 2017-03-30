using Game2DFramework.Cameras.Behaviors;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;

namespace Game2DFramework.Cameras
{
    public class Camera : GameObject
    {
        private ICameraBehavior _cameraBehavior;

        public Vector2 Position { get { return _cameraBehavior.Position; } }
        public Vector2 Offset { get { return _cameraBehavior.Position - _halfSize; } }

        private readonly Vector2 _halfSize;

        public Camera(Game2D game) : base(game)
        {
            _cameraBehavior = new StaticCameraBehavior(game);
            _halfSize = new Vector2(Game.ScreenWidth, Game.ScreenHeight) * 0.5f;
        }

        public void SetFollowWithBoundaryBehavior(Rectangle boundary, IFollowingTarget target)
        {
            _cameraBehavior = new FollowBoundaryCameraBehavior(Game, boundary, target);
        }

        public virtual void Update(float elapsed)
        {
            _cameraBehavior.Update(elapsed);
        }

        public Matrix WorldMatrix 
        {
            get { return Matrix.CreateTranslation(new Vector3((_halfSize - Position).SnapToPixels(), 0)); }
        }
    }
}
