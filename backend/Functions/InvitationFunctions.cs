using System.Net;
using Hegerovi.Backend.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace Hegerovi.Backend.Functions;

public class InvitationFunctions(IDataStore dataStore, IConfiguration configuration)
{
    private readonly string _adminApiKey = configuration["AdminApiKey"] ?? string.Empty;

    [Function("GetInvitations")]
    public async Task<HttpResponseData> GetInvitations(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invitations")] HttpRequestData req)
    {
        if (!AdminAuth.IsAuthorized(req, _adminApiKey))
        {
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var guests = await dataStore.GetGuestsAsync();
        var invitations = await dataStore.GetInvitationsAsync();

        var overview = invitations.Select(invitation => new
        {
            invitation.Id,
            invitation.Token,
            invitation.Status,
            invitation.PeopleCount,
            invitation.MenuPreference,
            invitation.Allergies,
            invitation.RespondedAt,
            GuestName = guests.FirstOrDefault(g => g.Id == invitation.GuestId)?.Name
        });

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(overview);
        return response;
    }
}
