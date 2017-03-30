using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing.Particles
{
    public class ParticleSystem
    {
        private readonly List<Particle> _particles;
        private readonly IParticleEmitter _emitter;
        private readonly IParticleRenderer _renderer;

        public ParticleSystem(IParticleEmitter emitter, IParticleRenderer renderer)
        {
            _particles = new List<Particle>();
            _emitter = emitter;
            _renderer = renderer;
        }

        public bool HasActiveParticles
        {
            get { return _particles.Count > 0; }
        }

        public bool IsEnabled 
        {
            get { return _emitter.IsEnabled; }
            set { _emitter.IsEnabled = value; }
        }

        public void Update(float elapsedTime)
        {
            var particle = _emitter.Update(elapsedTime);
            if (particle != null)
                _particles.Insert(0, particle);

            _particles.RemoveAll(p => p.Dead);
            _particles.ForEach(p => _emitter.UpdateParticle(elapsedTime, p));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _particles.ForEach(p => _renderer.Draw(spriteBatch, p));
        }
    }
}
