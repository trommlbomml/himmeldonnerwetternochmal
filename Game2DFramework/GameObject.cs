
namespace Game2DFramework
{
    public abstract class GameObject
    {
        protected GameObject(Game2D game)
        {
            Game = game;
        }

        public Game2D Game { get; private set; }

        public virtual void OnCollide(GameObject other)
        {
            
        }

        public virtual void ApplyDamage(int amount)
        {
            
        }
    }
}
