using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Pusula_Training_HealthCare_Blazor>("HealthCare-UI", launchProfileName: "Pusula.Training.HealthCare.Blazor");

builder.Build().Run();
