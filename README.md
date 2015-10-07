# AspNet.JWTAuthServer

The intention for this project was **LEARNING** ... and the need of a lightweight yet somehow 
"powerful" authentication server written in C# that could stand alone, issuing JWTs using the 
[OAuth 2.0 Resource Owner Password Credential Flow](http://oauthlib.readthedocs.org/en/latest/oauth2/grants/password.html) 
which can then be used for user authentication in a SPA that consumes a secured WebAPI. It 
should use ASP.NET Identity, be a self-hosted OWIN WebApi - not depending on IIS - and have 
the capability to be easily turned into a Windows Service with a small programming effort. 
Also the underlying database should be easily interchangable and not "nailed" to an MS SQL 
Server.

## The code used in this solution 

The AspNet.JWTAuthServer uses, adapts and combines code written by Taiseer Joudeh, found in the following 
gitHub repositories:
  * [tjoudeh/AngularJSAuthentication](https://github.com/tjoudeh/AngularJSAuthentication)
  * [tjoudeh/AspNetIdentity.WebApi](https://github.com/tjoudeh/AspNetIdentity.WebApi)

## The main features of this package

  * Multi-client capable
  * Ready to use for a "decoupled" or "integrated" scenario
  * Issuing JWT tokens
  * API endpoints for
    * Login (also external with limitations)
    * Register (also external with limitations) and send confirmation email
    * Confirm registration (from email link)
  * Infrastructure for user management:
    * Users
    * User roles
    * User claims
    * External logins _(External login is not yet functional in decoupled scenarios!)_
    * Clients
  * Extended authorization attribute for securing API endpoints

# Putting it together

**Before we start the little journey: It is highly recommended to read the articles by Taiseer Joudeh to understand all the techniques, put together in this project!**  

## The ASP.NET Identity Storage Provider for NPoco

Inspired by the [brilliant articles on authentication](http://bitoftech.net/category/asp-net-identity/)
written by [Taiseer Joudeh](https://twitter.com/TJoudeh), the base of all this was firstly 
implemented: An [ASP.NET Identity Storage Provider](https://github.com/miseeger/AspNet.Identity.NPoco) 
which uses [NPoco ORM](https://github.com/schotime/NPoco) by [Adam Schröder](https://twitter.com/schotime). 
Entity Framework has a quite large overhead and the plan was to have a handy and lightweight 
ORM that already supports various databases and so NPoco came into play.


## The extended ASP.NET Identity Storage Provider for NPoco

Having the implementation of the Identity Storage Provider for NPoco done, the self-hosted 
authentication server was implemented, using ASP.NET Identity and OWIN. The Web API provided 
by the server issues JWT tokens for authentication and is 'multi-client capable'. To achieve 
this, the ASP.NET Identity storage provider for NPoco was extended in order to handle 
multiple clients. It was integrated as a component of the JWTAuthServer solution (project 
AspNet.IdentityEx.NPoco). This implementation currently uses an 
[SQLite](https://www.sqlite.org/) database which is alredy created and populated with some 
data you need to get the system (demo) working. There's also a create-script for an empty 
database. It may be executed on other SQL servers because it's quite SQL92-ish.

## The stand-alone JWT issuing authentication server

The next step was to have the auth server working in a decoupled manner so that the server 
stands alone for issuing JWTs, requested by the various clients. These clients have access 
to their secured Web APIs (resourcess) which run separately. This was easily done because 
of the ability to handle multiple clients only some configuration work has to be done. 
It'll be described in the chapter on putting the server and its environment into operation. 

## The separated resource API and its AngularJS client

To demonstrate the decoupled scenario, another self-hosted micro ASP.NET OWIN WebAPI was 
created. It provides a secured resource for the MicroERP AngularJS demo client having 
only one endpoint. But that is all that was needed to show how to secure WebAPI 
endpoints. 

# Putting it into operation

First off grab the sourcecode as a [zip file](https://github.com/miseeger/AspNet.JWTAuthServer/archive/master.zip)
or [clone the repository](github-windows://openRepo/https://github.com/miseeger/AspNet.JWTAuthServer). 
Then open it in Visual Studio and restore the NuGet packages. The solution was developed 
with the VS2015 Community Edition. When done, then rebuild the solution.

Copy the SQLite database located in the **Data** (solution) folder to a local directory in 
your system and set the connection string in the app.config file of the AspNet.JWTAuthServer 
project, accordingly:

```xml
    <connectionStrings>
      <add name="IdentityDb" connectionString="Data Source=d:\mydatabasedir\Identity.db3;FKSupport=True" providerName="System.Data.SQLite" />
    </connectionStrings>
```

Make the projects **AspNet.JWTAuthServer** and **MicroErpApi** the startup projects and start 
the solution. Two console windows will pop up. One of them is the self-hosted JWTServer 
WebAPI and the other one is the resource API for the demo MicroERP.

Now to start the MicroERP AngularJS client which is contained in the MicroErp.Web folder 
of the solution. The prequisite for this is an IIS Express Server to be installed. The 
client comes with an `iisExpress.config` file and a batch file (`runapp.cmd`) to get it 
startet. You just have to set the correct path to the code:

#### iisExpress.config:
```xml
    <!-- Please keep the port. It's needed for security chekcs on allowed origins for API calls. -->
    <site name="MicroERP" id="1">
      <application path="/">
        <virtualDirectory path="/" physicalPath="d:\the\path\to\MicroERP.Web\" />
      </application>
      <bindings>
        <binding protocol="http" bindingInformation="*:9995:localhost" />
      </bindings>
    </site>  
```

##### runapp.cmd:
```
    "C:\Program Files (x86)\IIS Express\iisexpress" /config:d:\the\path\to\MicroErp.Web\iisexpress.config /site:MicroERP
```

In order to generate confirmation emails for a newly registered user, the system uses a 
simple SMTP mailer service. To test the emails in your environment, it's recommended to 
install [SMTP4DEV](http://smtp4dev.codeplex.com/)
a brilliant tool which intercepts outgoing emails so that you're able to check them. Using SMTP4DEV, a 
functinal SMTV sever is not needed. For this, the SMTP service is configured right out of the box. 

When all settings and installations are done, start the runapp.cmd and navigate to **http://localhost:9995** ...

The Login password for the demo(!) user "Admin" is _!Admin~P@ss!_ 

## app.config files

Despite of the client registration made in the `Identity` database, the WebAPI applications also have to
be configured. Here are the provided keys and their short description: 

### AspNet.JWTAuthServer

| Property                                     | Description                                                         | Example                 |
| :------------------------------------------- | :------------------------------------------------------------------ | :---------------------- |
| **JWTServer.Url**                            | Url with a placeholder for the port                                 | `http://localhost:{0}`  |  
| **JWTServer.AlternativeUrl**                 | Alternative Url notation, also with placeholder                     | `http://127.0.0.1:{0}`  |
| **JWTServer.MultiUrl**                       | Customizable Url                                                    | `http://{0}:{1}`        |
| **JWTServer.Port**                           | Port number for the hosted API                                      | `9999`                  |
| **JWTServer.AllowedOrigins**                 | Allowed Origins for the CORS middleware (separated by comma)        | `http://localhost:9995` |   
| **WTServer.JWTIssuer**                       | Url of the JWT issuer                                               | `http://localhost:9999` | 
| **JWTServer.TokenEndpointPath**              | Path to the token generating API endpoint                           | `/oauth/token`          |  
| **JWTServer.ConfirmationEmailTokenLifespan** | Lifespan of an email sent for confirming a new registration (hours) | `6`                     | 
| **JWTServer.InitialUserRole**                | Initial role of a newly registered user                             | `User`                  | 
| **JWTServer.ApiClientId**                    | Id of the accessing (here: management) client                       | `...`                   |
| **JWTServer.ApiClientSecret**                | Secret key to encrypt the token                                     | `...`                   |
| **EmailService.SenderAddress**               | Address that sends the auth server emails                           | `sender@myweb.net`      |
| **EmailService.SenderName**                  | Email sender's name                                                 | `Identity JWTServer`    |
| **EmailService.Account**                     | Account to authenticate at the SMTP server                          | `mymail.account`        |
| **EmailService.Password**                    | Password for SMTP server authentication                             | `mySMTPPass`            |
| **EmailService.MailServerAddress**           | IP address of the SMTP server                                       | `127.0.0.1`             |
| **EmailService.MailServerPort**              | SMTP Port                                                           | `25`                    |

### MicroErpApi

| Property                             | Description                                                  | Example                 |
| :----------------------------------- | :----------------------------------------------------------- | :---------------------- |
| ­**MicroErpApi.Url**                  | Url with a placeholder for the port                          | `http://localhost:{0}`  | 
| **MicroErpApi.AlternativeUrl**       | Alternative Url notation, also with placeholder              | `http://127.0.0.1:{0}`  |   
| **MicroErpApi.MultiUrl**             | Customizable Url                                             | `http://{0}:{1}`        |
| **MicroErpApi.Port**                 | Port number for the hosted API                               | `9990`                  |
| **MicroErpApi.AllowedOrigins**       | Allowed Origins for the CORS middleware (separated by comma) | `http://localhost:9995` |
| **MicroErpApi.JWTIssuer**            | Url of the JWT issuer                                        | `http://localhost:9999` | 
| **MicroErpApi.AudienceClientId**     | Id of the accessing client (audience)                        | `...`                   |
| **MicroErpApi.AudienceClientSecret** | Secret key to encrypt the token                              | `...`                   |

**All keys and IDs must be obtained from the according database entry of the client. They're 
automatically generated on creation by the extended ASP.NET Identity Provider.**

## Client/SPA configuration

Also the authentication service used in the AngularJS web application can be configured.
The `ngAuthSettings` are defined in the `app.js` file

```javascript
    app.constant('ngAuthSettings', {
      authBaseUri: 'http://localhost:9999/',      // Uri of the JWT authentication server
      resourceBaseUri: 'http://localhost:9990/',  // Uri of the consumed resource (Web API)
      clientId: 'CiPvyYsr7vX3'                    // Id of THIS client (MicroErp.Web)
    });
```

# Things to be done (orderby by their priority):
  * Provide a frontend application for client- and usermanagement.
  * Implement OAuth Refresh Tokens capability.
  * Get external authentication working in a decoupled scenario.
