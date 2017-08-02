using System;

private static readonly string subscriptionKey = Env("SubscriptionKey");

public static void Run(Stream input, Stream output)
{
}

private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);