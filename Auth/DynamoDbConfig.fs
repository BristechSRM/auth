module DynamoDbConfig

open Amazon
open Amazon.DynamoDBv2

open System
open System.Configuration

let config = 
    let serviceUrl = ConfigurationManager.AppSettings.Item("LocalDynamoDbUrl")
    if String.IsNullOrWhiteSpace serviceUrl then
        new AmazonDynamoDBConfig(RegionEndpoint = RegionEndpoint.EUWest1)
    else 
        new AmazonDynamoDBConfig(RegionEndpoint = RegionEndpoint.EUWest1, ServiceURL = serviceUrl)
