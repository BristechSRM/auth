namespace Services

open IdentityServer3.Core
open IdentityServer3.Core.Models
open IdentityServer3.Core.Extensions
open IdentityServer3.Core.Services
open System
open System.Threading.Tasks
open System.Security.Claims
open Serilog

open Entities
open UserRepository

type AuthUserService() = 

    let getClaimsFromUser (user : AuthUserEntity)= 
        [ Claim(Constants.ClaimTypes.Subject, user.Id.ToString())
          Claim(Constants.ClaimTypes.PreferredUserName, user.Email)
          Claim(Constants.ClaimTypes.Email, user.Email)
          Claim(Constants.ClaimTypes.GivenName, user.Firstname)
          Claim(Constants.ClaimTypes.FamilyName, user.Surname) ]

    let processExistingUser email provider = 
        let userEntity = getUserByEmailAsync email
        match userEntity with
        | Some user -> 
            let claims = getClaimsFromUser user
            AuthenticateResult(user.Id.ToString(), user.Email,claims,provider,"external")
        | None -> 
            AuthenticateResult("Could not authenticate user")

    interface IUserService with        
        member __.GetProfileDataAsync(context) = 
            async { 
                let subject = context.Subject
                let requestedClaimTypes = context.RequestedClaimTypes
                let success, subjectId = Guid.TryParse <| subject.GetSubjectId()
                if success then 
                    let! userEntity = getUserAsync subjectId
                    match userEntity with
                    | Some user -> 
                        context.IssuedClaims <- user
                                                |> getClaimsFromUser
                                                |> List.filter (fun x -> Seq.contains x.Type requestedClaimTypes)
                                                |> List.toSeq
                        return ()
                    | None -> raise <| ArgumentException "Invalid subject identifier: No matching user."
                else raise <| ArgumentException "Subject identifier was not a valid Guid"
            }
            |> Async.StartAsUnitTask

        member __.IsActiveAsync(context) = 
            async {
                let subject = context.Subject
                let success, subjectId = Guid.TryParse <| subject.GetSubjectId()
                if success then 
                    let! userEntity = getUserAsync subjectId                    
                    match userEntity with
                    | Some _ -> context.IsActive <- true
                    | None -> context.IsActive <- false
                else context.IsActive <- false
            }
            |> Async.StartAsUnitTask
        
        member __.AuthenticateExternalAsync(context) =
            async {
                Log.Information("Entering external login authentication method")
                let externalIdentity = context.ExternalIdentity
                if not <| isNull externalIdentity then                    
                    let emailClaim = context.ExternalIdentity.Claims |> Seq.tryFind(fun claim -> claim.Type = Constants.ClaimTypes.Email)
                    match emailClaim with 
                    | Some claim -> 
                        context.AuthenticateResult <- processExistingUser claim.Value context.ExternalIdentity.Provider
                    | None -> 
                        context.AuthenticateResult <- AuthenticateResult("External User email is missing. Cannot perform login.")
                else 
                    raise (ArgumentNullException("ExternalIdentity"))
            }
            |> Async.StartAsUnitTask
                
        member __.AuthenticateLocalAsync(_) = Task.Factory.StartNew(fun () -> ())
        member __.PostAuthenticateAsync(_) = Task.Factory.StartNew(fun () -> ())
        member __.PreAuthenticateAsync(_) = Task.Factory.StartNew(fun () -> ())
        member __.SignOutAsync(_) = Task.Factory.StartNew(fun () -> ())
        
