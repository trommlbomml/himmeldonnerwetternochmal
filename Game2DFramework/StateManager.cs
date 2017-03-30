using System;
using System.Collections.Generic;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework
{
    internal class StateManager : GameObject
    {
        private readonly Dictionary<Type, IState> _availableStates;
        private readonly Dictionary<Type, ITransition> _availableTransitions;
        private bool _afterTransitionQuit;
        private readonly RenderTarget2D _sourceRenderTarget;
        private readonly RenderTarget2D _targetRenderTarget;
        private bool _transitionStarted;
        private ITransition _currentTransition;
        private IState _oldStateForSourceTransition;
        private IState _currentState;
        private ClearOptions _clearOptions;

        public bool TransitionInProgress { get; private set; }

        public StateManager(Game2D game, ClearOptions clearOptions) : base(game)
        {
            _clearOptions = clearOptions;
            _availableStates = new Dictionary<Type, IState>();
            _availableTransitions = new Dictionary<Type, ITransition>();
            _sourceRenderTarget = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width,
                                                     Game.GraphicsDevice.Viewport.Height);
            _targetRenderTarget = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width,
                                                     Game.GraphicsDevice.Viewport.Height);
        }

        public void RegisterTransition(ITransition transition)
        {
            var t = transition.GetType();
            if (_availableTransitions.ContainsKey(t)) throw new InvalidOperationException("Transition Type alread exists");
            if (transition == null) throw new ArgumentNullException("transition");

            _availableTransitions.Add(t, transition);
        }

        public void RegisterState(IState state)
        {
            if (_availableStates.ContainsKey(state.GetType())) throw new InvalidOperationException("State Type already exists");
            if (state == null) throw new ArgumentNullException("state");

            _availableStates.Add(state.GetType(), state);
            state.Game = Game;
        }

        public void SetCurrentState(Type type, object enterInformation)
        {
            IState newState;
            if (!_availableStates.TryGetValue(type, out newState)) throw new InvalidOperationException("State not registered");

            if (_currentState != null) _currentState.OnLeave();
            _currentState = newState;
            newState.OnEnter(enterInformation);
        }

        public void QuitGame(Type transition)
        {
            _afterTransitionQuit = true;
            TransitionInProgress = true;
            _transitionStarted = false;
            _currentTransition = _availableTransitions[transition];
            _currentTransition.Source = null;
            _currentTransition.Target = null;
            _oldStateForSourceTransition = _currentState;

            _currentState.OnLeave();
            _currentState = null;
        }

        public void ChangeToState(Type state, Type transition, object enterInformation)
        {
            IState newState;
            if (!_availableStates.TryGetValue(state, out newState))
                throw new InvalidOperationException("State not registered");

            _afterTransitionQuit = false;
            TransitionInProgress = true;
            _transitionStarted = false;
            _currentTransition = _availableTransitions[transition];
            _currentTransition.Source = null;
            _currentTransition.Target = null;
            _oldStateForSourceTransition = _currentState;

            _currentState.OnLeave();
            _currentState = newState;
            newState.OnEnter(enterInformation);
        }

        public bool Update(float elapsedTime)
        {
            if (TransitionInProgress)
            {
                if (!_transitionStarted)
                {
                    _currentTransition.Begin();
                    _transitionStarted = true;
                }

                _currentTransition.Update(elapsedTime);
                if (_currentTransition.TransitionReady)
                {
                    TransitionInProgress = false;
                    if (_afterTransitionQuit) return false;
                }
            }
            else
            {
                var stateChangeInformation = _currentState.OnUpdate(elapsedTime);
                if (stateChangeInformation != StateChangeInformation.Empty)
                {
                    if (stateChangeInformation.QuitGame)
                    {
                        QuitGame(stateChangeInformation.Transition);
                    }
                    else
                    {
                        ChangeToState(
                            stateChangeInformation.TargetState,
                            stateChangeInformation.Transition,
                            stateChangeInformation.EnterInformation);
                    }
                }
            }

            return true;
        }

        private void DrawState(float elapsedTime, IState toRender)
        {
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Game.Camera.WorldMatrix);
            toRender.OnDraw(elapsedTime);
            Game.SpriteBatch.End();
        }

        private void PreRenderSourceAndTargetState(float elapsedTime)
        {
            Game.GraphicsDevice.SetRenderTarget(_sourceRenderTarget);
            Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
            DrawState(elapsedTime, _oldStateForSourceTransition);

            if (_currentState != null)
            {
                Game.GraphicsDevice.SetRenderTarget(_targetRenderTarget);
                Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
                DrawState(elapsedTime, _currentState);
            }

            Game.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw(float elapsedTime)
        {
            if (TransitionInProgress)
            {
                if (_currentTransition.Source == null && _currentTransition.Target == null)
                {
                    PreRenderSourceAndTargetState(elapsedTime);
                    _currentTransition.Source = _sourceRenderTarget;
                    _currentTransition.Target = _targetRenderTarget;
                }

                Game.GraphicsDevice.Clear(_clearOptions, Color.Black, 1.0f, 0);
                Game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
                _currentTransition.Render(Game.SpriteBatch);
                Game.SpriteBatch.End();
            }
            else if (_currentState != null)
            {
                DrawState(elapsedTime, _currentState);
            }
        }
    }
}
