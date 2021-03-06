﻿using System;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using CRM.Entities;
using CRM.WebApi.InfrastructureModel;
using CRM.WebApi.InfrastructureOAuth.CRM.UserManager;
using CRM.WebApi.Models.Identity;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private enum Role
        {
            SuperAdmin,
            Admin
        }

        private CrmUserManager manager;
        private MailManager mailmanager;

        public AccountController()
        {
            manager = CrmUserManager.UserManager; // get current from owin context
            this.mailmanager = new MailManager();
        }

        [HttpGet]
        [Authorize]
        [Route("admin/users")]
        public async Task<HttpResponseMessage> GetAllUsersAsync()
        {
            if (!IsInRole(Role.SuperAdmin)) return Request.CreateResponse(HttpStatusCode.NotFound);
            var data = await this.manager.Users.ToListAsync();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [Authorize]
        [Route("admin/delete")]
        public async Task<HttpResponseMessage> DeleteUserAsync([FromBody] Guid guid)
        {
            if (!IsInRole(Role.SuperAdmin)) return Request.CreateResponse(HttpStatusCode.Unauthorized);
            var user = await this.manager.FindByIdAsync(guid.ToString());
            IdentityResult result = await this.manager.DeleteAsync(user);
            if (!result.Succeeded) return Request.CreateResponse(HttpStatusCode.NotModified, result.Errors);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Authorize]
        [Route("admin/change")]
        public async Task<HttpResponseMessage> PutUserAsync(User user)
        {
            if (!IsInRole(Role.SuperAdmin)) return Request.CreateResponse(HttpStatusCode.Unauthorized);
            var result = await this.manager.UpdateAsync(user);
            if (!result.Succeeded) return Request.CreateResponse(HttpStatusCode.NotModified);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // register user
        [AllowAnonymous]
        [Route("register")]
        public async Task<HttpResponseMessage> PostRegisterUser(RegisterUserModel model)
        {
            User user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                PhoneNumber = model.PhoneNumber,
            };

            IdentityResult identity = await this.manager.CreateAsync(user, model.Password);

            if (!identity.Succeeded) return Request.CreateResponse(HttpStatusCode.NotAcceptable, identity.Errors);

            var code = await this.manager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callback = new Uri(Url.Link("ConfirmEmailRoute", new { userid = user.Id, code }));
            if (await this.mailmanager.SendConfirmationEmail(user.Email, callback.ToString()))
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Email sended.");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // email confirmation
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<HttpResponseMessage> GetConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("Error", "User Id and Code are required");
                return Request.CreateResponse(HttpStatusCode.NotFound, ModelState);
            }
            IdentityResult result = await this.manager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "You have successfully confirmed your email.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [Authorize, Route("admin/register")]
        public async Task<HttpResponseMessage> RegisterAdmin(RegisterUserModel model)
        {
            User user = new User()
            {
                Email = model.Email,
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName,
            };
            IdentityResult result = await this.manager.AddToRoleAsync(user.Id, Role.SuperAdmin.ToString());
            if (result.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Created");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, result.Errors);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.manager.Dispose();
            base.Dispose(disposing);
        }
        [NonAction]
        private bool IsInRole(Role role)
        {
            return RequestContext.Principal.IsInRole(role.ToString());
        }

        [NonAction]
        public HttpResponseMessage ResetDatabaseToStock()
        {
            var context = new CRMContext();
            context.ResetDatabaseToStock(); // stored proc from SQL server.
            string path = System.Web.HttpContext.Current?.Request.MapPath("~//reset//index.html");
            var response = new HttpResponseMessage { Content = new StringContent(File.ReadAllText(path)) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}
