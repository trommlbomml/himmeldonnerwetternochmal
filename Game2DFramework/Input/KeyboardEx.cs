﻿
using Microsoft.Xna.Framework.Input;

namespace Game2DFramework.Input
{
    public class KeyboardEx
    {
        private KeyboardState _lastState;
        private KeyboardState _currentState;
        private Keys[] _pressedKeys;

        public KeyboardEx()
        {
            _currentState = Keyboard.GetState();
        }

        public bool IsKeyDownOnce(Keys key)
        {
            return _currentState.IsKeyDown(key) && _lastState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return _currentState.IsKeyUp(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return _currentState.IsKeyUp(key) && _lastState.IsKeyDown(key);
        }

        public Keys[] GetPressedKeys()
        {
            return _pressedKeys;
        }

        public void Update()
        {
            _lastState = _currentState;
            _currentState = Keyboard.GetState();
            _pressedKeys = _currentState.GetPressedKeys();
        }
    }
}
