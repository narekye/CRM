namespace CRM.WebApi.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Web.Http;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    /// <summary>
    /// Api Logic for CRM system
    /// </summary>
    public class ContactsController : ApiController
    {
        private readonly CRMContext _database = new CRMContext();
        public IHttpActionResult GetAllContacts()
        {
            // TODO: login/auth check with token
            try
            {
                var data = _database.Contacts.Include(p => p.EmailLists).ToList();
                var dt = CreateObject(data) as IEnumerable;
                if (ReferenceEquals(dt, null)) return NotFound();
                return Ok(dt);
            }
            catch
            {
                return BadRequest();
            }
        }

        public IHttpActionResult GetContactById(int? id)
        {
            // TODO: login/auth check with token
            if (!id.HasValue) return BadRequest("Set parameter.");
            try
            {
                var data = _database.Contacts.Where(p => p.ContactId == id.Value).Select(c => new
                {
                    c.FullName,
                    c.CompanyName,
                    c.Position,
                    c.Country,
                    c.Email,
                    c.GuID,
                    c.DateInserted,
                    EmailLists = c.EmailLists.Select(k => k.EmailListName).ToList()
                }).ToList();
                if (!(data.Count > 0)) return NotFound();
                return Ok(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("api/contacts/count")]
        public int GetContactsPageCount()
        {
            return _database.Contacts.Count() > 10 ? _database.Contacts.Count() / 10 : 1;
        }

        public IHttpActionResult PutContact([FromBody] Contact c)
        {
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();

            Contact contact = _database.Contacts.FirstOrDefaultAsync(p => p.ContactId == c.ContactId).Result;
            if (ReferenceEquals(contact, null)) return NotFound();
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    _database.Entry(contact).CurrentValues.SetValues(c);
                    _database.SaveChangesAsync();
                    transaction.Commit();
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        public IHttpActionResult PostContact([FromBody] Contact c)
        {
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            using (var transaction = _database.Database.BeginTransaction())
            {
                c.GuID = Guid.NewGuid();
                c.DateInserted = DateTime.UtcNow;
                try
                {
                    _database.Contacts.Add(c);
                    _database.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
            return NotFound();
        }

        public IHttpActionResult DeleteContactById(int? id)
        {
            // TODO: login/auth check with token
            if (!id.HasValue) return BadRequest();
            using (var transaction = _database.Database.BeginTransaction())
            {
                var contact = _database.Contacts.FirstOrDefaultAsync(p => p.ContactId == id.Value).Result;
                if (ReferenceEquals(contact, null)) return NotFound();
                try
                {
                    _database.Contacts.Remove(contact);
                    transaction.Commit();
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        #region Helpers
        private bool ContactExsists(int id)
        {
            using (CRMContext database = new CRMContext())
                return database.Contacts.CountAsync(p => p.ContactId == id).Result > 0;
        }

        public dynamic CreateObject(List<Contact> list)
        {
            var dt = list.Select(c => new
            {
                c.FullName,
                c.CompanyName,
                c.Position,
                c.Country,
                c.Email,
                c.GuID,
                c.DateInserted,
                EmailLists = c.EmailLists.Select(k => k.EmailListName).ToList()
            });
            return dt;
        }
        #endregion
    }
}
