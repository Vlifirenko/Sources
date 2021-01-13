using System;
using System.Collections.Generic;
using Entitas;
using Game.Installers;
using Game.Views;
using UniRx;
using UnityEngine;

namespace Ecs.Sources.Game.Systems
{
    public class LoseSystem : ReactiveSystem<GameEntity>, IDisposable
    {
        private readonly GameContext _game;
        private readonly CanvasView _canvasView;
        private readonly GameSettingsInstaller.GameSettings _gameSettings;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public LoseSystem(GameContext game,
            CanvasView canvasView,
            GameSettingsInstaller.GameSettings gameSettings) : base(game)
        {
            _game = game;
            _canvasView = canvasView;
            _gameSettings = gameSettings;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
            => context.CreateCollector(GameMatcher.Lose);

        protected override bool Filter(GameEntity entity)
            => entity.isLose;

        protected override void Execute(List<GameEntity> entities)
        {
            if (_game.hasWinTimer)
                _game.RemoveWinTimer();

            Observable.Timer(TimeSpan.FromSeconds(_gameSettings.loseDelay))
                .Subscribe(_ => _canvasView.LoseScreen.Open())
                .AddTo(_disposable);
        }

        public void Dispose() => _disposable?.Dispose();
    }
}