using Autofac.Core;
using Castle.Core.Logging;
using ContactMicroService.Bussiness.Abstract;
using ContactMicroService.Bussiness.Concrete;
using ContactMicroService.Entities.Model;
using ContactMicroService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ContactMicroService.Test.Controllers
{
    public class ContactInfoControllerTests
    {
        private readonly Mock<IContactInfoService> _contactInfoService;
        private readonly Mock<ILogger<ContactInfoController>> logger;
        private readonly MockContactInfos _mockContactInfo;
        public ContactInfoControllerTests()
        {
            _contactInfoService = new Mock<IContactInfoService>();
            logger = new Mock<ILogger<ContactInfoController>>();
            _mockContactInfo = new MockContactInfos();
        }


        [Fact]
        public void GetContactInfo_ListOfContactInfo_ContactInfoExistsInRepo()
        {
            var contactInfos = _mockContactInfo.GetContactInfos();
            _contactInfoService.Setup(x => x.GetAllContactInfos())
                .ReturnsAsync(contactInfos);
            var controller = new ContactInfoController(_contactInfoService.Object, logger.Object);

            var actionResult = controller.GetAllData();
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as IEnumerable<ContactInfo>;


            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(contactInfos.Count(), actual.Count());
        }

        [Fact]
        public void GetContact_GetById_ContactInfoExistInRepo()
        {
            var contactInfos = _mockContactInfo.GetContactInfos();
            _contactInfoService.Setup(x => x.GetContactInfoById(1))
                .ReturnsAsync(contactInfos[0]);
            var controller = new ContactInfoController(_contactInfoService.Object, logger.Object);

            var actionResult = controller.GetById(1);
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as ContactInfo;

            var expected = contactInfos[0];

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetContactInfoById_ContactInfoObject_ContactInfowithSpecificeIdExists()
        {
            var contactInfos = _mockContactInfo.GetContactInfos();
            var expected = contactInfos[0];
            _contactInfoService.Setup(x => x.GetContactInfoById(1).Result)
                .Returns(expected);
            var controller = new ContactInfoController(_contactInfoService.Object, logger.Object);


            var actionResult = controller.GetById(1);
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as ContactInfo;

            Assert.IsType<HttpStatusCode>(statusCode);
            Assert.Equal(HttpStatusCode.OK, statusCode);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetContactInfoById_shouldReturnBadRequest_ContactInfowithSpecificeIdNotExists()
        {
            _contactInfoService.Setup(x => x.GetContactInfoById(1))
                .ReturnsAsync((ContactInfo)null);
            var controller = new ContactInfoController(_contactInfoService.Object, logger.Object);

            var actionResult = controller.GetById(1);
            var result = actionResult.Result;
            var statusCode = result.Code;

            Assert.IsType<HttpStatusCode>(statusCode);
            Assert.Equal(HttpStatusCode.NotFound, statusCode);

        }

        [Fact]
        public void CreateContactInfo_CreatedStatus_PassingContactInfoObjectToCreate()
        {
            var contactInfos = _mockContactInfo.GetContactInfos();
            var newContactInfo = contactInfos[0];
            newContactInfo.ContactId = 0;
            var controller = new ContactInfoController(_contactInfoService.Object, logger.Object);
            var actionResult = controller.CreateOrUpdate(newContactInfo);
            var result = actionResult.Result;
            Assert.IsType<HttpStatusCode>(result.Code);
            Assert.Equal(HttpStatusCode.OK, result.Code);
            Assert.Equal(newContactInfo, result.Data);
        }
        [Fact]
        public async void DeleteContactInfo_DeletedStatus_PassingDeleteIdToDelete()
        {
            var ContactInfos = _mockContactInfo.GetContactInfos();
            var deleteContactInfo = ContactInfos[0];
            var id = 1;
            _contactInfoService.Setup(c => c.GetContactInfoById(id)).ReturnsAsync(deleteContactInfo);
            _contactInfoService.Setup(c => c.DeleteContactInfo(It.IsAny<ContactInfo>())).Returns(Task.CompletedTask);

            var controller = new ContactInfoController(_contactInfoService.Object, logger.Object);
            await controller.Delete(1);

            _contactInfoService.Verify(c => c.DeleteContactInfo(It.IsAny<ContactInfo>()), Times.Once);

        }

        [Fact]
        public void UpdateContact_UpdatedStatus_PassingContactObjectToUpdate()
        {
            var contactInfos = _mockContactInfo.GetContactInfos();
            var newContactInfo = contactInfos[0];
            newContactInfo.Information = "Adana";
            var controller = new ContactInfoController(_contactInfoService.Object, logger.Object);
            var actionResult = controller.CreateOrUpdate(newContactInfo);
            var result = actionResult.Result;
            Assert.IsType<HttpStatusCode>(result.Code);
            Assert.Equal(HttpStatusCode.OK, result.Code);
            Assert.Equal(newContactInfo, result.Data);

        }
    }
}
