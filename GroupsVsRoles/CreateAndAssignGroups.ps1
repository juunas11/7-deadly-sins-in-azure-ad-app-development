$account = Connect-AzureAD

$user = Get-AzureADUser -Filter "userPrincipalName eq '$($account.Account)'"
$userId = $user.ObjectId

$groupCount = 500

for ($i = 0; $i -lt $groupCount; $i++){
    $group = New-AzureADGroup -DisplayName "Test Group $i" -SecurityEnabled $true -MailEnabled $false -MailNickName "testgroup$i"
    Add-AzureADGroupMember -ObjectId $group.ObjectId -RefObjectId $userId
}