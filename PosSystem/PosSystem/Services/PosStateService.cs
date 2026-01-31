using PosSystem.Data.Entities;

namespace PosSystem.Services
{
    public class PosStateService
    {
        // 1. Where are we selling?
        public Unit? CurrentUnit { get; private set; }

        // 2. Events to notify UI when things change
        public event Action? OnChange;

        public void SetUnit(Unit unit)
        {
            CurrentUnit = unit;
            NotifyStateChanged();
        }

        public void ClearUnit()
        {
            CurrentUnit = null;
            NotifyStateChanged();
        }

        public bool IsUnitSelected => CurrentUnit != null;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}