using System;
using Ecs.Sources.Item.Components.Tube;
using Entitas;
using Game.Installers;
using UniRx;
using UnityEngine;

namespace Ecs.Sources.Item.Systems.Tube
{
    public class TubeMoveSystem : IExecuteSystem
    {
        private readonly ItemContext _item;
        private readonly GameSettingsInstaller.ItemSettings _itemSettings;
        private readonly GameContext _game;
        private readonly GameSettingsInstaller.GameSettings _gameSettings;

        public TubeMoveSystem(ItemContext item,
            GameSettingsInstaller.ItemSettings itemSettings,
            GameContext game,
            GameSettingsInstaller.GameSettings gameSettings)
        {
            _item = item;
            _itemSettings = itemSettings;
            _game = game;
            _gameSettings = gameSettings;
        }

        public void Execute()
        {
            if (_game.isLose || _game.isWin)
                return;
            
            var entity = _item.tubeEntity;

            if (!entity.isTubeMove)
                return;

            var transform = entity.transform.Value;
            var direction = entity.tubeDirection.Value == ETubeDirection.Left ? -transform.right : transform.right;

            transform.Translate(direction * (entity.tubeSpeed.Value * Time.deltaTime));

            if (entity.tubeDirection.Value == ETubeDirection.Right &&
                transform.position.x >= _itemSettings.tubeRightBound)
                entity.ReplaceTubeDirection(ETubeDirection.Left);
            else if (entity.tubeDirection.Value == ETubeDirection.Left &&
                     transform.position.x <= _itemSettings.tubeLeftBound)
                entity.ReplaceTubeDirection(ETubeDirection.Right);

            if (!entity.isTubeSpeedup)
            {
                entity.ReplaceTubeTimer(entity.tubeTimer.Value + Time.deltaTime);
                if (entity.tubeTimer.Value >= _itemSettings.tubeStartSlowTimer)
                {
                    entity.isTubeSlowdown = true;
                }
            }


            if (entity.isTubeSlowdown)
            {
                var speed = entity.tubeSpeed.Value;

                speed -= Time.deltaTime * _itemSettings.tubeSlowdownChangeSpeed;
                entity.ReplaceTubeSpeed(speed);

                if (speed <= _itemSettings.tubeSpeedToSpawnBalls)
                {
                    entity.isTubeMove = false;
                    entity.isTubeSlowdown = false;
                    entity.isTubeSpawnBalls = true;

                    Observable.Timer(TimeSpan.FromSeconds(_gameSettings.delayBeforeSpawnBalls))
                        .Subscribe(_ => _game.isSpawnBalls = true);
                }
            }

            if (entity.isTubeSpeedup)
            {
                var speed = entity.tubeSpeed.Value;

                speed += Time.deltaTime * _itemSettings.tubeSlowdownChangeSpeed;
                entity.ReplaceTubeSpeed(speed);

                if (speed >= _itemSettings.tubeSpeed)
                {
                    entity.ReplaceTubeSpeed(_itemSettings.tubeSpeed);
                    entity.ReplaceTubeTimer(0f);
                    entity.isTubeSpeedup = false;
                }
            }
        }
    }
}