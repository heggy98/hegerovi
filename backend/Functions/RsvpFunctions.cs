using System.Net;
using Hegerovi.Backend.Models;
using Hegerovi.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Hegerovi.Backend.Functions;

public class RsvpFunctions(IDataStore dataStore, ILogger<RsvpFunctions> logger)
{
    [Function("GetRsvp")]
    public async Task<HttpResponseData> GetRsvp(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rsvp/{token}")] HttpRequestData req,
        string token)
    {
        var invitation = await dataStore.GetInvitationByTokenAsync(token);
        if (invitation is null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var guest = await dataStore.GetGuestAsync(invitation.GuestId);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            invitation.Status,
            invitation.PeopleCount,
            invitation.MenuPreference,
            invitation.Allergies,
            GuestName = guest?.Name,
            PlusOneAllowed = guest?.PlusOneAllowed ?? false
        });
        return response;
    }

    [Function("SubmitRsvp")]
    public async Task<HttpResponseData> SubmitRsvp(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "rsvp/{token}")] HttpRequestData req,
        string token)
    {
        var invitation = await dataStore.GetInvitationByTokenAsync(token);
        if (invitation is null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var body = await req.ReadFromJsonAsync<RsvpRequest>();
        if (body is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        invitation.Status = body.Attending ? InvitationStatus.Confirmed : InvitationStatus.Declined;
        invitation.PeopleCount = body.PeopleCount;
        invitation.MenuPreference = body.MenuPreference;
        invitation.Allergies = body.Allergies;
        invitation.RespondedAt = DateTimeOffset.UtcNow;

        await dataStore.UpsertInvitationAsync(invitation);
        logger.LogInformation("RSVP submitted for token {Token}: {Status}", token, invitation.Status);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { invitation.Status });
        return response;
    }
}
