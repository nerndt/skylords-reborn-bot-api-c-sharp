
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Api
{
    public interface IAspWrapperImplNew
    {
        string Name { get; }
        Deck[] DecksForMap(Maps map, string? name, UInt64 crc);
        void PrepareForBattle(Maps map, string? name, UInt64 crc, Deck deck);
        void MatchStart(GameStartState state);
        Command[] Tick(GameState state);
    }

    public class AspWrapperNew
    {
        public static void Run(string[] args, List<IAspWrapperImpl> implementations)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.MapPost("hello", (ApiHello hello) =>
            {
                if (hello.Version != ApiVersion.VERSION)
                {
                    return Results.UnprocessableEntity();
                }

                var name = hello.Map.CommunityMapDetails?.Name;
                var crc = hello.Map.CommunityMapDetails?.Crc ?? 0;

                foreach (var implementation in implementations)
                {
                    var decks = implementation.DecksForMap(hello.Map.Map, name, crc);

                    IResult resultTemp = Results.Ok(new AiForMap()
                    {
                        Name = implementation.Name,
                        Decks = decks
                    });
                }
                return Results.Ok(); 
            })
            .WithName("hello")
            .WithOpenApi();

            app.MapPost("prepare", (Prepare prepare) =>
            {
                var name = prepare.MapInfo.CommunityMapDetails?.Name;
                var crc = prepare.MapInfo.CommunityMapDetails?.Crc ?? 0;

                foreach (var implementation in implementations)
                {
                    var decks = implementation.DecksForMap(prepare.MapInfo.Map, name, crc);

                    var deck = decks.Single(d => d.Name == prepare.Deck);

                    implementation.PrepareForBattle(prepare.MapInfo.Map, name, crc, deck);
                }

                return Results.Ok();
            })
            .WithName("prepare")
            .WithOpenApi();

            app.MapPost("start", (GameStartState start) =>
            {
                foreach (var implementation in implementations)
                {
                    implementation.MatchStart(start);
                }
                return Results.Ok();
            })
            .WithName("start")
            .WithOpenApi();

            app.MapPost("tick", (GameState state) =>
            {
                foreach (var implementation in implementations)
                {
                    var commands = implementation.Tick(state);
                    commands.Select(c => new CommandHolder(c));
                }
                return Results.Ok();
            })
            .WithName("tick")
            .WithOpenApi();
            
            app.MapGet("end", () =>
            {
                return Results.Ok();
            })
            .WithName("end")
            .WithOpenApi();


            app.Run();
        }
    }
}
