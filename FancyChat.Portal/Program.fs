open Owin
open Microsoft.Owin.Hosting
open Microsoft.Owin.Cors
open EkonBenefits.FSharp.Dynamic
open Nancy

// Owin startup file configuring needed dependencies
type Startup() =
    member this.Configuration(app: Owin.IAppBuilder): unit =
        app
            .UseCors(CorsOptions.AllowAll)
            .MapSignalR()
            .UseNancy() 
        |> ignore

// SignalR hub to serve realtime requests
type ChatHub() =
    inherit Microsoft.AspNet.SignalR.Hub()

    member this.Send(name, message): unit =
        this.Clients.All ? addMessage(name, message)

// Nancy module to serve HTTP responses
type ChatModule() as this =
    inherit NancyModule()
    
    do
        this.Get.["/"] <- fun ctx -> box "Hello, Nancy!"

[<EntryPoint>]
let main argv = 
    
    let url = "http://localhost:8080"

    use webApp = WebApp.Start<Startup> url
    printfn "Nancy+SignalR server started on %s..." url
    System.Console.ReadLine() |> ignore

    0