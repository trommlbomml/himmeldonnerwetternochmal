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
    class Sun : GameObject
    {
        public Vector2 _position;
        Texture2D _sunTexture;
        public bool isBlending;
        public float _blendVal;
        public bool disable;
        public bool isFinishedWithWork;
        public float energy;
        public bool IsFullyChanged;
        public float scale;
        public float gt;

        // Gaaaanz böse Verletzung von OOP ;) (zu wenig Zeit)
        public Cloud _cloud;

        public Sun(Game2D game, Vector2 initPos)
            : base(game)
        {
            isFinishedWithWork = true;
            _sunTexture = game.Content.Load<Texture2D>("Textures\\Sun");
            _position = initPos;
            disable = true;
            _blendVal = 0.0f;
            isBlending = false;
            energy = 1;
            IsFullyChanged = false;
            scale = 0.95f;
        }

        public void Update(float fElapsed)
        {
            if(IsFullyChanged)
                gt += fElapsed * 5;

            isBlending = false;

            if (Game.Mouse.IsLeftButtonDown())
                isBlending = true;

            if (disable)
                isBlending = false;

            if (energy <= 0)
                isBlending = false;

            if (isBlending)
                _blendVal += 0.6f * fElapsed;
            else
                _blendVal -= 0.6f * fElapsed;

            if (_blendVal < 0f)
                _blendVal = 0f;
            else if (_blendVal > 1f)
                _blendVal = 1f;

            if (_blendVal <= 0.0f)
                isFinishedWithWork = true;

            if (_position.X <= 400f)
                IsFullyChanged = true;
        }

        public void Draw(float fElapsed)
        {
            _position.X = _cloud._position.X + 300f;

            if (IsFullyChanged)
                scale = Math.Abs(0.125f * (float)Math.Sin(gt) + 1f);

            Game.SpriteBatch.Draw(_sunTexture, _position, null, Color.White, 0.0f, _sunTexture.GetCenter(), scale, SpriteEffects.None, 0);
        }

        public void Enable()
        {
            IsFullyChanged = false;
            disable = false;
        }

        public void Disable()
        {
            IsFullyChanged = false;
            isFinishedWithWork = false;
            disable = true;
        }

        public void loadUp(float fElapsed)
        {
            if (isBlending)
                energy -= energy <= 0f ? 0f : 0.335f * fElapsed;
            else
                energy += energy >= 1 ? 0 : 0.083f * fElapsed;
        }
    }
}
