using Microsoft.AspNetCore.SignalR.Client;
using System;
var url = "https://localhost:7255/sampleHub";
var connection = new HubConnectionBuilder()
    .WithUrl(url)
    .Build();

await connection.StartAsync();

connection.On<string>("ReceiveMessage", ( message) =>
{
    Console.WriteLine($"{message}");
});



Console.WriteLine("Press Ctrl+C to exit.");

while (true)
{
    var userInput = Console.ReadLine();
    await connection.InvokeAsync("SendMessageToServer", userInput);

}

