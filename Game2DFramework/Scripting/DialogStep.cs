using Game2DFramework.Drawing;
using Microsoft.Xna.Framework.Input;

namespace Game2DFramework.Scripting
{
    public class DialogStep : IScriptedStep
    {
        private readonly Dialog _dialog;
        private readonly string _message;

        public DialogStep(Dialog dialog, string message)
        {
            _message = message;
            _dialog = dialog;
        }

        public void Start()
        {
            _dialog.SetText(_message);
            _dialog.Show();
        }

        public void Update(float elapsedTime, Game2D game)
        {
            _dialog.Update(elapsedTime);
            if (game.Keyboard.IsKeyDownOnce(Keys.Space) && _dialog.State == DialogState.Shown)
            {
                _dialog.Hide();
            }
        }

        public bool LocksControls { get { return true; } }
        public bool Finished { get { return _dialog.State == DialogState.Hidden; } }
    }
}
