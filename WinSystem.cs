using System;
using System.Collections.Generic;
using Entitas;
using Game.Installers;
using Game.Views;
using UniRx;
using UnityEngine;

namespace Ecs.Sources.Game.Systems
{
    public class WinSystem : ReactiveSystem<GameEntity>, IDisposable
    {
        private readonly CanvasView _canvasView;
        private readonly GameSettingsInstaller.GameSettings _gameSettings;
        private readonly CameraView _cameraView;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public WinSystem(IContext<GameEntity> context,
            CanvasView canvasView,
            GameSettingsInstaller.GameSettings gameSettings,
            CameraView cameraView) : base(context)
        {
            _canvasView = canvasView;
            _gameSettings = gameSettings;
            _cameraView = cameraView;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
            => context.CreateCollector(GameMatcher.Win);

        protected override bool Filter(GameEntity entity)
            => entity.isWin;

        protected override void Execute(List<GameEntity> entities)
        {
            Observable.Timer(TimeSpan.FromSeconds(_gameSettings.winDelay))
                .Subscribe(_ => _canvasView.WinScreen.Open())
                .AddTo(_disposable);
            
            Observable.Timer(TimeSpan.FromSeconds(_gameSettings.confettiDelay))
                .Subscribe(_ => _cameraView.ConfettiFx.Play())
                .AddTo(_disposable);
        }

        public void Dispose() => _disposable?.Dispose();
    }
}