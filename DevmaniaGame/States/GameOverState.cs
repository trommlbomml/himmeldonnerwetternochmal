using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevmaniaGame.States
{
    class GameOverState : InitializableState
    {
        private Texture2D _texture;
        private int _reachedPoints;
        private SpriteFont _spriteFont;

        protected override void OnEntered(object enterInformation)
        {
            _reachedPoints = (int) enterInformation;
        }

        protected override void OnInitialize(object enterInformation)
        {
            _texture = Game.Content.Load<Texture2D>("textures/slides/gameover");
            _spriteFont = Game.Content.Load<SpriteFont>("fonts/bigfont");
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            if (Game.Keyboard.IsKeyDownOnce(Keys.Enter) || Game.Keyboard.IsKeyDownOnce(Keys.Escape))
                return StateChangeInformation.QuitGameInformation(typeof (ThrowAwayTransition));

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            Game.SpriteBatch.Draw(_texture, Vector2.Zero, Color.White);
            Game.SpriteBatch.DrawString(_spriteFont, string.Format("Reached Points: {0}",_reachedPoints), 
                new Vector2(100,350), Color.Black);
        }
    }
}
