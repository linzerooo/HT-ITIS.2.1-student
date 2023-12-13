module Hw6.App

open System
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Net.Http

type MaybeBuilder() =
    member builder.Bind(a, f): Result<'e,'d> =
        match a with
        | Ok x -> f x
        | Error e -> Error e
    member builder.Return x: Result<'a,'b> = Ok x
let maybe = MaybeBuilder()

[<CLIMutable>]
type calcArgs = {
    value1 : double
    operation: string
    value2: double    
}

let parseOperation operation =
    match operation with
    | "Plus" -> "+"
    | "Minus" -> "-"
    | "Multiply" -> "*"
    | "Divide" -> "/"
    | _ -> operation

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let calculate (value1: double, operation: string, value2:double) =
    match operation with
    | "+" -> Ok $"{value1 + value2}"
    | "-" -> Ok $"{value1 - value2}"
    | "*" -> Ok $"{value1 * value2}"
    | "/" ->
        match value2 =0 with
        | true -> Ok "DivideByZero"
        | false -> Ok $"{value1 / value2}"
    | _ -> Error $"Could not parse value '{operation}'"
    

let sendRequestAsync (client: HttpClient) (url: string) =
    async {
        let! response = Async.AwaitTask (client.GetAsync url)
        let! result = Async.AwaitTask (response.Content.ReadAsStringAsync())
        return result
    }
    
let calculatorHandler: HttpHandler =
    fun next ctx ->
        let result = maybe{
            let! args = ctx.TryBindQueryString<calcArgs>()
            let! parsed = calculate (args.value1, parseOperation args.operation, args.value2)
            return parsed
        }   
        match result with
        | Ok ok -> (setStatusCode 200 >=> text (ok.ToString())) next ctx
        | Error error -> (setStatusCode 400 >=> text error) next ctx

let webApp =
    choose [
        GET >=> choose [
            route "/calculate" >=> calculatorHandler
            route "/" >=> text "Use //calculate?value1=<VAL1>&operation=<OPERATION>&value2=<VAL2>"
        ]
        setStatusCode 404 >=> text "Not Found" 
    ]
    
type Startup() =
    member _.ConfigureServices (services : IServiceCollection) = 
        services.AddGiraffe().AddMiniProfiler(fun option -> option.RouteBasePath <- "/profiler")  |> ignore

    member _.Configure (app : IApplicationBuilder) (_ : IHostEnvironment) (_ : ILoggerFactory) =
        app.UseMiniProfiler().UseGiraffe webApp
        
[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun whBuilder -> whBuilder.UseStartup<Startup>() |> ignore)
        .Build()
        .Run()
    0
    
    
