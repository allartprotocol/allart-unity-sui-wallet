using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageForEventAndEventID
{
    public List<EventPage> data;
}

public class EventPage {
    public string packageId;
    public string transactionModule;
    public string sender;
    public string type;
    public string timestampMs;
}
