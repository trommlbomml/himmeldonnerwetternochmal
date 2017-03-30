using System;
using DevmaniaGame.States;
using Game2DFramework;

namespace DevmaniaGame
{
    public class Game1 : Game2D
    {
        public Game1() : base(800, 600, false)
        {
        }

        protected override Type RegisterStates()
        {
            RegisterState(new MainGame());
            RegisterState(new IntroState());
            RegisterState(new PlayersRolesState());
            RegisterState(new OverviewState());
            RegisterState(new GameOverState());
            return typeof(IntroState);
        }

        static void Main(string[] args)
        {
            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
}
