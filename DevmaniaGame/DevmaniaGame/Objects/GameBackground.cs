using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Objects
{
    class GameBackground : GameObject
    {
        private float _animationSpeed;
        private readonly Texture2D[] _backgroundTextures;
        private float _currentTime;
        private int _currentFrame;
        private float _timePerFrame;

        public GameBackground(Game2D game) : base(game)
        {
            _backgroundTextures = new[]
                {
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg1"),
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg2"),
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg3"),
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg4"),
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg5"),
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg6"),
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg7"),
                    Game.Content.Load<Texture2D>("textures/backgrounds/bg8")
                };
        }

        public void SetAnimationTime(float animationSpeed)
        {
            _animationSpeed = animationSpeed;
            _timePerFrame = _animationSpeed/(float) _backgroundTextures.Length;
        }

        public void Update(float elapsedTime)
        {
            _currentTime += elapsedTime;
            if (_currentTime >= _timePerFrame)
            {
                _currentTime -= _timePerFrame;
                if (++_currentFrame == _backgroundTextures.Length)
                {
                    _currentFrame = 0;
                }
            }
        }

        public void Draw()
        {
            Game.SpriteBatch.Draw(_backgroundTextures[_currentFrame], Vector2.Zero, Color.White);
        }
    }
}
