var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.JiuLing_Platform>("jiuling-platform");

builder.Build().Run();
