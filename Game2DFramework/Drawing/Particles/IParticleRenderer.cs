
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing.Particles
{
    public interface IParticleRenderer
    {
        void Draw(SpriteBatch spriteBatch, Particle particle);
    }
}
