module Startup

open IdentityServer3.Core.Configuration
open IdentityServer3.Core.Models
open IdentityServer3.Core.Services
open Owin
open Services
open System.Collections.Generic

type Startup() = 
    member __.Configuration(app : IAppBuilder) = 
        let clientSecrets = [ Secret("".Sha256()) ]
        let scopeNames = [ "api" ]
        
        let scopes = 
            [ StandardScopes.OpenId
              StandardScopes.Profile
              Scope(Name = "api") ]
        
        let clients = 
            [ Client(ClientName = "Bristech SRM", 
                ClientId = "bristechsrm", 
                Enabled = true, 
                AccessTokenType = AccessTokenType.Reference, 
                Flow = Flows.Implicit, 
                ClientSecrets = List<Secret>(clientSecrets), 
                AllowedScopes = List<string>(scopeNames)) ]

        let factory = 
            let factory = IdentityServerServiceFactory().UseInMemoryClients(clients).UseInMemoryScopes(scopes)
            factory.UserService <- Registration<IUserService, AuthUserService>()
            factory

        let options = IdentityServerOptions(Factory = factory, RequireSsl = false)
        app.UseIdentityServer(options) |> ignore
