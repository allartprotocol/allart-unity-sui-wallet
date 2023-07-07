using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputObjectFilter{
    public InputObjectFilter(string inputObject)
    {
        ChangedObject = inputObject;
    }

    public string ChangedObject { get; set; }
}

public class FromAndToAddressFilter {

    public FromAndToAddressFilter(FromToObject fromAndToAddress)
    {
        FromAndToAddress = fromAndToAddress;
    }

    public FromToObject FromAndToAddress;
}

public class FromOrToAddressFilter {

    public FromOrToAddressFilter(FromOrObject fromAndToAddress)
    {
        FromOrToAddress = fromAndToAddress;
    }

    public FromOrObject FromOrToAddress;
}

public class FromOrObject
{
    public FromOrObject(string addr)
    {
        this.addr = addr;
    }
    public string addr { get; set; }
}

public class Filter{
    public Filter(object address)
    {
        this.filter = address;
    }
    public object filter;
}


public class FromAddresObject {
    public FromAddresObject(FromAddressFilter from)
    {
        this.FromAddress = from;
    }
    public FromAddressFilter FromAddress { get; set; }
}

public class ToAddresObject
{
    public ToAddresObject(ToAddressFilter to)
    {
        this.ToAddress = to;
    }
    public ToAddressFilter ToAddress { get; set; }
}

public class FromAddressFilter
{
    public FromAddressFilter(string from)
    {
        this.FromAddress = from;
    }

    public string FromAddress;
}

public class ToAddressFilter
{
    public ToAddressFilter(string from)
    {
        this.ToAddress = from;
    }

    public string ToAddress;
}

public class FromToObject{

    public FromToObject(string from, string to)
    {
        this.from = from;
        this.to = to;
    }
    public string from { get; set; }
    public string to { get; set; }
}