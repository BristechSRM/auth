module Startup

open IdentityServer3.Core.Configuration
open IdentityServer3.Core.Models
open IdentityServer3.Core.Services
open IdentityServer3.Core.Services.InMemory
open Owin
open Services
open System.Collections.Generic

type Startup() = 
    member __.Configuration(app : IAppBuilder) = 
        let clientSecrets = [ new Secret("".Sha256()) ]
        let scopeNames = [ "api" ]
        
        let scopes = 
            [ StandardScopes.OpenId
              StandardScopes.Profile
              new Scope(Name = "api") ]
        
        let clients = 
            [ new Client(ClientName = "Bristech SRM", 
                ClientId = "bristechsrm", 
                Enabled = true, 
                AccessTokenType = AccessTokenType.Reference, 
                Flow = Flows.Implicit, 
                ClientSecrets = new List<Secret>(clientSecrets), 
                AllowedScopes = new List<string>(scopeNames)) ]

        let factory = 
            let factory = IdentityServerServiceFactory().UseInMemoryClients(clients).UseInMemoryScopes(scopes)
            factory.UserService <- new Registration<IUserService, AuthUserService>()
            factory

        let options = new IdentityServerOptions(Factory = factory, RequireSsl = false)
        app.UseIdentityServer(options) |> ignore
