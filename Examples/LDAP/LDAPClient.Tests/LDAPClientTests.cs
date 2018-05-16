using System;
using System.DirectoryServices.Protocols;
using System.Linq;
using Xunit;

namespace LDAPClient.Tests
{
    public class LDAPClientTests
    {
        private readonly string _adminDn = "cn=admin,dc=example,dc=org";
        private readonly string _AdminPassword = "admin";
        private readonly AuthType _ldapAuthType = AuthType.Basic;
        private readonly string _LDAPEndpoint = "localhost:389";

        public LDAPClientTests()
        {
            LDAPClientSUT = new LDAPClient();
        }

        public LDAPClient LDAPClientSUT { get; }

        [Fact]
        public void _Bind_Binded()
        {
            //Arrange
            //Act
            BindAdmin();
            //Assert
        }

        private void BindAdmin()
        {
            LDAPClientSUT.Bind(_adminDn, _AdminPassword, _LDAPEndpoint, _ldapAuthType);
        }

        [Fact]
        public void NewUser_AddUser_UserAdded()
        {
            //Arrange
            BindAdmin();
            var newUser = CreateNewUser();
            //Act
            var isUserAdded = LDAPClientSUT.AddUser(newUser);
            //Assert
            Assert.True(isUserAdded);
        }

        [Fact]
        public void ExistingUser_DeleteUser_UserDeleted()
        {
            //Arrange
            BindAdmin();
            var userModel = CreateNewUser();
            LDAPClientSUT.AddUser(userModel);
            //Act

            var isUserDeleted = LDAPClientSUT.Delete(userModel.DN);
            //Assert
            Assert.True(isUserDeleted);
        }

        [Fact]
        public void AdminIsSeeded_Search_AdminExists()
        {
            //Arrange
            BindAdmin();
            //Act

            var searchResult = LDAPClientSUT.Search("dc=example,dc=org", "objectClass=*");
            var admin = searchResult.FirstOrDefault(record => record.TryGetValue("cn", out var value) && value == "admin");
           
            //Assert
            Assert.NotNull(admin);
        }

        private UserModel CreateNewUser()
        {
            return new UserModel("cn=john,dc=example,dc=org", "john", "John Doe", "john");
        }
    }
}