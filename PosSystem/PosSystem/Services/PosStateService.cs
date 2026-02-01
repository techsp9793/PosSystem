using PosSystem.Data.Entities;
using PosSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace PosSystem.Services
{
    public class PosStateService
    {
        public Unit? CurrentUnit { get; private set; }

        // This ID is unique to THIS browser tab
        public string LocalSessionId { get; } = Guid.NewGuid().ToString();

        public bool IsSessionValid { get; private set; } = true;

        public event Action? OnChange;

        public void SetUnit(Unit unit)
        {
            CurrentUnit = unit;
            // [FIX] Reset validity immediately so the user isn't locked out 
            // while the DB updates in the background.
            IsSessionValid = true;
            NotifyStateChanged();
        }

        public async Task ValidateSessionAsync(string userId, IDbContextFactory<LocalDbContext> dbFactory, bool takeControl = false)
        {
            try
            {
                using var db = await dbFactory.CreateDbContextAsync();
                var sessionKey = $"ActivePOS_{userId}";

                var dbSession = await db.SystemSettings.FirstOrDefaultAsync(s => s.Key == sessionKey);

                if (dbSession == null)
                {
                    // Create new session record
                    db.SystemSettings.Add(new SystemSetting
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = sessionKey,
                        Value = LocalSessionId,
                        CreatedAt = DateTime.UtcNow,
                        TenantId = CurrentUnit?.TenantId
                    });
                    await db.SaveChangesAsync();
                    IsSessionValid = true;
                }
                else if (takeControl)
                {
                    // [Last Login Wins] Force update the DB with MY LocalSessionId
                    if (dbSession.Value != LocalSessionId)
                    {
                        dbSession.Value = LocalSessionId;
                        dbSession.TenantId = CurrentUnit?.TenantId;
                        dbSession.LastModifiedAt = DateTime.UtcNow;
                        await db.SaveChangesAsync();
                    }
                    IsSessionValid = true;
                    NotifyStateChanged(); // Unlock UI if it was previously locked
                }
                else
                {
                    // [Background Check] Do I still match the DB?
                    if (dbSession.Value != LocalSessionId)
                    {
                        // Mismatch -> Someone else took control
                        if (IsSessionValid)
                        {
                            IsSessionValid = false;
                            NotifyStateChanged(); // Lock the UI
                        }
                    }
                    else
                    {
                        IsSessionValid = true;
                    }
                }
            }
            catch
            {
                // If DB fails, stay valid to avoid disrupting sales
                IsSessionValid = true;
            }
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