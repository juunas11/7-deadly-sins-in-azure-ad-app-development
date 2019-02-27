# Using ROPC flow for login

I do not recommend anyone to use this flow in the manner it is used here.
This flow has few really valid use cases, and using it to bypass the Microsoft login page is not of them.
Please, do not do this.

This sample shows how an app could try to bypass the Azure AD login page and implement its own.
The user inputs their credentials and the app logs the user in using the
Resource Owner Password Credentials grant flow.
It then calls the EmployeeApi on the user's behalf.

It does work for some users,
but still is generally bad for security/privacy + it teaches users to fall for phishing.

This login does not work if the user:

* Has MFA
* Has an expired password
* Is a federated account

Do not use the ROPC flow in new apps.