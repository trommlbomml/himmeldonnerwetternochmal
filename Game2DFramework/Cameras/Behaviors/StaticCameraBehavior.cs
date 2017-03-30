using Microsoft.Xna.Framework;

namespace Game2DFramework.Cameras.Behaviors
{
    public class StaticCameraBehavior : GameObject, ICameraBehavior
    {
        public StaticCameraBehavior(Game2D game) : base(game)
        {
            
        }

        public Vector2 Position { get { return new Vector2(Game.ScreenWidth, Game.ScreenHeight) * 0.5f; } }

        public void Update(float elapsed)
        {
        }
    }
}
