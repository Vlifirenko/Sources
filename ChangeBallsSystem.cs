using System.Collections.Generic;
using Entitas;
using Game.Installers;
using Game.Views;

namespace Ecs.Sources.Game.Systems
{
    public class ChangeBallsSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _game;
        private readonly ItemContext _item;
        private readonly GameSettingsInstaller.GameSettings _gameSettings;

        public ChangeBallsSystem(GameContext game,
            ItemContext item,
            GameSettingsInstaller.GameSettings gameSettings
        ) : base(game)
        {
            _game = game;
            _item = item;
            _gameSettings = gameSettings;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
            => context.CreateCollector(GameMatcher.Balls);

        protected override bool Filter(GameEntity entity)
            => entity.hasBalls;

        protected override void Execute(List<GameEntity> entities)
        {
            var tubeEntity = _item.tubeEntity;
            var gameObject = tubeEntity.gameObject.Value;
            var tubeView = gameObject.GetComponent<TubeView>();

            tubeView.BallsText.text = _game.balls.Value.ToString();

            if (_game.balls.Value == 0)
                EndBalls();
        }

        private void EndBalls()
        {
            var splineGroup = _item.GetGroup(ItemMatcher.Spline);
            foreach (var splineEntity in splineGroup.GetEntities())
            {
                if (splineEntity.percent.Value < 1f)
                {
                    _game.isLose = true;
                    return;
                }
            }
        }
    }
}