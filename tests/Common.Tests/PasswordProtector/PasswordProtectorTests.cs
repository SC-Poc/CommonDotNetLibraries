using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MasterPassword;
using Lykke.Common.MasterPassword.Exceptions;
using Microsoft.AspNetCore.DataProtection;
using Moq;
using Xunit;

namespace Common.Tests.PasswordProtector
{
    public class PasswordProtectorTests
    {
        [Fact]
        public async Task Can_Encrypt_And_Decrpypt_Without_Data_Loss()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector(Guid.NewGuid().ToString());

            var partCount = new Random().Next(2, 9);

            var service = new CompositePasswordProtectorService(repo, dataProvider, (uint)partCount);

            foreach (var i in Enumerable.Range(1, partCount))
            {
                await service.SetPasswordPartAsync(i, Guid.NewGuid().ToString());
            }
            
            var source = Guid.NewGuid();
            var encrypted = service.Ecrypt(source.ToString());

            var decrypted = service.Decrypt(encrypted);

            Assert.Equal(source.ToString(), decrypted);
        }

        [Fact]
        public async Task Throws_Exception_On_Failed_Decryption()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector(Guid.NewGuid().ToString());

            var service = new CompositePasswordProtectorService(repo, dataProvider, 3);

            await service.SetPasswordPartAsync(1, Guid.NewGuid().ToString());
            await service.SetPasswordPartAsync(2, Guid.NewGuid().ToString());
            await service.SetPasswordPartAsync(3, Guid.NewGuid().ToString());


            var invalidInput = Guid.NewGuid().ToString();


            Assert.Throws<MasterPasswordDecryptionException>(() => service.Decrypt(invalidInput));
        }

        [Fact]
        public void Throws_Exception_On_Decryption_Without_Initiation()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector(Guid.NewGuid().ToString());

            var service = new CompositePasswordProtectorService(repo, dataProvider, (uint) new Random().Next(1, 100));

            var invalidInput = Guid.NewGuid().ToString();
            
            Assert.Throws<MasterPasswordNotInitiatedException>(() => service.Decrypt(invalidInput));
        }


        [Fact]
        public async Task Indicates_That_Password_Initiated()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector(Guid.NewGuid().ToString());

            var service = new CompositePasswordProtectorService(repo, dataProvider, 3);

            await service.SetPasswordPartAsync(1, Guid.NewGuid().ToString());
            await service.SetPasswordPartAsync(2, Guid.NewGuid().ToString());
            await service.SetPasswordPartAsync(3, Guid.NewGuid().ToString());

            Assert.True(service.IsPasswordValid);
        }

        [Fact]
        public async Task Indicates_That_Password_Not_Initiated()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector(Guid.NewGuid().ToString());

            var service = new CompositePasswordProtectorService(repo, dataProvider, 3);

            await service.SetPasswordPartAsync(1, Guid.NewGuid().ToString());
            await service.SetPasswordPartAsync(2, Guid.NewGuid().ToString());
            await service.SetPasswordPartAsync(3, Guid.NewGuid().ToString());

            Assert.True(service.IsPasswordValid);
        }

        [Fact]
        public async Task Encryption_Results_Is_Constant()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector("Test1");

            var service = new CompositePasswordProtectorService(repo, dataProvider, 3);

            await service.SetPasswordPartAsync(1, "SomePswd1");
            await service.SetPasswordPartAsync(2, "SomePswd2");
            await service.SetPasswordPartAsync(3, "SomePswd3");

            var source = "Smth";
            var encrypted = service.Ecrypt(source);

            Assert.Equal("XXihp5QTOR2ByoWskp/uSg==", encrypted);
        }

        [Fact]
        public async Task Dencryption_Results_Is_Constant()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector("Test1");

            var service = new CompositePasswordProtectorService(repo, dataProvider, 3);



            await service.SetPasswordPartAsync(1, "SomePswd1");
            await service.SetPasswordPartAsync(2, "SomePswd2");
            await service.SetPasswordPartAsync(3, "SomePswd3");

            var source = "hV3LlwsKCQhfSO+wb9TApQ==";
            var encrypted = service.Decrypt(source);

            Assert.Equal("Smth1", encrypted);
        }

        [Fact]
        public async Task Password_Generation_Is_Constant()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector("Test1");

            var service = new CompositePasswordProtectorService(repo, dataProvider, 3);

            await service.SetPasswordPartAsync(1, "SomePswd1");
            await service.SetPasswordPartAsync(2, "SomePswd2");
            await service.SetPasswordPartAsync(3, "SomePswd3");

            var fullPswd = service.GetFullPassword();

            Assert.Equal("pvOhsVhMsU1S8MaA0MkFmVQlOrf821c4OvhGtcT51kI=", fullPswd);
        }


        [Fact]
        public async Task Can_Generate_And_Restore_Password()
        {
            var repo = CreateRepo().Object;
            var dataProtectorPurpose = Guid.NewGuid();

            var service = new CompositePasswordProtectorService(repo, CreateDataProtector(dataProtectorPurpose.ToString()), 3);

            var key1 = Guid.NewGuid();
            var key2 = Guid.NewGuid();
            var key3 = Guid.NewGuid();

            await service.SetPasswordPartAsync(1, key1.ToString());
            await service.SetPasswordPartAsync(2, key2.ToString());
            await service.SetPasswordPartAsync(3, key3.ToString());

            var generatedPassword = service.GetFullPassword();

            var serviceAfterRebuild = new CompositePasswordProtectorService(repo, CreateDataProtector(dataProtectorPurpose.ToString()), 3);

            await serviceAfterRebuild.SetPasswordPartAsync(1, key1.ToString());
            await serviceAfterRebuild.SetPasswordPartAsync(2, key2.ToString());
            await serviceAfterRebuild.SetPasswordPartAsync(3, key3.ToString());

            var passwordAfterRebuild = serviceAfterRebuild.GetFullPassword();

            Assert.Equal(generatedPassword, passwordAfterRebuild);
        }

        [Fact]
        public async Task Seed_Fake_Password_Hashes()
        {
            var repo = CreateRepo().Object;
            var dataProvider = CreateDataProtector("Test1");

            var service = new CompositePasswordProtectorService(repo, dataProvider, 3);


            await service.SetPasswordPartAsync(1, "SomePswd1");
            await service.SetPasswordPartAsync(2, "SomePswd2");
            await service.SetPasswordPartAsync(3, "SomePswd3");

            Assert.True((await repo.GetAsync()).Count()>2);
        }


        private Mock<IPasswordValidationHashRepository> CreateRepo()
        {
            var result = new Mock<IPasswordValidationHashRepository>();

            var storage = new List<string>();
            result.Setup(p => p.GetAsync())
                .ReturnsAsync(storage);

            result.Setup(p => p.InsertAsync(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.CompletedTask)
                .Callback((IEnumerable<string> insertion) =>
                {
                    storage.AddRange(insertion);
                });

            return result;
        }

        private IDataProtector CreateDataProtector(string purpose)
        {
            return new EphemeralDataProtectionProvider().CreateProtector
            (
                purpose: purpose
            );
        }
    }
}
