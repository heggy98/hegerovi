using System.Collections.Concurrent;
using Hegerovi.Backend.Models;

namespace Hegerovi.Backend.Services;

public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentDictionary<string, Guest> _guests = new();
    private readonly ConcurrentDictionary<string, Invitation> _invitations = new();

    public Task<List<Guest>> GetGuestsAsync() => Task.FromResult(_guests.Values.ToList());

    public Task<Guest?> GetGuestAsync(string id)
    {
        _guests.TryGetValue(id, out var guest);
        return Task.FromResult(guest);
    }

    public Task<Guest> UpsertGuestAsync(Guest guest)
    {
        _guests[guest.Id] = guest;
        return Task.FromResult(guest);
    }

    public Task DeleteGuestAsync(string id)
    {
        _guests.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<List<Invitation>> GetInvitationsAsync() => Task.FromResult(_invitations.Values.ToList());

    public Task<Invitation?> GetInvitationByTokenAsync(string token)
    {
        var invitation = _invitations.Values.FirstOrDefault(i => i.Token == token);
        return Task.FromResult(invitation);
    }

    public Task<Invitation> UpsertInvitationAsync(Invitation invitation)
    {
        _invitations[invitation.Id] = invitation;
        return Task.FromResult(invitation);
    }
}
