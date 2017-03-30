using System;
using System.Collections.Generic;
using Game2DFramework.Drawing;

namespace Game2DFramework.Scripting
{
    public class ScriptRunner : GameObject
    {
        private Dialog _dialog;
        private readonly Queue<IScriptedStep> _steps;
        private IScriptedStep _current;

        private List<IScriptedStep> _addingSteps = new List<IScriptedStep>(); 

        public ScriptRunner(Game2D game) : base(game)
        {
            _steps = new Queue<IScriptedStep>();
        }

        public void Configure(ScriptRunnerParameters parameters)
        {
            _dialog = parameters.Dialog;
        }

        public ScriptRunner AddDialogStep(string message)
        {
            _addingSteps.Add(new DialogStep(_dialog, message));
            return this;
        }

        public void Start()
        {
            if (_addingSteps.Count == 0) throw new InvalidOperationException("Fluent call must call AddStep");
            Start(_addingSteps);
            _addingSteps.Clear();
        }

        public void Start(IEnumerable<IScriptedStep> steps)
        {
            foreach (var scriptedStep in steps) _steps.Enqueue(scriptedStep);
            _current = _steps.Dequeue();
            _current.Start();
        }

        public bool LocksControls 
        {
            get { return _current != null && _current.LocksControls; }
        }

        public void Update(float elapsed)
        {
            if (!IsRunning) return;

            _current.Update(elapsed, Game);
            if (_current.Finished)
            {
                _current = _steps.Count == 0 ? null : _steps.Dequeue();
                if (_current != null) _current.Start();
            }
        }

        public bool IsRunning
        {
            get { return _current != null; }
        }
    }
}
