using System.Xml.Serialization;

namespace EventMinApi;

[XmlRoot("Event")] // Matches the root element of a single event if parsed individually
public class Event
{
    [XmlElement("Id")]
  public string Id { get; set; } = string.Empty;

  [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("Location")]
    public string Location { get; set; } = string.Empty;
}

// Wrapper class if the external server returns a list of events
[XmlRoot("Events")]
public class EventCollection
{
    [XmlElement("Event")]
    public List<Event> Events { get; set; } = new();
}
