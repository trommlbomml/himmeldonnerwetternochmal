using System;
using Game2DFramework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Game2DFramework.Extensions;
using Game2DFramework.Drawing;
using Microsoft.Xna.Framework.Input;

namespace DevmaniaGame.Objects
{
    enum PlayerState
    {
        UserControl,
        Respawning,
        Dead,
    }

    class Player : GameObject, IDepthSortable
    {
        public event Action OnRespawnFinished;
        public event Action OnDied;

        private float _currentSlideBonusTick;
        private float _currentPointTimingTick;
        private readonly ExplosionManager _explosionManager;
        public PlayerState PlayerState { get; private set; }
        private Texture2D _tachotexture;
        private SpriteFont _font;
        readonly Texture2D _playerTexture;
        public Vector2 Position;

        public Vector2 CurrentSpeed;
        public float IncSpeed = 500;
        public float MaxSpeed = 300;
        public float BrakeSpeed = 380;
        public int Points;

        public Weather weather { get; set; }
        public int Energy { get; private set; }
        public int Lifes { get; private set; }
        
        public Player(Game2D game, Vector2 initPos)
            : base(game)
        {
            _tachotexture = Game.Content.Load<Texture2D>("textures/tacho");
            _font = Game.Content.Load<SpriteFont>("Fonts/Font");
            _explosionManager = Game.GetGlobalObject<ExplosionManager>("ExplosionManager");
            PlayerState = PlayerState.UserControl;
            _playerTexture = game.Content.Load<Texture2D>("Textures\\Player");
            Position = initPos;
            Energy = 100;
            Lifes = 3;
            Respawn();
        }

        public void Update(float fElapsed)
        {
            switch (PlayerState)
            {
                case PlayerState.UserControl:
                    UpdatePlayerUserControl(fElapsed);
                    break;
                case PlayerState.Respawning:
                    UpdatePlayerRespawning(fElapsed);
                    break;
            }
        }

        private void UpdatePlayerRespawning(float fElapsed)
        {
            Position.Y -= 100.0f*fElapsed;
            if (Position.Y <= 500)
            {
                Position.Y = 500;
                PlayerState = PlayerState.UserControl;
                CurrentSpeed = Vector2.Zero;
                if (OnRespawnFinished != null) OnRespawnFinished();
            }
        }

        private void Respawn()
        {
            _currentPointTimingTick = 0;
            Energy = 100;
            Position = new Vector2(400, 800);
            PlayerState = PlayerState.Respawning;
        }

        private void UpdatePlayerUserControl(float fElapsed)
        {
            if (Math.Abs(Position.X - 400) > 0.0000001)
            {
                _currentPointTimingTick += fElapsed;
                if (_currentPointTimingTick >= 0.25f)
                {
                    _currentPointTimingTick -= 0.25f;
                    Points += 5;
                }
            }

            float brakeMultiplier = 5.0f;
            if (weather._cloud.isRaining)
                brakeMultiplier = 0.8f;

            if (Game.Keyboard.IsKeyDown(Keys.D))
            {
                CurrentSpeed.X += IncSpeed * brakeMultiplier * fElapsed;
                if (CurrentSpeed.X > MaxSpeed)
                    CurrentSpeed.X = MaxSpeed;
            }
            else if (Game.Keyboard.IsKeyDown(Keys.A))
            {
                CurrentSpeed.X -= IncSpeed * brakeMultiplier * fElapsed;
                if (CurrentSpeed.X < -MaxSpeed)
                    CurrentSpeed.X = -MaxSpeed;
            }
            else
            {
                if (CurrentSpeed.X > 0)
                {
                    CurrentSpeed.X -= BrakeSpeed * brakeMultiplier * fElapsed;
                    if (CurrentSpeed.X < 0) CurrentSpeed.X = 0;
                }
                else if (CurrentSpeed.X < 0)
                {
                    CurrentSpeed.X += BrakeSpeed * brakeMultiplier * fElapsed;
                    if (CurrentSpeed.X > 0) CurrentSpeed.X = 0;
                }
            }

            if (Math.Abs(CurrentSpeed.X) > MaxSpeed * 0.5f)
            {
                _currentSlideBonusTick += fElapsed;
                if (_currentSlideBonusTick >= 1.0f)
                {
                    _currentSlideBonusTick -= 1.0f;
                    Points += 50;
                }
            }
            else
            {
                _currentSlideBonusTick = 0;
            }

            Position += CurrentSpeed * fElapsed;

            var boundaries = GetXBoundary();
            if (Position.X < boundaries.Item1) ApplyForce(400 * DepthScale);
            else if (Position.X > boundaries.Item2) ApplyForce(-400 * DepthScale);

            if (Game.Keyboard.IsKeyDown(Keys.W))
            {
                Position.Y -= 150.0f * fElapsed;
                if (Position.Y <= 220) Position.Y = 220;
            }
            else if (Game.Keyboard.IsKeyDown(Keys.S))
            {
                Position.Y += 150.0f * fElapsed;
                if (Position.Y >= 580) Position.Y = 580;
            }

            var maxBoundsSizeX = (int)(40 * DepthScale);
            var maxBoundsSizeY = (int)(50 * DepthScale);
            Bounds = new Rectangle((int)Position.X - maxBoundsSizeX / 2, (int)Position.Y - maxBoundsSizeY / 2, maxBoundsSizeX, maxBoundsSizeY);
        }

