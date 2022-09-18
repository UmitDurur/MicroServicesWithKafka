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
    public class ContactControllerTests
    {
        private readonly Mock<IContactService> _contactService;
        private readonly Mock<ILogger<ContactController>> logger;
        private readonly MockContacts _mockContact;
        public ContactControllerTests()
        {
            _contactService = new Mock<IContactService>();
            logger = new Mock<ILogger<ContactController>>();
            _mockContact = new MockContacts();
        }



        [Fact]
        public void GetContact_ListOfContact_ContactExistsInRepo()
        {
            var contacts = _mockContact.GetContacts();
            _contactService.Setup(x => x.GetAllContacts())
                .ReturnsAsync(contacts);
            var controller = new ContactController(_contactService.Object, logger.Object);

            var actionResult = controller.GetAllData();
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as IEnumerable<Contact>;

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(_mockContact.GetContacts().Count(), actual.Count());
        }

        [Fact]
        public void GetFilteredContact_ListOfContact_ContactExistsInRepoIsNotDeleted()
        {
            var contacts = _mockContact.GetContacts();
            _contactService.Setup(x => x.GetDeleteFilteredAllContacts())
                .ReturnsAsync(contacts);
            var controller = new ContactController(_contactService.Object, logger.Object);

            var actionResult = controller.GetDeleteFilteredAllData();
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as IEnumerable<Contact>;

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(_mockContact.GetContacts().Count(), actual.Count());
        }

        [Fact]
        public void GetContact_GetById_ContactExistInRepo()
        {
            var contacts = _mockContact.GetContacts();
            _contactService.Setup(x => x.GetContactById(1))
                .ReturnsAsync(contacts[0]);
            var controller = new ContactController(_contactService.Object, logger.Object);

            var actionResult = controller.GetById(1);
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as Contact;

            var expected = _mockContact.GetById(1);

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetContactById_ContactObject_ContactwithSpecificeIdExists()
        {
            var contacts = _mockContact.GetContacts();
            var expected = contacts[0];
            _contactService.Setup(x => x.GetContactById(1).Result)
                .Returns(expected);
            var controller = new ContactController(_contactService.Object, logger.Object);

            var actionResult = controller.GetById(1);
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as Contact;

            Assert.IsType<HttpStatusCode>(statusCode);
            Assert.Equal(HttpStatusCode.OK, statusCode);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetContactById_shouldReturnBadRequest_ContactwithSpecificeIdNotExists()
        {
            _contactService.Setup(x => x.GetContactById(1))
                .ReturnsAsync((Contact)null);
            var controller = new ContactController(_contactService.Object, logger.Object);

            var actionResult = controller.GetById(1);
            var result = actionResult.Result;
            var statusCode = result.Code;

            Assert.IsType<HttpStatusCode>(statusCode);
            Assert.Equal(HttpStatusCode.NotFound, statusCode);

        }

        [Fact]
        public void CreateContact_CreatedStatus_PassingContactObjectToCreate()
        {
            var contacts = _mockContact.GetContacts();
            var newContact = contacts[0];
            newContact.ContactId = 0;
            var controller = new ContactController(_contactService.Object, logger.Object);
            var actionResult = controller.CreateOrUpdate(newContact);
            var result = actionResult.Result;
            Assert.IsType<HttpStatusCode>(result.Code);
            Assert.Equal(HttpStatusCode.OK, result.Code);
            Assert.Equal(newContact,result.Data);
        }
  

        [Fact]
        public async void GetContactReportByLocation_ContactReportObject_ContactwithSpecifiedInfoLocation()
        {
            string location = "Hatay";
            var contacts = _mockContact.GetContacts();
            var expected = new ContactReport();
            expected.Location = location;
            expected.NearbyPeopleCount = 2;
            expected.NearbySavedPhoneCount = 1;
            _contactService.Setup(x => x.GetReportData(location).Result)
                .Returns(expected);
            var controller = new ContactController(_contactService.Object, logger.Object);

            var actionResult = controller.GetContactReportByLocation(location);
            var actual = actionResult.Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateContact_UpdatedStatus_PassingContactObjectToUpdate()
        {
            var contacts = _mockContact.GetContacts();
            var newContact = contacts[0];
            newContact.Company = "NewComp";
            var controller = new ContactController(_contactService.Object, logger.Object);
            var actionResult = controller.CreateOrUpdate(newContact);
            var result = actionResult.Result;
            Assert.IsType<HttpStatusCode>(result.Code);
            Assert.Equal(HttpStatusCode.OK, result.Code);
            Assert.Equal(newContact, result.Data);

        }
    }
}
