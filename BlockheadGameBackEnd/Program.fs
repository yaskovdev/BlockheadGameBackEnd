namespace BlockheadGameBackEnd

#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddCors(fun options -> 
            options.AddPolicy("AllowAll", fun builder -> 
                 builder.AllowAnyHeader()
                        .AllowAnyOrigin()
                        .WithMethods("GET", "POST") |> ignore))

        builder.Services.AddControllers()

        let app = builder.Build()

        app.UseHttpsRedirection()
        
        app.UseCors("AllowAll");

        app.UseAuthorization()
        app.MapControllers()

        app.Run()

        exitCode
