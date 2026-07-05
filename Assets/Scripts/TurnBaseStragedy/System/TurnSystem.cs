using System;

namespace TurnBaseStragedy.System
{
    public class TurnSystem : MonoSingleton<TurnSystem>
    {
        public int TurnNumber { get; private set; } = 1;
        public bool IsPlayerTurn { get; private set; } = true;

        public event EventHandler OnTurnChanged;

        public void NextTurn()
        {
            TurnNumber++;
            IsPlayerTurn = !IsPlayerTurn;
            OnTurnChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}