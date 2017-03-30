using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing
{
    public enum DialogState
    {
        Hidden,
        TransitToShow,
        Shown,
        TransitToHide,
    }

    public class Dialog : GameObject
    {
        private readonly SpriteFont _font;
        private readonly NinePatchSprite _ninePatch;
        private string _message;
        private Vector2 _dialogSize;
        private float _transitionDelta;

        public DialogState State { get; private set; }
        public float TransitDuration { get; set; }

        public Dialog(Game2D game,Texture2D ninePatchTexture, Rectangle? sourceRectangle, SpriteFont font) : base(game)
        {
            _ninePatch = new NinePatchSprite(ninePatchTexture, sourceRectangle, 16);
            _font = font;
            State = DialogState.Hidden;
            _transitionDelta = 0;
            TransitDuration = 0.25f;
        }

        public void SetText(string message)
        {
            _message = message;
            var textSpace = _font.MeasureString(message);
            _dialogSize = new Vector2(textSpace.X + 2 * 16, textSpace.Y + 2 * 16);
        }

        public void Show()
        {
            if (State != DialogState.Hidden) throw new InvalidOperationException();
            State = DialogState.TransitToShow;
        }

        public void Hide()
        {
            if (State != DialogState.Shown) throw new InvalidOperationException();
            State = DialogState.TransitToHide;
        }

        public void Update(float elapsedTime)
        {
            switch (State)
            {
                case DialogState.TransitToShow:
                    UpdateTransitionToShow(elapsedTime);
                    break;
                case DialogState.TransitToHide:
                    UpdateTransitionToHide(elapsedTime);
                    break;
            }
        }

        private void UpdateTransitionToHide(float elapsed)
        {
            _transitionDelta -= elapsed * (1.0f / TransitDuration);
            if (_transitionDelta <= 0f)
            {
                _transitionDelta = 0f;
                State = DialogState.Hidden;
            }
        }

        private void UpdateTransitionToShow(float elapsed)
        {
            _transitionDelta += elapsed * (1.0f / TransitDuration);
            if (_transitionDelta >= 1f)
            {
                _transitionDelta = 1f;
                State = DialogState.Shown;
            }
        }

        public void Draw()
        {
            if (State == DialogState.Hidden) return;

            var dialogAlpha = MathHelper.SmoothStep(0, 0.75f, _transitionDelta);
            var dialogTextAlpha = MathHelper.SmoothStep(0, 01f, _transitionDelta);

            var targetRectangle = new Rectangle(
                (int) (Game.ScreenWidth - _dialogSize.X) / 2,
                (int)(Game.ScreenHeight - _dialogSize.Y), (int)_dialogSize.X, (int)_dialogSize.Y);

            _ninePatch.Draw(Game.SpriteBatch, targetRectangle, Color.White * dialogAlpha);

            var textPosition = new Vector2(targetRectangle.X + 16, targetRectangle.Y + 16);
            Game.SpriteBatch.DrawString(_font, _message, textPosition + new Vector2(2), Color.Black * dialogTextAlpha);
            Game.SpriteBatch.DrawString(_font, _message, textPosition, Color.White * dialogTextAlpha);
        }
    }
}
