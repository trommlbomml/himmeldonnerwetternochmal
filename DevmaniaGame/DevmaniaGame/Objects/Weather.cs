using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game2DFramework.Extensions;
namespace DevmaniaGame.Objects
{
    enum WeatherState
    {
        CLOUD,
        SUN
    }

    class Weather : GameObject
    {
        public Cloud _cloud;
        public Sun _sun;
        public WeatherState _state;
        public SpriteFont _font;
        private Texture2D _suntex;
        private Texture2D _cloudtex;

        public Weather(Game2D game)
            : base(game)
        {
            _cloud = new Cloud(game, new Vector2(400f, 100f));
            _sun = new Sun(game, new Vector2(700f, 100f));
            _sun._cloud = _cloud;
            _state = WeatherState.CLOUD;
            _suntex = game.Content.Load<Texture2D>("Textures\\sun_icon");
            _cloudtex = game.Content.Load<Texture2D>("Textures\\rain_icon");
            _font = game.Content.Load<SpriteFont>("Fonts\\Font");
        }

        public void Update(float fElapsed)
        {
            WeatherState _oldState = _state;

            if (Game.Mouse.IsRightButtonReleased())
                _state = _state == WeatherState.CLOUD ? WeatherState.SUN : WeatherState.CLOUD;

            _cloud.loadUp(fElapsed);
            _sun.loadUp(fElapsed);

            if (_oldState != _state)
            {
                if (_state == WeatherState.CLOUD)
                {
                    _sun.Disable();
                    _cloud.Enable();
                }
                else
                {
                    _cloud.Disable();
                    _sun.Enable();
                }
            }

            if (_state == WeatherState.CLOUD || !_cloud.isFinishedWithWork)
                _cloud.Update(fElapsed);
            if ((_state == WeatherState.SUN || !_sun.isFinishedWithWork))
                _sun.Update(fElapsed);
        }

        public void Draw(float fElapsed)
        {
            _sun.Draw(fElapsed);
            _cloud.Draw(fElapsed);

            // Draw selection
            //Game.ShapeRenderer.DrawRectangle(276, 2, 251, 192, Color.Red);
            //Game.ShapeRenderer.DrawRectangle(277, 3, 251, 192, Color.Red);
            //Game.ShapeRenderer.DrawRectangle(278, 4, 251, 192, Color.Red);

            int iVal = (int)(_sun.energy * 100f);
            iVal = 100 - iVal;
            Game.ShapeRenderer.DrawFilledRectangle(758, 492 + iVal, 30, 100 - iVal, Color.OrangeRed);
            Game.ShapeRenderer.DrawRectangle(758, 492, 30, 100, Color.Black);

            iVal = (int)(_cloud.energy * 100f);
            iVal = 100 - iVal;
            Game.ShapeRenderer.DrawFilledRectangle(715, 492 + iVal, 30, 100 - iVal, Color.SteelBlue);
            Game.ShapeRenderer.DrawRectangle(715, 492, 30, 100, Color.Black);

            Game.SpriteBatch.Draw(_cloudtex, new Vector2(715, 450), Color.White);
            Game.SpriteBatch.Draw(_suntex, new Vector2(758, 450), Color.White);
        }
    }
}
