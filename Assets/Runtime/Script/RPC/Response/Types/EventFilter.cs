using System.Collections.Generic;

public class EventQuery {
    public List<object> query;

    public EventQuery(List<object> query)
    {
        this.query = query;
    }
}

public class SenderFilter
{
    public SenderFilter(string address)
    {
        Sender = address;
    }

    public string Sender;
}

public class RecipientFilter
{
    public RecipientFilter(string address)
    {
        Recipient = new AddressOwnerFilter { AddressOwner = address };
    }
    public AddressOwnerFilter Recipient;
}

public class AddressOwnerFilter
{
    public string AddressOwner;
}

public class EventFilter
{
    public string jsonrpc;
    public int id;
    public string method;
    public List<object> @params;

    public EventFilter(string method, List<object> @params)
    {
        this.jsonrpc = "2.0";
        this.id = 1;
        this.method = method;
        this.@params = @params;
    }
}

public class EventResponse
{
    public string jsonrpc;
    public int id;
    public long result;
}

public class FilterAnd
{
    public List<object> And;
}

public class FilterOr
{
    public List<object> Or;

    public FilterOr(List<object> or)
    {
        Or = or;
    }

    public void Add(object obj)
    {
        Or.Add(obj);
    }
}