        private Tuple<float, float> GetXBoundary()
        {
            var distance = 130.0f/400.0f*(Position.Y - 200.0f);
            return new Tuple<float, float>(180-distance, 800-180+distance);
        }

        public Rectangle Bounds { get; private set; }

        private float DepthScale
        {
            get { return (Position.Y - 200.0f) / 250f + 0.5f; }
        }

        public void Draw(float fElapsed)
        {
            if (PlayerState == PlayerState.Dead) return;

            var effect = Position.X < 400 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Game.SpriteBatch.Draw(_playerTexture, Position, null, Color.White, 0.0f, _playerTexture.GetCenter(), DepthScale, effect, 0);

            Game.ShapeRenderer.DrawFilledRectangle((int) (Position.X - 20),(int) (Position.Y - (_playerTexture.Height / 2) * DepthScale - 8), 40, 8, Color.Black);
            var width = (int)(Energy/100.0f*40);
            var color = Energy >= 80 ? Color.Green : Energy >= 40 ? Color.Yellow : Color.Red;
            Game.ShapeRenderer.DrawFilledRectangle((int)(Position.X - 20), (int)(Position.Y - (_playerTexture.Height / 2) * DepthScale - 8), width, 8, color);

            Game.SpriteBatch.Draw(_tachotexture, new Vector2(0, 250), Color.White);
            Game.SpriteBatch.DrawString(_font, Lifes.ToString(), new Vector2(20, 320), Color.White);

            var size = _font.MeasureString(Points.ToString());
            Game.SpriteBatch.DrawString(_font, Points.ToString(), (new Vector2(120,386) - new Vector2(size.X, 0)).SnapToPixels(), Color.White);

            //Game.ShapeRenderer.DrawRectangle(Bounds, Color.Orange);
        }

        public override void ApplyDamage(int amount)
        {
            Energy -= amount;
            if (Energy <= 0)
            {
                Energy = 0;
                Die();
            }
            else
            {
                Points -= 50;
                if (Points <= 0)
                    Points = 0;
            }
        }

        public void ApplyForce(float xForce)
        {
            CurrentSpeed.X = xForce;
        }

        public int Depth
        {
            get { return (int)Math.Round(Position.Y); }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(0.0f);
        }

        public void Die()
        {
            if (OnDied != null) OnDied();
            _explosionManager.SpawnExplosion(Position);
            Lifes -= 1;
            if (Lifes < 0)
            {
                Lifes = 0;
                PlayerState = PlayerState.Dead;
            }
            else
            {
                Respawn();   
            }
        }
    }
}
