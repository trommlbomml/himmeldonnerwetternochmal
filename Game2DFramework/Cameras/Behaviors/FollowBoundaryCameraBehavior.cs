using Microsoft.Xna.Framework;

namespace Game2DFramework.Cameras.Behaviors
{
    public class FollowBoundaryCameraBehavior : GameObject, ICameraBehavior
    {
        public Rectangle Boundary { get; private set; }
        public IFollowingTarget Target { get; private set; }

        public FollowBoundaryCameraBehavior(Game2D game, Rectangle boundary, IFollowingTarget target) : base(game)
        {
            Boundary = boundary;
            Target = target;
        }

        public Vector2 Position { get; private set; }

        public void Update(float elapsed)
        {
            var newPosition = new Vector2(Boundary.Left + Boundary.Width/2, Boundary.Top + Boundary.Height/2);

            if (Boundary.Width > Game.ScreenWidth)
            {
                var minX = Boundary.Left + Game.ScreenWidth / 2;
                var maxX = Boundary.Right - Game.ScreenWidth / 2;
                newPosition.X = MathHelper.Clamp(Target.FollowingPosition.X, minX, maxX);
            }

            if (Boundary.Height > Game.ScreenHeight)
            {
                var minY = Boundary.Top + Game.ScreenHeight / 2;
                var maxY = Boundary.Bottom - Game.ScreenHeight / 2;
                newPosition.Y = MathHelper.Clamp(Target.FollowingPosition.Y, minY, maxY);
            }

            Position = newPosition;
        }
    }
}
