﻿namespace ApiAuthDemo.Infrastructure.BasicAuth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicAuthAttribute : TypeFilterAttribute
{
    public BasicAuthAttribute(string realm = "My Realm") : base(typeof(BasicAuthFilter))
    {
        Arguments = [realm];
    }
}