
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevmaniaGame.States
{
    class OverviewState : InitializableState
    {
        private Texture2D _texture;

        protected override void OnEntered(object enterInformation)
        {
        }

        protected override void OnInitialize(object enterInformation)
        {
            _texture = Game.Content.Load<Texture2D>("textures/Slides/overview");
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            if (Game.Keyboard.IsKeyDownOnce(Keys.Enter))
                return StateChangeInformation.StateChange(typeof (MainGame), typeof (FlipTransition));

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            Game.SpriteBatch.Draw(_texture, Vector2.Zero, Color.White);
        }
    }
}
