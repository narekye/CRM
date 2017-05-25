namespace CRM.WebApi.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Web.Http;
    using System.Linq;
    using Models;
    using Entities;
    using CRM.HelperLibrary;

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
                var data = ContactModel.GetContactModelList(_database.Contacts.ToList());
                if (ReferenceEquals(data, null)) return NotFound();
                return Ok(data);
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
                }).ToListAsync().Result;
                if (!(data.Count > 0)) return NotFound();
                return Ok(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        public IHttpActionResult GetContactByGuid([FromUri] Guid? guid)
        {
            if (ReferenceEquals(guid, null)) return NotFound();
            var data = ContactModel.GetContactModel(_database.Contacts.FirstOrDefault(p => p.GuID == guid.Value));
            return Ok(data);
        }

        public IHttpActionResult GetContactByPagination(int start, int numberOfRows, bool ascending)
        {
            var contacts = ascending
                ? _database.Contacts.OrderBy(x => x.ContactId).Skip(start - 1).Take(numberOfRows).Select(c => new
                {
                    c.FullName,
                    c.CompanyName,
                    c.Position,
                    c.Country,
                    c.Email,
                    c.GuID,
                    c.DateInserted,
                    EmailLists = c.EmailLists.Select(k => k.EmailListName).ToList()
                }).ToList()
                : _database.Contacts.OrderByDescending(x => x.ContactId)
                    .Skip(start - 1)
                    .Take(numberOfRows)
                    .Select(c => new
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
            if (ReferenceEquals(contacts, null)) return NotFound();
            return Ok(contacts);
        }

        [Route("api/contacts/count")]
        public int GetContactsPageCount()
        {
            return _database.Contacts.Count() > 10 ? _database.Contacts.Count() / 10 : 1;
        }

        // working
        public IHttpActionResult PutContact([FromBody] ContactModel c)
        {
            // x-www-form-urlencoded
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();

            Contact contact = _database.Contacts.FirstOrDefault(p => p.GuID == c.GuId);
            if (ReferenceEquals(contact, null)) return NotFound();
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    _database.Entry(contact).CurrentValues.SetValues(c);
                    _database.SaveChanges();
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

        // working
        public IHttpActionResult PostContact([FromBody] Contact c)
        {
            // x-www-form-urlencoded
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            using (var transaction = _database.Database.BeginTransaction())
            {
                c.GuID = Guid.NewGuid();
                c.DateInserted = DateTime.UtcNow;
                try
                {
                    _database.Contacts.Add(c);
                    _database.SaveChanges();
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

        // working
        public IHttpActionResult DeleteContactById(Guid? guid)
        {
            // TODO: login/auth check with token
            if (!guid.HasValue) return BadRequest();
            using (var transaction = _database.Database.BeginTransaction())
            {
                var contact = _database.Contacts.FirstOrDefault(p => p.GuID == guid.Value);
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

        private dynamic CreateListObjectFromContacts()
        {
            var dt = _database.Contacts.Select(c => new
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

        private dynamic CreateObjectFromModel(Contact c)
        {
            var dt = new
            {
                c.FullName,
                c.CompanyName,
                c.Position,
                c.Country,
                c.Email,
                c.GuID,
                c.DateInserted,
                EmailLists = c.EmailLists.Select(k => new
                {
                    k.EmailListID,
                    k.EmailListName
                }).ToList()
            };
            return dt;
        }

        #endregion

        public IHttpActionResult PostContactByteArray([FromBody] byte[] array)
        {
            string pathtowork = "";
            try
            {
                Parsing.GetContactsFromFile(array, pathtowork);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) _database.Dispose();
            base.Dispose(disposing);
        }

    }
}
