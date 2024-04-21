namespace F__api_and_example

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting


module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let bot_impl_to_use = example_bot.Impl


        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers().AddJsonOptions(fun options -> 
            options.JsonSerializerOptions.Converters.Add(new Types.CardIdConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.SquadIdConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.BuildingIdConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.SpellIdConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.AbilityIdConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.ModeIdConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.EntityIdConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.TickConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.TickCountConverter())
            
            options.JsonSerializerOptions.Converters.Add(new Types.AbilityEffectSpecificConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.AspectConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.SingleTargetConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.TargetConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.JobConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.CommandConverter())
            options.JsonSerializerOptions.Converters.Add(new Types.CommandRejectionReasonConverter())
        ) |> ignore

        builder.Services.AddSingleton(typeof<Controllers.BotImpl>, bot_impl_to_use) |> ignore

        builder.Services.AddControllers() |> ignore
        builder.Services.AddEndpointsApiExplorer() |> ignore
        builder.Services.AddSwaggerGen() |> ignore

        let app = builder.Build()

        if app.Environment.IsDevelopment()
        then
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore


        app.UseAuthorization() |> ignore
        app.MapControllers() |> ignore

        app.Run()

        exitCode
