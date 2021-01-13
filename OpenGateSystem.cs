using Entitas;
using Game.Installers;
using Game.Views;
using UnityEngine;

namespace Ecs.Sources.Game.Systems
{
    public class OpenGateSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly GateView _gateView;
        private readonly GameSettingsInstaller.GameSettings _gameSettings;

        public OpenGateSystem(GameContext game,
            GateView gateView,
            GameSettingsInstaller.GameSettings gameSettings)
        {
            _game = game;
            _gateView = gateView;
            _gameSettings = gameSettings;
        }

        public void Execute()
        {
            var leftTransform = _gateView.Left;
            var rightTransform = _gateView.Right;
            
            if (_game.isOpenGate)
            {
                leftTransform.rotation = Quaternion.Lerp(
                    leftTransform.rotation,
                    _gateView.LeftLimit.rotation,
                    Time.deltaTime * _gameSettings.gateSpeed
                );
                rightTransform.rotation = Quaternion.Lerp(
                    rightTransform.rotation,
                    _gateView.RightLimit.rotation,
                    Time.deltaTime * _gameSettings.gateSpeed
                );
            }
            else
            {
                leftTransform.rotation = Quaternion.Lerp(
                    leftTransform.rotation,
                    _gateView.LeftOrigin.rotation,
                    Time.deltaTime * _gameSettings.gateSpeed
                );
                rightTransform.rotation = Quaternion.Lerp(
                    rightTransform.rotation,
                    _gateView.RightOrigin.rotation,
                    Time.deltaTime * _gameSettings.gateSpeed
                );
            }
        }
    }
}