var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Basket_Service>("basketservice", launchProfileName: "https");
builder.AddProject<Projects.Identity_Service>("idetityservice", launchProfileName: "https");
builder.AddProject<Projects.Inventory_Service>("inventoryservice", launchProfileName: "https");
builder.AddProject<Projects.Ocelot_Gateway>("apigateway", launchProfileName: "https");

builder.Build().Run();