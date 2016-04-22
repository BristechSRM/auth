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
        [ new Claim(Constants.ClaimTypes.Subject, user.Id.ToString())
          new Claim(Constants.ClaimTypes.PreferredUserName, user.Email)
          new Claim(Constants.ClaimTypes.Email, user.Email)
          new Claim(Constants.ClaimTypes.GivenName, user.Firstname)
          new Claim(Constants.ClaimTypes.FamilyName, user.Surname) ]

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

        member __.AuthenticateLocalAsync(context) = 
            async {
                Log.Information("Entering Local login authentication test method")
                let email = context.UserName
                let password = context.Password
                let result = 
                    try
                        let userEntity = getUserByEmailAsync email
                        match userEntity with 
                        | Some user -> 
                            if user.TestPassword = password then
                                let claims = getClaimsFromUser user
                                AuthenticateResult(user.Id.ToString(),user.Firstname + user.Surname, claims)
                            else
                                 AuthenticateResult("There was error with the username or password")
                        | None -> AuthenticateResult("There was error with the username or password")
                    with
                        | :? ArgumentException -> AuthenticateResult("Account is not allowed to login. See Authentication server logs.")
                context.AuthenticateResult <- result
            } 
            |> Async.StartAsUnitTask
                
        member __.PostAuthenticateAsync(context) = Task.Factory.StartNew(fun () -> ())
        member __.PreAuthenticateAsync(context) = Task.Factory.StartNew(fun () -> ())
        member __.AuthenticateExternalAsync(context) = failwith "Not implemented yet"
        member __.SignOutAsync(context) = failwith "Not implemented yet"
