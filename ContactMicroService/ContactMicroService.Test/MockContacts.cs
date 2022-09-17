using ContactMicroService.Entities.Model;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactMicroService.Test
{
    public class MockContacts
    {
        private readonly List<Contact> contactList;
        public MockContacts()
        {
            contactList = new List<Contact>
            {
            new Contact
            {
                ContactId=1,
                FirstName = "Jhon",
                LastName = "Doe",
                Company="Corp"
            },
            new Contact
            {
                ContactId = 2,
                FirstName = "Jhon1",
                LastName = "Doe1",
                Company="Corp1"
            },
            new Contact
            {
                ContactId=3,
                FirstName = "Jhon2",
                LastName = "Doe2",
                Company="Corp2"
            }
            };
        }

        public List<Contact> GetContacts()
        {
            return contactList;
        }

        public Contact GetById(int id)
        {
            return contactList.First(c => c.ContactId == id);
        }

    }
}
