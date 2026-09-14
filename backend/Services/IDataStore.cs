using Hegerovi.Backend.Models;

namespace Hegerovi.Backend.Services;

public interface IDataStore
{
    Task<List<Guest>> GetGuestsAsync();
    Task<Guest?> GetGuestAsync(string id);
    Task<Guest> UpsertGuestAsync(Guest guest);
    Task DeleteGuestAsync(string id);

    Task<List<Invitation>> GetInvitationsAsync();
    Task<Invitation?> GetInvitationByTokenAsync(string token);
    Task<Invitation> UpsertInvitationAsync(Invitation invitation);
}
