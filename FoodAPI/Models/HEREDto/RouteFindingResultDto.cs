namespace FoodAPI.Models.HEREDto;

public class RouteFindingResultDto
{
    public List<Route>? routes { get; set; }
}

public class Arrival
{
    public DateTime? time { get; set; }
    public Place? place { get; set; }
}

public class Departure
{
    public DateTime? time { get; set; }
    public Place? place { get; set; }
}

public class Location
{
    public double lat { get; set; }
    public double lng { get; set; }
}

public class OriginalLocation
{
    public double lat { get; set; }
    public double lng { get; set; }
}

public class Place
{
    public string? type { get; set; }
    public Location? location { get; set; }
    public OriginalLocation? originalLocation { get; set; }
}

public class Route
{
    public string? id { get; set; }
    public List<Section>? sections { get; set; }
}

public class Section
{
    public string? id { get; set; }
    public string? type { get; set; }
    public Departure? departure { get; set; }
    public Arrival? arrival { get; set; }
    public TravelSummary? travelSummary { get; set; }
    public Transport? transport { get; set; }
}

public class Transport
{
    public string? mode { get; set; }
}

public class TravelSummary
{
    public int duration { get; set; }
    public int length { get; set; }
    public int baseDuration { get; set; }
}