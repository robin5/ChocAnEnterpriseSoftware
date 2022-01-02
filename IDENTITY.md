Identity Management
===================

Identity management is provided by Azure.  It is  a cloud focused solution because there is not a current on premises Active Directory solution for ChocAn.  There are five types of users in ChocAn enterprises: providers, members, employees, provider terminals, and external businesses (ACME).

External business identity is managed by Azure AD B2B  
-----------------------------------------------------
How this works, is that the business is sent an email invitation to join ChocAn's Azure AD tenant. Once they accept the invitation, they show up in the directory and are given access to designated ChocAn resources. Since these are external identities, they have an external identity provider. So the authentication of such users happens at the external provider and then they are redirected to the resources they need to access in ChocAn's tenant.

Providers and member identity is managed by Azure AD B2C
--------------------------------------------------------
Azure AD B2C is mainly used by businesses for handling identities of customers using their public-facing applications. Users can sign-up using email, or a third-party identity provider of their choice. They can edit their own profiles, reset passwords, and delete their accounts if they wish to.

Application Platforms
=====================
Provider and member applications can be provisioned through Azure SaaS.  The DataCenter and manager apps will be supported by Azure PaaS service.

Provider terminals
------------------
Provider terminals a managed via Azure AD join.