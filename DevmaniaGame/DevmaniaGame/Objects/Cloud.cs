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
    class Cloud : GameObject
    {
        public Vector2 _position;
        Texture2D _cloudTexture;
        Texture2D _rainTexture;
        public bool isRaining { get; private set; }
        private bool disable;
        public bool isFinishedWithWork;
        private Vector2 rainPos;
        private Vector2 rainPos2;
        private float _sin;
        public float energy;
        private float _alpha;
        public bool IsFullyChanged;
        public float scale;
        public float gt;

        public Cloud(Game2D game, Vector2 initPos)
            : base(game)
        {
            isFinishedWithWork = true;
            disable = false;
            _cloudTexture = game.Content.Load<Texture2D>("Textures\\Cloud");
            _rainTexture = game.Content.Load<Texture2D>("Textures\\Rain");
            _position = initPos;
            rainPos = new Vector2(0.0f, 0.0f);
            rainPos2 = rainPos;
            _sin = 0;
            energy = 1;
            _alpha = 0.0f;
            IsFullyChanged = true;
            scale = 0.75f;
        }

        public void Update(float fElapsed)
        {
            if (IsFullyChanged)
                gt += fElapsed * 5;

            isRaining = false;

            if (!disable && Game.Mouse.IsLeftButtonDown())
                isRaining = true;

            if (energy <= 0f)
                isRaining = false;

            if (isRaining)
                _alpha += 1.0f * fElapsed;
            else
                _alpha -= 1.75f * fElapsed;

            if (_alpha <= 0.0f)
                _alpha = 0.0f;
            else if (_alpha >= 0.75f)
                _alpha = 0.75f;

            if (disable && _position.X >= 100f)
            {
                _sin += 3.9f * fElapsed;
                _position.X -= 850f * fElapsed * (float)Math.Sin(_sin);
            }
            else if (!disable && _position.X <= 400f)
            {
                _sin += 3.9f * fElapsed;
                _position.X += 850f * fElapsed * (float)Math.Sin(_sin);
            }
            else
                isFinishedWithWork = true;

            if (_position.X >= 400f)
                IsFullyChanged = true;
        }

        public void Draw(float fElapsed)
        {
            if (IsFullyChanged)
            {
                scale = Math.Abs(0.125f * (float)Math.Sin(gt) + 1f);
                scale -= 0.2f;
            }

            Game.SpriteBatch.Draw(_cloudTexture, _position, null, Color.White, 0.0f, _cloudTexture.GetCenter(), scale, SpriteEffects.None, 0);
        }

        public void Enable()
        {
            //isFinishedWithWork = false;
            IsFullyChanged = false;
            _sin = 0;
            disable = false;
        }

        public void Disable()
        {
            IsFullyChanged = false;
            _sin = 0;
            isFinishedWithWork = false;
            disable = true;
        }

        public void RenderRainOnTop(float fTime)
        {
            //if (!isRaining)
                //return;

            rainPos2.Y += 500f * fTime;
            if (rainPos2.Y >= 600f)
                rainPos2.Y = 0;

            rainPos.Y += 300f * fTime;
            if (rainPos.Y >= 600f)
                rainPos.Y = 0;

            Game.SpriteBatch.Draw(_rainTexture, rainPos, Color.White * _alpha * 0.75f);
            Vector2 nRP = rainPos;
            nRP.Y -= 600f;
            Game.SpriteBatch.Draw(_rainTexture, nRP, Color.White * _alpha * 0.75f);

            Game.SpriteBatch.Draw(_rainTexture, rainPos2, Color.White * _alpha * 0.75f);
            Vector2 nRP2 = rainPos2;
            nRP2.Y -= 600f;
            Game.SpriteBatch.Draw(_rainTexture, nRP2, Color.White * _alpha * 0.75f);
        }

        public void loadUp(float fElapsed)
        {
            if (isRaining)
                energy -= energy <= 0f ? 0f : 0.21f * fElapsed;
            else// if (!Game.Mouse.IsLeftButtonDown() && !disable)
                energy += energy >= 1f ? 0f : 0.125f * fElapsed;
        }
    }
}
