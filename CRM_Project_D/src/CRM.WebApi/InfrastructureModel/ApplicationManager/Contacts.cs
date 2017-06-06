namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using Entities;
    using Models.Request;
    using Models.Response;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    public partial class ApplicationManager : IDisposable
    {
        private readonly CRMContext _database;
        private readonly ParserProvider _parser;
        public ApplicationManager()
        {
            _parser = new ParserProvider();
            _database = new CRMContext();
            _database.Configuration.LazyLoadingEnabled = false;
        }
        public async Task<List<ViewContactLess>> GetAllContactsAsync()
        {
            var list = await _database.Contacts.ToListAsync();
            if (ReferenceEquals(list, null)) return null;
            var data = new List<ViewContactLess>();
            AutoMapper.Mapper.Map(list, data);
            return data;
        }
        public async Task<ViewContact> GetContactByIdAsync(int? id)
        {
            var contact =
                     await
                         _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.ContactId == id.Value);
            if (ReferenceEquals(contact, null)) return null;
            var data = new ViewContact();
            AutoMapper.Mapper.Map(contact, data);
            data.EmailLists = contact.EmailLists.Select(p => p.EmailListName).ToList();
            return data;
        }
        public async Task<ViewContact> GetContactByGuidAsync(Guid? guid)
        {
            var contact =
                    await _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.GuID == guid.Value);
            if (ReferenceEquals(contact, null)) return null;
            var data = new ViewContact();
            AutoMapper.Mapper.Map(contact, data);
            data.EmailLists = contact.EmailLists.Select(p => p.EmailListName).ToList();
            return data;
        }
        public async Task<bool> UpdateContactAsync(ViewContactLess model)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var original = await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == model.GuID);
                    var replace = new Contact();
                    AutoMapper.Mapper.Map(model, replace);
                    replace.ContactId = original.ContactId;
                    replace.DateModified = DateTime.UtcNow;
                    _database.Entry(original).CurrentValues.SetValues(replace);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> AddContactAsync(ViewContactLess model)
        {
            var contact = new Contact();
            AutoMapper.Mapper.Map(model, contact);
            contact.GuID = Guid.NewGuid();
            contact.DateInserted = DateTime.UtcNow;
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    _database.Contacts.Add(contact);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> DeleteContactAsync(Guid guid)
        {
            var cont = await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid);
            _database.Contacts.Remove(cont);
            await _database.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteContactsAsync(List<Guid> guids)
        {
            var list = new List<Contact>();

            foreach (Guid guid in guids)
                list.Add(await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid));
            _database.Contacts.RemoveRange(list);
            await _database.SaveChangesAsync();
            return true;
        }
        public async Task<int> PageCountAsync()
        {
            return await _database.Contacts.CountAsync() / 10 + 1;
        }
        #region FilterOrderPaging
        public async Task<List<ViewContactLess>> FilterOrderByRequestAsync(RequestContact request)
        {
            if (ReferenceEquals(request, null)) return null;
            ViewContactLess filter = request.FilterBy;
            List<ViewContactLess> result = new List<ViewContactLess>();
            try
            {
                if (filter.FullName != null)
                    result = await FilterSortAsync(request, FilterBy.Name);
                if (filter.CompanyName != null)
                {
                    result = await FilterSortAsync(request, FilterBy.Company);
                    if (filter.FullName != null)
                        result = await FilterSortAsync(request, FilterBy.NameCompany);
                }
                if (filter.Position != null)
                {
                    result = await FilterSortAsync(request, FilterBy.Position);
                    if (filter.FullName != null)
                        result = await FilterSortAsync(request, FilterBy.NamePosition);
                    if (filter.CompanyName != null)
                        result = await FilterSortAsync(request, FilterBy.CompanyPosition);
                    if (filter.FullName != null && filter.CompanyName != null)
                        result = await FilterSortAsync(request, FilterBy.NameCompanyPosition);
                }
                if (filter.Country != null)
                {
                    result = await FilterSortAsync(request, FilterBy.Country);
                    if (filter.FullName != null)
                        result = await FilterSortAsync(request, FilterBy.NameCountry);
                    if (filter.Position != null)
                        result = await FilterSortAsync(request, FilterBy.CountryPosition);
                    if (filter.FullName != null && filter.Position != null)
                        result = await FilterSortAsync(request, FilterBy.NamePositionCountry);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ViewContactLess>> FilterSortAsync(RequestContact model, FilterBy filter)
        {
            var data = new List<Contact>();
            var sortby = model.SortBy;
            var filterby = model.FilterBy;
            #region SWITCH

            switch (filter)
            {
                case FilterBy.Name:
                    data = await
                        _database.Contacts
                            .Where(p => p.FullName == filterby.FullName)
                            .ToListAsync();
                    break;
                case FilterBy.Company:
                    data = await
                        _database.Contacts
                            .Where(p => p.CompanyName == filterby.CompanyName)
                            .ToListAsync();
                    break;
                case FilterBy.NameCompany:
                    data =
                        await
                            _database.Contacts.Where(
                                    p => p.FullName == filterby.FullName && p.CompanyName == filterby.CompanyName)
                                .ToListAsync();
                    break;
                case FilterBy.Position:
                    data = await _database.Contacts.Where(p => p.Position == filterby.Position).ToListAsync();
                    break;
                case FilterBy.NamePosition:
                    data =
                        await
                            _database.Contacts.Where(
                                p => p.FullName == filterby.FullName && p.Position == filterby.Position).ToListAsync();
                    break;
                case FilterBy.NameCompanyPosition:
                    data =
                        await
                            _database.Contacts.Where(
                                p =>
                                    p.FullName == filterby.FullName && p.CompanyName == filterby.CompanyName &&
                                    p.Position == filterby.Position).ToListAsync();
                    break;
                case FilterBy.Country:
                    data = await _database.Contacts.Where(p => p.Country == filterby.Country).ToListAsync();
                    break;
                case FilterBy.NameCountry:
                    data =
                        await
                            _database.Contacts.Where(
                                p => p.FullName == filterby.FullName && p.Country == filterby.Country).ToListAsync();
                    break;
                case FilterBy.NameCompanyCountry:
                    data =
                        await
                            _database.Contacts.Where(
                                p =>
                                    p.FullName == filterby.FullName && p.CompanyName == filterby.CompanyName &&
                                    p.Country == filterby.Country).ToListAsync();
                    break;
                case FilterBy.NameCompanyPositionCountry:
                    data =
                        await
                            _database.Contacts.Where(
                                p =>
                                    p.FullName == filterby.FullName && p.CompanyName == filterby.CompanyName &&
                                    p.Position == filterby.Position && p.Country == filterby.Country).ToListAsync();
                    break;
                case FilterBy.NamePositionCountry:
                    data =
                        await
                            _database.Contacts.Where(
                                p =>
                                    p.FullName == filterby.FullName && p.Position == filterby.Position &&
                                    p.Country == filterby.Country).ToListAsync();
                    break;
                case FilterBy.CountryPosition:
                    data =
                        await
                            _database.Contacts.Where(
                                p => p.Country == filterby.Country && p.Position == filterby.Position).ToListAsync();
                    break;
            }

            #endregion
            #region PAGING
            string first = "Start", second = "Count";
            if (sortby.ContainsKey(first) && sortby.ContainsKey(second))
            {
                int start;
                bool st = int.TryParse(sortby[first], out start);
                int count;
                bool end = int.TryParse(sortby[second], out count);
                if (st && end) data = Paging(data, start, count);
            }
            #endregion
            #region ORDERBY
            if (sortby.ContainsKey("OrderBy"))
            {
                string orderby = sortby["OrderBy"];
                if (orderby == "Ascending" || orderby == "0" || orderby == "Asc")
                    data = Sort(data, OrderBy.Ascending);
                if (orderby == "Descending" || orderby == "1" || orderby == "Desc")
                    data = Sort(data, OrderBy.Descending);
            }
            #endregion
            var list = new List<ViewContactLess>();
            AutoMapper.Mapper.Map(data, list);
            return list;
        }
        public List<Contact> Sort(List<Contact> contacts, OrderBy sortby)
        {
            List<Contact> result = new List<Contact>();
            if (ReferenceEquals(contacts, null)) return null;
            switch (sortby)
            {
                case OrderBy.Ascending:
                    result = contacts.OrderBy(p => p.ContactId).ToList();
                    break;
                case OrderBy.Descending:
                    result = contacts.OrderByDescending(p => p.ContactId).ToList();
                    break;
            }
            return result;
        }
        public List<Contact> Paging(List<Contact> contacts, int skip, int count)
        {
            if (count < skip || count == 0) return null;
            return contacts.Skip(skip).Take(count).ToList();
        }
        #endregion
        public async Task AddToDatabaseFromBytes(byte[] bytes)
        {
            string path = System.Web.HttpContext.Current?.Request.MapPath("~//log");
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    List<Contact> contacts = _parser.GetContactsFromBytes(bytes, path);
                    contacts.ForEach(p => p.GuID = Guid.NewGuid());
                    _database.Contacts.AddRange(contacts);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public void Dispose()
        {
            _database.Dispose();
        }
    }
    public enum FilterBy
    {
        Name,
        Company,
        NameCompany,
        Position,
        NamePosition,
        CompanyPosition,
        CountryPosition,
        NameCompanyPosition,
        Country,
        NameCountry,
        NamePositionCountry,
        NameCompanyCountry,
        NameCompanyPositionCountry,
        DateInserted
    }
    public enum OrderBy
    {
        Ascending,
        Descending
    }
}