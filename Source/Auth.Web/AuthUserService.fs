module AuthUserService

open System
open Amazon
open Amazon.DynamoDBv2
open Amazon.DynamoDBv2.DataModel
open Amazon.DynamoDBv2.DocumentModel
open Serilog
open Entities

let client = new AmazonDynamoDBClient(RegionEndpoint.EUWest1)
let context = new DynamoDBContext(client)

let getUsers() = 
    Log.Information("Accessing DynamoDb for AuthUsers")
    context.Scan<AuthUserEntity>()

let createUser firstName surName email = 
    let user = 
        { Id = Guid.NewGuid()
          Firstname = firstName
          Surname = surName
          Email = email }
    
    let userBatch = context.CreateBatchWrite<AuthUserEntity>()
    userBatch.AddPutItem(user)
    Log.Information("Executing user batch write")
    userBatch.Execute()
    let newUser = context.Load<AuthUserEntity>(user.Id)
    newUser
