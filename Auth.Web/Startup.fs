module Startup

open System
open System.Collections.Generic
open System.Configuration

open IdentityServer3.Core.Configuration
open IdentityServer3.Core.Models
open IdentityServer3.Core.Services

open Owin
open Services

open Owin.Security.AesDataProtectorProvider
open Microsoft.Owin.Security.Google


let configureIdentityProviders (app: IAppBuilder) (signIsAsType: string) =
  app.UseAesDataProtectorProvider()

  let id = ConfigurationManager.AppSettings.Item("googleClientId")
  let secret = ConfigurationManager.AppSettings.Item("googleClientSecret")

  let google = 
    new GoogleOAuth2AuthenticationOptions(
      SignInAsAuthenticationType = signIsAsType,
      ClientId = id,
      ClientSecret = secret)

  app.UseGoogleAuthentication(google) |> ignore


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

        let authenticationOptions = 
            new AuthenticationOptions(
                EnableLocalLogin = false,
                IdentityProviders = Action<_,_>(configureIdentityProviders))

        let options = 
          new IdentityServerOptions(
            Factory = factory, 
            RequireSsl = false,
            AuthenticationOptions = authenticationOptions)

        app.UseIdentityServer(options) |> ignore
