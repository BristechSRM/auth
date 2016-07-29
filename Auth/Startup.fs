module Startup

open System
open System.Collections.Generic
open System.Configuration
open System.Security.Cryptography.X509Certificates
open System.IO
open IdentityServer3.Core.Configuration
open IdentityServer3.Core.Models
open IdentityServer3.Core.Services
open Owin
open Owin.Security.AesDataProtectorProvider
open Microsoft.Owin.Security.Google
open Config
open Services


let configureIdentityProviders (app: IAppBuilder) (signIsAsType: string) =

  let id = Config.getString "googleClientId"
  let secret = Config.getString "googleClientSecret"

  let google = 
    new GoogleOAuth2AuthenticationOptions(
      SignInAsAuthenticationType = signIsAsType,
      ClientId = id,
      ClientSecret = secret)

  app.UseGoogleAuthentication(google) |> ignore


type Startup() = 
    member __.Configuration(app : IAppBuilder) = 

        let frontendUrl = Config.getString "FrontEndUrl"
        let signedInUrl = frontendUrl + "/signed-in"

        let clientSecrets = []
        
        let scopes = [ Scope(Name="api", DisplayName="SRM Administration API", Description="Bristech SRM Administration") ]

        let scopeNames =
            scopes |> Seq.map(fun s -> s.Name)
        
        let clientCorsOrigins = [ frontendUrl ]
        let clientRedirectUris = [ signedInUrl ]
        let clientPostLogoutUris = [ frontendUrl ]

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
            let file = Config.getString "certificateFile"
            let pwd = Config.getString "certificatePassword"
            
            use stream = new FileStream(file, FileMode.Open)
            let buffer = Array.zeroCreate <| int(stream.Length)
            stream.ReadAsync(buffer, 0, buffer.Length) 
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore

            X509Certificate2(buffer, pwd)

        let options = 
          new IdentityServerOptions(
            SiteName = "Bristech SRM",
            SigningCertificate = getEmbeddedCertificate(),
            Factory = factory, 
            RequireSsl = false,
            AuthenticationOptions = authenticationOptions)

        app.UseAesDataProtectorProvider()
            
        app.UseIdentityServer(options) |> ignore
