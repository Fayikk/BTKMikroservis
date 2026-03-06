using ProductGraphQLService.GraphQL;

var builder = WebApplication.CreateBuilder(args);



builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()     
    .AddSorting()       
    .AddProjections(); 




var app = builder.Build();

app.MapGraphQL();
app.MapGet("/", () => 
    Results.Redirect("/graphql"));

app.Run();

