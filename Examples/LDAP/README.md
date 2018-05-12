# LDAP Sample with Docker 
* Requires Docker/Docker for Windows

## Test Docker Deployment
* ```$ docker-compose up```
* Query LDAP with ldapsearch
  * ```$ docker exec openldap-app ldapsearch -x -H ldap://localhost -b dc=example,dc=org -D "cn=admin,dc=example,dc=org" -w admin```
  * ```-x``` - use simple authentication 
  * ```-D``` - Use bind user "cn=admin,dc=example,dc=org"
  * ```-W``` - Prompt for password
  * ```-w``` - password
  * ```-H``` - URL of LDAP server. Non-SSL in this case; use "ldaps://" for SSL
  * ```-b``` - The search base

# Acknowledgement
* Based on [Using LDAP and Active Directory with C# 101](https://auth0.com/blog/using-ldap-with-c-sharp/) and [blog-ldap-csharp-example](https://github.com/auth0-blog/blog-ldap-csharp-example)