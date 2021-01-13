using Entitas;
using UnityEngine;

namespace Ecs.Sources.Game.Systems
{
    public class LoseTimerSystem : IExecuteSystem
    {
        private readonly GameContext _game;

        public LoseTimerSystem(GameContext game)
        {
            _game = game;
        }

        public void Execute()
        {
            if (_game.isLose || _game.isWin)
                return;

            if (!_game.hasLoseTimer)
                return;

            var loseTimer = _game.loseTimer.Value;

            loseTimer -= Time.deltaTime;
            _game.ReplaceLoseTimer(loseTimer);

            if (loseTimer <= 0)
            {
                _game.isLose = true;
                _game.RemoveWinTimer();
            }
        }
    }
}