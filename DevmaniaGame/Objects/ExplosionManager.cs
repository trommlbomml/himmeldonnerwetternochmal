using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game2DFramework;
using Game2DFramework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Objects
{
    class ExplosionInstance
    {
        public Vector2 Position;
        private readonly AnimatedSprite _animatedSprite;

        public ExplosionInstance(Texture2D texture, Vector2 position)
        {
            Position = position;
            _animatedSprite = new AnimatedSprite(texture, 64, 64, 0, 0, 16, 0.8f, false, SpriteEffects.None);
            _animatedSprite.Play();
        }

        public void Update(float elapsedTime)
        {
            _animatedSprite.Update(elapsedTime);
        }

        public bool Finished {get { return !_animatedSprite.Running; }}

        public void Draw(SpriteBatch spriteBatch)
        {
            _animatedSprite.Draw(spriteBatch, Position);
        }
    }

    class ExplosionManager : GameObject
    {
        private SoundEffect _soundEffect;
        private SoundEffectInstance _instanceEffect;
        private readonly Texture2D _texture;
        private readonly List<ExplosionInstance> _explosionInstances;

        public ExplosionManager(Game2D game) : base(game)
        {
            _texture = Game.Content.Load<Texture2D>("textures/explosion");
            _explosionInstances = new List<ExplosionInstance>();
            _soundEffect = Game.Content.Load<SoundEffect>("sounds/explosion");
            _instanceEffect = _soundEffect.CreateInstance();
            _instanceEffect.IsLooped = false;
        }

        public void SpawnExplosion(Vector2 position)
        {
            _explosionInstances.Add(new ExplosionInstance(_texture, position));
            _instanceEffect.Play();
        }

        public void Update(float elapsedTime)
        {
            _explosionInstances.ForEach(e => e.Update(elapsedTime));
            _explosionInstances.RemoveAll(e => e.Finished);
        }

        public void Draw()
        {
            _explosionInstances.ForEach(e => e.Draw(Game.SpriteBatch));
        }
    }
}
