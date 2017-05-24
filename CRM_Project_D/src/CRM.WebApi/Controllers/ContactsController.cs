namespace CRM.WebApi.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Web.Http;
    using Entities;

    public class ContactsController : ApiController
    {
        public IHttpActionResult GetAllContacts()
        {
            // TODO: login/auth check with token
            using (CRMContext database = new CRMContext())
            {
                try
                {
                    var data = database.Contacts.ToListAsync().Result;
                    return Ok(data);
                }
                catch
                {
                    return BadRequest();
                }
            }
        }

        public IHttpActionResult GetContactById(int? id)
        {
            // TODO: login/auth check with token
            using (CRMContext database = new CRMContext())
            {
                if (!id.HasValue) return BadRequest("Set parameter.");
                try
                {
                    Contact data = database.Contacts.FirstOrDefaultAsync(p => p.ContactId == id.Value).Result;
                    if (ReferenceEquals(data, null)) return NotFound();
                    return Ok(data);
                }
                catch
                {
                    return NotFound();
                }
            }
        }

        public IHttpActionResult PutContact([FromBody] Contact c)
        {
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            using (CRMContext database = new CRMContext())
            {
                Contact contact = database.Contacts.FirstOrDefaultAsync(p => p.ContactId == c.ContactId).Result;
                if (ReferenceEquals(contact, null)) return NotFound();
                using (var transaction = database.Database.BeginTransaction())
                {
                    try
                    {
                        database.Entry(contact).CurrentValues.SetValues(c);
                        database.SaveChangesAsync();
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
        }

        public IHttpActionResult PostContact([FromBody] Contact c)
        {
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            using (var database = new CRMContext())
            using (var transaction = database.Database.BeginTransaction())
            {
                c.GuID = Guid.NewGuid();
                c.DateInserted = DateTime.UtcNow;
                try
                {
                    database.Contacts.Add(c);
                    database.SaveChangesAsync();
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
            using (CRMContext database = new CRMContext())
            using (var transaction = database.Database.BeginTransaction())
            {
                var contact = database.Contacts.FirstOrDefaultAsync(p => p.ContactId == id.Value).Result;
                if (ReferenceEquals(contact, null)) return NotFound();
                try
                {
                    database.Contacts.Remove(contact);
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

        private bool ContactExsists(int id)
        {
            using (CRMContext database = new CRMContext())
                return database.Contacts.CountAsync(p => p.ContactId == id).Result > 0;
        }
    }
}
