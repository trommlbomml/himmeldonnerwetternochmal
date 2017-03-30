﻿
using System;

namespace Game2DFramework.States
{
    public interface IState
    {
        Game2D Game { get; set; }
        void OnEnter(object enterInformation);
        void OnLeave();
        StateChangeInformation OnUpdate(float elapsedTime);
        void OnDraw(float elapsedTime);
    }
}
