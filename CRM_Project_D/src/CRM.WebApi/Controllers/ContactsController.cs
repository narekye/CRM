namespace CRM.WebApi.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Web.Http;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Entities;
    using HelperLibrary;
    
    /// <summary>
    /// Api RESTful logic for CRM system
    /// </summary>
    public class ContactsController : ApiController
    {
        public async Task<IHttpActionResult> GetAllContactsAsync()
        {
            // TODO: login/auth check with token
            try
            {
                using (var database = new CRMContext())
                {
                    var data = ContactModel.GetContactModelList(await database.Contacts.ToListAsync());
                    if (ReferenceEquals(data, null)) return NotFound();
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\n{ex.InnerException?.Message}");
            }
        }

        /* Will not used */
        public async Task<IHttpActionResult> GetContactByIdAsync(int? id)
        {
            // TODO: login/auth check with token
            if (!id.HasValue) return BadRequest("Set parameter.");
            try
            {
                using (var database = new CRMContext())
                {
                    var data = await
                                database.Contacts.Where(p => p.ContactId == id.Value)
                                .Select(c => ContactModel.GetContactModel(c))
                                .ToListAsync();
                    if (!(data.Count > 0)) return NotFound();
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IHttpActionResult> GetContactByGuidAsync([FromUri] Guid? guid)
        {
            if (ReferenceEquals(guid, null)) return NotFound();
            try
            {
                var data = ContactModel.GetContactModel(await GetContactFromContext(guid.Value));
                if (ReferenceEquals(null, data)) return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IHttpActionResult> GetContactByPaginationAsync(int start, int numberOfRows, bool ascending)
        {
            using (var database = new CRMContext())
            {
                try
                {
                    var contacts = ascending
                                    ? database.Contacts.OrderBy(x => x.ContactId)
                                        .Skip(start - 1)
                                        .Take(numberOfRows)
                                    : database.Contacts.OrderByDescending(x => x.ContactId)
                                        .Skip(start - 1)
                                        .Take(numberOfRows);
                    var result = ContactModel.GetContactModelList(await contacts.ToListAsync());
                    if (ReferenceEquals(result, null)) return NotFound();
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        // working
        public async Task<IHttpActionResult> PutContactAsync([FromBody] ContactModel c)
        {
            // x-www-form-urlencoded
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            Contact replace = await GetContactFromContactModel(c, false);
            using (var database = new CRMContext())
            {
                Contact contact = await GetContactFromContext(c.GuId);
                if (ReferenceEquals(contact, null)) return NotFound();
                using (var transaction = database.Database.BeginTransaction())
                {
                    try
                    {
                        database.Entry(contact).CurrentValues.SetValues(replace);
                        await database.SaveChangesAsync();
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

        // working
        public async Task<IHttpActionResult> PostContactAsync([FromBody] ContactModel c)
        {
            // x-www-form-urlencoded
            // TODO: login/auth check with token
            if (ReferenceEquals(c, null) || !ModelState.IsValid) return BadRequest();
            using (var database = new CRMContext())
            {
                var contact = await GetContactFromContactModel(c, true);
                using (var transaction = database.Database.BeginTransaction())
                {
                    try
                    {
                        database.Contacts.Add(contact);
                        await database.SaveChangesAsync();
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

        // working
        public async Task<IHttpActionResult> DeleteContactByGuIdAsync(Guid? guid)
        {
            // TODO: login/auth check with token
            if (!guid.HasValue) return BadRequest();
            using (var database = new CRMContext())
            {
                using (var transaction = database.Database.BeginTransaction())
                {
                    var contact = await GetContactFromContext(guid.Value);
                    if (ReferenceEquals(contact, null)) return NotFound();
                    try
                    {
                        database.Contacts.Remove(contact);
                        await database.SaveChangesAsync();
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

        // not tested yet
        [Route("api/contacts/upload")]
        public async Task<IHttpActionResult> PostContactByteArrayAsync([FromBody] byte[] array)
        {
            string pathtowork = ""; // path to work with file, on the end of function it will be deleted.
            using (var database = new CRMContext())
            using (var transaction = database.Database.BeginTransaction())
            {
                try
                {
                    var contacts = Parsing.GetContactsFromFile(array, pathtowork);
                    {
                        database.Contacts.AddRange(contacts);
                        await database.SaveChangesAsync();
                        transaction.Commit();
                        return Ok();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("api/contacts/count")]
        public int GetContactsPageCount()
        {
            //using (var database = new CRMContext())
            //{
            //    return database.Contacts.Count() > 10 ? database.Contacts.Count() / 10 : 1;
            //}
            return 10;
        }
        #region Helpers

        private async Task<Contact> GetContactFromContactModel(ContactModel model, bool flag, List<EmailList> emailList = null)
        {
            Contact contact;
            if (flag) // true returns new object with new Guid from model without contactId
            {
                contact = new Contact()
                {
                    GuID = Guid.NewGuid(),
                    DateInserted = DateTime.UtcNow,
                    FullName = model.FullName,
                    CompanyName = model.CompanyName,
                    Country = model.Country,
                    Email = model.Email,
                    Position = model.Position,
                    EmailLists = emailList
                };
            }
            else // false returns object from database 
            {
                using (var database = new CRMContext())
                {
                    contact = await database.Contacts.FirstOrDefaultAsync(p => p.GuID == model.GuId);
                    contact.FullName = model.FullName;
                    contact.CompanyName = model.CompanyName;
                    contact.Country = model.Country;
                    contact.Email = model.Email;
                    contact.Position = model.Position;
                    contact.EmailLists = emailList;
                }
            }
            return contact;
        }

        private async Task<Contact> GetContactFromContext(Guid guid)
        {
            Contact result;
            using (var database = new CRMContext())
            {
                result = await database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid);
            }
            return result;
        }
        #endregion

    }
}
