using System;
using System.Collections.Generic;
using Game2DFramework.Cameras;
using Game2DFramework.Drawing;
using Game2DFramework.Input;
using Game2DFramework.Scripting;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework
{
    public delegate void GlobalObjectChangedEventHandler(string name, GameObject oldValue, GameObject newValue);

    public abstract class Game2D : Game
    {
        private StateManager _stateManager;
        private readonly ClearOptions _clearOptions;
        private GamePadEx _gamePad;
        private Type _startupState;
        private readonly Dictionary<string, GameObject> _registeredGlobals;

        public int ScreenWidth { get { return GraphicsDevice.Viewport.Width; } }
        public int ScreenHeight { get { return GraphicsDevice.Viewport.Height; } }
        public DepthRenderer DepthRenderer { get; private set; }
        public ShapeRenderer ShapeRenderer { get; private set; }
        public KeyboardEx Keyboard { get; private set; }
        public MouseEx Mouse { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public Camera Camera { get; private set; }
        public ScriptRunner ScriptRunner { get; private set; }

        public GamePadEx GamePad
        {
            get
            {
                if (_gamePad == null) throw new InvalidOperationException("Gamepad must be enabled for game to use");
                return _gamePad;
            }
            private set { _gamePad = value; }
        }

        public event GlobalObjectChangedEventHandler GlobalObjectChanged; 
        
        protected Game2D(int screenWidth, int screenHeight, bool fullscreen, bool useGamePad = false, DepthFormat depthFormat = DepthFormat.None)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = screenWidth,
                PreferredBackBufferHeight = screenHeight,
                PreferredDepthStencilFormat = depthFormat,
                IsFullScreen = fullscreen
            };
            Content.RootDirectory = "Content";

            Keyboard = new KeyboardEx();
            Mouse = new MouseEx();
            DepthRenderer = new DepthRenderer();
            ScriptRunner = new ScriptRunner(this);
            if (useGamePad) GamePad = new GamePadEx();

            _clearOptions = ClearOptions.Target;
            if (depthFormat != DepthFormat.None)
            {
                _clearOptions |= ClearOptions.DepthBuffer;
            }

            _registeredGlobals = new Dictionary<string, GameObject>();
        }

        private void InvokeGlobalObjectChanged(string name, GameObject oldValue, GameObject newValue)
        {
            if (GlobalObjectChanged != null) GlobalObjectChanged(name, oldValue, newValue);
        }

        public T RegisterGlobalObject<T>(string name, T gameObject) where T: GameObject
        {
            if (_registeredGlobals.ContainsKey(name)) throw new InvalidOperationException("There is aleady an object registered with name " + name + ".");
            if (gameObject == null) throw new ArgumentNullException("gameObject");
            
            _registeredGlobals.Add(name, gameObject);
            InvokeGlobalObjectChanged(name, null, gameObject);

            return gameObject;
        }

        public void ReplaceGlobalObject(string name, GameObject gameObject)
        {
            if (!_registeredGlobals.ContainsKey(name)) throw new InvalidOperationException("There is no object registered with name " + name + ".");
            if (gameObject == null) throw new ArgumentNullException("gameObject");

            var oldValue = _registeredGlobals[name];
            _registeredGlobals[name] = gameObject;
            InvokeGlobalObjectChanged(name, oldValue, gameObject);
        }

        public T GetGlobalObject<T>(string name) where T: GameObject
        {
            if (!_registeredGlobals.ContainsKey(name)) throw new InvalidOperationException(string.Format("Object {0} is not registered", name));
            return (T) _registeredGlobals[name];
        }

        protected abstract Type RegisterStates();

        protected override void Initialize()
        {
            _stateManager = new StateManager(this, _clearOptions);
            
            RegisterTransition(new BlendTransition());
            RegisterTransition(new FlipTransition(GraphicsDevice));
            RegisterTransition(new GrowTransition(GraphicsDevice));
            RegisterTransition(new SlideTransition(GraphicsDevice));
            RegisterTransition(new CardTransition(GraphicsDevice));
            RegisterTransition(new ThrowAwayTransition(GraphicsDevice));

            _startupState = RegisterStates();
            Camera = new Camera(this);
            base.Initialize();
        }

        protected void RegisterTransition(ITransition transition)
        {
            _stateManager.RegisterTransition(transition);
        }

        protected void RegisterState(IState state)
        {
            _stateManager.RegisterState(state);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ShapeRenderer = new ShapeRenderer(SpriteBatch, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            var elapsedTime = gameTime.ElapsedGameTime.Milliseconds*0.001f;

            if (_startupState != null)
            {
                _stateManager.SetCurrentState(_startupState, null);
                _startupState = null;
            }

            if (!_stateManager.TransitionInProgress)
            {
                ScriptRunner.Update(elapsedTime);
                Camera.Update(elapsedTime);
                Keyboard.Update();
                Mouse.Update(elapsedTime);
                if (_gamePad != null) GamePad.Update();   
            }

            if (!_stateManager.Update(elapsedTime)) Exit();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var elapsedTime = gameTime.ElapsedGameTime.Milliseconds * 0.001f;
            _stateManager.Draw(elapsedTime);
            base.Draw(gameTime);
        }

        public void ActivateDefaultView()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
        }
    }
}
