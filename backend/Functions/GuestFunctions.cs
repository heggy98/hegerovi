using System.Net;
using Hegerovi.Backend.Models;
using Hegerovi.Backend.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace Hegerovi.Backend.Functions;

public class GuestFunctions(IDataStore dataStore, IConfiguration configuration)
{
    private readonly string _adminApiKey = configuration["AdminApiKey"] ?? string.Empty;

    [Function("GetGuests")]
    public async Task<HttpResponseData> GetGuests(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "guests")] HttpRequestData req)
    {
        if (!AdminAuth.IsAuthorized(req, _adminApiKey))
        {
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var guests = await dataStore.GetGuestsAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(guests);
        return response;
    }

    [Function("CreateGuest")]
    public async Task<HttpResponseData> CreateGuest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "guests")] HttpRequestData req)
    {
        if (!AdminAuth.IsAuthorized(req, _adminApiKey))
        {
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var guest = await req.ReadFromJsonAsync<Guest>();
        if (guest is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        guest.Id = Guid.NewGuid().ToString();
        var created = await dataStore.UpsertGuestAsync(guest);

        var invitation = new Invitation { GuestId = created.Id };
        await dataStore.UpsertInvitationAsync(invitation);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(new { Guest = created, Invitation = invitation });
        return response;
    }

    [Function("UpdateGuest")]
    public async Task<HttpResponseData> UpdateGuest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "guests/{id}")] HttpRequestData req,
        string id)
    {
        if (!AdminAuth.IsAuthorized(req, _adminApiKey))
        {
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var existing = await dataStore.GetGuestAsync(id);
        if (existing is null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var guest = await req.ReadFromJsonAsync<Guest>();
        if (guest is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        guest.Id = id;
        var updated = await dataStore.UpsertGuestAsync(guest);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(updated);
        return response;
    }

    [Function("DeleteGuest")]
    public async Task<HttpResponseData> DeleteGuest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "guests/{id}")] HttpRequestData req,
        string id)
    {
        if (!AdminAuth.IsAuthorized(req, _adminApiKey))
        {
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        await dataStore.DeleteGuestAsync(id);
        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
