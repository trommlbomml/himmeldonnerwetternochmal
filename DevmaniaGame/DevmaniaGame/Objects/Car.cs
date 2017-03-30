using System;
using Game2DFramework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Game2DFramework.Extensions;
using Game2DFramework.Drawing;

namespace DevmaniaGame.Objects
{
    class Car : GameObject, IDepthSortable
    {
        readonly Texture2D[] _carTexture;
        public Vector2 _position;
        private Vector2 _initPos;
        public float alpha;
        public bool _reachedHorizon;

        public int LaneNumber { get; private set; }

        public Car(Game2D game, Vector2 initPos, int laneNumber)
            : base(game)
        {
            LaneNumber = laneNumber;
            _carTexture = new[]
                {
                    game.Content.Load<Texture2D>("Textures\\lane_1"),
                    game.Content.Load<Texture2D>("Textures\\lane_2"),
                };
            _position = initPos;
            alpha = 1.0f;
            _initPos = initPos;
            _reachedHorizon = false;
        }

        public Rectangle Bounds { get; private set; }
        public Rectangle DeadlyBounds { get; private set; }

        public void Update(float fElapsed)
        {
            if (LaneNumber < 3)
            {
                _position.Y += 210.0f * fElapsed;
            }
            else
            {
                _position.Y -= 120.0f * fElapsed;
            }

            if (_position.Y <= 250f)
                alpha = (_position.Y - 200f) / 25f;

            if (LaneNumber == 1)
            {
                _position.X = _initPos.X - (_position.Y - 200f) * 0.3f;
            }
            else if (LaneNumber == 2)
            {
                _position.X = _initPos.X - (_position.Y - 200f) * 0.1f;
            }
            else if (LaneNumber == 3)
            {
                _position.X = _initPos.X + (_position.Y - 500f) * 0.1f;
            }
            else if (LaneNumber == 4)
            {
                _position.X = _initPos.X + (_position.Y - 500f) * 0.3f;
            }

            var maxBoundsSizeX = (int)(40 * DepthScale);
            var maxBoundsSizeY = (int)(50 * DepthScale);
            var offsetX = LaneNumber < 3 ? - 10 : 10;
            Bounds = new Rectangle((int)_position.X - maxBoundsSizeX / 2 - offsetX, (int)_position.Y - maxBoundsSizeY / 2, maxBoundsSizeX, maxBoundsSizeY);
            DeadlyBounds = new Rectangle((int)_position.X - maxBoundsSizeX / 4 - offsetX, (int)_position.Y - maxBoundsSizeY / 4, maxBoundsSizeX / 2, maxBoundsSizeY / 2);
        }

        private float DepthScale
        {
            get { return (_position.Y - 200.0f)/250f + 0.5f; }
        }

        public int Depth
        {
            get { return (int)Math.Round(_position.Y); }
        }

        public void Draw(float fElapsed)
        {
            var texture = LaneNumber < 3 ? _carTexture[0] : _carTexture[1];
            var offsetcorrector = LaneNumber < 3 ? - new Vector2(10, 0) : new Vector2(10, 0);

            Game.SpriteBatch.Draw(texture, _position, null, Color.White * alpha, 0.0f, texture.GetCenter() +offsetcorrector, DepthScale * 0.9f, SpriteEffects.None, 0);

            //Game.ShapeRenderer.DrawRectangle(Bounds, Color.Pink);
            //Game.ShapeRenderer.DrawRectangle(DeadlyBounds, Color.Pink);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(0.0f);
        }
    }
}
