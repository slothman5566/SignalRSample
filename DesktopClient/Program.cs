using Microsoft.AspNetCore.SignalR.Client;

var url = "https://localhost:7255";


Console.WriteLine("Login:");
var userId = Console.ReadLine();
var client = new HttpClient();
var connection = new HubConnectionBuilder()
    .WithUrl($"{url}/sampleHub", options =>
    {
        options.AccessTokenProvider = async () =>
        {
            return await client.GetStringAsync($"{url}/jwt?user={userId}");
        };

    })
    .Build();

await connection.StartAsync();

connection.On<string>("ReceiveMessage", (message) =>
{
    Console.WriteLine($"{message}");
});

connection.On<string>("ReceiveMessage", (message) =>
{
    Console.WriteLine($"{message}");
});

connection.On<string>("TimedService", (message) =>
{
    Console.WriteLine($"TimedService :{message}");
});


Console.WriteLine("Press Ctrl+C to exit.");

while (true)
{
    var userInput = Console.ReadLine();
    await connection.InvokeAsync("SendMessageToServer", userInput);

}

