
using Game2DFramework.Sound;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using DevmaniaGame.Objects;
using Microsoft.Xna.Framework.Media;

namespace DevmaniaGame.States
{
    class MainGame : InitializableState
    {
        private ExplosionManager _explosionManager;
        private GameBackground _gameBackground;
        private Player _player;
        private CarManager _carManager;
        private Weather _weather;
        public float _globalTime;
        private float _waitTimer;

        protected override void OnEntered(object enterInformation)
        {
            SoundService.PlaySong("MainTheme", true);
        }

        protected override void OnInitialize(object enterInformation)
        {
            _globalTime = 0f;
            SoundService.RegisterSong("MainTheme", Game.Content.Load<Song>("sounds/hardebm"));

            _explosionManager = Game.RegisterGlobalObject("ExplosionManager", new ExplosionManager(Game));

            _player = Game.RegisterGlobalObject("Player",  new Player(Game, new Vector2(500f, 400f)));
            _carManager = new CarManager(Game);
            _weather = new Weather(Game);
            _player.weather = _weather;
            Game.DepthRenderer.Register(_player);
            _gameBackground = new GameBackground(Game);
            _gameBackground.SetAnimationTime(0.25f);
        }

        public override void OnLeave()
        {
            
        }

        private bool _playerIsDead = false;

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            _globalTime += elapsedTime;

            _explosionManager.Update(elapsedTime);
            _gameBackground.Update(elapsedTime);
            _carManager.Update(elapsedTime);
            _player.Update(elapsedTime);
            _weather.Update(elapsedTime);

            if (_playerIsDead)
            {
                _waitTimer += elapsedTime;
                if (_waitTimer >= 3.0f)
                {
                    return StateChangeInformation.StateChange(typeof (GameOverState), typeof (FlipTransition),
                                                              _player.Points);
                }
            }
            else if (!_playerIsDead && _player.PlayerState == PlayerState.Dead)
            {
                SoundService.StopCurrentSong();
                _playerIsDead = true;
                _waitTimer = 0;
            }

            _carManager.spawnTime = (100 - _player.Points/50);
            if (_carManager.spawnTime <= 10)
                _carManager.spawnTime = 10;
            _carManager.points = _player.Points + 150;

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            _gameBackground.Draw();
            _weather.Draw(elapsedTime);
            Game.DepthRenderer.Draw(Game.SpriteBatch);

            _explosionManager.Draw();

            Game.ShapeRenderer.DrawFilledRectangle(0, 0, 800, 600, Color.White * _weather._sun._blendVal);
            _weather._cloud.RenderRainOnTop(elapsedTime);

            //Game.ShapeRenderer.DrawFilledRectangle(0, 0, 800, 600, Color.Gray * 0.5f);
        }
    }
}
