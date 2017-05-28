namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Linq;
    using Entities;
    using Models.Response;
    using Models.Request;

    // TODO: Sort and pagination
    public partial class ApplicationManager : IDisposable
    {
        private readonly CRMContext _database;
        private readonly ModelFactory _factory;
        private readonly ParserProvider _parser;
        public ApplicationManager()
        {
            _parser = new ParserProvider();
            _factory = new ModelFactory();
            _database = new CRMContext();
            _database.Configuration.LazyLoadingEnabled = false;
        }
        public async Task<List<ViewContactLess>> GetAllContactsAsync()
        {
            try
            {
                var list = await _database.Contacts.ToListAsync();
                var data = _factory.CreateViewContactLessList(list);
                return data;
            }
            catch (Exception ex)
            {
                throw new EntityException(ex.Message);
            }
        }
        public async Task<ViewContact> GetContactByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            try
            {
                var contact =
                    await _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.ContactId == id.Value);
                if (ReferenceEquals(contact, null)) return null;
                var data = _factory.CreateViewModel(contact);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ViewContact> GetContactByGuidAsync(Guid? guid)
        {
            try
            {
                var contact =
                    await _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.GuID == guid.Value);
                if (ReferenceEquals(contact, null)) return null;
                var data = _factory.CreateViewModel(contact);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> UpdateConactAsync(ViewContact contact)
        {
            if (ReferenceEquals(contact, null)) return false;
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var original =
                        await
                            _database.Contacts.Include(p => p.EmailLists)
                                .FirstOrDefaultAsync(p => p.GuID == contact.GuId);
                    var replace = await _factory.GetContactFromContactModel(contact, false);
                    replace.ContactId = original.ContactId;
                    _database.Entry(original).CurrentValues.SetValues(replace);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        public async Task<bool> AddContactAsync(ViewContact contact)
        {
            var cont = await _factory.GetContactFromContactModel(contact, true);
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    _database.Contacts.Add(cont);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        public async Task<bool> DeleteContactAsync(Guid guid)
        {
            var cont = await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid);
            try
            {
                _database.Contacts.Remove(cont);
                await _database.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<int> PageCountAsync()
        {
            try
            {
                var count = await _database.Contacts.CountAsync();
                return count / 10 + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<List<ViewContactLess>> PostBigRequest(RequestQuery request)
        {
            if (ReferenceEquals(request, null)) return null;
            var filter = request.FilterBy;
            var result = new List<ViewContactLess>();
            try
            {
                if (filter.FullName != null)
                    result = await Filter(request, FilterBy.Name);
                if (filter.CompanyName != null)
                {
                    result = await Filter(request, FilterBy.Company);
                    if (filter.FullName != null)
                        result = await Filter(request, FilterBy.NameCompany);
                }
                if (filter.Position != null)
                {
                    result = await Filter(request, FilterBy.Position);
                    if (filter.FullName != null)
                        result = await Filter(request, FilterBy.NamePosition);
                    if (filter.CompanyName != null)
                        result = await Filter(request, FilterBy.CompanyPosition);
                    if (filter.FullName != null && filter.CompanyName != null)
                        result = await Filter(request, FilterBy.NameCompanyPosition);
                }
                if (filter.Country != null)
                {
                    result = await Filter(request, FilterBy.Country);
                    if (filter.FullName != null)
                        result = await Filter(request, FilterBy.NameCountry);
                    if (filter.Position != null)
                        result = await Filter(request, FilterBy.CompanyPosition);
                    if (filter.FullName != null && filter.Position != null)
                        result = await Filter(request, FilterBy.NamePositionCountry);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ViewContactLess>> Filter(RequestQuery model, FilterBy filter)
        {
            var data = new List<Contact>();
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
            }
            #endregion
            var result = _factory.CreateViewContactLessList(data);
            return result;
        }
        public async Task<bool> AddToDatabaseFromBytes(byte[] bytes)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    var contacts = _parser.GetContactsFromBytes(bytes);
                    _database.Contacts.AddRange(contacts);
                    await _database.SaveChangesAsync();
                    transaction.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
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
        NameCompanyPosition,
        Country,
        NameCountry,
        NamePositionCountry,
        NameCompanyCountry,
        NameCompanyPositionCountry,
        DateInserted
    }
    public enum SortBy
    {
        NoSort,
        Ascending,
        Descending
    }
}