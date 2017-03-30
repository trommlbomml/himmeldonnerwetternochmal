using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game2DFramework;
using Microsoft.Xna.Framework;

namespace DevmaniaGame.Objects
{
    class CarManager : GameObject
    {
        private int _lastGeneratedLane;
        private readonly List<Car> _allCars = new List<Car>();
        private readonly Random _random = new Random();
        private int _spawnTime;
        private Player _player;
        private bool _pauseRespawn;
        public int spawnTime;
        public int points;

        public CarManager(Game2D game) : base(game)
        {
            _lastGeneratedLane = -1;
            _spawnTime = 0;
            _player = Game.GetGlobalObject<Player>("Player");
            _player.OnRespawnFinished += () =>_pauseRespawn = false;
            _player.OnDied += () => _pauseRespawn = true;
            _pauseRespawn = true;
            spawnTime = 60;
        }

        public void Update(float fElapsed)
        {
            if (_player.PlayerState != PlayerState.Respawning)
            {
                foreach (var car in _allCars)
                {
                    if (car.DeadlyBounds.Intersects(_player.Bounds))
                    {
                        _player.Die();
                        MoveCarsOutofField();
                        break;
                    }
                    if (car.Bounds.Intersects(_player.Bounds))
                    {
                        if (_player.Position.X < car._position.X)
                        {
                            _player.ApplyForce(-500);
                        }
                        else
                        {
                            _player.ApplyForce(500);
                        }
                        _player.ApplyDamage(car.LaneNumber < 3 ? 20 : 10);
                        if (_player.PlayerState == PlayerState.Respawning)
                        {
                            MoveCarsOutofField();
                            break;
                        }
                    }
                }
            }

            foreach (var enemy in _allCars.Where(e => e._position.Y < 200))
            {
                Game.DepthRenderer.UnRegister(enemy);
            }
            _allCars.RemoveAll(e => e._position.Y < 200);
            foreach (var car in _allCars) car.Update(fElapsed);

            if (_pauseRespawn) return;

            if (_spawnTime <= 0)
            {
                SpawnNewCar();
                _spawnTime = (int)(_random.NextDouble() * spawnTime) + 50;
            }

            float vari = points / 3.0f;

            if (vari >= 280.0f)
                vari = 280.0f;
            _spawnTime -= (int)((fElapsed) * vari);
        }

        private void MoveCarsOutofField()
        {
            _allCars.ForEach(e => { e._position = new Vector2(0, 1000); });
        }

        private void SpawnNewCar()
        {
            int newLane = _random.Next(4) + 1;
            while (newLane == _lastGeneratedLane) newLane = _random.Next(4) + 1;
            _lastGeneratedLane = newLane;
            float xPos = newLane*160f;
            float yPos = 800f;

            if (newLane < 3)
            {
                xPos = (newLane - 1)*105f + 242f;
                yPos = 200f;
            }

            var car = new Car(Game, new Vector2(xPos, yPos), newLane);
            Game.DepthRenderer.Register(car);
            _allCars.Add(car);
        }
    }
}
