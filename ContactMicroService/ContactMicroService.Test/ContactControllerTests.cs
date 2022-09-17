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

namespace ContactMicroService.Test
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
        //naming convention MethodName_expectedBehavior_StateUnderTest
        public void GetContact_ListOfContact_ContactExistsInRepo()
        {
            //arrange
            var contacts = _mockContact.GetContacts();
            _contactService.Setup(x => x.GetAllContacts().Result)
                .Returns(contacts);
            var controller = new ContactController(_contactService.Object, logger.Object);

            //act
            var actionResult = controller.GetAllData();
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as IEnumerable<Contact>;

            //assert
            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(_mockContact.GetContacts().Count(), actual.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        //naming convention MethodName_expectedBehavior_StateUnderTest
        public void GetEmployee_GetById_ContactExistInRepo(int id)
        {
            //arrange
            _contactService.Setup(x => x.GetContactById(id).Result)
                .Returns(_mockContact.GetById(id));
            var controller = new ContactController(_contactService.Object, logger.Object);

            //act
            var actionResult = controller.GetById(id);
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as Contact;

            var expected = _mockContact.GetById(id);

            //assert
            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetContactById_ContactObject_ContactwithSpecificeIdExists(int id)
        {
            //arrange
            var contacts = _mockContact.GetContacts();
            var expected = contacts.First(c=>c.ContactId==id);
            _contactService.Setup(x => x.GetContactById(id).Result)
                .Returns(expected);
            var controller = new ContactController(_contactService.Object, logger.Object);

            //act
            var actionResult = controller.GetById(id); 
            var result = actionResult.Result;
            var statusCode = result.Code;
            var actual = result.Data as Contact;

            //Assert
            Assert.IsType<HttpStatusCode>(statusCode);
            Assert.Equal(HttpStatusCode.OK,statusCode);

            Assert.Equal(expected,actual);
        }

        [Theory]
        [InlineData(11)]
        [InlineData(5)]
        [InlineData(6)]
        public void GetContactById_shouldReturnBadRequest_ContactwithSpecificeIdNotExists(int id)
        {
            //arrange
            _contactService.Setup(x => x.GetContactById(id))
                .ReturnsAsync((Contact)null);
            var controller = new ContactController(_contactService.Object, logger.Object);

            //act
            var actionResult = controller.GetById(id);
            var result = actionResult.Result;
            var statusCode = result.Code;

            //Assert
            Assert.IsType<HttpStatusCode>(statusCode);
            Assert.Equal(HttpStatusCode.NotFound, statusCode);

        }
    }
}
