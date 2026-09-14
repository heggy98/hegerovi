using Hegerovi.Backend.Models;
using Microsoft.Azure.Cosmos;

namespace Hegerovi.Backend.Services;

public class CosmosDataStore : IDataStore
{
    private readonly Container _guests;
    private readonly Container _invitations;

    public CosmosDataStore(CosmosClient client, string databaseName)
    {
        var database = client.GetDatabase(databaseName);
        _guests = database.GetContainer("Guests");
        _invitations = database.GetContainer("Invitations");
    }

    public static async Task EnsureCreatedAsync(CosmosClient client, string databaseName)
    {
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync("Guests", "/id");
        await database.Database.CreateContainerIfNotExistsAsync("Invitations", "/id");
    }

    public async Task<List<Guest>> GetGuestsAsync()
    {
        var results = new List<Guest>();
        using var iterator = _guests.GetItemQueryIterator<Guest>(new QueryDefinition("SELECT * FROM c"));
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }
        return results;
    }

    public async Task<Guest?> GetGuestAsync(string id)
    {
        try
        {
            var response = await _guests.ReadItemAsync<Guest>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Guest> UpsertGuestAsync(Guest guest)
    {
        var response = await _guests.UpsertItemAsync(guest, new PartitionKey(guest.Id));
        return response.Resource;
    }

    public async Task DeleteGuestAsync(string id)
    {
        await _guests.DeleteItemAsync<Guest>(id, new PartitionKey(id));
    }

    public async Task<List<Invitation>> GetInvitationsAsync()
    {
        var results = new List<Invitation>();
        using var iterator = _invitations.GetItemQueryIterator<Invitation>(new QueryDefinition("SELECT * FROM c"));
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }
        return results;
    }

    public async Task<Invitation?> GetInvitationByTokenAsync(string token)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.Token = @token").WithParameter("@token", token);
        using var iterator = _invitations.GetItemQueryIterator<Invitation>(query);
        if (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            return page.FirstOrDefault();
        }
        return null;
    }

    public async Task<Invitation> UpsertInvitationAsync(Invitation invitation)
    {
        var response = await _invitations.UpsertItemAsync(invitation, new PartitionKey(invitation.Id));
        return response.Resource;
    }
}
