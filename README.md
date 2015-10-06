# AspNet.JWTAuthServer

## The ASP.NET Identity Storage Provider for NPoco

Inspired by the brilliant [articles on authentication](http://bitoftech.net/category/asp-net-identity/)
written by [Taiseer Joudeh](https://twitter.com/TJoudeh) I firstly implemented an ASP.NET Identity 
storage provider which uses [NPoco ORM](https://github.com/schotime/NPoco) written by Adam Schroder. I 
felt the need for this because I find that the Entity Framework has a very big overhead - despite of its 
migrations capability. The aim was to have a handy ORM that supports various databases right out of the 
box. 

## The extended ASP.NET Identity Storage Provider for NPoco

Having this step done, I implemented a self-hosted authentication server using ASP.NET OWIN. It issues JWT 
tokens for authentication and is 'multi-client capable'. To achieve this the 
[ASP.NET Identity storage provider for NPoco](https://github.com/miseeger/AspNet.Identity.NPoco) was extended 
in order to cope with multiple clients. It was integrated as a component of the JWTAuthServer solution 
(AspNet.IdentityEx.NPoco). The unerlying database can be easily created by executing the provided create-script. 
This implementation uses an [SQLite](https://www.sqlite.org/) database but the script may be executed 
on other SQL servers because it's quite SQL92-ish.

## The stand-alone JWT issuing authentication server

The next (and final) step was to have the auth server working in a decoupled manner so that the server
stands alone for issuing JWTs requested by the various clients, following the 
[OAuth 2.0 Resource Owner Password Credential Flow](http://oauthlib.readthedocs.org/en/latest/oauth2/grants/password.html). 

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
    * External logins
    * Clients
  * Extended authorization attribute for securing API endpoints    

## The code used in this solution 

The AspNet.JWTAuthServer uses, adapts and combines code written by Taiseer Joudeh found in the following 
gitHub repositories:
  * [tjoudeh/AngularJSAuthentication](https://github.com/tjoudeh/AngularJSAuthentication)
  * [tjoudeh/AspNetIdentity.WebApi](https://github.com/tjoudeh/AspNetIdentity.WebApi)

#### The external authentication does not work yet in a decoupled scenario. This is why it is disabled in the code but may be enabled to work in an 'integrated' scenario where all APIs are hosted in one place (ASP.NET Web API) with the authentication server. 
 
## Things to be done:
  * Implement OAuth Refresh Tokens capability.
  * Enable external authentication in a decoupled scenario.
  * Provide a frontend application for client- and usermanagement 

## Putting it into operation

following asap ...
