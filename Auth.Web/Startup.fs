module Startup

open System
open System.Collections.Generic
open System.Configuration
open System.Security.Cryptography.X509Certificates

open IdentityServer3.Core.Configuration
open IdentityServer3.Core.Models
open IdentityServer3.Core.Services

open System.IO
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
        let clientSecrets = [ Secret("98A3BE08-7F60-47AA-A9EA-49DA00DC388F".Sha256()) ]
        
        let scopes = 
            [ StandardScopes.OpenId
              StandardScopes.Profile
              Scope(Name = "api") ]

        let scopeNames =
            scopes |> Seq.map(fun s -> s.Name)
        
        let clientCorsOrigins = ["http://srm.bris.tech"; "http://localhost:8080"]
        let clientRedirectUris = ["http://srm.bris.tech/signed-in"; "http://localhost:8080/signed-in"]
        let clientPostLogoutUris = ["http://srm.bris.tech"; "http://localhost:8080"]        

        let clients = 
            [ Client(ClientName = "Bristech SRM", 
                ClientId = "bristechsrm", 
                Enabled = true, 
                AccessTokenType = AccessTokenType.Reference, 
                Flow = Flows.Implicit, 
                ClientSecrets = List<Secret>(clientSecrets), 
                AllowedScopes = List<string>(scopeNames),
                AllowedCorsOrigins = List<string>(clientCorsOrigins),
                RedirectUris = List<string>(clientRedirectUris),
                PostLogoutRedirectUris = List<string>(clientPostLogoutUris)) ]

        let factory = 
            let factory = IdentityServerServiceFactory().UseInMemoryClients(clients).UseInMemoryScopes(scopes)
            factory.UserService <- Registration<IUserService, AuthUserService>()            
            factory

        let authenticationOptions = 
            new AuthenticationOptions(
                EnableLocalLogin = false,
                IdentityProviders = Action<_,_>(configureIdentityProviders))

        let getEmbeddedCertificate() = 
            use stream = __.GetType().Assembly.GetManifestResourceStream("idsrv3test.pfx")            
            let mutable buffer = Array.zeroCreate <| int(stream.Length)
            stream.ReadAsync(buffer, 0, buffer.Length) 
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore
            X509Certificate2(buffer, "idsrv3test")

        let options = 
          new IdentityServerOptions(
            SiteName = "Bristech SRM",
            SigningCertificate = getEmbeddedCertificate(),
            Factory = factory, 
            RequireSsl = false,
            AuthenticationOptions = authenticationOptions)

        app.UseIdentityServer(options) |> ignore
