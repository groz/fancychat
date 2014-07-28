open Owin
open Microsoft.Owin
open EkonBenefits.FSharp.Dynamic

// Owin startup class for configuring needed dependencies
type Startup() =
    member this.Configuration(app: Owin.IAppBuilder): unit =
        app
            .UseCors(Cors.CorsOptions.AllowAll)
            .MapSignalR()
            .UseNancy() 
        |> ignore

// SignalR hub for realtime communication
type ChatHub() =
    inherit Microsoft.AspNet.SignalR.Hub()

    // NB! do not forget to specify types here, generating hub proxy will fail otherwise
    member this.Send(name: string, message: string): unit =
        this.Clients.All ? addMessage(name, message)

// Nancy module for serving HTTP requests
type ChatModule() as this =
    inherit Nancy.NancyModule()
    
    do
        this.Get.["/"] <- fun ctx -> box this.View.["Index"]

[<EntryPoint>]
let main argv = 
    
    let url = "http://*:8080"

    use webApp = Hosting.WebApp.Start<Startup> url

    printfn "Nancy+SignalR server started on %s..." url
    System.Console.ReadLine() |> ignore

    0 // return code