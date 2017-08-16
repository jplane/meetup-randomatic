
#r "Newtonsoft.Json"
#r "System.Configuration"

using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

static Random randomatic = new Random(Environment.TickCount);

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    var payload = await req.Content.ReadAsStringAsync();

    dynamic json = JsonConvert.DeserializeObject(payload);

    var targetDate = DateTime.Parse(json.date.ToString());

    var meetupName = ConfigurationManager.AppSettings["meetupName"];

    var key = ConfigurationManager.AppSettings["meetupKey"];

    var evts = await GetEvents(meetupName, key);

    var evt = evts.FirstOrDefault(e => e.date.Date == targetDate.Date);

    if (evt == null)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Unable to find event.");
    }

    var rsvps = await GetRsvps(evt.id, meetupName, key);

    var idx = randomatic.Next(0, evt.rsvps);

    return req.CreateResponse(HttpStatusCode.OK, new {
        text = rsvps[idx].name
    });
}

private static async Task<List<dynamic>> GetRsvps(string eventId, string meetupName, string key)
{
    var http = new HttpClient();

    http.BaseAddress = new Uri($"https://api.meetup.com/{meetupName}/");

    var resp = await http.GetAsync($"events/{eventId}/rsvps?response=yes&key={key}");

    var json = await resp.Content.ReadAsStringAsync();

    dynamic rsvps = JsonConvert.DeserializeObject(json);

    var results = new List<dynamic>();

    foreach (var rsvp in rsvps)
    {
        results.Add(new
        {
            id = rsvp.member.id,
            name = rsvp.member.name
        });
    }

    return results;
}


private static async Task<IEnumerable<dynamic>> GetEvents(string meetupName, string key)
{
    var http = new HttpClient();

    http.BaseAddress = new Uri($"https://api.meetup.com/{meetupName}/");

    var resp = await http.GetAsync($"events?status=past,upcoming&key={key}");

    var json = await resp.Content.ReadAsStringAsync();

    dynamic events = JsonConvert.DeserializeObject(json);

    var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    var evts = new List<dynamic>();

    foreach (var evt in events)
    {
        evts.Add(new
        {
            id = (string) evt.id,
            name = evt.name,
            rsvps = (int) evt.yes_rsvp_count,
            date = epochStart.AddMilliseconds((double)evt.time)
        });
    }

    return evts;
}
