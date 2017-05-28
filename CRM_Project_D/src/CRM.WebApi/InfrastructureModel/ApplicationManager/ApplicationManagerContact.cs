﻿namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Models.Request;
    using Models.Response;
    // TODO: pagination
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
                List<Contact> list = await _database.Contacts.ToListAsync();
                List<ViewContactLess> data = _factory.CreateViewContactLessList(list);
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
                Contact contact =
                    await
                        _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.ContactId == id.Value);
                if (ReferenceEquals(contact, null)) return null;
                ViewContact data = _factory.CreateViewModel(contact);
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
                Contact contact =
                    await _database.Contacts.Include(p => p.EmailLists).FirstOrDefaultAsync(p => p.GuID == guid.Value);
                if (ReferenceEquals(contact, null)) return null;
                ViewContact data = _factory.CreateViewModel(contact);
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
            using (DbContextTransaction transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    Contact original =
                        await
                            _database.Contacts.Include(p => p.EmailLists)
                                .FirstOrDefaultAsync(p => p.GuID == contact.GuId);
                    Contact replace = await _factory.GetContactFromContactModel(contact, false);
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
            Contact cont = await _factory.GetContactFromContactModel(contact, true);
            using (DbContextTransaction transaction = _database.Database.BeginTransaction())
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
            Contact cont = await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid);
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
                int count = await _database.Contacts.CountAsync();
                return count / 10 + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ViewContactLess>> FilterOrderByRequestAsync(RequestQuery request)
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
                        result = await FilterSortAsync(request, FilterBy.CompanyPosition);
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
        public async Task<List<ViewContactLess>> FilterSortAsync(RequestQuery model, FilterBy filter)
        {
            List<Contact> data = new List<Contact>();
            Dictionary<string, string> sortby = model.SortBy;
            ViewContactLess filterby = model.FilterBy;
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
            if (sortby.ContainsKey("OrderBy"))
            {
                string orderby = sortby["OrderBy"];
                if (orderby == "Ascending" || orderby == "0")
                    data = Sort(OrderBy.Ascending, data);
                if (orderby == "Descending" || orderby == "1")
                    data = Sort(OrderBy.Descending, data);
            }
            List<ViewContactLess> result = _factory.CreateViewContactLessList(data);
            return result;
        }
        public List<Contact> Sort(OrderBy sortby, List<Contact> contacts)
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
        public async Task<bool> AddToDatabaseFromBytes(byte[] bytes)
        {
            using (DbContextTransaction transaction = _database.Database.BeginTransaction())
            {
                try
                {
                    List<Contact> contacts = _parser.GetContactsFromBytes(bytes);
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
    public enum OrderBy
    {
        NoSort,
        Ascending,
        Descending
    }
}