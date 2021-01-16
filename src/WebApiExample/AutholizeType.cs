using System;

namespace WebApiExample
{
    public enum AutholizeType
    {
        None,
        SeasonPass,
        Subscriber,
    }
 
    [AttributeUsage( AttributeTargets.Method)]
    public class UseIdempotent : System.Attribute
    {

    }
}
