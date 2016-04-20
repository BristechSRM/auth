module Entities

open System
open Amazon.DynamoDBv2.DataModel

[<DynamoDBTable("Auth.Users")>]
[<CLIMutable>]
type AuthUserEntity = 
    { Id : Guid
      Email : string
      Firstname : string
      Surname : string }
