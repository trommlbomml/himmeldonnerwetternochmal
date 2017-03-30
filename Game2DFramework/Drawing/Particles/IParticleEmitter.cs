
namespace Game2DFramework.Drawing.Particles
{
    public interface IParticleEmitter
    {
        bool IsEnabled { get; set; }
        Particle Update(float elapsed);
        void UpdateParticle(float elapsed, Particle particle);
    }
}
