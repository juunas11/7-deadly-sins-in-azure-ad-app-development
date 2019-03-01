# Storing secrets in version control

This sample contains two projects:

* SecretsInVcs: stores secret in config file
* SecretsInVcsDoneBetter: stores secret in Azure Key Vault

The sample app here is a console app meant to sync data from EmployeeApi to another data store.
It needs a client secret to acquire an access token to call the API.
In the first project the secret is in the appsettings.json file.
This is usually not a good idea.

The configuration only uses the JSON file:

```cs
private static IConfiguration BuildConfig()
{
    // We use an appsettings.json file as the configuration source
    // It should not be the place for secrets as it is in this case
    var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    return builder.Build();
}
```

The second project does it better by fetching additional configuration from an Azure Key Vault.
It uses the logged-in user's credentials from Visual Studio (Tools/Options/Azure Service Authentication),
or it can use the logged-in user of the AZ CLI.
If deployed to Azure, the same code could be used to utilize a Managed Identity.

To test this you'll need to:

* Create an Azure Key Vault
* (optional) Create a group in Azure AD and assign the user(s) in it
* Assign the user(s)/group to an access policy in Key Vault (secrets list + get)
* Add the client secret as a secret in Key Vault with name *ClientSecret*

The configuration builder is only slightly different:

```cs
private static IConfiguration BuildConfig()
{
    var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    // Build config so we can get KeyVault URL from config
    var config = builder.Build();
    // Add KeyVault as configuration source
    var keyVaultUrl = config["KeyVaultUrl"];
    builder.AddAzureKeyVault(keyVaultUrl);
    return builder.Build();
}
```

Don't put secrets in version control,
it increases the amount of people with access to the secrets.
Usually many of those people do not need the access.

## References

* [ASP.NET Core + Azure Key Vault + Azure AD MSI = Awesome way to do config](https://joonasw.net/view/aspnet-core-azure-keyvault-msi)
* [Azure AD Managed Service Identity](https://joonasw.net/view/azure-ad-managed-service-identity)
* [Azure Command-Line Interface (CLI)](https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest)