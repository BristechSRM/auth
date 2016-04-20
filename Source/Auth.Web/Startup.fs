module Startup

open System.Collections.Generic

open IdentityServer3.Core.Models;
open IdentityServer3.Core.Configuration
open IdentityServer3.Core.Services.InMemory

open Owin

type Startup() = 
   member __.Configuration (app: IAppBuilder) =
      let clientSecrets = [ new Secret("".Sha256()) ]
      let scopeNames = [ "api" ]
      let scopes = 
        [ StandardScopes.OpenId;
          StandardScopes.Profile; 
          new Scope(Name = "api");          
        ]        

      let clients = 
        [ new Client(
              ClientName = "Bristech SRM",
              ClientId = "bristechsrm",
              Enabled = true,
              AccessTokenType = AccessTokenType.Reference,
              Flow = Flows.Implicit,
              ClientSecrets = new List<Secret>(clientSecrets),
              AllowedScopes = new List<string>(scopeNames)
        ) ]
            
      let users = 
        [ new InMemoryUser(
              Username = "admin",
              Password = "admin",
              Subject = "1"
      ) ]

      let factory = 
        IdentityServerServiceFactory()
            .UseInMemoryClients(clients)
            .UseInMemoryScopes(scopes)
            .UseInMemoryUsers(new List<InMemoryUser>(users))

      let options = new IdentityServerOptions(Factory = factory, RequireSsl = false)

      app.UseIdentityServer(options) |> ignore
