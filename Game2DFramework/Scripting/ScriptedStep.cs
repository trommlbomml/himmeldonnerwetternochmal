
namespace Game2DFramework.Scripting
{
    public interface IScriptedStep
    {
        void Start();
        void Update(float elapsed, Game2D game);
        bool Finished { get; }
        bool LocksControls { get; }
    }
}
